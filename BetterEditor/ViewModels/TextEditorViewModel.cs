using BetterEditor.Models;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Windows.Input;
using VocabTrainer.Models;

namespace BetterEditor.ViewModels {
    internal class TextEditorViewModel : BaseViewModel {
        private static bool _appStart = true;
        private string _content = string.Empty;
        public string Content { 
            get => _content;
            set {
                _content = value;
                int index = UsedTabs.IndexOf(UsedTabs.Where(x => x.Index == Tab.Index).First());
                ObservableCollection<TabViewModel> usedTabs = new ObservableCollection<TabViewModel>(UsedTabs);
                if (Tab.TabName == "" || !File.Exists(Tab.FilePath)) 
                    usedTabs[index].TabName = (value.Length > 30) ? value.Substring(0, 27) + "..." : value;
                usedTabs[index].Content = Content;
                UsedTabs = usedTabs;
                Tabs[index].Content = value;
                Tab.Content = value;
                Settings.LOT = GetTabFromTabViewModel(Tab);
                SaveAutomatically();
                OnPropertyChanged(nameof(Content));
            } 
        }
        private ObservableCollection<Tab> _tabs = new ObservableCollection<Tab>();
        public ObservableCollection<Tab> Tabs {
            get => _tabs;
            set {
                _tabs = value;
                UsedTabs = TabsToUsedTabs(value);
                OnPropertyChanged(nameof(Tabs));
            }
        }

        private ObservableCollection<TabViewModel> _usedTabs = new ObservableCollection<TabViewModel>();
        public ObservableCollection<TabViewModel> UsedTabs {
            get => _usedTabs;
            set {
                _usedTabs = value;
                OnPropertyChanged(nameof(UsedTabs));
            }
        }
        public int Counter { get; set; }
        private Settings _settings;
        public Settings Settings { 
            get => _settings;
            set {
                _settings = value;
                OnPropertyChanged(nameof(Settings));
            }
        }
        public string MoveLeftIcon { get; set; } = "<";
        public string MoveRightIcon { get; set; } = ">";
        public string RenameIcon { get; set; } = "🖉";
        public string DeleteIcon { get; set; } = "✖";
        public string EditButtonBackground { get; set; }
        public string DeleteButtonBackground { get; set; }
        public TextEditorViewModel(List<Tab> tabs, Settings settings, string editBackgroundColor, string deleteBackgroundColor) {
            Tabs = new ObservableCollection<Tab>(tabs);
            Settings = settings;
            OpenFirstTab();
            _appStart = false;
            EditButtonBackground = editBackgroundColor;
            DeleteButtonBackground = deleteBackgroundColor;
        }

        private TabViewModel _tab = new TabViewModel();
        public TabViewModel Tab {
            get => _tab;
            set {
                _tab = value;
                Content = value.Content;
                OnPropertyChanged(nameof(Tab));
            }
        }
        private bool CanExecuteRenameCommand(object arg) {
            return true;
        }

        public ICommand RenameCommand => new RelayCommand(Rename, CanExecuteRenameCommand);
        private void Rename(object obj) {
            throw new NotImplementedException();
        }

        private bool CanExecuteCommand(object arg) {
            return true;
        }

        public ICommand DeleteCommand => new RelayCommand(Delete, CanExecuteRenameCommand);
        private void Delete(object obj) {
            TabViewModel tab = (TabViewModel)obj;
            int index = UsedTabs.IndexOf(UsedTabs.Where(x => x.Index == tab.Index).First());
            Tabs.Remove(GetTabFromTabViewModel(tab));
            ObservableCollection<TabViewModel> usedTabs = new ObservableCollection<TabViewModel>(UsedTabs);
            usedTabs.Remove(tab);
            UsedTabs = usedTabs;
            if (usedTabs.Count == 0) {
                Counter = 0;
                CreateNewTab(this);
                return;
            } else if (index == usedTabs.Count) {
                index--;
            } 
            usedTabs[index].IsActive = false;
            Tab = usedTabs[index];
            DataManager.WriteTabs(Tabs.ToList());
        }

        public ICommand OpenTabCommand => new RelayCommand(OpenTab, CanExecuteCommand);
        private void OpenTab(object obj) {
            try {
                TabViewModel tTVM = (TabViewModel)obj;
                TabViewModel tempTabViewModel = new TabViewModel(tTVM.FilePath, tTVM.Content, tTVM.MD, tTVM.TabName, tTVM.IsActive, tTVM.Index);
                int index = UsedTabs.IndexOf(UsedTabs.Where(x => x.Index == Tab.Index).First());
                Tabs[index].Content = Content;
                ObservableCollection<TabViewModel> tempUsedTabs = new ObservableCollection<TabViewModel>(UsedTabs);
                tempUsedTabs[index].Content = Content;
                if (Tab.TabName == "" || !File.Exists(Tab.FilePath))
                    tempUsedTabs[index].TabName = (tempUsedTabs[index].Content.Length > 30) ? tempUsedTabs[index].Content.Substring(0, 27) + "..." : tempUsedTabs[index].Content;
                UsedTabs = tempUsedTabs;
                Tab = tempTabViewModel;
                Content = Tab.Content;
                Settings.LOT = GetTabFromTabViewModel(Tab);
                if (UsedTabs.Count > 0) { 
                    ObservableCollection<TabViewModel> usedTabs = new ObservableCollection<TabViewModel>(UsedTabs);
                    for (int i = 0; i < usedTabs.Count; i++)
                        usedTabs[i].IsActive = true;
                    usedTabs[UsedTabs.IndexOf(Tab)].IsActive = false;
                    UsedTabs = usedTabs;
                }
                DataManager.WriteTabs(Tabs.ToList());
                DataManager.WriteSettings(Settings);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        private ObservableCollection<TabViewModel> TabsToUsedTabs(ObservableCollection<Tab> tabs) {
            Counter = 0;
            ObservableCollection<TabViewModel> usedTabs = new ObservableCollection<TabViewModel>();
            foreach (Tab tab in tabs) {
                string tabName = "";
                if (File.Exists(tab.FilePath))
                    tabName = tab.FilePath.Substring(tab.FilePath.LastIndexOf("\\"));
                else
                    tabName = (tab.Content.Length > 30)? tab.Content.Substring(0, 27) + "..." : tab.Content;
                usedTabs.Add(new TabViewModel(tab.FilePath, tab.Content, tab.MD, tabName, false, Counter));
                Counter++;
            }
            return usedTabs;
        }

        private void OpenFirstTab() {
            if (!Tabs.Contains(Settings.LOT)) {
                Tabs = new ObservableCollection<Tab>(Tabs.Append(Settings.LOT));
                Tab = UsedTabs.Last();
                DataManager.WriteTabs(Tabs.ToList());
                OpenTabCommand.Execute(UsedTabs[UsedTabs.Count - 1]);
            } else { 
                int index = Tabs.IndexOf(Settings.LOT);
                Tab = UsedTabs[index];
                OpenTabCommand.Execute(UsedTabs[index]);
            }
        }

        public ICommand CreateNewTabCommand => new RelayCommand(CreateNewTab, CanExecuteCommand);
        private void CreateNewTab(object obj) {
            Tabs = new ObservableCollection<Tab>(Tabs.Append(new Tab()));
            Tab = UsedTabs.Last();
            OpenTabCommand.Execute(UsedTabs[UsedTabs.Count - 1]);
        }

        public ICommand SaveCommand => new RelayCommand(Save, CanExecuteCommand);
        private void Save(object obj) {
            DateOnly todaysDate = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
            int index = Tabs.IndexOf(GetTabFromTabViewModel(Tab));
            Tab.Content = Content;
            Tabs[index].Content = Content;
            Tabs[index].MD = todaysDate;
            UsedTabs[index].Content = Content;
            UsedTabs[index].MD = todaysDate;
            DataManager.WriteTabs(Tabs.ToList());
            Settings.LOT = GetTabFromTabViewModel(Tab);
            DataManager.WriteSettings(Settings);
        }

        public ICommand OpenFileCommand => new RelayCommand(OpenFile, CanExecuteCommand);
        private void OpenFile(object obj) {
            try {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.ShowDialog();
                string filePath = openFileDialog.FileName;
                string content = File.ReadAllText(filePath);
                DateOnly date = DateOnly.FromDateTime(File.GetLastWriteTime(filePath).Date);
                Tabs = new ObservableCollection<Tab>(Tabs.Append(new Tab(filePath, content, date)));
                OpenTabCommand.Execute(UsedTabs.Last());
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        private void SaveAutomatically() {
            if (!_appStart) {
                DataManager.WriteTabs(Tabs.ToList());
                DataManager.WriteSettings(Settings);
            }
        }

        private Tab GetTabFromTabViewModel(TabViewModel tabViewModel) { 
            return new Tab(tabViewModel.FilePath, tabViewModel.Content, tabViewModel.MD);
        }

        public ICommand OpenNextTabCommand => new RelayCommand(OpenNextTab, CanExecuteCommand);
        private void OpenNextTab(object obj) {
            try {
                int index = Tabs.IndexOf(GetTabFromTabViewModel(Tab));
                if (index >= Tabs.Count - 1) {
                    OpenTabCommand.Execute(UsedTabs[0]);
                } else { 
                    OpenTabCommand.Execute(UsedTabs[index + 1]);
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }
    }
}
