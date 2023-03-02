using System.Diagnostics;

namespace Observatory.Util;

public class FontConfigUtil
{
    /// <summary>
    /// Runs 'fc-list' to get a list of installed fonts
    /// </summary>
    /// <returns>A list of installed fonts</returns>
    public static IEnumerable<string> GetInstalledFontNames()
    {
        // Launch process
        var process = new Process();
        process.StartInfo.FileName = "fc-list";
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();
        
        // Read output
        using var s = process.StandardOutput;
        var data = s.ReadToEnd();
        process.WaitForExit(1000);
        
        // Example output:
        // /usr/share/fonts/gsfonts/URWBookman-DemiItalic.otf: URW Bookman:style=Demi Italic
        // /usr/share/fonts/noto/NotoSansMono-Light.ttf: Noto Sans Mono,Noto Sans Mono Light:style=Light,Regular
        // /usr/share/fonts/TTF/DejaVuSerifCondensed.ttf: DejaVu Serif,DejaVu Serif Condensed:style=Condensed,Book
        // /usr/share/fonts/liberation/LiberationSerif-Italic.ttf: Liberation Serif:style=Italic

        // Parse output and return
        return data.Split('\n')
            .Where(i => i.Trim().Length > 0)
            .Select(i => i.Split(':', 2)[1].Trim().Split(':')[0])
            .ToArray();
    }
}