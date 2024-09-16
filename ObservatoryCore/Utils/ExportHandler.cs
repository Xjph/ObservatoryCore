using Observatory.Framework;
using Observatory.Framework.Interfaces;
using System.IO.Compression;
using System.Text;
using System.Xml.Linq;

namespace Observatory.Utils
{
    public static class ExportHandler
    {
        public static void ExportCSV(IObservatoryPlugin plugin)
        {
            // TODO: Allow custom
            string delimiter = "\t";

            string filetype = "csv";
            byte[] fileContent = plugin.ExportContent(delimiter, ref filetype);
            if (fileContent == null)
            {
                if (plugin.PluginUI.PluginUIType == PluginUI.UIType.Basic)
                {
                    StringBuilder exportString = new();
                    Panel pluginUI = (Panel)plugin.PluginUI.UI;
                    ListView pluginGrid = (ListView)pluginUI.Controls[0];

                    foreach (ColumnHeader column in pluginGrid.Columns)
                    {
                        exportString.Append(column.Text + delimiter);
                    }
                    exportString.AppendLine();

                    foreach (ListViewItem row in pluginGrid.Items)
                    {
                        foreach (ListViewItem.ListViewSubItem item in row.SubItems)
                        {
                            exportString.Append(item.Text + delimiter);
                        }
                        exportString.AppendLine();
                    }
                    fileContent = Encoding.UTF8.GetBytes(exportString.ToString());
                }
                else
                {
                    MessageBox.Show(
                        $"Plugin {plugin.Name} does not use a basic data grid and does not provide an ExportContent method.",
                        "Cannot Export",
                        MessageBoxButtons.OK);
                    return;
                }
            }
            SaveFileDialog saveAs = new()
            {
                Title = plugin.Name + " Export",
                Filter = filetype == "csv"
                ? "Tab-separated values (*.csv)|*.csv"
                : $"Plugin-specified file type (*.{filetype})|*.{filetype}",
                DefaultExt = filetype,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = $"Export-{plugin.ShortName}-{DateTime.Now:yyyy-MM-ddTHHmm}.{filetype}"
            };
            var result = saveAs.ShowDialog();
            if (result == DialogResult.OK)
            {
                File.WriteAllBytes(saveAs.FileName, fileContent);
            }
        }

        public static void ExportXlsx(IObservatoryPlugin plugin)
        {
            if (plugin.PluginUI.PluginUIType == PluginUI.UIType.Basic)
            {
                XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
                var sheet = new XElement(ns + "worksheet");
                var sheetData = new XElement(ns + "sheetData");
                Panel pluginUI = (Panel)plugin.PluginUI.UI;
                ListView pluginGrid = (ListView)pluginUI.Controls[0];

                var headerElement = new XElement(ns + "row", new XAttribute("r", "1"));
                char colChar = 'A';
                foreach (ColumnHeader column in pluginGrid.Columns)
                {
                    headerElement.Add(
                        new XElement(ns + "c", 
                        new XAttribute("r", colChar + "1"), 
                        new XAttribute("t", "inlineStr"),
                        new XElement(ns + "is", new XElement(ns + "t", column.Text))));
                    colChar++;
                }
                sheetData.Add(headerElement);

                int rownum = 2;
                
                foreach (ListViewItem row in pluginGrid.Items)
                {
                    colChar = 'A';
                    var rowElement = new XElement(ns + "row", new XAttribute("r", rownum.ToString()));
                    foreach (ListViewItem.ListViewSubItem item in row.SubItems)
                    {
                        rowElement.Add(
                            new XElement(ns + "c",
                            new XAttribute("r", colChar + rownum.ToString()),
                            new XAttribute("t", "inlineStr"),
                            new XElement(ns + "is", new XElement(ns + "t", item.Text))));
                        
                        colChar++;
                    }
                    sheetData.Add(rowElement);
                    rownum++;
                }
                sheet.Add(sheetData);

                SaveFileDialog saveAs = new()
                {
                    Title = plugin.Name + " Export",
                    Filter = "Office Open XML Spreadsheet (*.xlsx)|*.xlsx",
                    DefaultExt = "xlsx",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    FileName = $"Export-{plugin.ShortName}-{DateTime.Now:yyyy-MM-ddTHHmm}.xlsx"
                };
                var result = saveAs.ShowDialog();
                if (result == DialogResult.OK)
                {
                    var tempPath = Path.GetTempPath() + Path.DirectorySeparatorChar + Guid.NewGuid().ToString("D");

                    Directory.CreateDirectory(tempPath);
                    Directory.CreateDirectory(tempPath + Path.DirectorySeparatorChar + "_rels");
                    Directory.CreateDirectory(tempPath + Path.DirectorySeparatorChar + "xl");
                    Directory.CreateDirectory(tempPath + Path.DirectorySeparatorChar + "xl" + Path.DirectorySeparatorChar + "_rels");
                    Directory.CreateDirectory(tempPath + Path.DirectorySeparatorChar + "xl" + Path.DirectorySeparatorChar + "worksheets");

                    File.WriteAllText(tempPath + Path.DirectorySeparatorChar + "[Content_Types].xml", contentTypes);
                    File.WriteAllText(tempPath + Path.DirectorySeparatorChar + "_rels" + Path.DirectorySeparatorChar + ".rels", rels);
                    File.WriteAllText(tempPath + Path.DirectorySeparatorChar + "xl" + Path.DirectorySeparatorChar + "workbook.xml", workbook(plugin.ShortName + " Export"));
                    File.WriteAllText(tempPath + Path.DirectorySeparatorChar + "xl" + Path.DirectorySeparatorChar + "_rels" + Path.DirectorySeparatorChar + "workbook.xml.rels", workbookRels);
                    File.WriteAllText(tempPath + Path.DirectorySeparatorChar + "xl" + Path.DirectorySeparatorChar + "worksheets" + Path.DirectorySeparatorChar + "sheet1.xml", sheet.ToString());

                    ZipFile.CreateFromDirectory(tempPath, saveAs.FileName);

                    Directory.Delete(tempPath, true);
                }
            }
        }

        private const string rels = "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/></Relationships>";
        private const string contentTypes = "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\"><Default Extension=\"xml\" ContentType=\"application/xml\"/><Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/><Override PartName=\"/_rels/.rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/><Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/><Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/></Types>";
        private static string workbook(string sheetName) => $"<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"><sheets><sheet name=\"{sheetName}\" sheetId=\"1\" r:id=\"rId1\"/></sheets></workbook>";
        private const string workbookRels = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/></Relationships>";
    }
}
