using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Start9.Api.Tools
{
    class SystemPowerTools
    {
        [DllImport("user32")]
        static extern void LockWorkStation();

        [DllImport("user32")]
        static extern bool ExitWindowsEx(uint Flag, uint Reason);

        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        public static void LockUserAccount()
        {
            LockWorkStation();
        }

        public static void SignOutUserAccount()
        {
            ExitWindowsEx(0, 0);
        }

        public static void SleepSystem()
        {
            SetSuspendState(false, true, true);
        }

        public static void ShutDownSystem()
        {
            Process.Start("shutdown", "/s /t 0");
        }

        public static void RestartSystem()
        {
            Process.Start("shutdown", "/r /t 0");
        }
    }
}
