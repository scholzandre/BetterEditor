using BetterEditor.Models;
using BetterEditor.Views;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace BetterEditor.ViewModels {
    internal class ListTabsViewModel : BaseViewModel {
        #region Properties
        private string _searchText = string.Empty;
        public string SearchText {
            get => _searchText;
            set {
                _searchText = value.Trim();
                OnPropertyChanged(nameof(SearchText));
            }
        }
        private List<Tab> _tabs = new List<Tab>();
        public List<Tab> Tabs {
            get => _tabs;
            set {
                _tabs = value;
                OnPropertyChanged(nameof(Tabs));
            }
        }
        private Dictionary<string, bool> _fileTypes = new Dictionary<string, bool>();
        public Dictionary<string, bool> FileTypes {
            get => _fileTypes;
            set {
                _fileTypes = value;
                OnPropertyChanged(nameof(FileTypes));
            }
        }

        private bool _sortInAppOrder = true;
        public bool SortInAppOrder {
            get => _sortInAppOrder;
            set {
                _sortInAppOrder = value;
                if (value) {
                    SortAlpbabetically = false;
                    SortByDataType = false;
                    SortByModicifcationDate = false;
                    TabViewModels = new ObservableCollection<TabViewModel>(TabViewModels.ToList().OrderBy(x => x.Index));
                }
                OnPropertyChanged(nameof(SortInAppOrder));
            }
        }

        private bool _sortAlpbabetically;
        public bool SortAlpbabetically {
            get => _sortAlpbabetically;
            set {
                _sortAlpbabetically = value;
                if (value) {
                    SortInAppOrder = false;
                    SortByDataType = false;
                    SortByModicifcationDate = false;
                    TabViewModels = new ObservableCollection<TabViewModel>(TabViewModels.ToList().OrderBy(x => (SearchTabContents && !SearchTabNames)? x.Content : x.TabName));
                }
                OnPropertyChanged(nameof(SortAlpbabetically));
            }
        }

        private bool _sortByModicifcationDate;
        public bool SortByModicifcationDate {
            get => _sortByModicifcationDate;
            set {
                _sortByModicifcationDate = value;
                if (value) {
                    SortInAppOrder = false;
                    SortByDataType = false;
                    SortAlpbabetically = false;
                    TabViewModels = new ObservableCollection<TabViewModel>(TabViewModels.ToList().OrderBy(x => x.MD));
                }
                OnPropertyChanged(nameof(SortByModicifcationDate));
            }
        }

        private bool _sortByDataType;
        public bool SortByDataType {
            get => _sortByDataType;
            set {
                _sortByDataType = value;
                if (value) {
                    SortInAppOrder = false;
                    SortByModicifcationDate = false;
                    SortAlpbabetically = false;
                    TabViewModels = new ObservableCollection<TabViewModel>(TabViewModels.ToList().OrderBy(x => Path.GetExtension(x.FilePath)));
                }
                OnPropertyChanged(nameof(SortByDataType));
            }
        }

        private bool _searchTabNames = true;
        public bool SearchTabNames {
            get => _searchTabNames;
            set {
                _searchTabNames = value;
                if (value)
                    SearchTabContents = false;
                OnPropertyChanged(nameof(SearchTabNames));
            }
        }

        private bool _searchTabContents;
        public bool SearchTabContents {
            get => _searchTabContents;
            set {
                _searchTabContents = value;
                if (value)
                    SearchTabNames = false;
                OnPropertyChanged(nameof(SearchTabContents));
            }
        }

        private ObservableCollection<TabViewModel> _tabViewModels = new ObservableCollection<TabViewModel>();
        public ObservableCollection<TabViewModel> TabViewModels {
            get => _tabViewModels;
            set {
                _tabViewModels = value;
                OnPropertyChanged(nameof(TabViewModels));
            }
        }

        public int TabCounter { get; set; } = 0;
        #endregion

        #region Fields
        private MainViewModel _parent;
        private Action<object> _save;
        #endregion

        #region Constructors
        public ListTabsViewModel(MainViewModel parent, Action<object> save) { 
            _parent = parent;
            _save = save;
            Tabs = _parent.Tabs;
            GetFileTypes();
        }
        public ListTabsViewModel() { }
        #endregion

        #region Commands and methods
        public void UpdateAdvancedSearch() {
            Tabs = _parent.Tabs;
            TabsToTabViewModels();
            GetFileTypes();
        }

        private void GetFileTypes() {
            FileTypes = new Dictionary<string, bool> ();
            for (int i = 0; i < Tabs.Count; i++) {
                if (Tabs[i].FilePath == string.Empty && !FileTypes.Keys.Contains("None")) {
                    FileTypes.Add("None", true);
                } else if (Tabs[i].FilePath != string.Empty && !FileTypes.Keys.Contains(Path.GetExtension(Tabs[i].FilePath))) { 
                    FileTypes.Add(Path.GetExtension(Tabs[i].FilePath), true);
                }
            }
            OnPropertyChanged(nameof(FileTypes));
        }

        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand ChangeUserControlCommand => new RelayCommand(ChangeUserControl, CanExecuteCommand);
        private void ChangeUserControl(object obj) {
            try {
                _parent.OpenTextEditorUserControlCommand.Execute(null);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand SearchTabsCommand => new RelayCommand(SearchTabs, CanExecuteCommand);
        private void SearchTabs(object obj) {
            try {
                TabsToTabViewModels();
                for (int i = TabViewModels.Count - 1; i >= 0; i--)
                    if (!TabViewModels[i].FilePath.ToLower().Contains(SearchText.ToLower().Trim()) && !TabViewModels[i].Content.ToLower().Contains(SearchText.ToLower().Trim()))
                        TabViewModels.Remove(TabViewModels[i]);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand OpenTabCommand => new RelayCommand(OpenTab, CanExecuteCommand);
        private void OpenTab(object obj) {
            try {
                _parent.TextEditorViewModel.OpenTabCommand.Execute(obj);
                _parent.OpenTextEditorUserControlCommand.Execute(obj);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand CloseTabCommand => new RelayCommand(CloseTab, CanExecuteCommand);
        private void CloseTab(object obj) {
            try {
                TabViewModel tabViewModel = (TabViewModel)obj;
                TabViewModels.Remove(TabViewModels[tabViewModel.Index]);
                Tabs.Remove(Tabs[tabViewModel.Index]);
                _parent.TextEditorViewModel.Tabs.Remove(Tabs[tabViewModel.Index]);
                _parent.TextEditorViewModel.TabViewModels.Remove(tabViewModel);
                if (tabViewModel.FilePath != "" &&
                    tabViewModel.FilePath != string.Empty &&
                    tabViewModel.FilePath != null && 
                    File.Exists(tabViewModel.FilePath) &&
                    _parent.Settings.CAD == true)
                    File.Delete(tabViewModel.FilePath);
                DataManager.WriteTabs(Tabs);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        private void TabsToTabViewModels() {
            TabViewModels.Clear();
            try {
                if (Tabs.Count > 0) {
                    TabCounter = 0;
                    foreach (Tab tab in Tabs) {
                        TabViewModels.Add(new TabViewModel(tab.FilePath, tab.Content, tab.MD, CreateTabname(tab.FilePath, tab.Content), true, TabCounter));
                        TabCounter++;
                    }
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public string CreateTabname(string filePath, string newContent) {
            string tabName = string.Empty;
            try {
                if (File.Exists(filePath)) {
                    tabName += filePath.Substring(filePath.LastIndexOf("\\") + 1);
                } else {
                    tabName = (newContent.Length > 30) ? newContent.Substring(0, 30) : newContent;
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
            return tabName;
        }

        public ICommand SearchDataTypeCommand => new RelayCommand(SearchDataType, CanExecuteCommand);
        private void SearchDataType(object obj) {
            try {
                TabsToTabViewModels();
                FileTypes[(string)obj] = !FileTypes[(string)obj];
                for (int i = TabViewModels.Count-1; i >= 0; i--) {
                    if (TabViewModels[i].FilePath != "" &&
                        FileTypes.Keys.Contains(Path.GetExtension(TabViewModels[i].FilePath)) &&
                        FileTypes[Path.GetExtension(TabViewModels[i].FilePath)] == false)
                        TabViewModels.Remove(TabViewModels[i]);
                    else if (TabViewModels[i].FilePath == "" && 
                        FileTypes["None"] == false)
                        TabViewModels.Remove(TabViewModels[i]);
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand RenameTabCommand => new RelayCommand(RenameTab, CanExecuteCommand);
        private void RenameTab(object obj) {
            try {
                if (obj is TabViewModel tabViewModel) {
                    RenameTabView renameTabView = new RenameTabView();
                    renameTabView.DataContext = new RenameTabViewModel(tabViewModel.FilePath, tabViewModel.Index, Tabs, TabsToTabViewModels, renameTabView.Close, _save);
                    renameTabView.Show();
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        private bool CanExecuteOpenFilePathCommand(object arg) {
            TabViewModel tabViewModel = (TabViewModel)arg;
            return tabViewModel.FilePath != "" && tabViewModel.FilePath != null && tabViewModel.FilePath != string.Empty;
        }

        public ICommand OpenFilePathCommand => new RelayCommand(OpenFilePath, CanExecuteOpenFilePathCommand);
        private void OpenFilePath(object obj) {
            try {
                if (obj is TabViewModel tabViewModel) {
                    if (File.Exists(tabViewModel.FilePath)) {
                        Process.Start("explorer.exe", Path.GetDirectoryName(tabViewModel.FilePath));
                    }
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }
        #endregion
    }
}
