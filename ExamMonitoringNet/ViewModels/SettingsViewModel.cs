using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;

namespace ExamMonitoringNet.ViewModels
{
    class SettingsViewModel : ViewModelBase
    {
        #region Toggled buttons
        private bool _useStaticticsChecked;
        public bool UseStaticticsChecked
        {
            get { return _useStaticticsChecked;}
            set { _useStaticticsChecked = value;OnPropertyChanged(); }
        }
        private bool _useModerationChecked;
        public bool UseModerationChecked
        {
            get { return _useModerationChecked; }
            set { _useModerationChecked = value; OnPropertyChanged(); }
        }
        private bool _savingToFileChecked;
        public bool SavingToFileChecked
        {
            get { return _savingToFileChecked; }
            set { _savingToFileChecked = value; OnPropertyChanged(); }
        }
        public ICommand UseStaticticsCommand { get; set; }
        public ICommand UseModerationCommand { get; set; }
        #endregion

        #region Buttons
        private string _selectPathWithForbiddenWordsText;
        public string SelectPathWithForbiddenWordsText
        {
            get { return _selectPathWithForbiddenWordsText; }
            set { _selectPathWithForbiddenWordsText = value;OnPropertyChanged(); }
        }
        private string _selectPathWithForbiddenProgramsText;
        public string SelectPathWithForbiddenProgramsText
        {
            get { return _selectPathWithForbiddenProgramsText; }
            set { _selectPathWithForbiddenProgramsText = value; OnPropertyChanged(); }
        }
        private string _selectSavePathText;
        public string SelectSavePathText
        {
            get { return _selectSavePathText; }
            set { _selectSavePathText = value; OnPropertyChanged(); }
        }

        private bool _selectPathWordsEnabled;
        public bool SelectPathWordsEnabled
        {
            get { return _selectPathWordsEnabled; }
            set { _selectPathWordsEnabled= value; OnPropertyChanged();}
        }
        private bool _selectPathProgramsEnabled;
        public bool SelectPathProgramsEnabled
        {
            get { return _selectPathProgramsEnabled; }
            set { _selectPathProgramsEnabled = value; OnPropertyChanged(); }
        }
        private bool _selectSavePathEnabled;
        public bool SelectSavePathEnabled
        {
            get { return _selectSavePathEnabled; }
            set { _selectSavePathEnabled = value; OnPropertyChanged(); }
        }

        private double _selectPathWordsOpacity;
        public double SelectPathWordsOpacity
        {
            get { return _selectPathWordsOpacity; }
            set { _selectPathWordsOpacity = value; OnPropertyChanged(); }
        }
        private double _selectPathProgramsOpacity;
        public double SelectPathProgramsOpacity
        {
            get { return _selectPathProgramsOpacity; }
            set { _selectPathProgramsOpacity = value; OnPropertyChanged(); }
        }
        private double _selectSavePathOpacity;
        public double SelectSavePathOpacity
        {
            get { return _selectSavePathOpacity; }
            set { _selectSavePathOpacity = value; OnPropertyChanged(); }
        }

        public ICommand SelectPathWithForbiddenProgramsCommand { get; set; }
        public ICommand SelectPathWithForbiddenWordsCommand { get; set; }
        public ICommand SelectSavePathCommand { get; set; }
        #endregion

        public SettingsViewModel()
        {
            UseStaticticsCommand = new RelayCommand(UseStaticticsCommandEvent);
            UseModerationCommand = new RelayCommand(UseModerationCommandEvent);

            SelectPathWithForbiddenProgramsCommand = new RelayCommand(SelectPathWithForbiddenProgramsCommandEvent);
            SelectPathWithForbiddenWordsCommand = new RelayCommand(SelectPathWithForbiddenWordsCommandEvent);
            SelectSavePathCommand = new RelayCommand(SelectSavePathCommandEvent);
            SelectPathWithForbiddenWordsText = "Select file with forbidden words";
            SelectPathWithForbiddenProgramsText = "Select file with forbidden programs";
            SelectSavePathText = "Select save path";

            SelectSavePathEnabled = false;
            SelectPathWordsEnabled = false;
            SelectPathProgramsEnabled = false;
            SelectSavePathOpacity = 0.5;
            SelectPathProgramsOpacity = 0.5;
            SelectPathWordsOpacity = 0.5;



            if(MainViewModel.EnableStatistics == true)
            {
                UseStaticticsChecked = true;
                SelectSavePathEnabled = true;
                SelectSavePathOpacity =1;
            }
            if(MainViewModel.EnableModeration == true)
            {
                UseModerationChecked = true;
                SelectPathWordsEnabled = true;
                SelectPathProgramsEnabled = true;
                SelectPathProgramsOpacity = 1;
                SelectPathWordsOpacity = 1;
            }
            if (MainViewModel.PathSave != null)
                SelectSavePathText = MainViewModel.PathSave;
            if(MainViewModel.PathForbiddenPrograms != null && MainViewModel.PathForbiddenWords != null)
            {
                SelectPathWithForbiddenProgramsText= MainViewModel.PathForbiddenPrograms;
                SelectPathWithForbiddenWordsText = MainViewModel.PathForbiddenWords;
            }     
        }

        #region Toggled buttons
        private void UseStaticticsCommandEvent(object obj)
        {
            if ((bool)obj)
            {
                UseStaticticsChecked = true;
                SelectSavePathEnabled = true;
                SelectSavePathOpacity = 1;
                MainViewModel.EnableStatistics = true;

            }
            else
            {
                UseStaticticsChecked = false;
                SelectSavePathEnabled = false;
                SelectSavePathOpacity = 0.5;
                MainViewModel.EnableStatistics = false;

            }        
        }
        private void UseModerationCommandEvent(object obj)
        {
            if ((bool)obj)
            {
                UseModerationChecked = true;

                SelectPathWordsEnabled= true;
                SelectPathProgramsEnabled = true;
                SelectPathProgramsOpacity = 1;
                SelectPathWordsOpacity = 1;
                MainViewModel.EnableModeration = true;
            }
            else
            {
                UseModerationChecked = false;
                SelectPathWordsEnabled = false;
                SelectPathProgramsEnabled = false;
                SelectPathProgramsOpacity = 0.5;
                SelectPathWordsOpacity = 0.5;
                MainViewModel.EnableModeration = false;
            }
        }
        #endregion

        #region Buttons
        private void SelectPathWithForbiddenProgramsCommandEvent(object obj)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                MainViewModel.forbiddenApplication = new List<string>();
                SelectPathWithForbiddenProgramsText = dialog.FileName; 
                MainViewModel.PathForbiddenPrograms = SelectPathWithForbiddenProgramsText;

                using (StreamReader reader = new StreamReader(SelectPathWithForbiddenProgramsText))
                {
                    string line;
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        MainViewModel.forbiddenApplication.Add(line);
                    }
                }
            }

        }
        private void SelectPathWithForbiddenWordsCommandEvent(object obj)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                MainViewModel.forbiddenWords = new List<string>();
                SelectPathWithForbiddenWordsText = dialog.FileName;
                MainViewModel.PathForbiddenWords = SelectPathWithForbiddenWordsText;
                using (StreamReader reader = new StreamReader(SelectPathWithForbiddenWordsText)) {
                    string line;
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        MainViewModel.forbiddenWords.Add(line);
                    }
                }
            }
        }
        private void SelectSavePathCommandEvent(object obj)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SelectSavePathText = dialog.SelectedPath;
                MainViewModel.PathSave = SelectSavePathText;
            }
        }
        #endregion
    }
}
