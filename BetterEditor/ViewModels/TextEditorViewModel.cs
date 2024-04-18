using BetterEditor.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Windows;
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
                usedTabs[index].TabName = CreateTabName(Tab.FilePath, value);
                usedTabs[index].Content = Content;
                UsedTabs = usedTabs;
                Tabs[index].Content = value;
                Tab.Content = value;
                _staticTabs = new ObservableCollection<Tab>();
                foreach (TabViewModel tab in usedTabs)
                    _staticTabs.Add(GetTabFromTabViewModel(tab));
                Settings.LOT = GetTabFromTabViewModel(Tab);
                _staticSettings = Settings;
                if (Settings.SA)
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
        public string EditButtonBackground { get; set; }
        public string DeleteButtonBackground { get; set; }
        private MainViewModel _parent;

        public TextEditorViewModel(List<Tab> tabs, Settings settings, string editBackgroundColor, string deleteBackgroundColor, MainViewModel parent) {
            Tabs = new ObservableCollection<Tab>(tabs);
            Settings = settings;
            OpenFirstTab();
            _appStart = false;
            EditButtonBackground = editBackgroundColor;
            DeleteButtonBackground = deleteBackgroundColor;
            _parent = parent;
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

        public ICommand DeleteSpecificTabCommand => new RelayCommand(DeleteTab, CanExecuteRenameCommand);
        private void DeleteTab(object obj) {
            try {
                TabViewModel tab = (TabViewModel)obj;
                DeleteTab(UsedTabs.IndexOf(UsedTabs.Where(x => x.Index == tab.Index).First()));
            } catch (Exception e) { 
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand DeleteCurrentTabCommand => new RelayCommand(DeleteCurrentTab, CanExecuteRenameCommand);
        private void DeleteCurrentTab(object obj) {
            try {
                DeleteTab(UsedTabs.IndexOf(UsedTabs.Where(x => x.Index == Tab.Index).First()));
            } catch (Exception e) { 
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        private void DeleteTab(int index) {
            try {
                Tabs.Remove(Tabs[index]);
                ObservableCollection<TabViewModel> usedTabs = new ObservableCollection<TabViewModel>(UsedTabs);
                if (Settings.CAD && File.Exists(Tab.FilePath))
                    File.Delete(Tab.FilePath);
                else if (!Settings.CAD && File.Exists(Tab.FilePath)) { 
                    MessageBoxResult messageBoxResult = MessageBox.Show($"Do you want to delete the following file: {Tab.TabName}", "Delete file", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes) {
                        File.Delete(Tab.FilePath);
                    }
                }
                usedTabs.Remove(Tab);
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
                Settings.LOT = GetTabFromTabViewModel(Tab);
                DataManager.WriteSettings(Settings);
            } catch (Exception e) { 
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand OpenTabCommand => new RelayCommand(OpenTab, CanExecuteCommand);
        private void OpenTab(object obj) {
            try {
                int index = UsedTabs.IndexOf(UsedTabs.Where(x => x.Index == Tab.Index).First());
                Tabs[index].Content = Content;
                ObservableCollection<TabViewModel> tempUsedTabs = new ObservableCollection<TabViewModel>(UsedTabs);
                tempUsedTabs[index].Content = Content;
                tempUsedTabs[index].TabName = CreateTabName(tempUsedTabs[index].FilePath, tempUsedTabs[index].Content);
                UsedTabs = tempUsedTabs;
                DataManager.WriteTabs(Tabs.ToList());

                TabViewModel tempTabViewModel = (TabViewModel)obj;
                if (tempTabViewModel.FilePath != "" && !File.Exists(tempTabViewModel.FilePath)) {
                    MessageBoxResult messageBoxResult = MessageBox.Show($"{tempTabViewModel.FilePath} doesn't exist anymore.\nDo you want to keep the tab?", "File is missing", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.No) {
                        DeleteSpecificTabCommand.Execute(tempTabViewModel);
                        return;
                    } else {
                        index = UsedTabs.IndexOf(UsedTabs.Where(x => x.FilePath == tempTabViewModel.FilePath).First());
                        Tabs[index].FilePath = "";
                        tempTabViewModel.FilePath = "";
                        DataManager.WriteTabs(Tabs.ToList());
                    }
                }
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
                DataManager.WriteSettings(Settings);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        private ObservableCollection<TabViewModel> TabsToUsedTabs(ObservableCollection<Tab> tabs) {
            ObservableCollection<TabViewModel> usedTabs = new ObservableCollection<TabViewModel>();
            try {
                Counter = 0;
                foreach (Tab tab in tabs) {
                    string tabName = CreateTabName(tab.FilePath, tab.Content);
                    usedTabs.Add(new TabViewModel(tab.FilePath, tab.Content, tab.MD, tabName, false, Counter));
                    Counter++;
                }
            } catch (Exception e) { 
                BaseViewModel.ShowErrorMessage(e);
            }
            return usedTabs;
        }

        private void OpenFirstTab() {
            try {
                if (Settings.LOT.FilePath != "" && !File.Exists(Settings.LOT.FilePath)) {
                    MessageBoxResult messageBoxResult = MessageBox.Show($"{Settings.LOT.FilePath} doesn't exist anymore.\nDo you want to keep the tab?", "File is missing", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.No) {
                        if (Tabs.Contains(Settings.LOT)) {
                            Tabs.Remove(Settings.LOT);
                            UsedTabs = TabsToUsedTabs(Tabs);
                            DataManager.WriteTabs(Tabs.ToList());
                        }
                        Tab = UsedTabs.Last();
                        OpenTabCommand.Execute(UsedTabs.Last());
                        return;
                    } else {
                        if (Tabs.Contains(Settings.LOT)) {
                            int index = Tabs.IndexOf(Settings.LOT);
                            Tabs[index].FilePath = "";
                            UsedTabs[index].FilePath = "";
                        }
                        Settings.LOT.FilePath = "";
                    }
                }
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
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand CreateNewTabCommand => new RelayCommand(CreateNewTab, CanExecuteCommand);
        private void CreateNewTab(object obj) {
            try { 
                Tabs = new ObservableCollection<Tab>(Tabs.Append(new Tab("", "", new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day))));
                Tab = UsedTabs.Last();
                OpenTabCommand.Execute(UsedTabs[UsedTabs.Count - 1]);
            } catch (Exception e) { 
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand SaveCommand => new RelayCommand(Save, CanExecuteCommand);
        private void Save(object obj) {
            try {
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
                if (File.Exists(Tab.FilePath))
                    File.WriteAllText(Tab.FilePath, Content);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
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

        private static void SaveAutomatically(object? state) {
            try {
                if (!_appStart) {
                    DataManager.WriteTabs(_staticTabs.ToList());
                    DataManager.WriteSettings(_staticSettings);
                }
            } catch (Exception e) { 
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        private Tab GetTabFromTabViewModel(TabViewModel tabViewModel) {
            try {
                return new Tab(tabViewModel.FilePath, tabViewModel.Content, tabViewModel.MD);
            } catch (Exception e) { 
                BaseViewModel.ShowErrorMessage(e);
                return new Tab();
            }
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

        public ICommand ChangeUserControlCommand => new RelayCommand(ChangeUserControl, CanExecuteCommand);
        private void ChangeUserControl(object obj) {
            try {
                _parent.ChangeUserControlCommand.Execute(_parent);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand ZoomInCommand => new RelayCommand(ZoomIn, CanExecuteCommand);
        private void ZoomIn(object obj) {
            try {
                _parent.ZoomInCommand.Execute(_parent);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand ZoomOutCommand => new RelayCommand(ZoomOut, CanExecuteCommand);
        private void ZoomOut(object obj) {
            try {
                _parent.ZoomOutCommand.Execute(_parent);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        private string CreateTabName(string filePath, string newContent) {
            string tabName = "";
            try {
                if (File.Exists(filePath)) {
                    tabName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                } else {
                    string tempSubstring = (newContent.Length > 30) ? newContent.Substring(0, 27) + "..." : newContent;
                    if (tempSubstring.Contains("\n"))
                        tempSubstring = tempSubstring.Substring(0, tempSubstring.IndexOf("\n"));
                    if (tempSubstring.Contains("\r"))
                        tempSubstring = tempSubstring.Substring(0, tempSubstring.IndexOf("\r"));
                    tabName = tempSubstring.Trim();
                }
            } catch (Exception e) { 
                BaseViewModel.ShowErrorMessage(e);
            }
            return tabName;
        }
    }
}
