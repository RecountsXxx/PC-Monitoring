using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExamMonitoringNet.ViewModels
{
    class ReportViewModel : ViewModelBase
    {
        private List<string> _listRunningApps;
        public List<string> ListRunningApps
        {
            get { return _listRunningApps; }
            set { _listRunningApps = value; OnPropertyChanged(); }
        }


        private List<string> _listInputKeywords;
        public List<string> ListInputKeywords
        {
            get { return _listInputKeywords; }
            set { _listInputKeywords = value; OnPropertyChanged(); }
        }
        

        public ReportViewModel()
        {
            Thread threadRefreshList = new Thread(RefreshList);
            threadRefreshList.Start();
        }
        public void RefreshList()
        {
            while(true)
            {
                try
                {
                    ListRunningApps = MainViewModel.listRunningAplication.ToList();
                    ListInputKeywords = MainViewModel.listInputKeywords.ToList();
                }
                catch
                {

                }
            }
        }
    }
}
