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
        public string _searchText = string.Empty;
        public string SearchText {
            get => _searchText;
            set {
                _searchText = value;
                _textChanged = true;    
            } 
        }
        private bool _textChanged = false;
        public bool SAT { get; set; }
        private TextEditorViewModel _parent;
        private ObservableCollection<TabViewModel> _tabs;
        private int _openedMatch = 0;
        private ObservableCollection<int> _matchingTabs = new ObservableCollection<int>();
        private TabViewModel _tab;
        public SearchViewModel(TextEditorViewModel parent) { 
            _parent = parent;
            _tabs = _parent.UsedTabs;
            _tab = _parent.Tab;
            BackgroundColorGrid = _parent.Settings.SVM.BGT;
            BackgroundColorTextbox = _parent.Settings.SVM.BGTE;
            ForegroundColorTextbox = _parent.Settings.SVM.Foreground;
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
                if (SearchText != string.Empty) { 
                    if (_textChanged) {
                        GetFilteredTabIds();
                        _openedMatch = 0;
                        _textChanged = false;
                    } else if (_openedMatch == _matchingTabs.Count)
                        _openedMatch = 0;
                    _parent.OpenTabCommand.Execute(_parent.UsedTabs[_matchingTabs[_openedMatch]]);
                    _openedMatch++;
                }
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
                _parent.OpenSearchViewCommand.Execute(this);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        private void GetFilteredTabIds() {
            bool matchesChanged = false;
            for (int i = 0; i < _tabs.Count; i++) {
                if (_tabs[i].Content.Contains(SearchText)) { 
                    _matchingTabs.Add(_tabs[i].Index);
                    matchesChanged = true;
                }
            }
            if (!matchesChanged)
                _matchingTabs = new ObservableCollection<int>();
        }
    }
}
