using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PatientBridge.Input;

/// <summary>
/// Reads a USB-HID card reader that emulates a keyboard (e.g. CRF-200U). A global
/// low-level keyboard hook buffers typed characters and raises <see cref="CardScanned"/>
/// when Enter is received. This is the only class that knows about the keyboard;
/// swapping to another reader means providing a different <see cref="ICardReader"/>.
/// </summary>
public class KeyboardCardReader : ICardReader
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;

    private readonly LowLevelKeyboardProc _proc;
    private readonly System.Text.StringBuilder _buffer = new();
    private nint _hookId;

    public event Action<string>? CardScanned;

    public KeyboardCardReader()
    {
        // Keep the delegate alive for the lifetime of the reader so it is not GC'd.
        _proc = HookCallback;
    }

    public void Start()
    {
        if (_hookId == 0)
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
                    CardScanned?.Invoke(scanned);
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
        if (_hookId != 0)
        {
            UnhookWindowsHookEx(_hookId);
            _hookId = 0;
        }
    }

    private delegate nint LowLevelKeyboardProc(int nCode, nint wParam, nint lParam);

    [DllImport("user32.dll")] static extern nint SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, nint hMod, uint dwThreadId);
    [DllImport("user32.dll")] static extern bool UnhookWindowsHookEx(nint hhk);
    [DllImport("user32.dll")] static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);
    [DllImport("kernel32.dll")] static extern nint GetModuleHandle(string lpModuleName);
}
