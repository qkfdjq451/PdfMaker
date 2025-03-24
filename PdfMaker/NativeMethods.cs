using System;
using System.Runtime.InteropServices;

public static class NativeMethods
{
    // RECT 구조체 선언 (윈도우 위치 및 크기를 담음)
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public const int WM_NCLBUTTONDOWN = 0xA1;
    public const int HTCAPTION = 0x2;

    [DllImport("user32.dll")]
    public static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    // GetWindowRect 함수 선언
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    public static Rectangle GetWindowRectangle(IntPtr hWnd)
    {
        if (GetWindowRect(hWnd, out RECT rect))
        {
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;
            return new Rectangle(rect.Left, rect.Top, width, height);
        }
        else
        {
            throw new Exception("윈도우 정보를 가져올 수 없습니다.");
        }
    }

}