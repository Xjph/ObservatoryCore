using System.Reflection;
using System.Data;
using Observatory.Framework.Interfaces;
using Observatory.Framework;
using System.Text.Json;
using Observatory.Utils;
using Microsoft.Security.Extensions;

namespace Observatory.PluginManagement
{
    public class PluginManager
    {
        public static PluginManager GetInstance
        {
            get
            {
                return _instance.Value;
            }
        }

        private static readonly Lazy<PluginManager> _instance = new Lazy<PluginManager>(NewPluginManager);

        private static PluginManager NewPluginManager()
        {
            return new PluginManager();
        }


        public readonly List<(string error, string? detail)> errorList;
        public readonly List<Panel> pluginPanels;
        public readonly List<DataTable> pluginTables;
        private readonly List<(IObservatoryWorker plugin, PluginStatus signed)> _workerPlugins;
        private readonly List<(IObservatoryNotifier plugin, PluginStatus signed)> _notifyPlugins;
        private readonly PluginCore core;
        private readonly PluginEventHandler pluginHandler;
        
        public List<(IObservatoryWorker plugin, PluginStatus signed)> EnabledWorkerPlugins
        {
            get => _workerPlugins.Where(p => !pluginHandler.DisabledPlugins.Contains(p.plugin)).ToList();
        }

        public List<(IObservatoryNotifier plugin, PluginStatus signed)> EnabledNotifyPlugins
        {
            get => _notifyPlugins.Where(p => !pluginHandler.DisabledPlugins.Contains(p.plugin)).ToList();
        }

        public bool HasPopupOverrideNotifiers
        {
            get => EnabledNotifyPlugins.Any(n => n.plugin.OverridePopupNotifications);
        }
        public bool HasAudioOverrideNotifiers
        {
            get => EnabledNotifyPlugins.Any(n => n.plugin.OverrideAudioNotifications);
        }

        private PluginManager()
        {
            errorList = LoadPlugins(out _workerPlugins, out _notifyPlugins);

            pluginHandler = new PluginEventHandler(_workerPlugins.Select(p => p.plugin), _notifyPlugins.Select(p => p.plugin));
            var logMonitor = LogMonitor.GetInstance;
            pluginPanels = new();
            pluginTables = new();

            logMonitor.JournalEntry += pluginHandler.OnJournalEvent;
            logMonitor.StatusUpdate += pluginHandler.OnStatusUpdate;
            logMonitor.LogMonitorStateChanged += pluginHandler.OnLogMonitorStateChanged;

            core = new PluginCore();

            List<IObservatoryPlugin> errorPlugins = new();
            
            foreach (var plugin in _workerPlugins.Select(p => p.plugin))
            {
                try
                {
                    LoadSettings(plugin);
                    plugin.Load(core);
                }
                catch (PluginException ex)
                {
                    errorList.Add((FormatErrorMessage(ex), ex.StackTrace));
                    errorPlugins.Add(plugin);
                }
            }

            _workerPlugins.RemoveAll(w => errorPlugins.Contains(w.plugin));
            errorPlugins.Clear();

            foreach (var plugin in _notifyPlugins.Select(p => p.plugin))
            {
                // Notifiers which are also workers need not be loaded again (they are the same instance).
                if (!plugin.GetType().IsAssignableTo(typeof(IObservatoryWorker)))
                {
                    try
                    {
                        LoadSettings(plugin);
                        plugin.Load(core);
                    }
                    catch (PluginException ex)
                    {
                        errorList.Add((FormatErrorMessage(ex), ex.StackTrace));
                        errorPlugins.Add(plugin);
                    }
                    catch (Exception ex)
                    {
                        errorList.Add(($"{plugin.ShortName}: {ex.Message}", ex.StackTrace));
                        errorPlugins.Add(plugin);
                    }
                }
            }

            _notifyPlugins.RemoveAll(n => errorPlugins.Contains(n.plugin));

            core.Notification += pluginHandler.OnNotificationEvent;
            core.PluginMessage += pluginHandler.OnPluginMessageEvent;

            if (errorList.Any())
                ErrorReporter.ShowErrorPopup("Plugin Load Error" + (errorList.Count > 1 ? "s" : String.Empty), errorList);
        }

        private static string FormatErrorMessage(PluginException ex)
        {
            return $"{ex.PluginName}: {ex.UserMessage}";
        }

        private void LoadSettings(IObservatoryPlugin plugin)
        {
            string savedSettings = Properties.Core.Default.PluginSettings;
            Dictionary<string, object> pluginSettings;

            if (!String.IsNullOrWhiteSpace(savedSettings))
            {
                var settings = JsonSerializer.Deserialize<Dictionary<string, object>>(savedSettings);
                if (settings != null)
                {
                    pluginSettings = settings;
                }
                else
                {
                    pluginSettings = new();
                }
            }
            else
            {
                pluginSettings = new();
            }

            if (pluginSettings.ContainsKey(plugin.Name))
            {
                var settingsElement = (JsonElement)pluginSettings[plugin.Name];
                var settingsObject = JsonSerializer.Deserialize(settingsElement.GetRawText(), plugin.Settings.GetType());
                plugin.Settings = settingsObject;    
            }
        }

        public static Dictionary<PropertyInfo, string> GetSettingDisplayNames(object settings)
        {
            var settingNames = new Dictionary<PropertyInfo, string>();

            if (settings != null)
            {
                var properties = settings.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var attrib = property.GetCustomAttribute<SettingDisplayName>();
                    if (attrib == null)
                    {
                        settingNames.Add(property, property.Name);
                    }
                    else
                    {
                        settingNames.Add(property, attrib.DisplayName);
                    }
                }
            }
            return settingNames;
        }

        public void SaveSettings(IObservatoryPlugin plugin)
        {
            string savedSettings = Properties.Core.Default.PluginSettings;
            Dictionary<string, object>? pluginSettings;

            if (!String.IsNullOrWhiteSpace(savedSettings))
            {
                pluginSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(savedSettings);
                pluginSettings ??= new();
            }
            else
            {
                pluginSettings = new();
            }

            if (pluginSettings.ContainsKey(plugin.Name))
            {
                pluginSettings[plugin.Name] = plugin.Settings;
            }
            else
            {
                pluginSettings.Add(plugin.Name, plugin.Settings);
            }

            string newSettings = JsonSerializer.Serialize(pluginSettings, new JsonSerializerOptions()
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
            });

            Properties.Core.Default.PluginSettings = newSettings;
            SettingsManager.Save();
        }

        public void SetPluginEnabled(IObservatoryPlugin plugin, bool enabled)
        {
            pluginHandler.SetPluginEnabled(plugin, enabled);
        }

        private static List<(string, string?)> LoadPlugins(out List<(IObservatoryWorker plugin, PluginStatus signed)> observatoryWorkers, out List<(IObservatoryNotifier plugin, PluginStatus signed)> observatoryNotifiers)
        {
            observatoryWorkers = new();
            observatoryNotifiers = new();
            var errorList = new List<(string, string?)>();

            string pluginPath = $"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}plugins";
            
            string? ownExe = System.Diagnostics.Process.GetCurrentProcess()?.MainModule?.FileName;
            FileSignatureInfo ownSig;

            // This will throw if ownExe is null, but that's an error condition regardless.
            using (var stream = File.OpenRead(ownExe ?? String.Empty)) 
                ownSig = FileSignatureInfo.GetFromFileStream(stream);
            

            if (Directory.Exists(pluginPath))
            {
                ExtractPlugins(pluginPath);

                var pluginLibraries = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}plugins", "*.dll");
                foreach (var dll in pluginLibraries)
                {
                    try
                    {
                        PluginStatus pluginStatus = PluginStatus.SigCheckDisabled;
                        bool loadOkay = true;

                        if (!Properties.Core.Default.AllowUnsigned)
                        {
                            if (ownSig.Kind == SignatureKind.Embedded)
                            {
                                FileSignatureInfo pluginSig;
                                using (var stream = File.OpenRead(dll))
                                    pluginSig = FileSignatureInfo.GetFromFileStream(stream);

                                if (pluginSig.Kind == SignatureKind.Embedded)
                                {
                                    if (pluginSig.SigningCertificate.Thumbprint == ownSig.SigningCertificate.Thumbprint)
                                    {
                                        pluginStatus = PluginStatus.Signed;
                                    }
                                    else
                                    {
                                        pluginStatus = PluginStatus.InvalidSignature;
                                    }
                                }
                                else
                                {
                                    pluginStatus = PluginStatus.Unsigned;
                                }
                            }
                            else
                            {
                                pluginStatus = PluginStatus.NoCert;
                            }

                            if (pluginStatus != PluginStatus.Signed && pluginStatus != PluginStatus.NoCert)
                            {
                                string pluginHash = ComputeSha512Hash(dll);

                                if (Properties.Core.Default.UnsignedAllowed == null)
                                    Properties.Core.Default.UnsignedAllowed = new();

                                if (!Properties.Core.Default.UnsignedAllowed.Contains(pluginHash))
                                {
                                    string warning;
                                    warning = $"Unable to confirm signature of plugin library {dll}.\r\n\r\n";
                                    warning += "Please ensure that you trust the source of this plugin before loading it.\r\n\r\n";
                                    warning += "Do you wish to continue loading the plugin? If you load this plugin you will not be asked again for this file.";

                                    var response = MessageBox.Show(warning, "Plugin Signature Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                                    if (response == DialogResult.OK)
                                    {
                                        Properties.Core.Default.UnsignedAllowed.Add(pluginHash);
                                        SettingsManager.Save();
                                    }
                                    else
                                    {
                                        loadOkay = false;
                                    }
                                }
                            }
                        }

                        if (loadOkay)
                        {
                            string error = LoadPluginAssembly(dll, observatoryWorkers, observatoryNotifiers, pluginStatus);
                            if (!string.IsNullOrWhiteSpace(error))
                            {
                                errorList.Add((error, string.Empty));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errorList.Add(($"ERROR: {new FileInfo(dll).Name}, {ex.Message}", ex.StackTrace ?? String.Empty));
                        LoadPlaceholderPlugin(dll, PluginStatus.InvalidLibrary, observatoryNotifiers);
                    }
                }
            }
            return errorList;
        }

        public void ObservatoryReady()
        {
            var workers = EnabledWorkerPlugins.Select(p => p.plugin);
            var notifiers = EnabledNotifyPlugins.Select(p => p.plugin);

            foreach (IObservatoryPlugin plugin in workers.Concat<IObservatoryPlugin>(notifiers))
            {
                try
                {
                    plugin.ObservatoryReady();
                }
                catch (Exception ex)
                {
                    core.GetPluginErrorLogger(plugin)(ex, "in ObservatoryReady()");
                }
            }
        }

        private static string ComputeSha512Hash(string filePath)
        {
            using (var SHA512 = System.Security.Cryptography.SHA512.Create())
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                    return BitConverter.ToString(SHA512.ComputeHash(fileStream)).Replace("-", "").ToLowerInvariant();
            }
        }

        private static void ExtractPlugins(string pluginFolder)
        {
            var files = Directory.GetFiles(pluginFolder, "*.zip")
                .Concat(Directory.GetFiles(pluginFolder, "*.eop")); // Elite Observatory Plugin

            foreach (var file in files)
            {
                try
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory(file, pluginFolder, true);
                    File.Delete(file);
                }
                catch 
                { 
                    // Just ignore files that don't extract successfully.
                }
            }
        }

        private static string LoadPluginAssembly(string dllPath, List<(IObservatoryWorker plugin, PluginStatus signed)> workers, List<(IObservatoryNotifier plugin, PluginStatus signed)> notifiers, PluginStatus pluginStatus)
        {
            string recursionGuard = string.Empty;

            System.Runtime.Loader.AssemblyLoadContext.Default.Resolving += (context, name) => {
            
                if ((name?.Name?.EndsWith("resources")).GetValueOrDefault(false))
                {
                    return null;
                }

                // Importing Observatory.Framework in the Explorer Lua scripts causes an attempt to reload
                // the assembly, just hand it back the one we already have.
                if ((name?.Name?.StartsWith("Observatory.Framework")).GetValueOrDefault(false) || name?.Name == "ObservatoryFramework")
                {
                    return context.Assemblies.Where(a => (a.FullName?.Contains("ObservatoryFramework")).GetValueOrDefault(false)).First();
                }

                var foundDlls = Directory.GetFileSystemEntries(new FileInfo($"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}plugins{Path.DirectorySeparatorChar}deps").FullName, name.Name + ".dll", SearchOption.TopDirectoryOnly);
                if (foundDlls.Any())
                {
                    return context.LoadFromAssemblyPath(foundDlls[0]);
                }

                if (name.Name != recursionGuard && name.Name != null)
                {
                    recursionGuard = name.Name;
                    return context.LoadFromAssemblyName(name);
                }
                else
                {
                    throw new Exception("Unable to load assembly " + name.Name);
                }
            };

            var pluginAssembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(new FileInfo(dllPath).FullName);
            Type[] types;
            string err = string.Empty;
            int pluginCount = 0;
            try
            {
                types = pluginAssembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.OfType<Type>().ToArray();
            }
            catch
            {
                types = Array.Empty<Type>();
            }

            IEnumerable<Type> workerTypes = types.Where(t => t.IsAssignableTo(typeof(IObservatoryWorker)));
            foreach (Type worker in workerTypes)
            {
                ConstructorInfo? constructor = worker.GetConstructor(Array.Empty<Type>());
                if (constructor != null)
                {
                    object instance = constructor.Invoke(Array.Empty<object>());
                    workers.Add(((instance as IObservatoryWorker)!, pluginStatus));
                    if (instance is IObservatoryNotifier)
                    {
                        // This is also a notifier; add to the notifier list as well, so the work and notifier are
                        // the same instance and can share state.
                        notifiers.Add(((instance as IObservatoryNotifier)!, pluginStatus));
                    }
                    pluginCount++;
                }
            }

            // Filter out items which are also workers as we've already created them above.
            var notifyTypes = types.Where(t =>
                    t.IsAssignableTo(typeof(IObservatoryNotifier)) && !t.IsAssignableTo(typeof(IObservatoryWorker)));
            foreach (Type notifier in notifyTypes)
            {
                ConstructorInfo? constructor = notifier.GetConstructor(Array.Empty<Type>());
                if (constructor != null)
                {
                    object instance = constructor.Invoke(Array.Empty<object>());
                    notifiers.Add(((instance as IObservatoryNotifier)!, pluginStatus));
                    pluginCount++;
                }
            }

            if (pluginCount == 0)
            {
                err += $"ERROR: Library '{dllPath}' contains no suitable interfaces.";
                LoadPlaceholderPlugin(dllPath, PluginStatus.InvalidPlugin, notifiers);
            }

            return err;
        }

        internal void Shutdown()
        {
            core.Shutdown();
        }

        private static void LoadPlaceholderPlugin(string dllPath, PluginStatus pluginStatus, List<(IObservatoryNotifier plugin, PluginStatus signed)> notifiers)
        {
            PlaceholderPlugin placeholder = new(new FileInfo(dllPath).Name);
            notifiers.Add((placeholder, pluginStatus));
        }

        /// <summary>
        /// Possible plugin load results and signature statuses.
        /// </summary>
        public enum PluginStatus
        {
            /// <summary>
            /// Plugin valid and signed with matching certificate.
            /// </summary>
            Signed,
            /// <summary>
            /// Plugin valid but not signed with any certificate.
            /// </summary>
            Unsigned,
            /// <summary>
            /// Plugin valid but not signed with valid certificate.
            /// </summary>
            InvalidSignature,
            /// <summary>
            /// Plugin invalid and cannot be loaded. Possible version mismatch.
            /// </summary>
            InvalidPlugin,
            /// <summary>
            /// Plugin not a CLR library.
            /// </summary>
            InvalidLibrary,
            /// <summary>
            /// Plugin valid but executing assembly has no certificate to match against.
            /// </summary>
            NoCert,
            /// <summary>
            /// Plugin signature checks disabled.
            /// </summary>
            SigCheckDisabled
        }
    }
}
