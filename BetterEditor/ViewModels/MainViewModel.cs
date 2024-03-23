using BetterEditor.Views;
using BetterEditor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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

        public MainViewModel() {
            ChangeUserControlCommand.Execute(this);
        }

        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand ChangeUserControlCommand => new RelayCommand(ChangeUserControl, CanExecuteCommand);
        private void ChangeUserControl(object obj) {
            if (UserControl.DataContext == null || UserControl.GetType() == _listTabsViewType) {
                UserControl = (UserControl)Activator.CreateInstance(_textEditorViewType);
                UserControl.DataContext = TextEditorViewModel;
            } else {
                UserControl = (UserControl)Activator.CreateInstance(_listTabsViewType);
                UserControl.DataContext = ListTabsViewModel;
            }
        }
    }
}
