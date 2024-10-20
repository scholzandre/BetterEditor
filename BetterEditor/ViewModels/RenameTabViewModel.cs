using BetterEditor.Models;
using System;
using System.Collections.Generic;
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
        private Action<object> _save;
        private Action _update;
        private List<Tab> _tabs;
        #endregion

        #region Constructores
        public RenameTabViewModel(string filePath, int index, List<Tab> tabs, Action<object, EventArgs> tabsToTabViewModels, Action close, Action<object> save) {
            FilePath = filePath;
            this._index = index;
            this._eventUpdate = tabsToTabViewModels;
            this._tabs = tabs;
            this._closeWindow = close;
            this._save = save;
        }

        public RenameTabViewModel(string filePath, int index, List<Tab> tabs, Action tabsToTabViewModels, Action close, Action<object> save) {
            FilePath = filePath;
            this._index = index;
            this._tabs = tabs;
            this._update = tabsToTabViewModels;
            this._closeWindow = close;
            this._save = save;
        }
        ~RenameTabViewModel() { }
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
                _tabs[_index].FilePath = Path.GetDirectoryName(FilePath) + "\\" + NewFilename + Path.GetExtension(FilePath);
                if (_update != null)
                    _update();
                else
                    _eventUpdate.Invoke(null, null);
                _save.Invoke(null);
                _closeWindow();
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }
        #endregion
    }
}
