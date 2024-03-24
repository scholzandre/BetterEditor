using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public TextEditorViewModel(List<Tab> tabs, Settings settings) {
            Tabs = tabs;
            Settings = settings;
        }
    }
}
