using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterEditor.ViewModels {
    class TabViewModel : Tab {
        public string TabName { get; set; }
        public bool IsActive { get; set; }
        public TabViewModel(string filePath = "", string content = "", DateOnly mD = default(DateOnly), string tabName = "", bool isActive = true) : base(filePath, content, mD) { 
            TabName = tabName;
            IsActive = isActive;
        }
    }
}
