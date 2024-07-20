using System;
using System.Windows.Input;
using System.Windows;
using BetterEditor.Models;
using System.Collections.ObjectModel;

namespace BetterEditor.ViewModels {
    internal class ReplaceViewModel {
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
        TextEditorViewModel _parent;
        private ObservableCollection<TabViewModel> _tabs;
        private int _openedMatch = 0;
        private ObservableCollection<int> _matchingTabs = new ObservableCollection<int>();
        public ReplaceViewModel(TextEditorViewModel parent) {
            _parent = parent;
            _tabs = _parent.UsedTabs;
            SetColors();
        }

        private void SetColors() {
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
                if (SearchText != string.Empty) {
                    if (_textChanged) {
                        GetFilteredTabIds();
                        _openedMatch = 0;
                        _textChanged = false;
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
                    if (_textChanged) {
                        GetFilteredTabIds();
                        _openedMatch = 0;
                        _textChanged = false;
                    } else if (_openedMatch == _matchingTabs.Count)
                        MessageBox.Show("No further matches found!");
                    if (_matchingTabs.Count > 0 && _openedMatch != _matchingTabs.Count) {
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
                    if (_textChanged) {
                        GetFilteredTabIds();
                        _openedMatch = _matchingTabs.Count - 1;
                        _textChanged = false;
                    } else if (_openedMatch < 0)
                        MessageBox.Show("No further matches found!");
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
