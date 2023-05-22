using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WK.Libraries.SharpClipboardNS;
using System.Diagnostics;

namespace WindowsFormsApp14
{
    public partial class Form1 : Form
    {
        private static List<string> clipboardDataList = new List<string>();
        private static int x = 0;
        private static bool ckeck = true;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static IntPtr _hookID = IntPtr.Zero;
        private static LowLevelKeyboardProc _proc = HookCallback;
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (vkCode == (int)Keys.V && Control.ModifierKeys == Keys.Control)
                {
                    ckeck = false;
                    Clipboard.SetText(clipboardDataList[x].ToString());
                    ckeck= true;
                    if(x<(clipboardDataList.Count-1))
                    {
                        x++;
                    }
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }


        public Form1()
        {
            DateTime dateTime = DateTime.Now;
            DateTime dateTime1 = DateTime.Parse("2023-06-2");

            if(dateTime>dateTime1)
            {
                MessageBox.Show("time_out");
                this.Close();
                return;
            }
            ckeck = false;
            Clipboard.Clear();
            x = 0;
            ckeck= true;
            InitializeComponent();
            _hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
            SharpClipboard clipboard = new SharpClipboard();
            clipboard.ClipboardChanged -= OnClipboardContentChanged;
            clipboard.ClipboardChanged += OnClipboardContentChanged;
        }


        private void label_change(string text)
        {
            this.label2.Text = text;
        }

        private void OnClipboardContentChanged(object sender, SharpClipboard.ClipboardChangedEventArgs e)
        {
            if (ckeck)
            {
                if (e.ContentType == SharpClipboard.ContentTypes.Text)
                {
                    string text = e.Content.ToString();
                    if (clipboardDataList.Count>0 && text == clipboardDataList[clipboardDataList.Count-1])
                    {
                        return;
                    }
                    else
                    {
                        clipboardDataList.Add(text);
                        label_change(clipboardDataList.Count.ToString());
                    }
                }
            }
        }
    }
}