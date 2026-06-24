using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PatientBridge.Core;

public class KeyboardHook : IDisposable
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;

    private readonly LowLevelKeyboardProc _proc;
    private readonly nint _hookId;
    private readonly System.Text.StringBuilder _buffer = new();
    private readonly Action<string> _onCardScanned;

    public KeyboardHook(Action<string> onCardScanned)
    {
        _onCardScanned = onCardScanned;
        _proc = HookCallback;
        _hookId = SetHook(_proc);
    }

    private nint SetHook(LowLevelKeyboardProc proc)
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule!;
        return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
            GetModuleHandle(curModule.ModuleName), 0);
    }

    private nint HookCallback(int nCode, nint wParam, nint lParam)
    {
        if (nCode >= 0 && wParam == WM_KEYDOWN)
        {
            var vkCode = Marshal.ReadInt32(lParam);
            var key = (Keys)vkCode;

            if (key == Keys.Return)
            {
                var scanned = _buffer.ToString();
                _buffer.Clear();
                if (!string.IsNullOrWhiteSpace(scanned))
                    _onCardScanned(scanned);
            }
            else
            {
                var ch = GetCharFromKey(key);
                if (ch != '\0')
                    _buffer.Append(ch);
            }
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private static char GetCharFromKey(Keys key)
    {
        if (key >= Keys.A && key <= Keys.Z)
            return (char)('A' + (key - Keys.A));
        if (key >= Keys.D0 && key <= Keys.D9)
            return (char)('0' + (key - Keys.D0));
        return key switch
        {
            Keys.Space => ' ',
            _ => '\0'
        };
    }

    public void Dispose()
    {
        UnhookWindowsHookEx(_hookId);
    }

    private delegate nint LowLevelKeyboardProc(int nCode, nint wParam, nint lParam);

    [DllImport("user32.dll")] static extern nint SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, nint hMod, uint dwThreadId);
    [DllImport("user32.dll")] static extern bool UnhookWindowsHookEx(nint hhk);
    [DllImport("user32.dll")] static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);
    [DllImport("kernel32.dll")] static extern nint GetModuleHandle(string lpModuleName);
}
