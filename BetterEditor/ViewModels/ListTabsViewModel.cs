using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterEditor.ViewModels {
    internal class ListTabsViewModel : BaseViewModel {
        #region Properties
        private string _searchText = string.Empty;
        public string SearchText {
            get => _searchText;
            set {
                _searchText = value.Trim();
                OnPropertyChanged(nameof(SearchText));
            }
        }
        #endregion
        #region Fields
        private MainViewModel _parent;
        #endregion
        #region Constructors
        public ListTabsViewModel(MainViewModel parent) { 
            _parent = parent;
        }
        public ListTabsViewModel() { }
        #endregion
        #region Commands and methods
        #endregion
    }
}
