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
    }
}
