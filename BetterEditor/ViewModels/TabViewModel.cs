using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterEditor.ViewModels {
    class TabViewModel : Tab {
        public string TabName { get; set; }
        public bool IsActive { get; set; }
        public int Index { get; set; }
        public TabViewModel(string filePath = "", string content = "", DateOnly mD = default(DateOnly), string tabName = "", bool isActive = true, int index = 0) : base(filePath, content, mD) { 
            TabName = tabName;
            IsActive = isActive;
            Index = index;
        }
        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            TabViewModel otherEntry = (TabViewModel)obj;
            return this.Index == otherEntry.Index;
        }
    }
}
