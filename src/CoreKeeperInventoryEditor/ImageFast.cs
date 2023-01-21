using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

public class ImageFast
{
    // Code taken from: http://web.archive.org/web/20120707213018/http://weblogs.asp.net/justin_rogers/pages/131704.aspx
    [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    private static extern int GdipLoadImageFromFile(string filename, out IntPtr image);

    [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    private static extern int GdiplusStartup(out IntPtr token, ref StartupInput input, out StartupOutput output);

    [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    private static extern int GdiplusShutdown(IntPtr token);

    [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    private static extern int GdipGetImageType(IntPtr image, out GdipImageTypeEnum type);

    private static readonly IntPtr gdipToken = IntPtr.Zero;

    static ImageFast()
    {
        if (gdipToken == IntPtr.Zero)
        {
            StartupInput input = StartupInput.GetDefaultStartupInput();
            StartupOutput output;

            int status = GdiplusStartup(out gdipToken, ref input, out output);

            if (status == 0)
                AppDomain.CurrentDomain.ProcessExit += new EventHandler(Cleanup_Gdiplus);
        }
    }

    private static void Cleanup_Gdiplus(object sender, EventArgs e)
    {
        if (gdipToken != IntPtr.Zero)
            GdiplusShutdown(gdipToken);
    }

    private static readonly Type bmpType = typeof(System.Drawing.Bitmap);
    private static readonly Type emfType = typeof(System.Drawing.Imaging.Metafile);

    public static Image FromFile(string filename) {
        filename = Path.GetFullPath(filename);

        // We are not using ICM at all, fudge that, this should be FAAAAAST!
        if (GdipLoadImageFromFile(filename, out IntPtr loadingImage) != 0)
        {
            throw new Exception("GDI+ threw a status error code.");
        }

        if (GdipGetImageType(loadingImage, out GdipImageTypeEnum imageType) != 0)
        {
            throw new Exception("GDI+ couldn't get the image type");
        }

        switch (imageType)
        {
            case GdipImageTypeEnum.Bitmap:
                return (Bitmap)bmpType.InvokeMember("FromGDIplus", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { loadingImage });
            case GdipImageTypeEnum.Metafile:
                return (Metafile)emfType.InvokeMember("FromGDIplus", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { loadingImage });
            case GdipImageTypeEnum.Unknown:
                break;
        }

        throw new Exception("Couldn't convert underlying GDI+ object to managed object");
    }

    private ImageFast() { }
}

[StructLayout(LayoutKind.Sequential)]
internal struct StartupInput {
    public int GdiplusVersion;
    public IntPtr DebugEventCallback;
    public bool SuppressBackgroundThread;
    public bool SuppressExternalCodecs;
   
    public static StartupInput GetDefaultStartupInput() {
        StartupInput result = new StartupInput
        {
            GdiplusVersion = 1,
            SuppressBackgroundThread = false,
            SuppressExternalCodecs = false
        };
        return result;
    }
}

[StructLayout(LayoutKind.Sequential)]
internal struct StartupOutput
{
    public IntPtr Hook;
    public IntPtr Unhook;
}

internal enum GdipImageTypeEnum {
    Unknown = 0,
    Bitmap = 1,
    Metafile = 2
}