using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AV_Nosotras
{
   
        public class ClassAutoMBX
        {
            System.Threading.Timer _timeoutTimer;
            string _caption;
            static WindowCustomMessage win;
            ClassAutoMBX(string text, string caption, int timeout)
            {
                _caption = caption;
                _timeoutTimer = new System.Threading.Timer(OnTimerElapsed, null, timeout, System
                .Threading.Timeout.Infinite);
                win = new WindowCustomMessage();
                win.showMessage(text, caption);

                // System.Windows.MessageBox.Show(text, caption);
            }

            public static void Show(string text, string caption, int timeout)
            {
                new ClassAutoMBX(text, caption, timeout);
            }

            void OnTimerElapsed(object state)
            {
                //IntPtr mbWnd = FindWindow(null, _caption);
                //if (mbWnd != IntPtr.Zero)
                //	{ SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero); }
                win.closeWindow();
                _timeoutTimer.Dispose();
            }

            const int WM_CLOSE = 0x0010;
            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]

            static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
            [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime
            .InteropServices.CharSet.Auto)]

            static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

            
            
        }
    }

