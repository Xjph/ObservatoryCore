using Observatory.Framework;
using Observatory.Framework.Interfaces;
using Observatory.Utils;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        private readonly List<IObservatoryWorker> _workerPlugins;
        private readonly List<IObservatoryNotifier> _notifyPlugins;
        private readonly Dictionary<IObservatoryPlugin, PluginStatus> _pluginStatus = [];
        private readonly PluginCore core;
        private readonly PluginEventHandler pluginHandler;

        private readonly JsonSerializerOptions SettingsJsonSerializerOptions = new JsonSerializerOptions()
        {
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
        };
        

        public PluginCore Core { get { return core; } }


        // Intended for rendering Tabs. Includes Disabled plugins.
        public List<IObservatoryWorker> AllUIPlugins
        {
            get => _workerPlugins.Where(p => p.PluginUI.PluginUIType != Framework.PluginUI.UIType.None).ToList();
        }

        public List<IObservatoryWorker> EnabledWorkerPlugins
        {
            get => _workerPlugins.Where(p => !pluginHandler.DisabledPlugins.Contains(p)).ToList();
        }

        public List<IObservatoryNotifier> EnabledNotifyPlugins
        {
            get => _notifyPlugins.Where(p => !pluginHandler.DisabledPlugins.Contains(p)).ToList();
        }

        public List<IObservatoryPlugin> AllPlugins
        {
            get => _workerPlugins.Cast<IObservatoryPlugin>()
                .Concat(_notifyPlugins.Cast<IObservatoryPlugin>())
                .Distinct().ToList();
        }

        public PluginStatus GetPluginStatus(IObservatoryPlugin plugin) => _pluginStatus[plugin];

        public bool HasPopupOverrideNotifiers
        {
            get => EnabledNotifyPlugins.Any(n => n.OverridePopupNotifications);
        }
        public bool HasAudioOverrideNotifiers
        {
            get => EnabledNotifyPlugins.Any(n => n.OverrideAudioNotifications);
        }

        private PluginManager()
        {
            errorList = LoadPlugins(out _workerPlugins, out _notifyPlugins);

            pluginHandler = new PluginEventHandler(_workerPlugins, _notifyPlugins);
            var logMonitor = LogMonitor.GetInstance;
            pluginPanels = new();
            pluginTables = new();

            logMonitor.JournalEntry += pluginHandler.OnJournalEvent;
            logMonitor.StatusUpdate += pluginHandler.OnStatusUpdate;
            logMonitor.LogMonitorStateChanged += pluginHandler.OnLogMonitorStateChanged;

            core = new PluginCore();
            var allPluginSettings = LoadAllPluginSettings();

            foreach (var plugin in _workerPlugins)
            {
                try
                {
                    LoadPluginSettings(plugin, allPluginSettings);
                }
                catch (Exception ex)
                {
                    errorList.Add(($"{plugin.ShortName}: {ex.Message}", ex.StackTrace));
                    _pluginStatus[plugin] = PluginStatus.SettingsReset;
                }

                try
                {
                    plugin.Load(core);
                }
                catch (PluginException ex)
                {
                    errorList.Add((FormatErrorMessage(ex), ex.StackTrace));
                    _pluginStatus[plugin] = PluginStatus.Errored;
                }
                catch (Exception ex)
                {
                    errorList.Add(($"{plugin.ShortName}: {ex.Message}", ex.StackTrace));
                    _pluginStatus[plugin] = PluginStatus.Errored;
                }
            }

            foreach (var plugin in _notifyPlugins)
            {
                // Notifiers which are also workers need not be loaded again (they are the same instance).
                if (!plugin.GetType().IsAssignableTo(typeof(IObservatoryWorker)))
                {
                    try
                    {
                        LoadPluginSettings(plugin, allPluginSettings);
                    }
                    catch (Exception ex)
                    {
                        errorList.Add(($"{plugin.ShortName}: {ex.Message}", ex.StackTrace));
                        _pluginStatus[plugin] = PluginStatus.SettingsReset;
                    }

                    try
                    {
                        plugin.Load(core);
                    }
                    catch (PluginException ex)
                    {
                        errorList.Add((FormatErrorMessage(ex), ex.StackTrace));
                        _pluginStatus[plugin] = PluginStatus.Errored;
                    }
                    catch (Exception ex)
                    {
                        errorList.Add(($"{plugin.ShortName}: {ex.Message}", ex.StackTrace));
                        _pluginStatus[plugin] = PluginStatus.Errored;
                    }
                }
            } 

            core.Notification += pluginHandler.OnNotificationEvent;
            core.UpdateNotificationEvent += pluginHandler.OnNotificationUpdate;
            core.CancelNotificationEvent += pluginHandler.OnNotificationCancel;
            core.LegacyPluginMessage += pluginHandler.OnLegacyPluginMessageEvent;
            core.PluginMessage += pluginHandler.OnPluginMessageEvent;

            if (errorList.Any())
                ErrorReporter.ShowErrorPopup("Plugin Load Error" + (errorList.Count > 1 ? "s" : String.Empty), errorList);
            else
                MaybePruneUnknownPluginSettings();
        }

        private static string FormatErrorMessage(PluginException ex)
        {
            return $"{ex.PluginName}: {ex.UserMessage}";
        }

        private Dictionary<string, object> LoadAllPluginSettings()
        {
            string savedSettings = Properties.Core.Default.PluginSettings;
            Dictionary<string, object> pluginSettings;

            if (!String.IsNullOrWhiteSpace(savedSettings))
            {
                var settings = JsonSerializer.Deserialize<Dictionary<string, object>>(savedSettings, SettingsJsonSerializerOptions);
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
            return pluginSettings;
        }

        internal static Guid GetPluginGuid(IObservatoryPlugin plugin)
        {
            var guidProp = plugin.GetType().GetProperty("Guid");
            var guid = guidProp?.GetValue(plugin.GetType()) as Guid?;
            return guid ?? Guid.Empty;
        }

        private void LoadPluginSettings(IObservatoryPlugin plugin, Dictionary<string, object> pluginSettings)
        {
            // Temporary fallback to using plugin name for backward compatibility.

            JsonElement? settingsElement = null;
            Guid pluginGuid = GetPluginGuid(plugin);
            string settingsKey = pluginGuid == Guid.Empty ? plugin.Name : pluginGuid.ToString();

            if (pluginSettings.TryGetValue(settingsKey, out object? value))
            {
                settingsElement = (JsonElement)value;
            } 
            else if (pluginSettings.TryGetValue(plugin.Name, out object? nameKeyedValue)) 
            {
                settingsElement = (JsonElement)nameKeyedValue;
            }

            if (settingsElement != null)
            {
                var settingsObject = JsonSerializer.Deserialize(
                    settingsElement?.GetRawText()!, 
                    plugin.Settings.GetType(), 
                    SettingsJsonSerializerOptions);
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
                pluginSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(savedSettings, SettingsJsonSerializerOptions);
                pluginSettings ??= new();
            }
            else
            {
                pluginSettings = new();
            }

            Guid pluginGuid = GetPluginGuid(plugin);
            string settingsKey = pluginGuid == Guid.Empty ? plugin.Name : pluginGuid.ToString();

            if (pluginSettings.ContainsKey(settingsKey))
            {
                pluginSettings[settingsKey] = plugin.Settings;
            }
            else
            {
                pluginSettings.Add(settingsKey, plugin.Settings);
            }

            // Remove the old name-keyed settings if they exist but plugin has GUID
            if (pluginGuid != Guid.Empty && pluginSettings.ContainsKey(plugin.Name))
            {
                pluginSettings.Remove(plugin.Name);
            }

            string newSettings = JsonSerializer.Serialize(pluginSettings, SettingsJsonSerializerOptions);

            Properties.Core.Default.PluginSettings = newSettings;
            SettingsManager.Save();
        }

        private void MaybePruneUnknownPluginSettings()
        {
            HashSet<string> knownPluginNames = AllPlugins.Select(p => p.Name).ToHashSet();

            string savedSettings = Properties.Core.Default.PluginSettings;
            Dictionary<string, object>? pluginSettings;

            if (!String.IsNullOrWhiteSpace(savedSettings))
            {
                pluginSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(savedSettings, SettingsJsonSerializerOptions);
                pluginSettings ??= new();
            }
            else
            {
                pluginSettings = new();
            }

            bool isDirty = false;
            foreach (string settingKey in pluginSettings.Keys.ToList()) // copy to avoid errors due to removing from the list we're iterating
            {
                if (!knownPluginNames.Contains(settingKey))
                {
                    pluginSettings.Remove(settingKey);
                    isDirty = true;
                    Debug.WriteLine($"Purged stale settings for unknown plugin with key {settingKey}");
                }
            }

            if (isDirty)
            {
                string newSettings = JsonSerializer.Serialize(pluginSettings, SettingsJsonSerializerOptions);

                Properties.Core.Default.PluginSettings = newSettings;
                SettingsManager.Save();
            }
        }

        public void SetPluginEnabled(IObservatoryPlugin plugin, bool enabled)
        {
            pluginHandler.SetPluginEnabled(plugin, enabled);
        }

        private List<(string, string?)> LoadPlugins(out List<IObservatoryWorker> observatoryWorkers, out List<IObservatoryNotifier> observatoryNotifiers)
        {
            observatoryWorkers = new();
            observatoryNotifiers = new();
            var errorList = new List<(string, string?)>();

            string pluginPath = $"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}plugins";

            if (Directory.Exists(pluginPath))
            {
                ExtractPlugins(pluginPath);

                var pluginLibraries = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}plugins", "*.dll");
                foreach (var dll in pluginLibraries)
                {
                    try
                    {
                        string error = LoadPluginAssembly(dll, observatoryWorkers, observatoryNotifiers);
                        if (!string.IsNullOrWhiteSpace(error))
                        {
                            errorList.Add((error, string.Empty));
                        }
                    }
                    catch (Exception ex)
                    {
                        errorList.Add(($"ERROR: {new FileInfo(dll).Name}, {ex.Message}", ex.StackTrace ?? String.Empty));
                        _pluginStatus.Add(LoadPlaceholderPlugin(dll, observatoryNotifiers), PluginStatus.InvalidLibrary);
                    }
                }
            }
            return errorList;
        }

        public void ObservatoryReady()
        {
            var workers = EnabledWorkerPlugins;
            var notifiers = EnabledNotifyPlugins;

            foreach (IObservatoryPlugin plugin in workers.Union<IObservatoryPlugin>(notifiers))
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

        private string LoadPluginAssembly(string dllPath, List<IObservatoryWorker> workers, List<IObservatoryNotifier> notifiers)
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

            var frameworkRef = pluginAssembly.GetReferencedAssemblies().Where(a => a.Name == "ObservatoryFramework");

            if (frameworkRef.Any())
            {
                var status = frameworkRef.First().Version?.Major >= 1 ? PluginStatus.OK : PluginStatus.Outdated;

                IEnumerable<Type> workerTypes = types.Where(t => t.IsAssignableTo(typeof(IObservatoryWorker)));
                foreach (Type worker in workerTypes)
                {
                    ConstructorInfo? constructor = worker.GetConstructor(Array.Empty<Type>());
                    if (constructor != null)
                    {
                        object instance = constructor.Invoke(Array.Empty<object>());
                        workers.Add((instance as IObservatoryWorker)!);
                        if (instance is IObservatoryNotifier)
                        {
                            // This is also a notifier; add to the notifier list as well, so the work and notifier are
                            // the same instance and can share state.
                            notifiers.Add((instance as IObservatoryNotifier)!);
                        }
                        _pluginStatus.Add(instance as IObservatoryPlugin, status);
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
                        notifiers.Add((instance as IObservatoryNotifier)!);
                        _pluginStatus.Add(instance as IObservatoryPlugin, status);
                        pluginCount++;
                    }
                }

                if (pluginCount == 0)
                {
                    err += $"ERROR: Library '{dllPath}' contains no suitable interfaces.";
                    _pluginStatus.Add(LoadPlaceholderPlugin(dllPath, notifiers), PluginStatus.InvalidPlugin);
                }
            }
            else
            {
                err += $"ERROR: Library '{dllPath}' is not an Observatory Plugin.";
                _pluginStatus.Add(LoadPlaceholderPlugin(dllPath, notifiers), PluginStatus.InvalidPlugin);
            }
            

            return err;
        }

        internal void Shutdown()
        {
            core.Shutdown();
        }

        private static IObservatoryPlugin LoadPlaceholderPlugin(string dllPath, List<IObservatoryNotifier> notifiers)
        {
            PlaceholderPlugin placeholder = new(new FileInfo(dllPath).Name);
            notifiers.Add(placeholder);
            return placeholder;
        }

        /// <summary>
        /// Possible plugin load results.
        /// </summary>
        public enum PluginStatus
        {
            /// <summary>
            /// Plugin valid.
            /// </summary>
            OK,
            /// <summary>
            /// Plugin invalid and cannot be loaded. Possible version mismatch.
            /// </summary>
            InvalidPlugin,
            /// <summary>
            /// Plugin not a CLR library.
            /// </summary>
            InvalidLibrary,
            /// <summary>
            /// Plugin was built using an older Framwork with breaking changes.
            /// </summary>
            Outdated,
            /// <summary>
            /// The plugin falied to load.
            /// </summary>
            Errored,
            /// <summary>
            /// Settings for the plugin failed to load and were reset.
            /// </summary>
            SettingsReset,
            
        }
    }
}
