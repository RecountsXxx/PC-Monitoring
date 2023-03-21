using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;
using MessageBox = System.Windows.MessageBox;

namespace ExamMonitoringNet.ViewModels
{
    class StartUpViewModel : ViewModelBase
    {
        #region Buttons variable
        private double _runMonitoringOpacity;
        public double RunMonitoringOpacity
        {
            get { return _runMonitoringOpacity; }
            set { _runMonitoringOpacity = value; OnPropertyChanged(); }
        }
        private double _stopMonitoringOpacity;
        public double StopMonitoringOpacity
        {
            get { return _stopMonitoringOpacity; }
            set { _stopMonitoringOpacity = value; OnPropertyChanged(); }
        }
        private bool _runMonitoringEnabled;
        public bool RunMonitoringEnabled
        {
            get { return _runMonitoringEnabled; }
            set { _runMonitoringEnabled = value; OnPropertyChanged(); }
        }
        private bool _stopMonitoringEnabled;
        public bool StopMonitoringEnabled
        {
            get { return _stopMonitoringEnabled; }
            set { _stopMonitoringEnabled = value; OnPropertyChanged(); }
        }

        public ICommand RunMonitoringCommand { get; set; }
        public ICommand StopMonitoringCommand { get; set; }
        public ICommand OpenSavePathCommand { get; set; }
        #endregion

        private static string temping = string.Empty;

        #region DllImport
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static LowLevelKeyboardProc _proc = HookCallback;

        private static IntPtr _hookID = IntPtr.Zero;


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        [DllImport("user32.dll")]
        private static extern UInt32 GetWindowThreadProcessId(Int32 hWnd, out Int32 lpdwProcessId);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion

        #region Hook keyboard
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
           
            //InputLanguage currentLang = InputLanguage.CurrentInputLanguage;
            //CultureInfo currentCulture = currentLang.Culture;
            //string currentLangId = currentCulture.Name;
            //Console.WriteLine(currentLangId);

            while (true)
            {
                if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
                {
                    int vkCode = Marshal.ReadInt32(lParam);

                    string str = string.Empty;
                    if (Enum.IsDefined(typeof(KeysUS), vkCode))
                    {
                        str += Convert.ToString((KeysUS)vkCode);
                        temping += str;
                        if(str == "Space")
                        {
                            temping = string.Empty;
                        }
                        str = str.Replace("Space", " ");
     
                    }
                    if (MainViewModel.EnableStatistics == true)
                    {
                        if (MainViewModel.PathSave != null)
                        {

                            File.AppendAllText(MainViewModel.PathSave + "/RecordKeyboard.txt", str);
                        }
                    }

                    if (MainViewModel.EnableModeration == true)
                    {
                        if (MainViewModel.PathForbiddenWords != null)
                        {
                            foreach (var item in MainViewModel.forbiddenWords)
                            {
                                string[] strSplit = temping.Split(' ');
                                for (int i = 0; i < strSplit.Length; i++)
                                {
                                    if (strSplit[i] == item)
                                    {
                                        MessageBox.Show("You enter forbidden word: " + "'" + item +"'", "Erorr", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                    }
                                }
                            }
                        }
                    }

                    MainViewModel.listInputKeywords.Add(str);
                }
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }
        }
        static void RecordForegroundKeyboard()
        {
            _hookID = SetHook(_proc);
            System.Windows.Forms.Application.Run();
            UnhookWindowsHookEx(_hookID);
        }
        #endregion

        #region Get application
        static void RecordForegroundApplication()
        {
            if (File.Exists(MainViewModel.PathSave + "/LaunchedApps.txt"))
                File.Delete(MainViewModel.PathSave + "/LaunchedApps.txt");

            while (true)
            {
                IntPtr hwnd = GetForegroundWindow();
                StringBuilder sb = new StringBuilder(256);
                GetWindowText(hwnd, sb, 256);

                string applicationName = sb.ToString();
                if(MainViewModel.EnableModeration == true)
                {
                    if (MainViewModel.PathForbiddenPrograms != null)
                    {
                        foreach (var item in MainViewModel.forbiddenApplication)
                        {
                            if (item == applicationName)
                            {
                                IntPtr ptr = FindWindow(null, item);
                                Int32 handle = ptr.ToInt32();
                                Int32 ProcessID;
                                GetWindowThreadProcessId(handle, out ProcessID);
                                MessageBox.Show("You can't come here", "Erorr", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                Process ActiveProcess = Process.GetProcessById(ProcessID);
                                ActiveProcess.Kill();
                            }
                        }
                    }
                }
                string str = $"Application: '{applicationName}' at {DateTime.Now}{Environment.NewLine}";

                if (MainViewModel.EnableStatistics == true)
                {
                    if (MainViewModel.PathSave != null)
                    {
                        File.AppendAllText(MainViewModel.PathSave + "/LaunchedApps.txt", str);
                    }
                }
                MainViewModel.listRunningAplication.Add(str);


                Thread.Sleep(1000);
            }


        }
        #endregion

        public StartUpViewModel()
        {
            RunMonitoringCommand = new RelayCommand(RunMonitoringCommandEvent);
            StopMonitoringCommand = new RelayCommand(StopMonitoringCommandEvent);
            OpenSavePathCommand = new RelayCommand(OpenSavePathComamandEvent);

            StopMonitoringEnabled = false;
            StopMonitoringOpacity = 0.5;
            RunMonitoringEnabled = true;
            RunMonitoringOpacity = 1;

            if (MainViewModel.IsMonitoring == true)
            {
                StopMonitoringEnabled = true;
                StopMonitoringOpacity = 1;
                RunMonitoringEnabled = false;
                RunMonitoringOpacity = 0.5;
            }
            else
            {
                StopMonitoringEnabled = false;
                StopMonitoringOpacity = 0.5;
                RunMonitoringEnabled = true;
                RunMonitoringOpacity = 1;
            }
        }

        #region Buttons
        private void RunMonitoringCommandEvent(object obj)
        {
            bool checkPathes = true;

            if (File.Exists(MainViewModel.PathSave + "/RecordKeyboard.txt"))
                File.Delete(MainViewModel.PathSave + "/RecordKeyboard.txt");

            StopMonitoringEnabled = true;
            StopMonitoringOpacity = 1;
            RunMonitoringEnabled = false;
            RunMonitoringOpacity = 0.5;

            MainViewModel.listInputKeywords =new List<string>();
            MainViewModel.listRunningAplication = new List<string>();
            
            MainViewModel.IsMonitoring = true;

            if (MainViewModel.EnableStatistics == true)
            {
                if (MainViewModel.PathSave == null)
                {
                        MessageBox.Show("You turned on statistics and did not set save path", "Erorr", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    checkPathes = false;
                }
            }
             if (MainViewModel.EnableModeration == true)
            {
                if (MainViewModel.PathForbiddenPrograms == null)
                {
                    MessageBox.Show("You turned moderation and did not set input path forbidden programs", "Erorr", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    checkPathes = false;
                }
                 if (MainViewModel.PathForbiddenWords == null)
                {
                    MessageBox.Show("You turned moderation and did not set input path forbidden words", "Erorr", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    checkPathes = false;
                }
            }
            if (checkPathes == false)
            MessageBox.Show("Applications will monitor, but will not write information to the file or also read from the file", "Erorr", MessageBoxButton.OK, MessageBoxImage.Information);


            MainViewModel.foregroundWindowThread = new Thread(RecordForegroundApplication);
            MainViewModel.foregroundWindowThread.Start();

            MainViewModel.backgroundKeyboardThread = new Thread(RecordForegroundKeyboard);
            MainViewModel.backgroundKeyboardThread.Start();


        }
        private void StopMonitoringCommandEvent(object obj)
        {
            RunMonitoringEnabled = true;
            RunMonitoringOpacity = 1;
            StopMonitoringEnabled = false;
            StopMonitoringOpacity = 0.5;

            MainViewModel.IsMonitoring = false;

            if(MainViewModel.foregroundWindowThread != null)
            MainViewModel.foregroundWindowThread.Abort();
            if (MainViewModel.backgroundKeyboardThread != null)
                MainViewModel.backgroundKeyboardThread.Abort();
        }
        private void OpenSavePathComamandEvent(object obj)
        {
            if (MainViewModel.PathSave != null)
                Process.Start(MainViewModel.PathSave);
            else
            {
                MessageBox.Show("Selected path is empty", "Erorr", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        #endregion
    }

    enum KeysRU
    {
        D0 = 48,
        D1 = 49,
        D2 = 50,
        D3 = 51,
        D4 = 52,
        D5 = 53,
        D6 = 54,
        D7 = 55,
        D8 = 56,
        D9 = 57,
        А = 70,
        Б = 188,
        В = 68,
        Г = 85,
        Д = 76,
        Е = 84,
        Ё = 192,
        Ж = 186,
        З = 80,
        И = 66,
        Й = 81,
        К = 82,
        Л = 75,
        М = 86,
        Н = 89,
        О = 74,
        П = 71,
        Р = 72,
        С = 67,
        Т = 78,
        У = 69,
        Ф = 65,
        Ч = 88,
        Х = 219,
        Ц = 87,
        Ю = 190,
        Я = 90,
        Space  = 32,
        Capital = 20,
        LShiftKey = 160,
        RShiftKey = 161,

    }
    enum KeysUS
    {
        D0 = 48,
        D1 = 49,
        D2 = 50,
        D3 = 51,
        D4 = 52,
        D5 = 53,
        D6 = 54,
        D7 = 55,
        D8 = 56,
        D9 = 57,
        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90,
        Space = 32
    }
}


