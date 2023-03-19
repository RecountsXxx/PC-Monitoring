using ExamMonitoringNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExamMonitoringNet
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon notify = new NotifyIcon();
        public static MainWindow window;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            window = this;
        }

        #region Windows Buttons
        private void Drag(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                MainWindow.window.DragMove();
            }
        }
        private void HideClick(object sender, RoutedEventArgs e)
        {
                //help
                notify.Icon = new System.Drawing.Icon("..\\..\\Styles\\Monitoring.ico");
                notify.Visible = true;
                notify.Text = "PC Monitoring";
                ShowInTaskbar = false;
                notify.MouseClick += Notify_MouseClick; ;
                notify.ShowBalloonTip(2, "PC Monitoring", "PC Monitoring will run in the background", ToolTipIcon.Info);
                Hide();

            
        }
        private void Notify_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Show();
                ShowInTaskbar = true;

            }
        }
        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        #endregion
    }
}
