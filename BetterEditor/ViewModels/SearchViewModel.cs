using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public string SearchText { get; set; }
        private bool _textChanged = false;
        public bool SAT { get; set; }
        private TextEditorViewModel _parent;
        private ObservableCollection<TabViewModel> _tabs;
        private ObservableCollection<int> _matchingTabs;
        private TabViewModel _tab;
        public SearchViewModel(TextEditorViewModel parent) { 
            _parent = parent;
            _tabs = _parent.UsedTabs;
            _tab = _parent.Tab; 
        }

        private bool CanExecuteCommand(object arg) {
            return true;
        }

        public ICommand ReplaceViewCommand => new RelayCommand(ReplaceView, CanExecuteCommand);
        private void ReplaceView(object obj) {
            try {
                _parent.OpenReplaceViewCommand.Execute(obj);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand SearchCommand => new RelayCommand(Search, CanExecuteCommand);
        private void Search(object obj) {
            try {
                if (_textChanged)
                    GetFilteredTabIds();
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

        private void GetFilteredTabIds() { 
            for (int i = 0; i < _tabs.Count; i++) {
                if (_tabs[i].Content.Contains(SearchText))
                    _matchingTabs.Add(_tabs[i].Index);
            }
        }
    }
}
