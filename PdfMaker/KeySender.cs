using System;
using System.Runtime.InteropServices;

public class KeySender
{
    // keybd_event 플래그
    private const int KEYEVENTF_EXTENDEDKEY = 0x1;
    private const int KEYEVENTF_KEYUP = 0x2;

    // 가상 키 코드 (Shift, F1)
    private const byte VK_SHIFT = 0x10;
    private const byte VK_F1 = 0x70;
    private const byte VK_RIGHT = 0x27;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    public static void SendRightArrow()
    {
        // 오른쪽 방향키 누르기
        keybd_event(VK_RIGHT, 0, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
        // 오른쪽 방향키 떼기
        keybd_event(VK_RIGHT, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
    }
}