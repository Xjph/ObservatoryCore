using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Observatory.Framework.Interfaces;
using Observatory.UI.Models;
using ReactiveUI;

namespace Observatory.UI.ViewModels
{
    public class CoreViewModel : ViewModelBase
    {
        private readonly ObservableCollection<IObservatoryNotifier> notifiers;
        private readonly ObservableCollection<IObservatoryWorker> workers;
        private readonly ObservableCollection<CoreModel> tabs;
        private string toggleButtonText;
        private bool _UpdateAvailable;
        
        public CoreViewModel(IEnumerable<(IObservatoryWorker plugin, PluginManagement.PluginManager.PluginStatus signed)> workers, IEnumerable<(IObservatoryNotifier plugin, PluginManagement.PluginManager.PluginStatus signed)> notifiers)
        {
            _UpdateAvailable = CheckUpdate();
            
            this.notifiers = new ObservableCollection<IObservatoryNotifier>(notifiers.Select(p => p.plugin));
            this.workers = new ObservableCollection<IObservatoryWorker>(workers.Select(p => p.plugin));
            ToggleButtonText = "Start Monitor";
            tabs = new ObservableCollection<CoreModel>();
            
            foreach(var worker in workers.Select(p => p.plugin))
            {
                if (worker.PluginUI.PluginUIType == Framework.PluginUI.UIType.Basic)
                {
                    CoreModel coreModel = new();
                    coreModel.Name = worker.ShortName;
                    coreModel.UI = new BasicUIViewModel(worker.PluginUI.DataGrid)
                    {
                        UIType = worker.PluginUI.PluginUIType
                    };
                    
                    tabs.Add(coreModel);
                }
            }

            foreach(var notifier in notifiers.Select(p => p.plugin))
            {
                Panel notifierPanel = new Panel();
                TextBlock notifierTextBlock = new TextBlock();
                notifierTextBlock.Text = notifier.Name;
                notifierPanel.Children.Add(notifierTextBlock);
                //tabs.Add(new CoreModel() { Name = notifier.ShortName, UI = (ViewModelBase)notifier.UI });
            }

            
            tabs.Add(new CoreModel() { Name = "Core", UI = new BasicUIViewModel(new ObservableCollection<object>()) { UIType = Framework.PluginUI.UIType.Core } });

            if (Properties.Core.Default.StartMonitor)
                ToggleMonitor();
            
            if (Properties.Core.Default.StartReadAll)
                ReadAll();

        }

        public void ReadAll()
        {
            LogMonitor.GetInstance.ReadAllJournals();
        }

        public void ToggleMonitor()
        {
            var logMonitor = LogMonitor.GetInstance;

            if (logMonitor.IsMonitoring())
            {
                logMonitor.Stop();
                ToggleButtonText = "Start Monitor";
            }
            else
            {
                logMonitor.Start();
                ToggleButtonText = "Stop Monitor";
            }
        }

        public void OpenGithub()
        {
            ProcessStartInfo githubOpen = new("https://github.com/Xjph/ObservatoryCore");
            githubOpen.UseShellExecute = true;
            Process.Start(githubOpen);
        }

        public void OpenDonate()
        {
            ProcessStartInfo donateOpen = new("https://paypal.me/eliteobservatory");
            donateOpen.UseShellExecute = true;
            Process.Start(donateOpen);
        }

        public void GetUpdate()
        {
            ProcessStartInfo githubOpen = new("https://github.com/Xjph/ObservatoryCore/releases");
            githubOpen.UseShellExecute = true;
            Process.Start(githubOpen);
        }

        public async void ExportGrid()
        {
            try
            {
                var exportFolder = Properties.Core.Default.ExportFolder;
                if (string.IsNullOrEmpty(exportFolder) || !Directory.Exists(exportFolder))
                {
                    exportFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                    OpenFolderDialog openFolderDialog = new()
                    {
                        Directory = exportFolder
                    };

                    var application = (IClassicDesktopStyleApplicationLifetime)Avalonia.Application.Current.ApplicationLifetime;

                    var selectedFolder = await openFolderDialog.ShowAsync(application.MainWindow);

                    if (!string.IsNullOrEmpty(selectedFolder))
                    {
                        Properties.Core.Default.ExportFolder = selectedFolder;
                        Properties.Core.Default.Save();
                        exportFolder = selectedFolder;
                    }
                }

                var exportStyle = Properties.Core.Default.ExportStyle;
                if (string.IsNullOrEmpty(exportStyle))
                {
                    exportStyle = "Fixed width";
                    Properties.Core.Default.ExportStyle = exportStyle;
                    Properties.Core.Default.Save();
                }

                foreach (var tab in tabs.Where(t => t.Name != "Core"))
                {
                    var ui = (BasicUIViewModel)tab.UI;
                    List<object> selectedData;
                    bool specificallySelected = ui.SelectedItems?.Count > 1;

                    if (specificallySelected)
                    {
                        selectedData = new();

                        foreach (var item in ui.SelectedItems)
                            selectedData.Add(item);
                    }
                    else
                    {
                        selectedData = ui.BasicUIGrid.ToList();
                    }

                    System.Text.StringBuilder exportData;
                    switch (exportStyle)
                    {
                        case "Tab separated":
                            exportData = ExportTabSeparated(selectedData);
                            break;
                        default: // Fixed width.
                            exportData = ExportFixedWidth(selectedData);
                            break;
                    }

                    string exportPath = $"{exportFolder}{System.IO.Path.DirectorySeparatorChar}Observatory Export - {DateTime.UtcNow:yyyyMMdd-HHmmss} - {tab.Name}.txt";

                    System.IO.File.WriteAllText(exportPath, exportData.ToString());
                }
            }
            catch (Exception e)
            {
                ObservatoryCore.LogError(e, "while exporting data");
                ErrorReporter.ShowErrorPopup("Error encountered!",
                    new List<(string, string)> { ("An error occurred while exporting; output may be missing or incomplete." + Environment.NewLine +
                    "Please check the error log (found in your Documents folder) for more details and visit our discord to report it.", e.Message) });
            }

            static System.Text.StringBuilder ExportTabSeparated(List<object> selectedData)
            {
                System.Text.StringBuilder exportData = new();

                var columnNames = selectedData[0].GetType().GetProperties().Select(c => c.Name).ToList();
                exportData.AppendJoin('\t', columnNames).AppendLine();

                var lastColumn = columnNames.Last();
                foreach (var line in selectedData)
                {
                    var lineType = line.GetType(); // some plugins have different line types, so don't move this out of loop
                    foreach (var columnName in columnNames)
                    {
                        var cellValue = lineType.GetProperty(columnName)?.GetValue(line)?.ToString() ?? string.Empty;
                        exportData.Append(cellValue).Append('\t');
                    }
                    exportData.AppendLine();
                }
                return exportData;
            }

            static System.Text.StringBuilder ExportFixedWidth(List<object> selectedData)
            {
                Dictionary<string, int> colSize = new();
                Dictionary<string, List<string>> colContent = new();

                var columns = selectedData[0].GetType().GetProperties();
                foreach (var column in columns)
                {
                    colSize.Add(column.Name, column.Name.Length);
                    colContent.Add(column.Name, new());
                }

                foreach (var line in selectedData)
                {
                    var lineType = line.GetType(); // some plugins have different line types, so don't move this out of loop
                    foreach (var column in colContent)
                    {
                        var cellValue = lineType.GetProperty(column.Key)?.GetValue(line)?.ToString() ?? string.Empty;
                        column.Value.Add(cellValue);
                        if (colSize[column.Key] < cellValue.Length)
                            colSize[column.Key] = cellValue.Length;
                    }
                }

                System.Text.StringBuilder exportData = new();


                foreach (var colTitle in colContent.Keys)
                {
                    if (colSize[colTitle] < colTitle.Length)
                        colSize[colTitle] = colTitle.Length;

                    exportData.Append(colTitle.PadRight(colSize[colTitle]) + "  ");
                }
                exportData.AppendLine();

                for (int i = 0; i < colContent.First().Value.Count; i++)
                {
                    foreach (var column in colContent)
                    {
                        if (column.Value[i].Length > 0 && !char.IsNumber(column.Value[i][0]) && column.Value[i].Count(char.IsLetter) / (float)column.Value[i].Length > 0.25)
                            exportData.Append(column.Value[i].PadRight(colSize[column.Key]) + "  ");
                        else
                            exportData.Append(column.Value[i].PadLeft(colSize[column.Key]) + "  ");
                    }
                    exportData.AppendLine();
                }

                return exportData;
            }
        }

        public void ClearGrid()
        {
            foreach (var tab in tabs.Where(t => t.Name != "Core"))
            {
                var ui = (BasicUIViewModel)tab.UI;

                var rowTemplate = ui.BasicUIGrid.First();

                foreach (var property in rowTemplate.GetType().GetProperties())
                {
                    property.SetValue(rowTemplate, null);
                }

                ui.BasicUIGrid.Clear();
                ui.BasicUIGrid.Add(rowTemplate);

                // For some reason UIType's change event will properly
                // redraw the grid, not BasicUIGrid's.
                ui.RaisePropertyChanged(nameof(ui.UIType));
            }
        }

        public string ToggleButtonText
        {
            get => toggleButtonText;
            set
            {
                if (toggleButtonText != value)
                {
                    toggleButtonText = value;
                    this.RaisePropertyChanged(nameof(ToggleButtonText));
                }
            }
        }

        public ObservableCollection<IObservatoryWorker> Workers
        {
            get { return workers; }
        }

        public ObservableCollection<IObservatoryNotifier> Notifiers
        {
            get { return notifiers; }
        }

        public ObservableCollection<CoreModel> Tabs
        {
            get { return tabs; }
        }

        private static bool CheckUpdate()
        {
            try
            {
                string releasesResponse;

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://api.github.com/repos/xjph/ObservatoryCore/releases"),
                    Headers = { { "User-Agent", "Xjph/ObservatoryCore" } }
                };

                releasesResponse = HttpClient.SendRequest(request).Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(releasesResponse))
                {
                    var releases = System.Text.Json.JsonDocument.Parse(releasesResponse).RootElement.EnumerateArray();

                    foreach (var release in releases)
                    {
                        var tag = release.GetProperty("tag_name").ToString();
                        var verstrings = tag[1..].Split('.');
                        var ver = verstrings.Select(verString => { _ = int.TryParse(verString, out int ver); return ver; }).ToArray();
                        if (ver.Length == 4)
                        {
                            Version version = new(ver[0], ver[1], ver[2], ver[3]);
                            if (version > System.Reflection.Assembly.GetEntryAssembly().GetName().Version)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        private bool UpdateAvailable
        {
            get => _UpdateAvailable;
            set
            {
                this.RaiseAndSetIfChanged(ref _UpdateAvailable, value);
            }
        }

    }
}
