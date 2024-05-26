using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabTrainer.Models;

namespace BetterEditor.ViewModels {
    internal class SearchViewModel {
        public string BackgroundColorGrid { get; set; }
        public string BackgroundColorTextbox { get; set; }
        public string ForegroundColorTextbox { get; set; }
        public bool SAT { get; set; }
        private TextEditorViewModel _parent;
        public SearchViewModel(TextEditorViewModel parent) { 
            _parent = parent;
        }

        private bool CanExecuteCommand(object arg) {
            return true;
        }

        public ICommand ReplaceViewCommand => new RelayCommand(ReplaceView, CanExecuteCommand);
        private void ReplaceView(object obj) {
            try {
                throw new NotImplementedException();
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

        public ICommand CloseSearchBarCommand => new RelayCommand(CloseSearchBar, CanExecuteCommand);
        private void CloseSearchBar(object obj) {
            try {
                throw new NotImplementedException();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }
    }
}
