using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using BetterEditor.Models;

namespace BetterEditor.ViewModels {
    internal class SearchViewModel {
        public string BackgroundColorGrid { get; set; }
        public string BackgroundColorTextbox { get; set; }
        public string ForegroundColorTextbox { get; set; }
        public string _searchText = string.Empty;
        public string SearchText {
            get => _searchText;
            set {
                _searchText = value.Trim();
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
        private string _noMatchesText = "No further matches found";
        private bool _tabsChangedSearch = false;
        private bool _tabsChangedSearchNext = false;
        private bool _tabsChangedSearchPrevious = false;
        private int _prevTabsCount = 0;

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
                    CheckTabs();
                    if (_textChanged || _tabsChangedSearch) {
                        GetFilteredTabIds();
                        _openedMatch = 0;
                        _textChanged = false;
                        _tabsChangedSearch = false;
                    } else if (_openedMatch == _matchingTabs.Count)
                        _openedMatch = 0;
                    if (_matchingTabs.Count > 0) { 
                        _parent.OpenTabCommand.Execute(_parent.UsedTabs[_matchingTabs[_openedMatch]]);
                        _parent.RequestSelectText(_parent.UsedTabs[_matchingTabs[_openedMatch]].Content.IndexOf(SearchText), SearchText.Length);
                        _openedMatch++;
                    }
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand SearchNextCommand => new RelayCommand(SearchNext, CanExecuteCommand);
        private void SearchNext(object obj) {
            try {
                if (SearchText != string.Empty) {
                    CheckTabs();
                    if (_textChanged || _tabsChangedSearchNext) {
                        GetFilteredTabIds();
                        _openedMatch = 0;
                        _textChanged = false;
                        _tabsChangedSearchNext = false;
                    } else if (_openedMatch == _matchingTabs.Count)
                        MessageBox.Show(_noMatchesText);
                    if (_matchingTabs.Count > 0 && _openedMatch < _matchingTabs.Count) {
                        _parent.OpenTabCommand.Execute(_parent.UsedTabs[_matchingTabs[_openedMatch]]);
                        _parent.RequestSelectText(_parent.UsedTabs[_matchingTabs[_openedMatch]].Content.IndexOf(SearchText), SearchText.Length);
                        _openedMatch++;
                    }
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand SearchPreviousCommand => new RelayCommand(SearchPrevious, CanExecuteCommand);
        private void SearchPrevious(object obj) {
            try {
                if (SearchText != string.Empty) {
                    CheckTabs();
                    if (_textChanged || _tabsChangedSearchPrevious) {
                        GetFilteredTabIds();
                        _openedMatch = _matchingTabs.Count-1;
                        _textChanged = false;
                        _tabsChangedSearchPrevious = false;
                    } else if (_openedMatch < 0)
                        MessageBox.Show(_noMatchesText);
                    if (_matchingTabs.Count > 0 && _openedMatch >= 0) {
                        _parent.OpenTabCommand.Execute(_parent.UsedTabs[_matchingTabs[_openedMatch]]);
                        _parent.RequestSelectText(_parent.UsedTabs[_matchingTabs[_openedMatch]].Content.IndexOf(SearchText), SearchText.Length);
                        _openedMatch--;
                    }
                }
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

        private void CheckTabs() {
            if (_tabs.Count != _prevTabsCount) { 
                _prevTabsCount = _tabs.Count;
                _tabsChangedSearch = true;
                _tabsChangedSearchNext = true;
                _tabsChangedSearchPrevious = true; 
            }
        }
    }
}
