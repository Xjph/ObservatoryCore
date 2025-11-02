using Observatory.Framework;
using Observatory.Framework.Interfaces;
using Observatory.Utils;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.Loader;
using System.IO.Compression;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

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
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
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
            core = new PluginCore();

            errorList = LoadPlugins(out _workerPlugins, out _notifyPlugins);

            pluginHandler = new PluginEventHandler(_workerPlugins, _notifyPlugins);
            var logMonitor = LogMonitor.GetInstance;
            pluginPanels = new();
            pluginTables = new();

            logMonitor.JournalEntry += pluginHandler.OnJournalEvent;
            logMonitor.StatusUpdate += pluginHandler.OnStatusUpdate;
            logMonitor.LogMonitorStateChanged += pluginHandler.OnLogMonitorStateChanged;

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

                pluginSettings = settings ?? [];
            }
            else
            {
                pluginSettings = [];
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
                pluginSettings ??= [];
            }
            else
            {
                pluginSettings = [];
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
            // Get list of both names and GUIDs to preserve,
            // migration will handle pruning if plugin present.
            HashSet<string> knownPluginNames = [.. AllPlugins.Select(p => 
            { 
                var guid = GetPluginGuid(p);
                return guid == Guid.Empty ? p.Name : guid.ToString();
            }),
            ..AllPlugins.Select(p => p.Name)];

            string savedSettings = Properties.Core.Default.PluginSettings;
            Dictionary<string, object>? pluginSettings;

            if (!String.IsNullOrWhiteSpace(savedSettings))
            {
                pluginSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(savedSettings, SettingsJsonSerializerOptions);
                pluginSettings ??= [];
            }
            else
            {
                pluginSettings = [];
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
            observatoryWorkers = [];
            observatoryNotifiers = [];
            var errorList = new List<(string, string?)>();

            string pluginPath = $"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}plugins";

            if (Directory.Exists(pluginPath))
            {
                var pluginLibraries = CollectPlugins(pluginPath, errorList);

                var legacyPlugins = CollectLegacyPlugins(pluginPath, errorList);

                if (legacyPlugins.Count > 0)
                {
                    var duplicatedLegacyPlugins = legacyPlugins
                        .Where(lp => pluginLibraries
                            .Any(p => p.PluginName.Equals(lp.PluginName, StringComparison.CurrentCultureIgnoreCase)));

                    foreach (var dupPlugin in duplicatedLegacyPlugins)
                    {
                        legacyPlugins.Remove(dupPlugin);
                        File.Delete(dupPlugin.PluginFile.FullName);
                        if (legacyPlugins.Count == 0)
                        {
                            Directory.Delete($"{pluginPath}{Path.DirectorySeparatorChar}deps", true);
                            break;
                        }
                    }

                    pluginLibraries.AddRange(legacyPlugins);
                }

                PluginCleanup(pluginPath, pluginLibraries);

                string recursionGuard = string.Empty;

                [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
                static extern bool SetDllDirectory(string lpPathName);

                // Kind of gross, but we need to place non-managed dependencies
                // in a location that can be found automatically by the loader.
                foreach (var dep in pluginLibraries.SelectMany(p => p.PluginDependencies))
                {
                    var peHeaderOffset = dep.Value[0x3C];
                    var peHeader = dep.Value[peHeaderOffset..(peHeaderOffset+2)];
                    if (!(peHeader[0] == 'P' && peHeader[1] == 'E'))
                    {
                        Debug.WriteLine($"Non-managed dependency: {dep.Key}");

                        var storageFolder = core.GetStorageFolderForPlugin("Core");

                        var depPath = $"{storageFolder}{Path.DirectorySeparatorChar}{dep.Key}";
                        SetDllDirectory(storageFolder);
                        File.WriteAllBytes(depPath, dep.Value);
                    }
                }

                AssemblyLoadContext.Default.Resolving += (context, name) => {

                    if ((name?.Name?.EndsWith("resources")).GetValueOrDefault(false))
                    {
                        return null;
                    }

                    // Some plugins appear to attempt reloading Observatory.Framework,
                    // just hand them back the one Core has already loaded.
                    if ((name?.Name?.StartsWith("Observatory.Framework")).GetValueOrDefault(false) || name?.Name == "ObservatoryFramework")
                    {
                        return context.Assemblies.Where(a => (a.FullName?.Contains("ObservatoryFramework")).GetValueOrDefault(false)).First();
                    }

                    var depLibraries = pluginLibraries
                        .SelectMany(p => p.PluginDependencies)
                        .Where(d => d.Key == name!.Name + ".dll")
                        .ToList();
                    
                    if (depLibraries.Count != 0)
                    {
                        Debug.WriteLine($"Loading plugin dependency {name.Name}");
                        return context.LoadFromStream(new MemoryStream(depLibraries[0].Value));
                    }

                    if (name?.Name != null && name.Name != recursionGuard)
                    {
                        recursionGuard = name.Name;
                        return context.LoadFromAssemblyName(name);
                    }
                    else
                    {
                        throw new Exception("Unable to load assembly " + name?.Name);
                    }
                };



                foreach (var plugin in pluginLibraries)
                {
                    try
                    {
                        string error = LoadPluginAssembly(plugin.PluginFile.Name, plugin.PluginLibrary, observatoryWorkers, observatoryNotifiers);
                        if (!string.IsNullOrWhiteSpace(error))
                        {
                            errorList.Add((error, string.Empty));
                        }
                    }
                    catch (Exception ex)
                    {
                        errorList.Add(($"ERROR: {plugin.PluginFile.Name}, {ex.Message}", ex.StackTrace ?? String.Empty));
                        _pluginStatus.Add(LoadPlaceholderPlugin(plugin.PluginFile.Name, observatoryNotifiers), PluginStatus.InvalidLibrary);
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

        private static List<PluginPackage> CollectPlugins(string pluginFolder, List<(string, string?)> errorList)
        {
            var files = Directory.GetFiles(pluginFolder, "*.eop"); // Elite Observatory Plugin
            var directories = Directory.GetDirectories(pluginFolder).Where(d => !d.EndsWith("\\deps"));
            var plugins = new List<PluginPackage>();

            Dictionary<string, (Version Version, DateTime Modified)> foundPlugins = [];

            foreach (var file in files)
            {
                try
                {
                    var pluginPackage = new PluginPackage(file);
                    plugins.Add(pluginPackage);
                }
                catch (Exception ex)
                { 
                    errorList.Add(("ERROR: Failed to extract plugin archive: " + file, ex.Message));
                }
            }
            foreach (var directory in directories)
            {
                try
                {
                    var pluginPackage = new PluginPackage(directory);
                    plugins.Add(pluginPackage);
                }
                catch (Exception ex)
                {
                    errorList.Add(("ERROR: Failed to process plugin bundle: " + directory, ex.Message));
                }
            }
            return plugins;
        }

        private static List<PluginPackage> CollectLegacyPlugins(string pluginFolder, List<(string, string?)> errorList)
        {
            var plugins = new List<PluginPackage>();
            var files = Directory.GetFiles(pluginFolder, "*.dll", SearchOption.TopDirectoryOnly);
            bool firstFile = true;
            foreach (var file in files)
            {
                try
                {
                    var pluginPackage = new PluginPackage(file, firstFile);
                    plugins.Add(pluginPackage);
                    firstFile = false;
                }
                catch (Exception ex)
                {
                    errorList.Add(("ERROR: Failed to load legacy plugin: " + file, ex.Message));
                }
            }

            return plugins;
        }

        private string LoadPluginAssembly(string pluginFile, byte[] pluginBytes, List<IObservatoryWorker> workers, List<IObservatoryNotifier> notifiers)
        {
            var pluginAssembly = AssemblyLoadContext.Default.LoadFromStream(new MemoryStream(pluginBytes));

            Type[] types;
            string err = string.Empty;
            int pluginCount = 0;
            try
            {
                types = pluginAssembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = [.. ex.Types.OfType<Type>()];
            }
            catch
            {
                types = [];
            }

            var frameworkRef = pluginAssembly.GetReferencedAssemblies().Where(a => a.Name == "ObservatoryFramework");

            if (frameworkRef.Any())
            {
                var status = frameworkRef.First().Version?.Major >= 1 ? PluginStatus.OK : PluginStatus.Outdated;

                IEnumerable<Type> workerTypes = types.Where(t => t.IsAssignableTo(typeof(IObservatoryWorker)));
                foreach (Type worker in workerTypes)
                {
                    ConstructorInfo? constructor = worker.GetConstructor([]);
                    if (constructor != null)
                    {
                        object instance = constructor.Invoke([]);
                        workers.Add((instance as IObservatoryWorker)!);
                        if (instance is IObservatoryNotifier)
                        {
                            // This is also a notifier; add to the notifier list as well, so the work and notifier are
                            // the same instance and can share state.
                            notifiers.Add((instance as IObservatoryNotifier)!);
                        }
                        _pluginStatus.Add((instance as IObservatoryPlugin)!, status);
                        pluginCount++;
                    }
                }

                // Filter out items which are also workers as we've already created them above.
                var notifyTypes = types.Where(t =>
                        t.IsAssignableTo(typeof(IObservatoryNotifier)) && !t.IsAssignableTo(typeof(IObservatoryWorker)));
                foreach (Type notifier in notifyTypes)
                {
                    ConstructorInfo? constructor = notifier.GetConstructor([]);
                    if (constructor != null)
                    {
                        object instance = constructor.Invoke([]);
                        notifiers.Add((instance as IObservatoryNotifier)!);
                        _pluginStatus.Add((instance as IObservatoryPlugin)!, status);
                        pluginCount++;
                    }
                }

                if (pluginCount == 0)
                {
                    err += $"ERROR: Library '{pluginFile}' contains no suitable interfaces.";
                    _pluginStatus.Add(LoadPlaceholderPlugin(pluginFile, notifiers), PluginStatus.InvalidPlugin);
                }
            }
            else
            {
                err += $"ERROR: Library '{pluginFile}' is not an Observatory Plugin.";
                _pluginStatus.Add(LoadPlaceholderPlugin(pluginFile, notifiers), PluginStatus.InvalidPlugin);
            }

            pluginBytes = [];
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

        private static void PluginCleanup(string pluginPath, List<PluginPackage> plugins)
        {
            // Group by plugin name (library name)
            var grouped = plugins
                .GroupBy(p => p.PluginFile.Extension.Equals(".dll", StringComparison.OrdinalIgnoreCase)
                    ? p.PluginName.ToLowerInvariant()
                    : p.PluginName.ToLowerInvariant());

            var toRemove = new List<PluginPackage>();

            foreach (var group in grouped)
            {
                // Keep highest version; for legacy, keep most recently modified
                PluginPackage keep;
                if (group.All(p => p.Legacy))
                {
                    keep = group.OrderByDescending(p => p.PluginFile.LastWriteTimeUtc).First();
                }
                else
                {
                    keep = group.OrderByDescending(p => p.Version).First();
                }

                // Mark all others for removal
                toRemove.AddRange(group.Where(p => p != keep));
            }

            // Remove duplicate plugin files from disk and from the list
            foreach (var plugin in toRemove)
            {
                try
                {
                    if (plugin.PluginFile.Exists)
                        plugin.PluginFile.Delete();
                }
                catch { /* Ignore file delete errors */ }
                plugins.Remove(plugin);
            }

            // Handle unpacked deps folder cleanup
            var unpackedPlugins = plugins.Where(p => p.PluginFile.Extension.Equals(".dll", StringComparison.OrdinalIgnoreCase)).ToList();
            if (unpackedPlugins.Count == 0)
            {
                var depsFolder = pluginPath + $"{Path.DirectorySeparatorChar}deps";
                if (Directory.Exists(depsFolder))
                {
                    try
                    {
                        Directory.Delete(depsFolder, true);
                    }
                    catch { /* Ignore folder delete errors */ }
                }
            }
        }

        private class PluginBundle
        {
            internal PluginBundle(string filePath)
            {
                var fileCheck = File.GetAttributes(filePath);
                if (!fileCheck.HasFlag(FileAttributes.Directory) && filePath.EndsWith(".dll"))
                {
                    IsLegacyInstall = true;
                }
                else if (fileCheck.HasFlag(FileAttributes.Directory))
                {
                    var directory = new DirectoryInfo(filePath);
                    ReadDirectory(directory);
                }
                else
                {
                    using var ZipArchive = ZipFile.OpenRead(filePath);
                    ReadArchive(ZipArchive);
                }
            }

            private void ReadArchive(ZipArchive archive)
            {
                bool manifestFound = false;

                foreach (var entry in archive.Entries)
                {
                    byte[] fileBytes;
                    entry.Open().CopyTo(new MemoryStream(fileBytes = new byte[entry.Length]));
                    Files.Add(entry.FullName, fileBytes);
                    if (entry.FullName.EndsWith(".deps.json"))
                    {
                        if (manifestFound)
                            throw new Exception("Malformed plugin archive: Contains multiple .deps.json files");

                        manifestFound = true;

                        byte[] manifestBytes = [];
                        entry.Open().Read(manifestBytes = new byte[entry.Length]);
                        string manifestJson = System.Text.Encoding.UTF8.GetString(manifestBytes);
                        Manifest = JsonSerializer.Deserialize<DependencyManifest>(manifestJson);
                    }
                }
            }

            private void ReadDirectory(DirectoryInfo directory)
            {
                bool manifestFound = false;
                foreach (var file in directory.GetFiles("*", SearchOption.AllDirectories))
                {
                    byte[] fileBytes = File.ReadAllBytes(file.FullName);
                    var relPath = Path.GetRelativePath(directory.FullName, file.FullName);
                    Files.Add(relPath.Replace(Path.DirectorySeparatorChar, '/'), fileBytes);

                    if (file.FullName.EndsWith(".deps.json"))
                    {
                        if (manifestFound)
                            throw new Exception("Malformed plugin bundle: Contains multiple .deps.json files");

                        manifestFound = true;

                        byte[] manifestBytes = File.ReadAllBytes(file.FullName);
                        string manifestJson = System.Text.Encoding.UTF8.GetString(manifestBytes);
                        Manifest = JsonSerializer.Deserialize<DependencyManifest>(manifestJson);
                    }
                }
            }

            public Dictionary<string, byte[]> Files = [];

            public DependencyManifest? Manifest = null;

            public bool IsLegacyPlugin => Manifest == null;

            public bool IsLegacyInstall;
        }

        private class PluginPackage
        {
            internal PluginPackage(string filePath, bool includeLegacyDeps = false)
            {
                var bundle = new PluginBundle(filePath);

                PluginFile = new FileInfo(filePath);
                PluginDependencies = [];
                if (bundle.IsLegacyInstall)
                {
                    Legacy = true;
                    PluginLibrary = File.ReadAllBytes(filePath);
                    PluginName = PluginFile.Name;
                    Version = new Version(1, 0, 0, 0);

                    // Impossible to determine which legacy deps belong to which plugin,
                    // just collect them all when indicated (first plugin).
                    if (includeLegacyDeps)
                    {
                        var depFiles = Directory.GetFiles($"{PluginFile.DirectoryName}{Path.DirectorySeparatorChar}deps", "*.dll", SearchOption.TopDirectoryOnly);
                        foreach (var depFile in depFiles)
                        {
                            PluginDependencies.Add(new FileInfo(depFile).Name, File.ReadAllBytes(depFile));
                        }
                    }
                }
                else if (bundle.IsLegacyPlugin)
                {
                    ProcessLegacyPlugin(bundle);
                }
                else
                {
                    ProcessPluginManifest(bundle);
                }
            }

            [MemberNotNull(nameof(PluginName))]
            [MemberNotNull(nameof(PluginLibrary))]
            [MemberNotNull(nameof(Version))]
            private void ProcessPluginManifest(PluginBundle bundle)
            {
                // Should never happen but just in case.
                if (bundle.Manifest == null)
                    throw new Exception("Malformed plugin archive: Missing .deps.json");

                var manifest = bundle.Manifest;

                Legacy = false;
                
                var pluginLibraryEntry = manifest.Libraries
                    .Where(l => l.Value.Type == "project")
                    .FirstOrDefault();

                if (pluginLibraryEntry.Equals(default(KeyValuePair<string, Library>)))
                    throw new Exception("Malformed plugin archive: No project library found in .deps.json");

                var targets = manifest.Targets?[manifest.RuntimeTarget.Name] ?? [];
                var pluginTarget = targets[pluginLibraryEntry.Key!];

                bundle.Files.TryGetValue(pluginTarget.Runtime.First().Key, out byte[]? pluginLibBytes);
                if (pluginLibBytes == null)
                    throw new Exception("Malformed plugin archive: Plugin library file not found in archive.");
                else
                    PluginLibrary = pluginLibBytes;

                
                var nameAndVersion = pluginLibraryEntry.Key.Split('/') ?? ["No Library", "0"];
                PluginName = nameAndVersion.First() ?? string.Empty;
                Version = new Version(nameAndVersion.Last() ?? "0");

                foreach (var dependency in targets.Where(t => t.Key != pluginLibraryEntry.Key))
                {
                    var depLibName = dependency.Value.Runtime.First().Key.Split('/').Last();
                    
                    bundle.Files.TryGetValue(depLibName, out byte[]? depBytes);
                    if (depBytes != null)
                    {
                        PluginDependencies.Add(depLibName, depBytes);
                    }
                    foreach (var runtimeTarget in dependency.Value.RuntimeTargets.Where(rt => rt.Value.Rid == "win-x64"))
                    {
                        var rtLibName = runtimeTarget.Key;
                        bundle.Files.TryGetValue(rtLibName, out byte[]? rtBytes);
                        if (rtBytes != null)
                        {
                            PluginDependencies.Add(rtLibName.Split('/').Last(), rtBytes);
                        }
                    }
                }
            }

            [MemberNotNull(nameof(PluginName))]
            [MemberNotNull(nameof(PluginLibrary))]
            [MemberNotNull(nameof(Version))]
            private void ProcessLegacyPlugin(PluginBundle bundle)
            {
                Version = new Version(1, 0, 0, 0);
                Legacy = true;
                var entries = bundle.Files.Keys.Where(f => f.EndsWith(".dll"));
                foreach (var entry in entries)
                {
                    var fileName = Path.GetFileName(entry);
                    if (entry.Contains("deps/") && entry.EndsWith(".dll"))
                    {
                        PluginDependencies.Add(fileName, bundle.Files[entry]);
                    }
                    else if (entry.EndsWith(".dll"))
                    {
                        PluginLibrary = bundle.Files[entry];
                        PluginName = fileName;
                    }
                }
                if (string.IsNullOrWhiteSpace(PluginName) || PluginLibrary == null)
                    throw new Exception("Malformed plugin archive: No plugin DLL found.");
            }

            public string PluginName;
            public FileInfo PluginFile;
            public byte[] PluginLibrary;
            public Dictionary<string, byte[]> PluginDependencies;
            public DateTime Modified => PluginFile.LastWriteTime;
            public Version Version;
            public bool Legacy;
        }

        private class DependencyManifest
        {
            [JsonPropertyName("runtimeTarget")]
            public RuntimeTarget RuntimeTarget { get; set; } = new();
            [JsonPropertyName("targets")]
            public Dictionary<string, Dictionary<string, Target>> Targets { get; set; } = [];
            [JsonPropertyName("libraries")]
            public Dictionary<string, Library> Libraries { get; set; } = [];
        }

        private class Library
        {
            [JsonPropertyName("type")]
            public string Type { get; set; } = string.Empty;
            [JsonPropertyName("serviceable")]
            public bool Serviceable { get; set; }
            [JsonPropertyName("sha512")]
            public string Sha512 { get; set; } = string.Empty;
            [JsonPropertyName("path")]
            public string Path { get; set; } = string.Empty;
            [JsonPropertyName("hashPath")]
            public string HashPath { get; set; } = string.Empty;
        }

        private class RuntimeTarget
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
            [JsonPropertyName("signature")]
            public string Signature { get; set; } = string.Empty;
        }

        private class Target
        {
            [JsonPropertyName("dependencies")]
            public Dictionary<string, string> Dependencies { get; set; } = [];
            [JsonPropertyName("runtime")]
            public Dictionary<string, VersionInfo> Runtime { get; set; } = [];
            [JsonPropertyName("runtimeTargets")]
            public Dictionary<string, DependencyTarget> RuntimeTargets { get; set; } = [];
        }

        private class VersionInfo
        {
            [JsonPropertyName("assemblyVersion")]
            public string AssemblyVersion { get; set; } = string.Empty;
            [JsonPropertyName("fileVersion")]
            public string FileVersion { get; set; } = string.Empty;
        }

        private class DependencyTarget
        {
            [JsonPropertyName("rid")]
            public string Rid { get; set; } = string.Empty;
            [JsonPropertyName("assetType")]
            public string AssetType { get; set; } = string.Empty;
            [JsonPropertyName("fileVersion")]
            public string FileVersion { get; set; } = string.Empty;
        }
    }
}
