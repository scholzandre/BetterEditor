using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterEditor.ViewModels
{
    internal class RenameTabViewModel : BaseViewModel {
        private string _filePath = string.Empty;
        public string FilePath {
            get => _filePath;
            set { 
                _filePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }
        public RenameTabViewModel(string filePath) {
            FilePath = filePath;
        } 

        ~RenameTabViewModel() { }
    }
}
