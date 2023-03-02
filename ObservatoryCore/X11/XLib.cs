using System.Runtime.InteropServices;

namespace Observatory.X11;

public class XLib
{
    public const int ShapeInput = 2;

    [DllImport("libXfixes.so")]
    // XserverRegion XFixesCreateRegion (Display *dpy, XRectangle *rectangles, int nrectangles)
    public static extern IntPtr XFixesCreateRegion(IntPtr dpy, IntPtr rectangles, int nrectangles);

    [DllImport("libXfixes.so")]
    // void XFixesSetWindowShapeRegion (Display *dpy, Window win, int shape_kind, int x_off, int y_off, XserverRegion region)
    public static extern IntPtr XFixesSetWindowShapeRegion(IntPtr dpy, IntPtr win, int shape_kind, int x_off, int y_off, IntPtr region);

    [DllImport("libXfixes.so")]
    // void XFixesDestroyRegion (Display *dpy, XserverRegion region)
    public static extern IntPtr XFixesDestroyRegion(IntPtr dpy, IntPtr region);
}