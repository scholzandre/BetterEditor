using System;

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

        public override int GetHashCode() {
            int hash = 17;
            hash = hash * 31 + TabName.GetHashCode();
            hash = hash * 31 + IsActive.GetHashCode();
            hash = hash * 31 + Index.GetHashCode();
            return hash;
        }
    }
}
