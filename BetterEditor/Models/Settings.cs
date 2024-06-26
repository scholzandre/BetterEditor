﻿namespace BetterEditor.ViewModels {
    internal class Settings {
        /// <summary>
        /// LOT = Last Opened Tab Instance
        /// </summary>
        public Tab LOT { get; set; }

        /// <summary>
        /// SVM = Selected ViewMode Instance
        /// </summary>
        public ViewMode SVM { get; set; }

        /// <summary>
        /// SA = Save Automatically Boolean
        /// </summary>
        public bool SA { get; set; }

        /// <summary>
        /// CAD = Close And Delete Boolean
        /// </summary>
        public bool CAD { get; set; }

        /// <summary>
        /// General font size
        /// </summary>
        public int FontSize { get; set; }

        public Settings(Tab lOT, ViewMode sVM, bool sA, bool cAD, int fontSize = 14) { 
            LOT = lOT;
            SVM = sVM;
            SA = sA;
            CAD = cAD;
            FontSize = fontSize;
        }

        public Settings() { }
    }
}
