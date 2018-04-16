//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Ntreev.Crema.ApplicationHost
{
    static class FlashWindowUtility
    {
        private const UInt32 FLASHW_STOP = 0; //Stop flashing. The system restores the window to its original state.        private const UInt32 FLASHW_CAPTION = 1; //Flash the window caption.        
        private const UInt32 FLASHW_TRAY = 2; //Flash the taskbar button.        
        private const UInt32 FLASHW_ALL = 3; //Flash both the window caption and taskbar button.        
        private const UInt32 FLASHW_TIMER = 4; //Flash continuously, until the FLASHW_STOP flag is set.        
        private const UInt32 FLASHW_TIMERNOFG = 12; //Flash continuously until the window comes to the foreground.  

        [StructLayout(LayoutKind.Sequential)]
        private struct FLASHWINFO
        {
            public UInt32 cbSize; //The size of the structure in bytes.            
            public IntPtr hwnd; //A Handle to the Window to be Flashed. The window can be either opened or minimized.


            public UInt32 dwFlags; //The Flash Status.            
            public UInt32 uCount; // number of times to flash the window            
            public UInt32 dwTimeout; //The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the default cursor blink rate.        
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        public static void FlashWindow(this Window win, UInt32 count = UInt32.MaxValue)
        {
            //Don't flash if the window is active            
            if (win.IsActive) return;
            WindowInteropHelper h = new WindowInteropHelper(win);
            FLASHWINFO info = new FLASHWINFO
            {
                hwnd = h.Handle,
                dwFlags = FLASHW_ALL | FLASHW_TIMER,
                uCount = count,
                dwTimeout = 0
            };

            info.cbSize = Convert.ToUInt32(Marshal.SizeOf(info));
            FlashWindowEx(ref info);
        }

        public static void StopFlashingWindow(this Window win)
        {
            WindowInteropHelper h = new WindowInteropHelper(win);
            FLASHWINFO info = new FLASHWINFO();
            info.hwnd = h.Handle;
            info.cbSize = Convert.ToUInt32(Marshal.SizeOf(info));
            info.dwFlags = FLASHW_STOP;
            info.uCount = UInt32.MaxValue;
            info.dwTimeout = 0;
            FlashWindowEx(ref info);
        }
    }
}
