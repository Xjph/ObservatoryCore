// Source: https://stackoverflow.com/questions/51578104/how-to-create-a-semi-transparent-or-blurred-backcolor-in-a-windows-form

using System.Runtime.InteropServices;
using System.Security;

[SuppressUnmanagedCodeSecurity]
public class DwmHelper
{
    public const int WM_DWMCOMPOSITIONCHANGED = 0x031E;

    public struct MARGINS
    {
        public int leftWidth;
        public int rightWidth;
        public int topHeight;
        public int bottomHeight;

        public MARGINS(int LeftWidth, int RightWidth, int TopHeight, int BottomHeight)
        {
            leftWidth = LeftWidth;
            rightWidth = RightWidth;
            topHeight = TopHeight;
            bottomHeight = BottomHeight;
        }

        public void NoMargins()
        {
            leftWidth = 0;
            rightWidth = 0;
            topHeight = 0;
            bottomHeight = 0;
        }

        public void SheetOfGlass()
        {
            leftWidth = -1;
            rightWidth = -1;
            topHeight = -1;
            bottomHeight = -1;
        }
    }

    [Flags]
    public enum DWM_BB
    {
        Enable = 1,
        BlurRegion = 2,
        TransitionOnMaximized = 4
    }

    // https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/ne-dwmapi-dwmwindowattribute
    public enum DWMWINDOWATTRIBUTE : uint
    {
        NCRenderingEnabled = 1,       //Get atttribute
        NCRenderingPolicy,            //Enable or disable non-client rendering
        TransitionsForceDisabled,
        AllowNCPaint,
        CaptionButtonBounds,          //Get atttribute
        NonClientRtlLayout,
        ForceIconicRepresentation,
        Flip3DPolicy,
        ExtendedFrameBounds,          //Get atttribute
        HasIconicBitmap,
        DisallowPeek,
        ExcludedFromPeek,
        Cloak,
        Cloaked,                      //Get atttribute. Returns a DWMCLOACKEDREASON
        FreezeRepresentation,
        PassiveUpdateMode,
        UseHostBackDropBrush,
        AccentPolicy = 19,            // Win 10 (undocumented)
        ImmersiveDarkMode = 20,       // Win 11 22000
        WindowCornerPreference = 33,  // Win 11 22000
        BorderColor,                  // Win 11 22000
        CaptionColor,                 // Win 11 22000
        TextColor,                    // Win 11 22000
        VisibleFrameBorderThickness,  // Win 11 22000
        SystemBackdropType            // Win 11 22621
    }

    public enum DWMCLOACKEDREASON : uint
    {
        DWM_CLOAKED_APP = 0x0000001,       //cloaked by its owner application.
        DWM_CLOAKED_SHELL = 0x0000002,     //cloaked by the Shell.
        DWM_CLOAKED_INHERITED = 0x0000004  //inherited from its owner window.
    }

    public enum DWMNCRENDERINGPOLICY : uint
    {
        UseWindowStyle, // Enable/disable non-client rendering based on window style
        Disabled,       // Disabled non-client rendering; window style is ignored
        Enabled,        // Enabled non-client rendering; window style is ignored
    };

    public enum DWMACCENTSTATE
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_INVALID_STATE = 4
    }

    [Flags]
    public enum CompositionAction : uint
    {
        DWM_EC_DISABLECOMPOSITION = 0,
        DWM_EC_ENABLECOMPOSITION = 1
    }

    // Values designating how Flip3D treats a given window.
    enum DWMFLIP3DWINDOWPOLICY : uint
    {
        Default,        // Hide or include the window in Flip3D based on window style and visibility.
        ExcludeBelow,   // Display the window under Flip3D and disabled.
        ExcludeAbove,   // Display the window above Flip3D and enabled.
    };

    public enum ThumbProperties_dwFlags : uint
    {
        RectDestination = 0x00000001,
        RectSource = 0x00000002,
        Opacity = 0x00000004,
        Visible = 0x00000008,
        SourceClientAreaOnly = 0x00000010
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct AccentPolicy
    {
        public DWMACCENTSTATE AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;

        public AccentPolicy(DWMACCENTSTATE accentState, int accentFlags, int gradientColor, int animationId)
        {
            AccentState = accentState;
            AccentFlags = accentFlags;
            GradientColor = gradientColor;
            AnimationId = animationId;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DWM_BLURBEHIND
    {
        public DWM_BB dwFlags;
        public int fEnable;
        public IntPtr hRgnBlur;
        public int fTransitionOnMaximized;

        public DWM_BLURBEHIND(bool enabled)
        {
            dwFlags = DWM_BB.Enable;
            fEnable = (enabled) ? 1 : 0;
            hRgnBlur = IntPtr.Zero;
            fTransitionOnMaximized = 0;
        }

        public Region Region => Region.FromHrgn(hRgnBlur);

        public bool TransitionOnMaximized
        {
            get => fTransitionOnMaximized > 0;
            set
            {
                fTransitionOnMaximized = (value) ? 1 : 0;
                dwFlags |= DWM_BB.TransitionOnMaximized;
            }
        }

        public void SetRegion(Graphics graphics, Region region)
        {
            hRgnBlur = region.GetHrgn(graphics);
            dwFlags |= DWM_BB.BlurRegion;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WinCompositionAttrData
    {
        public DWMWINDOWATTRIBUTE Attribute;
        public IntPtr Data;  //Will point to an AccentPolicy struct, where Attribute will be DWMWINDOWATTRIBUTE.AccentPolicy
        public int SizeOfData;

        public WinCompositionAttrData(DWMWINDOWATTRIBUTE attribute, IntPtr data, int sizeOfData)
        {
            Attribute = attribute;
            Data = data;
            SizeOfData = sizeOfData;
        }
    }

    private static int GetBlurBehindPolicyAccentFlags()
    {
        int drawLeftBorder = 20;
        int drawTopBorder = 40;
        int drawRightBorder = 80;
        int drawBottomBorder = 100;
        return (drawLeftBorder | drawTopBorder | drawRightBorder | drawBottomBorder);
    }

    //https://msdn.microsoft.com/en-us/library/windows/desktop/aa969508(v=vs.85).aspx
    [DllImport("dwmapi.dll")]
    internal static extern int DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);

    [DllImport("dwmapi.dll", PreserveSig = false)]
    public static extern void DwmEnableComposition(CompositionAction uCompositionAction);

    //https://msdn.microsoft.com/it-it/library/windows/desktop/aa969512(v=vs.85).aspx
    [DllImport("dwmapi.dll")]
    internal static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

    //https://msdn.microsoft.com/en-us/library/windows/desktop/aa969515(v=vs.85).aspx
    [DllImport("dwmapi.dll")]
    internal static extern int DwmGetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref int attrValue, int attrSize);

    //https://msdn.microsoft.com/en-us/library/windows/desktop/aa969524(v=vs.85).aspx
    [DllImport("dwmapi.dll")]
    internal static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref int attrValue, int attrSize);

    [DllImport("User32.dll", SetLastError = true)]
    internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WinCompositionAttrData data);

    [DllImport("dwmapi.dll")]
    internal static extern int DwmIsCompositionEnabled(ref int pfEnabled);

    public static bool IsCompositionEnabled()
    {
        int pfEnabled = 0;
        int result = DwmIsCompositionEnabled(ref pfEnabled);
        return (pfEnabled == 1) ? true : false;
    }

    public static bool IsNonClientRenderingEnabled(IntPtr hWnd)
    {
        int gwaEnabled = 0;
        int result = DwmGetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.NCRenderingEnabled, ref gwaEnabled, sizeof(int));
        return gwaEnabled == 1;
    }

    public static bool WindowSetAttribute(IntPtr hWnd, DWMWINDOWATTRIBUTE attribute, int attributeValue)
    {
        int result = DwmSetWindowAttribute(hWnd, attribute, ref attributeValue, sizeof(int));
        return (result == 0);
    }

    public static void Windows10EnableBlurBehind(IntPtr hWnd)
    {
        DWMNCRENDERINGPOLICY policy = DWMNCRENDERINGPOLICY.Enabled;
        WindowSetAttribute(hWnd, DWMWINDOWATTRIBUTE.NCRenderingPolicy, (int)policy);

        AccentPolicy accPolicy = new AccentPolicy()
        {
            AccentState = DWMACCENTSTATE.ACCENT_ENABLE_BLURBEHIND,
        };

        int accentSize = Marshal.SizeOf(accPolicy);
        IntPtr accentPtr = Marshal.AllocHGlobal(accentSize);
        Marshal.StructureToPtr(accPolicy, accentPtr, false);
        var data = new WinCompositionAttrData(DWMWINDOWATTRIBUTE.AccentPolicy, accentPtr, accentSize);

        SetWindowCompositionAttribute(hWnd, ref data);
        Marshal.FreeHGlobal(accentPtr);
    }

    public static bool WindowEnableBlurBehind(IntPtr hWnd)
    {
        DWMNCRENDERINGPOLICY policy = DWMNCRENDERINGPOLICY.Enabled;
        WindowSetAttribute(hWnd, DWMWINDOWATTRIBUTE.NCRenderingPolicy, (int)policy);

        DWM_BLURBEHIND dwm_BB = new DWM_BLURBEHIND(true);
        int result = DwmEnableBlurBehindWindow(hWnd, ref dwm_BB);
        return result == 0;
    }

    public static bool WindowExtendIntoClientArea(IntPtr hWnd, MARGINS margins)
    {
        // Extend frame on the bottom of client area
        int result = DwmExtendFrameIntoClientArea(hWnd, ref margins);
        return result == 0;
    }

    public static bool WindowBorderlessDropShadow(IntPtr hWnd, int shadowSize)
    {
        MARGINS margins = new MARGINS(0, shadowSize, 0, shadowSize);
        int result = DwmExtendFrameIntoClientArea(hWnd, ref margins);
        return result == 0;
    }

    public static bool WindowSheetOfGlass(IntPtr hWnd)
    {
        MARGINS margins = new MARGINS();

        //Margins set to All:-1 - Sheet Of Glass effect
        margins.SheetOfGlass();
        int result = DwmExtendFrameIntoClientArea(hWnd, ref margins);
        return result == 0;
    }

    public static bool WindowDisableRendering(IntPtr hWnd)
    {
        int ncrp = (int)DWMNCRENDERINGPOLICY.Disabled;
        // Disable non-client area rendering on the window.
        int result = DwmSetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.NCRenderingPolicy, ref ncrp, sizeof(int));
        return result == 0;
    }
}