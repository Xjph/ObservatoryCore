// https://stackoverflow.com/a/4616637

using System.Runtime.InteropServices;

namespace Observatory.UI
{
    internal static class SystemMenu
    {
        // P/Invoke constants
        private const int WM_SYSCOMMAND = 0x112;
        private const int MF_STRING = 0x0;
        private const int MF_SEPARATOR = 0x800;
        private const int MF_BYCOMMAND = 0x0;
        private const int MF_CHECKED = 0x8;

        // P/Invoke declarations
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ModifyMenu(IntPtr hMenu, int uPosition, int uFlags, int uIDNewItem, string lpNewItem);

        // ID for the About item on the system menu
        private const int SYSMENU_ONTOP_ID = 0x070C;

        internal static void AddAlwaysOnTop(Form form)
        {
            // Get a handle to a copy of this form's system (window) menu
            IntPtr hSysMenu = GetSystemMenu(form.Handle, false);

            // Add a separator
            AppendMenu(hSysMenu, MF_SEPARATOR, 0, string.Empty);

            // Add the About menu item
            AppendMenu(hSysMenu, MF_STRING, SYSMENU_ONTOP_ID, "&Always On Top");
        }

        internal static void WndProcHandler(Form form, ref Message m)
        {
            if (m.Msg == WM_SYSCOMMAND && m.WParam == SYSMENU_ONTOP_ID)
            {
                form.TopMost = !form.TopMost;
                var flags = MF_BYCOMMAND | (form.TopMost ? MF_CHECKED : 0);
                ModifyMenu(GetSystemMenu(form.Handle, false), SYSMENU_ONTOP_ID, flags, SYSMENU_ONTOP_ID, "&Always On Top");
            }
        }
    }
}
