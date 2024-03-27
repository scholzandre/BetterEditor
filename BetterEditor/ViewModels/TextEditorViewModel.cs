using BetterEditor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using VocabTrainer.Models;

namespace BetterEditor.ViewModels {
    internal class TextEditorViewModel : BaseViewModel {
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

        private TabViewModel _tab;
        public TabViewModel Tab {
            get => _tab;
            set {
                _tab = value;
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
                Tab tab = (Tab)obj;
                Tab = new TabViewModel(tab.FilePath, tab.Content, tab.MD);
                Settings.LOT = tab;
                if (UsedTabs.Count > 0) { 
                    ObservableCollection<TabViewModel> usedTabs = new ObservableCollection<TabViewModel>(UsedTabs);
                    for (int i = 0; i < usedTabs.Count; i++)
                        usedTabs[i].IsActive = true;
                    usedTabs[UsedTabs.IndexOf(Tab)].IsActive = false;
                    UsedTabs = usedTabs;
                }
                DataManager.WriteSettings(Settings);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        private ObservableCollection<TabViewModel> TabsToUsedTabs(ObservableCollection<Tab> tabs) {
            ObservableCollection<TabViewModel> usedTabs = new ObservableCollection<TabViewModel>();
            foreach (Tab tab in tabs) {
                string tabName = "";
                if (File.Exists(tab.FilePath))
                    tabName = tab.FilePath.Substring(tab.FilePath.LastIndexOf("\\"));
                else
                    tabName = (tab.Content.Length > 30)? tab.Content.Substring(0, 30) : tab.Content;
                usedTabs.Add(new TabViewModel(tab.FilePath, tab.Content, tab.MD, tabName));
            }
            return usedTabs;
        }

        private void OpenFirstTab() {
            if (!Tabs.Contains(Settings.LOT)) {
                Tabs = new ObservableCollection<Tab>(Tabs.Append(Settings.LOT));
                DataManager.WriteTabs(Tabs.ToList());
                OpenTabCommand.Execute(Tabs[Tabs.Count - 1]);
            } else { 
                int index = Tabs.IndexOf(Settings.LOT);
                OpenTabCommand.Execute(Tabs[index]);
            }
        } 
    }
}
