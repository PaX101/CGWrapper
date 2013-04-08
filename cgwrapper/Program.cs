using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace cgwrapper
{
    class Program
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SystemParametersInfo(uint uAction, uint uParam, ref bool lpvParam, int fWinIni);

        [DllImport("user32", SetLastError = true)]
        private static extern IntPtr OpenDesktop(string lpszDesktop, uint dwFlags, bool fInherit, uint dwDesiredAccess);

        [DllImport("user32", SetLastError = true)]
        private static extern IntPtr OpenInputDesktop(uint dwFlags, bool fInherit, uint dwDesiredAccess);

        [DllImport("user32", SetLastError = true)]
        private static extern IntPtr CloseDesktop(IntPtr hDesktop);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SwitchDesktop(IntPtr hDesktop);

        public static bool IsWorkstationLocked()
        {
            const int DESKTOP_SWITCHDESKTOP = 256;
            IntPtr hwnd = OpenInputDesktop(0, false, DESKTOP_SWITCHDESKTOP);

            if (hwnd == IntPtr.Zero)
            {
                hwnd = OpenDesktop("Default", 0, false, DESKTOP_SWITCHDESKTOP);
            }

            if (hwnd != IntPtr.Zero)
            {
                if (SwitchDesktop(hwnd))
                {
                    CloseDesktop(hwnd);
                }
                else
                {
                    CloseDesktop(hwnd);
                    return true;
                }
            }
            return false;
        }

        public static bool IsScreensaverRunning()
        {
            const int SPI_GETSCREENSAVERRUNNING = 114;
            bool isRunning = false;

            if (!SystemParametersInfo(SPI_GETSCREENSAVERRUNNING, 0, ref isRunning, 0))
            {
                return false;
            }

            if (isRunning)
            {
                return true;
            }

            return false;
        }

        static void Main(string[] args)
        {
            bool loop = false;

            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                if (IsWorkstationLocked() == true || IsScreensaverRunning() == true && loop == false)
                {

                    loop = true;
                    //Change your exe you want to launch here
                    string command = "C:\\LTC\\cgminer -o stratum+tcp://coinotron.com:3334 -u dxkpax.1 -p 1 --scrypt";
                    System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", "/c " + command);
                    procStartInfo.UseShellExecute = false;
                    procStartInfo.CreateNoWindow = true;
                    procStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    process.StartInfo = procStartInfo;
                    process.Start();
                }
                else
                {
                    Process[] prs = Process.GetProcesses();
                    foreach (Process pr in prs)
                    {
                        //Change Process name here:
                        if (pr.ProcessName == "cgminer")
                        {
                            pr.Kill();
                            loop = false;
                        }
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }
    }
}