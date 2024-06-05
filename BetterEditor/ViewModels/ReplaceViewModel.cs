using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using VocabTrainer.Models;

namespace BetterEditor.ViewModels {
    internal class ReplaceViewModel {
        public string BackgroundColorGrid { get; set; }
        public string BackgroundColorTextbox { get; set; }
        public string ForegroundColorTextbox { get; set; }
        public string SearchText { get; set; }
        private bool _textChanged = false;
        public bool SAT { get; set; }
        TextEditorViewModel _parent;
        public ReplaceViewModel(TextEditorViewModel parent) { 
            _parent = parent;
            BackgroundColorGrid = _parent.Settings.SVM.BGT;
            BackgroundColorTextbox = _parent.Settings.SVM.BGTE;
            ForegroundColorTextbox = _parent.Settings.SVM.Foreground;
        }
        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand SearchViewCommand => new RelayCommand(SearchView, CanExecuteCommand);
        private void SearchView(object obj) {
            try {
                _parent.OpenSearchViewCommand.Execute(obj);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand SearchCommand => new RelayCommand(Search, CanExecuteCommand);
        private void Search(object obj) {
            try {
                throw new NotImplementedException();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand SearchNextCommand => new RelayCommand(SearchNext, CanExecuteCommand);
        private void SearchNext(object obj) {
            try {
                throw new NotImplementedException();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand SearchPreviousCommand => new RelayCommand(SearchPrevious, CanExecuteCommand);
        private void SearchPrevious(object obj) {
            try {
                throw new NotImplementedException();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand CloseReplaceBarCommand => new RelayCommand(CloseSearchBar, CanExecuteCommand);
        private void CloseSearchBar(object obj) {
            try {
                _parent.OpenReplaceViewCommand.Execute(this);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand ReplaceCommand => new RelayCommand(Replace, CanExecuteCommand);
        private void Replace(object obj) {
            try {
                throw new NotImplementedException();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand ReplaceAllCommand => new RelayCommand(ReplaceAll, CanExecuteCommand);
        private void ReplaceAll(object obj) {
            try {
                throw new NotImplementedException();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }
    }
}
