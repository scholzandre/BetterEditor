using BetterEditor.Models;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using VocabTrainer.Models;

namespace BetterEditor.ViewModels {
    internal class TextEditorViewModel : BaseViewModel {
        private List<Tab> _tabs;
        public List<Tab> Tabs {
            get => _tabs;
            set {
                _tabs = value; 
                OnPropertyChanged(nameof(Tabs));
            }
        }
        public Settings Settings { get; set; }
        public string MoveLeftIcon { get; set; } = "<";
        public string MoveRightIcon { get; set; } = ">";
        public string RenameIcon { get; set; } = "🖉";
        public string DeleteIcon { get; set; } = "✖";
        public TextEditorViewModel(List<Tab> tabs, Settings settings) {
            Tabs = tabs;
            Settings = settings;
        }

        private Tab _tab;
        public Tab Tab { 
            get => _tab;
            set { 
                _tab = value;
                OnPropertyChanged(nameof(Tab));
            }
        }
        private bool CanExecuteRenameCommand(object arg) {
            return true;
        }

        public ICommand RenameCommand => new RelayCommand(Rename, CanExecuteRenameCommand);
        private void Rename(object obj) {
            throw new NotImplementedException();
        }

        private bool CanExecuteCommand(object arg) {
            return true;
        }

        public ICommand DeleteCommand => new RelayCommand(Delete, CanExecuteRenameCommand);
        private void Delete(object obj) {
            throw new NotImplementedException();
        }

        public ICommand OpenTabCommand => new RelayCommand(OpenTab, CanExecuteRenameCommand);
        private void OpenTab(object obj) {
            try {
                Tab = (Tab)obj;
                Settings.LOT = Tab;
                DataManager.WriteSettings(Settings);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
        }
    }
}
