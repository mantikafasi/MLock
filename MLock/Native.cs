using System.Runtime.InteropServices;
using System.Text;

namespace MLock
{
    public static class Native
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetVolumeInformation(string letter, StringBuilder name, uint nameSize,
            out uint serialNumber, uint serialNumberLength, uint flags, StringBuilder systemName, uint systemNameSize);

        [DllImport("libs/vlock.dll")]
        public static extern bool InstallKHook();

        [DllImport("libs/vlock.dll")]
        public static extern void SetKHookConfig(KHConfig config);

        [DllImport("libs/vlock.dll")]
        public static extern void UninstallKHook();

        [DllImport("libs/vlock.dll")]
        public static extern bool DisableTskMan();

        [DllImport("libs/vlock.dll")]
        public static extern bool RestoreTskMan();

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct KHConfig
        {
            [MarshalAs(UnmanagedType.U1)] public bool ClearCountsAsFail;
            [MarshalAs(UnmanagedType.U1)] public bool RequireEnter;
            [MarshalAs(UnmanagedType.I4)] public int PwLength;
            [MarshalAs(UnmanagedType.LPWStr)] public string Pw;
            [MarshalAs(UnmanagedType.FunctionPtr)] public OnFailDelegate OnFail;
            [MarshalAs(UnmanagedType.FunctionPtr)] public OnSuccessDelegate OnSuccess;
            [MarshalAs(UnmanagedType.FunctionPtr)] public OnInputDelegate OnInput;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
            public delegate void OnFailDelegate([MarshalAs(UnmanagedType.LPWStr)] string pw);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
            public delegate void OnSuccessDelegate();

            [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
            public delegate void OnInputDelegate(char[] inp, int size,
                [MarshalAs(UnmanagedType.LPWStr)] string fullInp);
        }
    }
}