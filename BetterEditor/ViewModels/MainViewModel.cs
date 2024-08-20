using BetterEditor.Views;
using BetterEditor.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Linq;

namespace BetterEditor.ViewModels {
    internal class MainViewModel : BaseViewModel {

        #region Properties
        private UserControl _userControl = new UserControl();
        public UserControl UserControl {
            get => _userControl;
            set {
                _userControl = value;
                OnPropertyChanged(nameof(UserControl));
            }
        }
        private UserControl _textEditorUserControl = new UserControl();
        private TextEditorViewModel _textEditorViewModel;
        public TextEditorViewModel TextEditorViewModel { 
            get => _textEditorViewModel;
            set {
                _textEditorViewModel = value; 
                OnPropertyChanged(nameof(TextEditorViewModel));
            }
        }

        private UserControl _listTabsUserControl = new UserControl();
        private ListTabsViewModel _listTabsView;
        public ListTabsViewModel ListTabsViewModel {
            get => _listTabsView;
            set {
                _listTabsView = value;
                OnPropertyChanged(nameof(ListTabsViewModel));
            }
        }

        public List<ViewMode> ViewModes { get; set; } = DataManager.GetViewModes();
        public List<Tab> Tabs { get; set; } = DataManager.GetTabs();
        private Settings _settings = DataManager.GetSettings();
        public Settings Settings { 
            get => _settings;
            set {
                _settings = value; 
                OnPropertyChanged(nameof(Settings));
            }
        }

        private string _changeSizeButton = string.Empty;
        public string ChangeSizeButton {
            get => _changeSizeButton;
            set {
                _changeSizeButton = value;
                OnPropertyChanged(nameof(ChangeSizeButton));
            }
        }

        private bool _textEditorOpened = false;
        public bool TextEditorOpened { 
            get => _textEditorOpened;
            set {
                _textEditorOpened = value;
                OnPropertyChanged(nameof(TextEditorOpened));
            }
        }
        #endregion

        #region Fields
        private string _editButtonBackground = "#D0CEE2";
        private string _deleteButtonBackground = "#FECAC6";
        public event Action UndoTextbox;
        public event Action RedoTextbox;
        #endregion

        #region Constructors
        public MainViewModel() {
            try {
                CreateTextEditorView();
                CreateListTabsView();
                ChangeUserControlCommand.Execute(this);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }
        #endregion

        #region Commands and Methods
        private void CreateTextEditorView() {
            TextEditorViewModel = new TextEditorViewModel(Tabs, Settings, _editButtonBackground, _deleteButtonBackground, this);
            TextEditorView textEditorView = new TextEditorView();
            textEditorView.DataContext = TextEditorViewModel;
            TextEditorViewModel.SelectTextRequested += textEditorView.OnSelectText;
            TextEditorViewModel.SelectTextbox += textEditorView.OnFocusTextBox;
            TextEditorViewModel.ScrollLeftEnd += textEditorView.OnMoveScrollbarLeft;
            TextEditorViewModel.ScrollRightEnd += textEditorView.OnMoveScrollbarRight;
            UndoTextbox += textEditorView.OnUndoChange;
            RedoTextbox += textEditorView.OnRedoChange;
            _textEditorUserControl.Content = textEditorView;
        }

        private void CreateListTabsView() {
            ListTabsViewModel = new ListTabsViewModel(this);
            ListTabsView listTabsView = new ListTabsView();
            listTabsView.DataContext = ListTabsViewModel;
            _listTabsUserControl.Content = listTabsView;
        }

        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand ChangeUserControlCommand => new RelayCommand(ChangeUserControl, CanExecuteCommand);
        private void ChangeUserControl(object obj) {
            try {
                if (UserControl.DataContext == null || !TextEditorOpened) {
                    OpenTextEditorUserControlCommand.Execute(this);
                } else {
                    OpenAdvancedSearchUserControlCommand.Execute(this);
                }
            } catch (Exception e) { 
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand OpenTextEditorUserControlCommand => new RelayCommand(OpenTextEditorUserControl, CanExecuteCommand);
        private void OpenTextEditorUserControl(object obj) {
            try {
                UserControl = _textEditorUserControl;
                TextEditorOpened = true;
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand OpenAdvancedSearchUserControlCommand => new RelayCommand(OpenAdvancedSearchUserControl, CanExecuteCommand);
        private void OpenAdvancedSearchUserControl(object obj) {
            try {
                UserControl = _listTabsUserControl;
                Tabs = TextEditorViewModel.Tabs.ToList();
                ListTabsViewModel.UpdateAdvancedSearch();
                TextEditorOpened = false;
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }


        public ICommand ChangeViewModeCommand => new RelayCommand(ChangeViewMode, CanExecuteCommand);
        private void ChangeViewMode(object obj) {
            try {
                Settings settings = Settings;
                settings.SVM = (ViewMode)obj;
                Settings = settings;
                TextEditorViewModel.Settings = Settings;
                DataManager.WriteSettings(Settings);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand SaveCommand => new RelayCommand(Save, CanExecuteCommand);
        private void Save(object obj) {
            try {
                TextEditorViewModel.SaveCommand.Execute(obj);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand OpenFileCommand => new RelayCommand(OpenFile, CanExecuteCommand);
        private void OpenFile(object obj) {
            try {
                TextEditorViewModel.OpenFileCommand.Execute(obj);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand ZoomInCommand => new RelayCommand(ZoomIn, CanExecuteCommand);
        private void ZoomIn(object obj) {
            try {
                Settings.FontSize += 2;
                TextEditorViewModel.Settings = Settings;
                DataManager.WriteSettings(Settings);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand ZoomOutCommand => new RelayCommand(ZoomOut, CanExecuteCommand);
        private void ZoomOut(object obj) {
            try {
                if (Settings.FontSize > 0) { 
                    Settings.FontSize = (Settings.FontSize - 2 > 0)? Settings.FontSize - 2 : 2;
                    TextEditorViewModel.Settings = Settings;
                    DataManager.WriteSettings(Settings);
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand CloseAppCommand => new RelayCommand(CloseApp, CanExecuteCommand);
        private void CloseApp(object obj) {
            try {
                TextEditorViewModel.SaveCommand.Execute(this);
                Application.Current.Shutdown();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand CloseTabCommand => new RelayCommand(CloseTab, CanExecuteCommand);
        private void CloseTab(object obj) {
            try {
                TextEditorViewModel.DeleteCurrentTabCommand.Execute(this);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand OpenNewTabCommand => new RelayCommand(OpenNewTab, CanExecuteCommand);
        private void OpenNewTab(object obj) {
            try {
                TextEditorViewModel.CreateNewTabCommand.Execute(this);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand SaveAsCommand => new RelayCommand(SaveAs, CanExecuteCommand);
        private void SaveAs(object obj) {
            try {
                TextEditorViewModel.SaveAsCommand.Execute(DataManager.GetFilePath());
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand SaveAutomaticallyCommand => new RelayCommand(SaveSettings, CanExecuteCommand);
        public ICommand CADCommand => new RelayCommand(SaveSettings, CanExecuteCommand);
        private void SaveSettings(object obj) {
            try {
                TextEditorViewModel.Settings = Settings;
                DataManager.WriteSettings(Settings);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand OpenSettingsFileCommand => new RelayCommand(OpenSettingsFile, CanExecuteCommand);
        private void OpenSettingsFile(object obj) {
            try {
                TextEditorViewModel.OpenFileCommand.Execute(DataManager.GetFilePath());
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand OpenLicenseFileCommand => new RelayCommand(OpenLicenseFile, CanExecuteCommand);
        private void OpenLicenseFile(object obj) {
            try {
                string filePath = DataManager.GetFolderPath();
                filePath = filePath.Substring(0,filePath.Length - "BetterEditor".Length-1) + "LICENSE.txt";
                TextEditorViewModel.OpenFileCommand.Execute(filePath);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand SwitchViewModeCommand => new RelayCommand(SwitchViewMode, CanExecuteCommand);
        private void SwitchViewMode(object obj) {
            try {
                int index = ViewModes.IndexOf(Settings.SVM);
                if (index != -1 && index != ViewModes.Count-1) {
                    Settings.SVM = ViewModes[index+1];
                } else { 
                    Settings.SVM = ViewModes[0];
                }
                OnPropertyChanged(nameof(Settings));
                TextEditorViewModel.Settings = Settings;
                DataManager.WriteSettings(Settings);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand CreateNewTabCommand => new RelayCommand(CreateNewTab, CanExecuteCommand);
        private void CreateNewTab(object obj) {
            try {
                TextEditorViewModel.CreateNewTabCommand.Execute(this);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand OpenNextTabCommand => new RelayCommand(OpenNextTab, CanExecuteCommand);
        private void OpenNextTab(object obj) {
            try {
                TextEditorViewModel.OpenNextTabCommand.Execute(this);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand DeleteCurrentTabCommand => new RelayCommand(DeleteCurrentTab, CanExecuteCommand);
        private void DeleteCurrentTab(object obj) {
            try {
                TextEditorViewModel.DeleteCurrentTabCommand.Execute(this);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand OpenSearchViewCommand => new RelayCommand(OpenSearch, CanExecuteCommand);
        private void OpenSearch(object obj) {
            try {
                TextEditorViewModel.OpenSearchViewCommand.Execute(this);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand UndoCommand => new RelayCommand(Undo, CanExecuteCommand);
        private void Undo(object obj) {
            try {
                UndoTextbox?.Invoke();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand RedoCommand => new RelayCommand(Redo, CanExecuteCommand);
        private void Redo(object obj) {
            try {
                RedoTextbox?.Invoke();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand OpenReplaceViewCommand => new RelayCommand(OpenReplace, CanExecuteCommand);
        private void OpenReplace(object obj) {
            try {
                TextEditorViewModel.OpenReplaceViewCommand.Execute(this);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand OpenTextEditorCommand => new RelayCommand(OpenTextEditor, CanExecuteCommand);
        private void OpenTextEditor(object obj) {
            try {
                if (UserControl.DataContext != null) {
                    UserControl = _textEditorUserControl;
                    TextEditorOpened = true;
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand OpenTabsOverviewCommand => new RelayCommand(OpenTabsOverview, CanExecuteCommand);
        private void OpenTabsOverview(object obj) {
            try {
                if (UserControl.DataContext != null) {
                    TextEditorOpened = false;
                    UserControl = _listTabsUserControl;
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand OpenProgramInfoCommand => new RelayCommand(OpenProgramInfo, CanExecuteCommand);
        private void OpenProgramInfo(object obj) {
            try {
                string filePath = DataManager.GetFolderPath();
                filePath = filePath.Substring(0, filePath.Length - "BetterEditor".Length - 1) + "README.md";
                TextEditorViewModel.OpenFileCommand.Execute(filePath);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }
        #endregion
    }
}
