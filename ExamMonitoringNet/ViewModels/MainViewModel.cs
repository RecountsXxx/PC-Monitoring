using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using ExamMonitoringNet;
using ExamMonitoringNet.Pages;

namespace ExamMonitoringNet.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        #region Settings
        public static string PathSave { get; set;}
        public static string PathForbiddenWords { get; set; }
        public static string PathForbiddenPrograms { get; set; }
        public static bool EnableStatistics { get; set; }
        public static bool EnableModeration { get; set; }
        #endregion

        #region StartUp
        public static bool IsMonitoring { get; set; }
        public static Thread foregroundWindowThread = null;
        public static Thread backgroundKeyboardThread = null;
        public static List<string> listInputKeywords { get; set; } = new List<string>();
        public static List<string> listRunningAplication { get; set; } = new List<string>();
        public static List<string> forbiddenWords { get; set; } = new List<string>();
        public static List<string> forbiddenApplication { get; set; } = new List<string>();
        #endregion

        #region Changes pages
        private WindowState _windowState;
        public WindowState WindowState
        {
            get { return _windowState; }
            set { _windowState = value; OnPropertyChanged(); }
        }

        private double _frameOpacity;
        public double FrameOpacity
        {
            get { return _frameOpacity; }
            set { _frameOpacity = value; OnPropertyChanged(); }
        }

        private Page _currentPage;
        public Page CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; OnPropertyChanged(); }
        }
        public ICommand StartUpMenuClick { get; set; }
        public ICommand SettingsMenuClick { get; set; }
        public ICommand ReportMenuClick { get; set; }
        #endregion

        public MainViewModel() {
            StartUpMenuClick = new RelayCommand(StartUpMenuClickEvent);
            SettingsMenuClick = new RelayCommand(SettingsMenuClickEvent);
            ReportMenuClick = new RelayCommand(ReportMenuClickEvent);

            FrameOpacity = 1;
            CurrentPage = new StartUpPage();

        }

        #region Changes pages
        public void StartUpMenuClickEvent(object obj)
        {
            SlowOpacity(new StartUpPage());
        }
        public void SettingsMenuClickEvent(object obj)
        {
            SlowOpacity(new SettingsPage());
        }
        public void ReportMenuClickEvent(object obj)
        {
            SlowOpacity(new ReportPage());
        }
        private async void SlowOpacity(Page page)
        {
            await Task.Factory.StartNew(() =>
            {

                for (double i = 1.0; i > 0.0; i -= 0.1)
                {
                    FrameOpacity = i;
                    Thread.Sleep(5);
                }
                CurrentPage = page;
                for (double i = 0.0; i < 1.1; i += 0.1)
                {
                    FrameOpacity = i;
                    Thread.Sleep(5);

                }
            });
        }
        #endregion
    }
}
