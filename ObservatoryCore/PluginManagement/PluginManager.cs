using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Data;
using Observatory.Framework.Interfaces;
using System.IO;
using Observatory.Framework;
using System.Text.Json;

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


        public readonly List<(string error, string detail)> errorList;
        public readonly List<Panel> pluginPanels;
        public readonly List<DataTable> pluginTables;
        public readonly List<(IObservatoryWorker plugin, PluginStatus signed)> workerPlugins;
        public readonly List<(IObservatoryNotifier plugin, PluginStatus signed)> notifyPlugins;
        private readonly PluginCore core;
        
        private PluginManager()
        {
            errorList = LoadPlugins(out workerPlugins, out notifyPlugins);

            var pluginHandler = new PluginEventHandler(workerPlugins.Select(p => p.plugin), notifyPlugins.Select(p => p.plugin));
            var logMonitor = LogMonitor.GetInstance;
            pluginPanels = new();
            pluginTables = new();

            logMonitor.JournalEntry += pluginHandler.OnJournalEvent;
            logMonitor.StatusUpdate += pluginHandler.OnStatusUpdate;
            logMonitor.LogMonitorStateChanged += pluginHandler.OnLogMonitorStateChanged;

            core = new PluginCore();

            List<IObservatoryPlugin> errorPlugins = new();
            
            foreach (var plugin in workerPlugins.Select(p => p.plugin))
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

            workerPlugins.RemoveAll(w => errorPlugins.Contains(w.plugin));
            errorPlugins.Clear();

            foreach (var plugin in notifyPlugins.Select(p => p.plugin))
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
                }
            }

            notifyPlugins.RemoveAll(n => errorPlugins.Contains(n.plugin));

            core.Notification += pluginHandler.OnNotificationEvent;
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
                pluginSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(savedSettings);
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
                    var attrib = property.GetCustomAttribute<Framework.SettingDisplayName>();
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

        public void SaveSettings(IObservatoryPlugin plugin, object settings)
        {
            string savedSettings = Properties.Core.Default.PluginSettings;
            Dictionary<string, object> pluginSettings;

            if (!String.IsNullOrWhiteSpace(savedSettings))
            {
                pluginSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(savedSettings);
            }
            else
            {
                pluginSettings = new();
            }

            if (pluginSettings.ContainsKey(plugin.Name))
            {
                pluginSettings[plugin.Name] = settings;
            }
            else
            {
                pluginSettings.Add(plugin.Name, settings);
            }

            string newSettings = JsonSerializer.Serialize(pluginSettings, new JsonSerializerOptions()
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
            });

            Properties.Core.Default.PluginSettings = newSettings;
            Properties.Core.Default.Save();
        }

        private static List<(string, string)> LoadPlugins(out List<(IObservatoryWorker plugin, PluginStatus signed)> observatoryWorkers, out List<(IObservatoryNotifier plugin, PluginStatus signed)> observatoryNotifiers)
        {
            observatoryWorkers = new();
            observatoryNotifiers = new();
            var errorList = new List<(string, string)>();

            string pluginPath = $"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}plugins";

            if (Directory.Exists(pluginPath))
            {
                ExtractPlugins(pluginPath);

                //Temporarily skipping signature checks. Need to do this the right way later.
                var pluginLibraries = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}plugins", "*.dll");
                //var coreToken = Assembly.GetExecutingAssembly().GetName().GetPublicKeyToken();
                foreach (var dll in pluginLibraries)
                {
                    try
                    {
                        //var pluginToken = AssemblyName.GetAssemblyName(dll).GetPublicKeyToken();
                        //PluginStatus signed;

                        //if (pluginToken.Length == 0)
                        //{
                        //    errorList.Add($"Warning: {dll} not signed.");
                        //    signed = PluginStatus.Unsigned;
                        //}
                        //else if (!coreToken.SequenceEqual(pluginToken))
                        //{
                        //    errorList.Add($"Warning: {dll} signature does not match.");
                        //    signed = PluginStatus.InvalidSignature;
                        //}
                        //else
                        //{
                        //    errorList.Add($"OK: {dll} signed.");
                        //    signed = PluginStatus.Signed;
                        //}

                        //if (signed == PluginStatus.Signed || Properties.Core.Default.AllowUnsigned)
                        //{
                            string error = LoadPluginAssembly(dll, observatoryWorkers, observatoryNotifiers);
                            if (!string.IsNullOrWhiteSpace(error))
                            {
                                errorList.Add((error, string.Empty));
                            }
                        //}
                        //else
                        //{
                        //    LoadPlaceholderPlugin(dll, signed, observatoryNotifiers);
                        //}
                        

                    }
                    catch (Exception ex)
                    {
                        errorList.Add(($"ERROR: {new FileInfo(dll).Name}, {ex.Message}", ex.StackTrace));
                        LoadPlaceholderPlugin(dll, PluginStatus.InvalidLibrary, observatoryNotifiers);
                    }
                }
            }
            return errorList;
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

        private static string LoadPluginAssembly(string dllPath, List<(IObservatoryWorker plugin, PluginStatus signed)> workers, List<(IObservatoryNotifier plugin, PluginStatus signed)> notifiers)
        {

            string recursionGuard = string.Empty;

            System.Runtime.Loader.AssemblyLoadContext.Default.Resolving += (context, name) => {
            
                if (name.Name.EndsWith("resources"))
                {
                    return null;
                }

                // Importing Observatory.Framework in the Explorer Lua scripts causes an attempt to reload
                // the assembly, just hand it back the one we already have.
                if (name.Name.StartsWith("Observatory.Framework") || name.Name == "ObservatoryFramework")
                {
                    return context.Assemblies.Where(a => a.FullName.Contains("ObservatoryFramework")).First();
                }

                var foundDlls = Directory.GetFileSystemEntries(new FileInfo($"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}plugins{Path.DirectorySeparatorChar}deps").FullName, name.Name + ".dll", SearchOption.TopDirectoryOnly);
                if (foundDlls.Any())
                {
                    return context.LoadFromAssemblyPath(foundDlls[0]);
                }

                if (name.Name != recursionGuard)
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
                types = ex.Types.Where(t => t != null).ToArray();
            }
            catch
            {
                types = Array.Empty<Type>();
            }

            var workerTypes = types.Where(t => t.IsAssignableTo(typeof(IObservatoryWorker)));
            foreach (var worker in workerTypes)
            {
                ConstructorInfo constructor = worker.GetConstructor(Array.Empty<Type>());
                object instance = constructor.Invoke(Array.Empty<object>());
                workers.Add((instance as IObservatoryWorker, PluginStatus.Signed));
                if (instance is IObservatoryNotifier)
                {
                    // This is also a notifier; add to the notifier list as well, so the work and notifier are
                    // the same instance and can share state.
                    notifiers.Add((instance as IObservatoryNotifier, PluginStatus.Signed));
                }
                pluginCount++;
            }

            // Filter out items which are also workers as we've already created them above.
            var notifyTypes = types.Where(t =>
                    t.IsAssignableTo(typeof(IObservatoryNotifier)) && !t.IsAssignableTo(typeof(IObservatoryWorker)));
            foreach (var notifier in notifyTypes)
            {
                ConstructorInfo constructor = notifier.GetConstructor(Array.Empty<Type>());
                object instance = constructor.Invoke(Array.Empty<object>());
                notifiers.Add((instance as IObservatoryNotifier, PluginStatus.Signed));
                pluginCount++;
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

        public enum PluginStatus
        {
            Signed,
            Unsigned,
            InvalidSignature,
            InvalidPlugin,
            InvalidLibrary
        }
    }
}
