using BetterEditor.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace BetterEditor.ViewModels {
    internal class RenameTabViewModel : BaseViewModel {
        private string _filePath = string.Empty;
        public string FilePath {
            get => _filePath;
            set { 
                _filePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }

        private string _originalFilename = string.Empty;
        public string OriginalFilename {
            get => _originalFilename;
            set {
                _originalFilename = value;
                OnPropertyChanged(nameof(OriginalFilename));
            }
        }

        private string _newFilename = string.Empty;
        public string NewFilename {
            get => _newFilename;
            set {
                _newFilename = value;
                OnPropertyChanged(nameof(NewFilename));
            }
        }

        private string _fileType = string.Empty;
        public string FileType {
            get => _fileType;
            set {
                _fileType = value;
                OnPropertyChanged(nameof(FileType));
            }
        }

        #region Fields
        private Action _closeWindow;
        private int _index;
        private Action<object, EventArgs> _eventUpdate;
        private Action _update;
        private ObservableCollection<TabViewModel> _tabViewModels;
        #endregion

        #region Constructores
        public RenameTabViewModel(string filePath, int index, ObservableCollection<TabViewModel> tabViewModels, Action<object, EventArgs> tabsToTabViewModels, Action close) {
            FilePath = filePath;
            this._index = index;
            this._eventUpdate = tabsToTabViewModels;
            this._tabViewModels = tabViewModels;
            this._closeWindow = close;
        }

        public RenameTabViewModel(string filePath, int index, ObservableCollection<TabViewModel> tabViewModels, Action tabsToTabViewModels, Action close) {
            FilePath = filePath;
            this._index = index;
            this._tabViewModels = tabViewModels;
            this._update = tabsToTabViewModels;
            this._closeWindow = close;
        }
        #endregion

        #region Commands and Methods
        private void GetFilePathParts() { 
            OriginalFilename = Path.GetFileName(FilePath);
            FileType = Path.GetExtension(FilePath);
        }

        private bool CanExecuteCommand(object arg) {
            return true;
        }

        public ICommand CancelCommand => new RelayCommand(Cancel, CanExecuteCommand);
        private void Cancel(object obj) {
            try {
                _closeWindow();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }

        public ICommand ApplyCommand => new RelayCommand(Apply, CanExecuteCommand);
        private void Apply(object obj) {
            try {
                _tabViewModels[_index].FilePath = Path.GetDirectoryName(FilePath) + "\\" + NewFilename + Path.GetExtension(FilePath);
                if (_update != null)
                    _update();
                else
                    _eventUpdate.Invoke(null, null);
                _closeWindow();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }
        #endregion
        ~RenameTabViewModel() { }
    }
}
