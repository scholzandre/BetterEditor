using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterEditor.ViewModels {
    internal class ViewMode {
        /// <summary>
        /// Label of ViewMode.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// BGTE = Background Texteditor. Hexadecimal Value.
        /// </summary>
        public string BGTE { get; set; }

        /// <summary>
        /// BGT = Background Tab. Hexadecimal Value.
        /// </summary>
        public string BGT { get; set; }

        /// <summary>
        /// General foreground for color changing elements
        /// </summary>
        public string Foreground { get; set; }

        public ViewMode(string label, string bGTe, string bGT, string foreground) {
            Label = label; 
            BGTE = bGTe;
            BGT = bGT;
            Foreground = foreground;
        }
    }
}
