using BetterEditor.Models;
using System;
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
        #endregion
        #region Fields
        private MainViewModel _parent;
        #endregion
        #region Constructors
        public ListTabsViewModel(MainViewModel parent) { 
            _parent = parent;
        }
        public ListTabsViewModel() { }
        #endregion
        #region Commands and methods
        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand ChangeUserControlCommand => new RelayCommand(ChangeUserControl, CanExecuteCommand);
        private void ChangeUserControl(object obj) {
            try {
                _parent.ChangeUserControlCommand.Execute(null);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand SearchTabsCommand => new RelayCommand(SearchTabs, CanExecuteCommand);
        private void SearchTabs(object obj) {
            try {
                throw new NotImplementedException();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }
        #endregion
    }
}
