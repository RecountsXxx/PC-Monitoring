﻿using System.Diagnostics;
using System.Runtime.InteropServices;

namespace d
{
    internal class Program
    {
        // Константы для хуков клавиатуры
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        // Делегат для процедуры обработки хука клавиатуры
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        // Объявляем экземпляр делегата
        private static LowLevelKeyboardProc _proc = HookCallback;

        // Объявляем переменную для хранения хука
        private static IntPtr _hookID = IntPtr.Zero;

        public static void Main(string[] args)
        {
            // Устанавливаем хук клавиатуры
            _hookID = SetHook(_proc);

            // Запускаем цикл обработки сообщений
            while (true)
            {
                System.Net.Mime.MediaTypeNames.Application.DoEvents();
                // Добавьте здесь код, который будет выполняться в цикле
            }

            // Отменяем хук клавиатуры перед выходом
            UnhookWindowsHookEx(_hookID);

        }

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
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                // Обработка нажатия клавиши
                Console.WriteLine(vkCode);
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        // Импортируем функции из библиотеки User32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}