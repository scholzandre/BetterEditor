using BetterEditor.Views;
using BetterEditor.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using VocabTrainer.Models;

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
        private TextEditorViewModel _textEditorViewModel = new TextEditorViewModel();
        public TextEditorViewModel TextEditorViewModel { 
            get => _textEditorViewModel;
            set {
                _textEditorViewModel = value; 
                OnPropertyChanged(nameof(TextEditorViewModel));
            }
        }
        private Type _listTabsViewType = typeof(ListTabsView);
        private ListTabsViewModel _listTabsView = new ListTabsViewModel();
        public ListTabsViewModel ListTabsViewModel {
            get => _listTabsView;
            set {
                _listTabsView = value;
                OnPropertyChanged(nameof(ListTabsViewModel));
            }
        }

        public List<ViewMode> ViewModes { get; set; } = DataManager.GetViewModes();
        private Settings _settings = DataManager.GetSettings();
        public Settings Settings { 
            get => _settings;
            set {
                _settings = value; 
                OnPropertyChanged(nameof(Settings));
            }
        }
        
        public MainViewModel() {
            ChangeUserControlCommand.Execute(this);
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
                DataManager dataManager = DataManager.GetData();
                dataManager.Settings = Settings;
                DataManager.WriteData(dataManager);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }
    }
}
