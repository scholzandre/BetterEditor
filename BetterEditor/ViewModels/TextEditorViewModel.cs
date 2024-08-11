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
                ContentUpdated(value);
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

        private ObservableCollection<TabViewModel> _tabViewModels = new ObservableCollection<TabViewModel>();
        public ObservableCollection<TabViewModel> TabViewModels {
            get => _tabViewModels;
            set {
                _tabViewModels = value;
            }
        }
        public int TabCounter { get; set; }
        private Settings _settings;
        public Settings Settings { 
            get => _settings;
            set {
                _settings = value;
                OnPropertyChanged(nameof(Settings));
            }
        }

        private Visibility _userControlVisibility = Visibility.Collapsed;
        public Visibility UserControlVisibility { 
            get => _userControlVisibility;
            set {
                _userControlVisibility = value; 
                OnPropertyChanged(nameof(UserControlVisibility));
            } 
        }


        private TabViewModel _openedTab = new TabViewModel();
        public TabViewModel OpenedTab {
            get => _openedTab;
            set {
                _openedTab = value;
                Content = value.Content;
                OnPropertyChanged(nameof(OpenedTab));
            }
        }

        public event Action<int, int> SelectTextRequested;
        public event Action SelectTextbox;
        public event Action ScrollLeftEnd;
        public event Action ScrollRightEnd;
        public string MoveLeftIcon { get; set; } = "<";
        public string MoveRightIcon { get; set; } = ">";
        public string RenameIcon { get; set; } = "🖉";
        public string DeleteIcon { get; set; } = "✖";
        public string EditButtonBackground { get; set; }
        public string DeleteButtonBackground { get; set; }
        #endregion

        #region Fields
        private Timer _timer = new Timer(SaveAutomatically, null, 10000, Timeout.Infinite);
        private static event EventHandler _contentChanged;
        private static ObservableCollection<Tab> _staticTabs;
        private static Settings _staticSettings;
        private static bool _appStart = true;
        private int _index = 0;
        private bool _tabSwitch = false;
        private UserControl _searchUserControl;
        private UserControl _replaceUserControl;
        private MainViewModel _parent;
        #endregion

        #region Constructors
        public TextEditorViewModel(List<Tab> tabs, Settings settings, string editBackgroundColor, string deleteBackgroundColor, MainViewModel parent) {
            _contentChanged += TabsToTabViewModels;
            Tabs = new ObservableCollection<Tab>(tabs);
            Settings = settings;
            OpenFirstTab();
            _appStart = false;
            SetColors(editBackgroundColor, deleteBackgroundColor);
            _parent = parent;
            CreateUserControls();
        }

        private void SetColors(string editBackgroundColor, string deleteBackgroundColor) {
            EditButtonBackground = editBackgroundColor;
            DeleteButtonBackground = deleteBackgroundColor;
        }

        private void CreateUserControls() {
            _searchUserControl = (UserControl)Activator.CreateInstance(typeof(SearchView));
            _searchUserControl.DataContext = new SearchViewModel(this);
            _replaceUserControl = (UserControl)Activator.CreateInstance(typeof(ReplaceView));
            _replaceUserControl.DataContext = new ReplaceViewModel(this);
        }

        public TextEditorViewModel() {
        }
        #endregion

        #region Commands and Methods
        private bool CanExecuteCommand(object arg) {
            return true;
        }

        public ICommand RenameCommand => new RelayCommand(Rename, CanExecuteCommand);
        private void Rename(object obj) {
            throw new NotImplementedException();
        }

        public ICommand DeleteSpecificTabCommand => new RelayCommand(DeleteTab, CanExecuteCommand);
        private void DeleteTab(object obj) {
            try {
                TabViewModel tab = (TabViewModel)obj;
                DeleteTab(TabViewModels.IndexOf(TabViewModels.Where(x => x.Index == tab.Index).First()));
            } catch (Exception e) { 
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand DeleteCurrentTabCommand => new RelayCommand(DeleteCurrentTab, CanExecuteCommand);
        private void DeleteCurrentTab(object obj) {
            try {
                DeleteTab(_index);
            } catch (Exception e) { 
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        private void DeleteTab(int index) {
            try {
                if (Tabs.Count > 0) { 
                    Tabs.Remove(Tabs[index]);
                    if (Settings.CAD && File.Exists(OpenedTab.FilePath))
                        DataManager.DeleteFile(OpenedTab.FilePath);
                    else if (!Settings.CAD && File.Exists(OpenedTab.FilePath) && OpenedTab.FilePath != DataManager.GetFilePath() && OpenedTab.FilePath != DataManager.GetFilePath().Substring(0, DataManager.GetFilePath().Length - "BetterEditor".Length * 2 - 1) + "LICENSE.txt") { 
                        MessageBoxResult messageBoxResult = MessageBox.Show($"Do you want to delete the following file: {OpenedTab.TabName}", "Delete file", MessageBoxButton.YesNo);
                        if (messageBoxResult == MessageBoxResult.Yes) {
                            DataManager.DeleteFile(OpenedTab.FilePath);
                        }
                    }
                    TabViewModels.Remove(TabViewModels[index]);
                    if (TabViewModels.Count == 0) {
                        TabCounter = 0;
                        CreateNewTab(this);
                        return;
                    } else if (index == TabViewModels.Count) {
                        index--;
                    }
                    TabViewModels[index].IsActive = false;
                    OpenedTab = TabViewModels[index];
                    DataManager.WriteTabs(Tabs.ToList());
                    Settings.LOT = GetTabFromTabViewModel(OpenedTab);
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
                _index = GetCurrentIndex();

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
                        _index = GetCurrentIndex();
                        Tabs[_index].FilePath = "";
                        tempTabViewModel.FilePath = "";
                        DataManager.WriteTabs(Tabs.ToList());
                    }
                }
                OpenedTab = tempTabViewModel;
                Content = OpenedTab.Content;
                Settings.LOT = GetTabFromTabViewModel(OpenedTab);
                if (TabViewModels.Count > 0) { 
                    for (int i = 0; i < TabViewModels.Count; i++)
                        TabViewModels[i].IsActive = true;
                    TabViewModels[TabViewModels.IndexOf(OpenedTab)].IsActive = false;
                }
                DataManager.WriteSettings(Settings);
                _tabSwitch = false;
                SelectTextboxMethod();

                _index = GetCurrentIndex();

                if (_index == 0 && ScrollLeftEnd != null)
                    ScrollLeftEnd();
                else if (_index == TabViewModels.Count-1 && ScrollRightEnd != null)
                    ScrollRightEnd();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        private void TabsToTabViewModels(object sender, EventArgs args) {
            TabViewModels.Clear();
            try {
                if (Tabs.Count > 0) { 
                    TabCounter = 0;
                    foreach (Tab tab in Tabs) {
                        TabViewModels.Add(new TabViewModel(tab.FilePath, tab.Content, tab.MD, CreateTabname(tab.FilePath, tab.Content, TabCounter), true, TabCounter));
                        TabCounter++;
                    }
                    TabViewModels[TabViewModels.IndexOf(OpenedTab)].IsActive = false;
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
                        OpenedTab = TabViewModels.Last();
                        OpenTabCommand.Execute(TabViewModels.Last());
                    } else {
                        if (Tabs.Contains(Settings.LOT)) {
                            int index = Tabs.IndexOf(Settings.LOT);
                            Tabs[index].FilePath = "";
                            TabViewModels[index].FilePath = "";
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
                        TabViewModels[index].Content = Settings.LOT.Content;
                        OpenedTab.Content = Settings.LOT.Content;
                        DataManager.WriteTabs(Tabs.ToList());
                    }
                } else if (!Tabs.Contains(Settings.LOT)) {
                    Tabs = new ObservableCollection<Tab>(Tabs.Append(Settings.LOT));
                    OpenedTab = TabViewModels.Last();
                    DataManager.WriteTabs(Tabs.ToList());
                    OpenTabCommand.Execute(TabViewModels[TabViewModels.Count - 1]);
                } else {
                    int index = Tabs.IndexOf(Settings.LOT);
                    OpenedTab = TabViewModels[index];
                    OpenTabCommand.Execute(TabViewModels[index]);
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand CreateNewTabCommand => new RelayCommand(CreateNewTab, CanExecuteCommand);
        private void CreateNewTab(object obj) {
            try { 
                Tabs.Add(new Tab("", "", new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day)));
                TabViewModels.Add(new TabViewModel("", "", new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day), "", false, TabCounter++));
                OnPropertyChanged(nameof(Tabs));
                OpenedTab = TabViewModels.Last();
                OpenTabCommand.Execute(TabViewModels[TabViewModels.Count - 1]);
            } catch (Exception e) { 
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand SaveCommand => new RelayCommand(Save, CanExecuteCommand);
        private void Save(object obj) {
            try {
                DateOnly todaysDate = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
                OpenedTab.Content = Content;
                OpenedTab.TabName = CreateTabname(OpenedTab.FilePath, Content, _index);
                OpenedTab.MD = todaysDate;
                Tabs[_index].Content = Content;
                Tabs[_index].MD = todaysDate;
                _contentChanged?.Invoke(this, EventArgs.Empty);
                _parent.Tabs = Tabs.ToList();
                DataManager.WriteTabs(Tabs.ToList());
                _parent.Tabs = Tabs.ToList();
                Settings.LOT = GetTabFromTabViewModel(OpenedTab);
                DataManager.WriteSettings(Settings);
                if (File.Exists(OpenedTab.FilePath))
                    File.WriteAllText(OpenedTab.FilePath, Content);
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
                    OpenTabCommand.Execute(TabViewModels.Last());
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }
        public ICommand SaveAsCommand => new RelayCommand(SaveAs, CanExecuteCommand);
        private void SaveAs(object obj) {
            try {
                SaveCommand.Execute(TabViewModels.Last());
                using (var openFolderDialog = new System.Windows.Forms.FolderBrowserDialog()) {
                    openFolderDialog.ShowDialog();
                    if (openFolderDialog.SelectedPath != "" && Directory.Exists(openFolderDialog.SelectedPath)) { 
                        string filePath = openFolderDialog.SelectedPath + "\\";
                        if (OpenedTab.FilePath != "")
                            filePath += OpenedTab.FilePath;
                        else
                            filePath += (OpenedTab.Content.Length >= 30)? OpenedTab.Content.Substring(0, 30) + ".txt" : OpenedTab.Content + ".txt";
                        OpenedTab.FilePath = filePath;
                        Tabs[_index].FilePath = filePath;
                        TabViewModels[_index].FilePath = filePath;
                        SaveCommand.Execute(TabViewModels.Last());
                        File.WriteAllText(filePath, OpenedTab.Content);
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
                filePath = OpenedTab.FilePath;
            if (newContent == null)
                newContent = OpenedTab.Content;
            string tabName = (!_appStart && OpenedTab.Content != newContent && tabIndex == _index && !_tabSwitch) ? "*" : string.Empty;
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
                if (_index >= Tabs.Count - 1) {
                    OpenTabCommand.Execute(TabViewModels[0]);
                } else { 
                    OpenTabCommand.Execute(TabViewModels[_index + 1]);
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand OpenPreviousTabCommand => new RelayCommand(OpenPreviousTab, CanExecuteCommand);
        private void OpenPreviousTab(object obj) {
            try {
                if (_index == 0) {
                    OpenTabCommand.Execute(TabViewModels[TabViewModels.Count-1]);
                } else {
                    OpenTabCommand.Execute(TabViewModels[_index - 1]);
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
                if (UserControlVisibility == Visibility.Collapsed)
                    UserControlVisibility = Visibility.Visible;
                else if (UserControl.GetType() == _searchUserControl.GetType())
                    UserControlVisibility = Visibility.Collapsed;
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
                if (UserControlVisibility == Visibility.Collapsed)
                    UserControlVisibility = Visibility.Visible;
                else if (UserControl.GetType() == _replaceUserControl.GetType())
                    UserControlVisibility = Visibility.Collapsed;
                if (UserControl.GetType() != _replaceUserControl.GetType()) {
                    UserControl = _replaceUserControl;
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public void RequestSelectText(int start, int length) {
            SelectTextRequested?.Invoke(start, length);
        }

        public void SelectTextboxMethod() {
            SelectTextbox?.Invoke();
        }

        private int GetCurrentIndex()
            => TabViewModels.IndexOf(TabViewModels.Where(x => x.Index == OpenedTab.Index).First());

        private void ContentUpdated(string value) {
            _index = GetCurrentIndex();
            Tabs[_index].Content = value;
            _contentChanged?.Invoke(this, EventArgs.Empty);
            OpenedTab.Content = value;
            _staticTabs = new ObservableCollection<Tab>(Tabs);
            Settings.LOT = GetTabFromTabViewModel(OpenedTab);
            _staticSettings = Settings;
            if (Settings.SA)
                _timer.Change(10000, Timeout.Infinite);
        }
        #endregion
    }
}
