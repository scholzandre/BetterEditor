using BetterEditor.Models;
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
        private Timer _timer = new Timer(SaveAutomatically, null, 10000, Timeout.Infinite);
        private static ObservableCollection<Tab> _staticTabs = new ObservableCollection<Tab>();
        private static Settings _staticSettings;
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
                _staticTabs = new ObservableCollection<Tab>();
                foreach (TabViewModel tab in usedTabs)
                    _staticTabs.Add(GetTabFromTabViewModel(tab));
                Tab.Content = value;
                Settings.LOT = GetTabFromTabViewModel(Tab);
                _staticSettings = Settings;
                _timer.Change(10000, Timeout.Infinite);
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
        public TextEditorViewModel(List<Tab> tabs, Settings settings) {
            Tabs = new ObservableCollection<Tab>(tabs);
            Settings = settings;
            OpenFirstTab();
            _appStart = false;
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
            Tab tab = new Tab();
            TabViewModel usedTab = new TabViewModel(tab.FilePath, tab.Content, tab.MD, "", false, Counter);
            Tabs = new ObservableCollection<Tab>(Tabs.Append(tab));
            UsedTabs.Append(usedTab);
            Tab = usedTab;
            Counter++;
            OpenTabCommand.Execute(UsedTabs[UsedTabs.Count - 1]);
        }

        public ICommand SaveCommand => new RelayCommand(Save, CanExecuteCommand);
        private void Save(object obj) {
            int index = Tabs.IndexOf(GetTabFromTabViewModel(Tab));
            Tab.Content = Content;
            Tabs[index].Content = Content;
            UsedTabs[index].Content = Content;
            DataManager.WriteTabs(Tabs.ToList());
            Settings.LOT = GetTabFromTabViewModel(Tab);
            DataManager.WriteSettings(Settings);
        }
        private static void SaveAutomatically(object state) {
            if (!_appStart) {
                DataManager.WriteTabs(_staticTabs.ToList());
                DataManager.WriteSettings(_staticSettings);
            }
        }

        private Tab GetTabFromTabViewModel(TabViewModel tabViewModel) { 
            return new Tab(tabViewModel.FilePath, tabViewModel.Content, tabViewModel.MD);
        }
    }
}
