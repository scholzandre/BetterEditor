using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using BetterEditor.Models;

namespace BetterEditor.ViewModels {
    internal class SearchViewModel {

        #region Properties
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
        public bool SAT { get; set; }
        #endregion

        #region Fields
        private bool _textChanged = false;
        private TextEditorViewModel _parent;
        private ObservableCollection<TabViewModel> _tabs;
        private List<List<int>> _tabMatchings = new List<List<int>>();
        private int _openedMatchingTab = 0;
        private int _openedMatch = 0;
        private ObservableCollection<int> _matchingTabs = new ObservableCollection<int>();
        private string _noMatchesText = "No further matches found";
        private bool _tabsChangedSearch = false;
        private bool _tabsChangedSearchNext = false;
        private bool _tabsChangedSearchPrevious = false;
        private int _prevTabsCount = 0;
        #endregion

        #region Constructors
        public SearchViewModel(TextEditorViewModel parent) {
            _parent = parent;
            _tabs = _parent.TabViewModels;
            SetColors();
        }

        public SearchViewModel() { }
        #endregion

        #region Commands and Methods
        private void SetColors() {
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
                        _openedMatchingTab = 0;
                        _openedMatch = 0;
                        _textChanged = false;
                        _tabsChangedSearch = false;
                    } else if (_openedMatchingTab == _matchingTabs.Count-1) { 
                        _openedMatchingTab = 0;
                        _openedMatch = 0;
                        _parent.OpenTabCommand.Execute(_parent.TabViewModels[_matchingTabs[_openedMatchingTab]]);
                    }
                    if (_matchingTabs.Count > 0) { 
                        if (_openedMatch < _tabMatchings[_openedMatchingTab].Count) {
                            _parent.RequestSelectText(_tabMatchings[_openedMatchingTab][_openedMatch], SearchText.Length);
                            _openedMatch++;
                        } else {
                            _openedMatchingTab++;
                            _parent.OpenTabCommand.Execute(_parent.TabViewModels[_matchingTabs[_openedMatchingTab]]);
                            _openedMatch = 0;
                            _parent.RequestSelectText(_tabMatchings[_openedMatchingTab][_openedMatch], SearchText.Length);
                            _openedMatch++;
                        }
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
                        _openedMatchingTab = 0;
                        _textChanged = false;
                        _tabsChangedSearchNext = false;
                    } else if (_openedMatchingTab == _matchingTabs.Count - 1 && _openedMatch == _tabMatchings[_tabMatchings.Count - 1].Count)
                        MessageBox.Show(_noMatchesText);
                    if (_matchingTabs.Count > 0 && _openedMatchingTab < _matchingTabs.Count) {
                        if (_openedMatch < _tabMatchings[_openedMatchingTab].Count) {
                            _parent.RequestSelectText(_tabMatchings[_openedMatchingTab][_openedMatch], SearchText.Length);
                            _openedMatch++;
                        } else if (_openedMatchingTab != _tabMatchings.Count-1) {
                            _openedMatchingTab++;
                            _parent.OpenTabCommand.Execute(_parent.TabViewModels[_matchingTabs[_openedMatchingTab]]);
                            _openedMatch = 0;
                            _parent.RequestSelectText(_tabMatchings[_openedMatchingTab][_openedMatch], SearchText.Length);
                            _openedMatch++;
                        }
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
                        _openedMatchingTab = _matchingTabs.Count-1;
                        _textChanged = false;
                        _tabsChangedSearchPrevious = false;
                    } else if (_openedMatchingTab < 0)
                        MessageBox.Show(_noMatchesText);
                    if (_matchingTabs.Count > 0 && _openedMatchingTab >= 0) {
                        if (_openedMatch > 0) {
                            _parent.RequestSelectText(_tabMatchings[_openedMatchingTab][_openedMatch], SearchText.Length);
                            _openedMatch--;
                        } else if (_openedMatchingTab != 0) {
                            _openedMatchingTab--;
                            _parent.OpenTabCommand.Execute(_parent.TabViewModels[_matchingTabs[_openedMatchingTab]]);
                            _openedMatch = _tabMatchings[_openedMatchingTab].Count - 1;
                            _parent.RequestSelectText(_tabMatchings[_openedMatchingTab][_openedMatch], SearchText.Length);
                            _openedMatch--;
                        }
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
            _matchingTabs = new ObservableCollection<int>();
            bool matchesChanged = false;
            for (int i = 0; i < _tabs.Count; i++) {
                if (_tabs[i].Content.Contains(SearchText)) {
                    _tabMatchings.Add(new List<int>());
                    _matchingTabs.Add(_tabs[i].Index);
                    int index = 0;
                    while ((index = _tabs[i].Content.IndexOf(SearchText, index, StringComparison.OrdinalIgnoreCase)) != -1) {
                        _tabMatchings[_tabMatchings.Count - 1].Add(index);
                        index += SearchText.Length;
                    }
                    matchesChanged = true;
                }
            }
            if (!matchesChanged)
                _matchingTabs = new ObservableCollection<int>();
            else if (_matchingTabs.Count > 0) 
                _parent.OpenTabCommand.Execute(_parent.TabViewModels[_matchingTabs[0]]);
        }

        private void CheckTabs() {
            if (_tabs.Count != _prevTabsCount) { 
                _prevTabsCount = _tabs.Count;
                _tabsChangedSearch = true;
                _tabsChangedSearchNext = true;
                _tabsChangedSearchPrevious = true; 
            }
        }
        #endregion
    }
}
