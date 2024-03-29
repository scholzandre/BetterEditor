using BetterEditor.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using VocabTrainer.Models;

namespace BetterEditor.ViewModels {
    internal class TextEditorViewModel : BaseViewModel {
        private string _content = string.Empty;
        public string Content { 
            get => _content;
            set {
                _content = value;
                if (Tab.TabName == "" && UsedTabs.Count != 0 || !File.Exists(Tab.FilePath)) {
                    int index = Tabs.IndexOf(new Tab(Tab.FilePath, Tab.Content, Tab.MD));
                    ObservableCollection<TabViewModel> usedTabs = new ObservableCollection<TabViewModel>(UsedTabs);
                    usedTabs[index].TabName = (value.Length > 30) ? value.Substring(0, 27) + "..." : value;
                    UsedTabs = usedTabs;
                }
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
        public Settings Settings { get; set; }
        public string MoveLeftIcon { get; set; } = "<";
        public string MoveRightIcon { get; set; } = ">";
        public string RenameIcon { get; set; } = "🖉";
        public string DeleteIcon { get; set; } = "✖";
        public TextEditorViewModel(List<Tab> tabs, Settings settings) {
            Tabs = new ObservableCollection<Tab>(tabs);
            Settings = settings;
            OpenFirstTab();
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
            throw new NotImplementedException();
        }

        public ICommand OpenTabCommand => new RelayCommand(OpenTab, CanExecuteCommand);
        private void OpenTab(object obj) {
            try {
                TabViewModel tTVM = (TabViewModel)obj;
                TabViewModel tempTabViewModel = new TabViewModel(tTVM.FilePath, tTVM.Content, tTVM.MD, tTVM.TabName, tTVM.IsActive, tTVM.Index);
                int index = Tabs.IndexOf(new Tab(Tab.FilePath, Tab.Content, Tab.MD));
                Tabs[index].Content = Content;
                UsedTabs[index].Content = Content;
                Tab = tempTabViewModel;
                Content = Tab.Content;
                Settings.LOT = new Tab(Tab.FilePath, Tab.Content, Tab.MD);
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
            ObservableCollection<TabViewModel> usedTabs = new ObservableCollection<TabViewModel>();
            foreach (Tab tab in tabs) {
                Counter++;
                string tabName = "";
                if (File.Exists(tab.FilePath))
                    tabName = tab.FilePath.Substring(tab.FilePath.LastIndexOf("\\"));
                else
                    tabName = (tab.Content.Length > 30)? tab.Content.Substring(0, 27) + "..." : tab.Content;
                usedTabs.Add(new TabViewModel(tab.FilePath, tab.Content, tab.MD, tabName, false, Counter));
            }
            return usedTabs;
        }

        private void OpenFirstTab() {
            if (!Tabs.Contains(Settings.LOT)) {
                Counter++;
                Tabs = new ObservableCollection<Tab>(Tabs.Append(Settings.LOT));
                UsedTabs.Append(new TabViewModel(Settings.LOT.FilePath, Settings.LOT.Content, Settings.LOT.MD, (Settings.LOT.Content.Length > 30) ? Settings.LOT.Content.Substring(0, 30) : Settings.LOT.Content, false, Counter));
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
            Counter++;
            Tab tab = new Tab();
            TabViewModel usedTab = new TabViewModel(tab.FilePath, tab.Content, tab.MD, "", false, Counter);
            Tabs = new ObservableCollection<Tab>(Tabs.Append(tab));
            UsedTabs.Append(usedTab);
            OpenTabCommand.Execute(UsedTabs[UsedTabs.Count - 1]);
        }

        public ICommand SaveCommand => new RelayCommand(Save, CanExecuteCommand);
        private void Save(object obj) {
            int index = Tabs.IndexOf(new Tab(Tab.FilePath, Tab.Content, Tab.MD));
            Tab.Content = Content;
            Tabs[index].Content = Content;
            UsedTabs[index].Content = Content;
            DataManager.WriteTabs(Tabs.ToList());
        }
    }
}
