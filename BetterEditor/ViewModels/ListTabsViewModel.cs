using BetterEditor.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
        private List<string> _fileTypes = new List<string>();
        public List<string> FileTypes {
            get => _fileTypes;
            set {
                _fileTypes = value;
                OnPropertyChanged(nameof(FileTypes));
            }
        }
        #endregion
        #region Fields
        private MainViewModel _parent;
        #endregion
        #region Constructors
        public ListTabsViewModel(MainViewModel parent) { 
            _parent = parent;
            Tabs = _parent.Tabs;
            GetFileTypes();
        }
        public ListTabsViewModel() { }
        #endregion
        #region Commands and methods
        public void UpdateAdvancedSearch() {
            Tabs = _parent.Tabs;
            GetFileTypes();
        }

        private void GetFileTypes() {
            for (int i = 0; i < Tabs.Count; i++) {
                if (Tabs[i].FilePath == string.Empty && !FileTypes.Contains("None")) {
                    FileTypes.Add("None");
                } else if (!FileTypes.Contains(Path.GetExtension(Tabs[i].FilePath))) { 
                    FileTypes.Add(Path.GetExtension(Tabs[i].FilePath));
                }
            }
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
                throw new NotImplementedException();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand OpenTabCommand => new RelayCommand(OpenTab, CanExecuteCommand);
        private void OpenTab(object obj) {
            try {
                throw new NotImplementedException();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand CloseTabCommand => new RelayCommand(CloseTab, CanExecuteCommand);
        private void CloseTab(object obj) {
            try {
                throw new NotImplementedException();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }
        #endregion
    }
}
