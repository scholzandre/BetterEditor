using System;
using System.IO;
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

        public RenameTabViewModel(string filePath) {
            FilePath = filePath;
            GetFilePathParts();
        }

        private void GetFilePathParts() { 
            OriginalFilename = Path.GetFileName(FilePath);
            FileType = Path.GetExtension(FilePath);
        }

        ~RenameTabViewModel() { }
    }
}
