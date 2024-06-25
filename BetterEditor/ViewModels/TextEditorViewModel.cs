using BetterEditor.Models;
using BetterEditor.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BetterEditor.ViewModels {
    internal class TextEditorViewModel : BaseViewModel {

        #region Properties
        private UserControl _userControl = new UserControl();
        public UserControl UserControl {
            get => _userControl;
            set {
                _userControl = value;
                OnPropertyChanged(nameof(UserControl));
            }
        }
        private string _content = string.Empty;
        public string Content {
            get => _content;
            set {
                _content = value;
                _index = UsedTabs.IndexOf(UsedTabs.Where(x => x.Index == Tab.Index).First());
                Tabs[_index].Content = value;
                _contentChanged?.Invoke(this, EventArgs.Empty);
                Tab.Content = value;
                _staticTabs = new ObservableCollection<Tab>(Tabs);
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
                _contentChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private ObservableCollection<TabViewModel> _usedTabs = new ObservableCollection<TabViewModel>();
        public ObservableCollection<TabViewModel> UsedTabs {
            get => _usedTabs;
            set {
                _usedTabs = value;
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

        private Visibility _visibility = Visibility.Collapsed;
        public Visibility Visibility { 
            get => _visibility;
            set {
                _visibility = value; 
                OnPropertyChanged(nameof(Visibility));
            } 
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

        private int _selectionStart = 0;
        public int SelectionStart {
            get => _selectionStart;
            set {
                _selectionStart = value;
                OnPropertyChanged(nameof(SelectionStart));
            }
        }

        private int _selectionLength = 0;
        public int SelectionLength {
            get => _selectionLength;
            set {
                _selectionLength = value;
                OnPropertyChanged(nameof(SelectionLength));
            }
        }
        #endregion

        #region Fields
        private Timer _timer = new Timer(SaveAutomatically, null, 10000, Timeout.Infinite);
        private static event EventHandler _contentChanged;
        private static ObservableCollection<Tab> _staticTabs;
        private static Settings _staticSettings;
        private static bool _appStart = true;
        private int _index = 0;
        private bool _tabSwitch = false;
        public string MoveLeftIcon { get; set; } = "<";
        public string MoveRightIcon { get; set; } = ">";
        public string RenameIcon { get; set; } = "🖉";
        public string DeleteIcon { get; set; } = "✖";
        public string EditButtonBackground { get; set; }
        public string DeleteButtonBackground { get; set; }
        private UserControl _searchUserControl;
        private UserControl _replaceUserControl;
        private MainViewModel _parent;
        #endregion

        public TextEditorViewModel(List<Tab> tabs, Settings settings, string editBackgroundColor, string deleteBackgroundColor, MainViewModel parent) {
            _contentChanged += TabsToUsedTabs;
            Tabs = new ObservableCollection<Tab>(tabs);
            Settings = settings;
            OpenFirstTab();
            _appStart = false;
            EditButtonBackground = editBackgroundColor;
            DeleteButtonBackground = deleteBackgroundColor;
            _parent = parent;
            _searchUserControl = (UserControl)Activator.CreateInstance(typeof(SearchView));
            _searchUserControl.DataContext = new SearchViewModel(this);
            _replaceUserControl = (UserControl)Activator.CreateInstance(typeof(ReplaceView));
            _replaceUserControl.DataContext = new ReplaceViewModel(this);
        }

        public TextEditorViewModel() {
        }

        private bool CanExecuteCommand(object arg) {
            return true;
        }

        private bool CanExecuteRenameCommand(object arg) {
            return true;
        }

        public ICommand RenameCommand => new RelayCommand(Rename, CanExecuteRenameCommand);
        private void Rename(object obj) {
            throw new NotImplementedException();
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
                if (Tabs.Count > 0) { 
                    Tabs.Remove(Tabs[index]);
                    if (Settings.CAD && File.Exists(Tab.FilePath))
                        DataManager.DeleteFile(Tab.FilePath);
                    else if (!Settings.CAD && File.Exists(Tab.FilePath)) { 
                        MessageBoxResult messageBoxResult = MessageBox.Show($"Do you want to delete the following file: {Tab.TabName}", "Delete file", MessageBoxButton.YesNo);
                        if (messageBoxResult == MessageBoxResult.Yes) {
                            DataManager.DeleteFile(Tab.FilePath);
                        }
                    }
                    UsedTabs.Remove(UsedTabs[index]);
                    if (UsedTabs.Count == 0) {
                        Counter = 0;
                        CreateNewTab(this);
                        return;
                    } else if (index == UsedTabs.Count) {
                        index--;
                    }
                    UsedTabs[index].IsActive = false;
                    Tab = UsedTabs[index];
                    DataManager.WriteTabs(Tabs.ToList());
                    Settings.LOT = GetTabFromTabViewModel(Tab);
                    DataManager.WriteSettings(Settings);
                }
            } catch (Exception e) { 
                BaseViewModel.ShowErrorMessage(e);
            }
        }
        public ICommand OpenTabCommand => new RelayCommand(OpenTab, CanExecuteCommand);
        private void OpenTab(object obj) {
            try {
                _tabSwitch = true;
                _index = UsedTabs.IndexOf(UsedTabs.Where(x => x.Index == Tab.Index).First());
                Tabs[_index].Content = Content;
                _contentChanged?.Invoke(this, EventArgs.Empty);
                DataManager.WriteTabs(Tabs.ToList());

                TabViewModel tempTabViewModel = (TabViewModel)obj;
                if (tempTabViewModel.FilePath != "" && !File.Exists(tempTabViewModel.FilePath)) {
                    MessageBoxResult messageBoxResult = MessageBox.Show($"{tempTabViewModel.FilePath} doesn't exist anymore.\nDo you want to keep the tab?", "File is missing", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.No) {
                        DeleteSpecificTabCommand.Execute(tempTabViewModel);
                        return;
                    } else {
                        _index = UsedTabs.IndexOf(UsedTabs.Where(x => x.FilePath == tempTabViewModel.FilePath).First());
                        Tabs[_index].FilePath = "";
                        tempTabViewModel.FilePath = "";
                        DataManager.WriteTabs(Tabs.ToList());
                    }
                }
                Tab = tempTabViewModel;
                Content = Tab.Content;
                Settings.LOT = GetTabFromTabViewModel(Tab);
                if (UsedTabs.Count > 0) { 
                    for (int i = 0; i < UsedTabs.Count; i++)
                        UsedTabs[i].IsActive = true;
                    UsedTabs[UsedTabs.IndexOf(Tab)].IsActive = false;
                }
                DataManager.WriteSettings(Settings);
                _tabSwitch = false;
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        private void TabsToUsedTabs(object sender, EventArgs args) {
            UsedTabs.Clear();
            try {
                if (Tabs.Count > 0) { 
                    Counter = 0;
                    foreach (Tab tab in Tabs) {
                        UsedTabs.Add(new TabViewModel(tab.FilePath, tab.Content, tab.MD, CreateTabname(tab.FilePath, tab.Content, Counter), true, Counter));
                        Counter++;
                    }
                    if (UsedTabs.Count > 1)
                        UsedTabs[UsedTabs.IndexOf(Tab)].IsActive = false;
                    else 
                        UsedTabs[0].IsActive = false;
                }
            } catch (Exception e) { 
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        private void OpenFirstTab() {
            try {
                if (Settings.LOT.FilePath != "" && !File.Exists(Settings.LOT.FilePath)) {
                    MessageBoxResult messageBoxResult = MessageBox.Show($"{Settings.LOT.FilePath} doesn't exist anymore.\nDo you want to keep the tab?", "File is missing", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.No) {
                        if (Tabs.Contains(Settings.LOT)) {
                            Tabs.Remove(Settings.LOT);
                            _contentChanged?.Invoke(this, EventArgs.Empty);
                            DataManager.WriteTabs(Tabs.ToList());
                        }
                        Tab = UsedTabs.Last();
                        OpenTabCommand.Execute(UsedTabs.Last());
                    } else {
                        if (Tabs.Contains(Settings.LOT)) {
                            int index = Tabs.IndexOf(Settings.LOT);
                            Tabs[index].FilePath = "";
                            UsedTabs[index].FilePath = "";
                        }
                        Settings.LOT.FilePath = "";
                    }
                } else if (Settings.LOT.FilePath != "" &&
                           File.Exists(Settings.LOT.FilePath) &&
                           Settings.LOT.Content != File.ReadAllText(Settings.LOT.FilePath)) { 
                    MessageBoxResult messageBoxResult = MessageBox.Show($"{Settings.LOT.FilePath} has changed.\nDo you want to update the tab?", "File has changed", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes ) { 
                        int index = Tabs.IndexOf(Settings.LOT);
                        Settings.LOT.Content = File.ReadAllText(Settings.LOT.FilePath);
                        DataManager.WriteSettings(Settings);
                        Tabs[index].Content = Settings.LOT.Content;
                        UsedTabs[index].Content = Settings.LOT.Content;
                        Tab.Content = Settings.LOT.Content;
                        DataManager.WriteTabs(Tabs.ToList());
                    }
                } else if (!Tabs.Contains(Settings.LOT)) {
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
                Tabs.Add(new Tab("", "", new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day)));
                UsedTabs.Add(new TabViewModel("", "", new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day), "", false, Counter++));
                OnPropertyChanged(nameof(Tabs));
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
                Tab.Content = Content;
                Tab.TabName = CreateTabname(Tab.FilePath, Content, _index);
                Tabs[_index].Content = Content;
                Tabs[_index].MD = todaysDate;
                _contentChanged?.Invoke(this, EventArgs.Empty);
                _parent.Tabs = Tabs.ToList();
                DataManager.WriteTabs(Tabs.ToList());
                _parent.Tabs = Tabs.ToList();
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
                string filePath = obj?.ToString();
                if (!File.Exists(filePath)) {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.ShowDialog();
                    filePath = openFileDialog.FileName;
                }
                if (File.Exists(filePath)) { 
                    string content = File.ReadAllText(filePath);
                    DateOnly date = DateOnly.FromDateTime(File.GetLastWriteTime(filePath).Date);
                    Tabs = new ObservableCollection<Tab>(Tabs.Append(new Tab(filePath, content, date)));
                    OpenTabCommand.Execute(UsedTabs.Last());
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }
        public ICommand SaveAsCommand => new RelayCommand(SaveAs, CanExecuteCommand);
        private void SaveAs(object obj) {
            try {
                SaveCommand.Execute(UsedTabs.Last());
                using (var openFolderDialog = new System.Windows.Forms.FolderBrowserDialog()) {
                    openFolderDialog.ShowDialog();
                    if (openFolderDialog.SelectedPath != "" && Directory.Exists(openFolderDialog.SelectedPath)) { 
                        string filePath = openFolderDialog.SelectedPath + "\\";
                        if (Tab.FilePath != "")
                            filePath += Tab.FilePath;
                        else
                            filePath += (Tab.Content.Length >= 30)? Tab.Content.Substring(0, 30) + ".txt" : Tab.Content + ".txt";
                        Tab.FilePath = filePath;
                        Tabs[_index].FilePath = filePath;
                        UsedTabs[_index].FilePath = filePath;
                        SaveCommand.Execute(UsedTabs.Last());
                        File.WriteAllText(filePath, Tab.Content);
                    }
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        private static void SaveAutomatically(object? state) {
            try {
                if (!_appStart) {
                    Application.Current.Dispatcher.Invoke(() => {
                        DataManager.WriteTabs(_staticTabs.ToList());
                        DataManager.WriteSettings(_staticSettings);
                        _contentChanged?.Invoke(null, EventArgs.Empty);
                    });
                }
            } catch (Exception e) { 
                BaseViewModel.ShowErrorMessage(e);
            }
        }
        public string CreateTabname(string filePath, string newContent, int tabIndex) {
            if (filePath == null)
                filePath = Tab.FilePath;
            if (newContent == null)
                newContent = Tab.Content;
            string tabName = (!_appStart && Tab.Content != newContent && tabIndex == _index && !_tabSwitch) ? "*" : string.Empty;
            try {
                if (File.Exists(filePath)) {
                    tabName += filePath.Substring(filePath.LastIndexOf("\\") + 1);
                } else {
                    string tempSubstring = (newContent.Length > 30) ? newContent.Substring(0, 27) + "..." : newContent;
                    if (tempSubstring.Contains("\n"))
                        tempSubstring = tempSubstring.Substring(0, tempSubstring.IndexOf("\n"));
                    if (tempSubstring.Contains("\r"))
                        tempSubstring = tempSubstring.Substring(0, tempSubstring.IndexOf("\r"));
                    tabName += tempSubstring.Trim();
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
            return tabName;
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

        public ICommand SwitchViewModeCommand => new RelayCommand(SwitchViewMode, CanExecuteCommand);
        private void SwitchViewMode(object obj) {
            try {
                _parent.SwitchViewModeCommand.Execute(_parent);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand OpenSearchViewCommand => new RelayCommand(OpenSearchView, CanExecuteCommand);
        private void OpenSearchView(object obj) {
            try {
                if (Visibility == Visibility.Collapsed)
                    Visibility = Visibility.Visible;
                else if (UserControl.GetType() == _searchUserControl.GetType())
                    Visibility = Visibility.Collapsed;
                if (UserControl.GetType() != _searchUserControl.GetType()) {
                    UserControl = _searchUserControl;
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand OpenReplaceViewCommand => new RelayCommand(OpenReplaceView, CanExecuteCommand);
        private void OpenReplaceView(object obj) {
            try {
                if (Visibility == Visibility.Collapsed)
                    Visibility = Visibility.Visible;
                else if (UserControl.GetType() == _replaceUserControl.GetType())
                    Visibility = Visibility.Collapsed;
                if (UserControl.GetType() != _replaceUserControl.GetType()) {
                    UserControl = _replaceUserControl;
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }
    }
}
