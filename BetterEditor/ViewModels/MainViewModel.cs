using BetterEditor.Views;
using BetterEditor.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using VocabTrainer.Models;
using System.Windows;

namespace BetterEditor.ViewModels {
    internal class MainViewModel : BaseViewModel{
        private UserControl _userControl = new UserControl();
        public UserControl UserControl {
            get => _userControl;
            set {
                _userControl = value;
                OnPropertyChanged(nameof(UserControl));
            }
        }
        private Type _textEditorViewType = typeof(TextEditorView);
        private TextEditorViewModel _textEditorViewModel;
        public TextEditorViewModel TextEditorViewModel { 
            get => _textEditorViewModel;
            set {
                _textEditorViewModel = value; 
                OnPropertyChanged(nameof(TextEditorViewModel));
            }
        }
        private Type _listTabsViewType = typeof(ListTabsView);
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

        private string _editButtonBackground = "#D0CEE2";
        private string _deleteButtonBackground = "#FECAC6";

        public MainViewModel() {
            try { 
                TextEditorViewModel = new TextEditorViewModel(Tabs, Settings, _editButtonBackground, _deleteButtonBackground, this);
                ListTabsViewModel = new ListTabsViewModel();
                ChangeUserControlCommand.Execute(this);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand ChangeUserControlCommand => new RelayCommand(ChangeUserControl, CanExecuteCommand);
        private void ChangeUserControl(object obj) {
            try {
                if (UserControl.DataContext == null || UserControl.GetType() == _listTabsViewType) {
                    UserControl = (UserControl)Activator.CreateInstance(_textEditorViewType);
                    UserControl.DataContext = TextEditorViewModel;
                } else {
                    TextEditorViewModel.SaveCommand.Execute(this);
                    UserControl = (UserControl)Activator.CreateInstance(_listTabsViewType);
                    UserControl.DataContext = ListTabsViewModel;
                }
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
                Settings.FontSize -= 2;
                TextEditorViewModel.Settings = Settings;
                DataManager.WriteSettings(Settings);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand CloseAppCommand => new RelayCommand(CloseApp, CanExecuteCommand);
        private void CloseApp(object obj) {
            try {
                TextEditorViewModel.SaveCommand.Execute(this);
                DataManager.WriteSettings(Settings);
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
                throw new NotImplementedException();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand SaveAutomaticallyCommand => new RelayCommand(SaveAutomatically, CanExecuteCommand);
        private void SaveAutomatically(object obj) {
            try {
                throw new NotImplementedException();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }
        public ICommand CADCommand => new RelayCommand(CAD, CanExecuteCommand);
        private void CAD(object obj) {
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
                throw new NotImplementedException();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }
    }
}
