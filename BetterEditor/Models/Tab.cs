﻿using System;

namespace BetterEditor.ViewModels {
    internal class Tab {
        /// <summary>
        /// File Path. Will be a random number if no linked file.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Tab Content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// MD = Modification Date
        /// </summary>
        public DateOnly MD { get; set; }

        public Tab(string filePath = "", string content = "", DateOnly mD = new DateOnly()) { 
            FilePath = filePath;
            Content = content;
            MD = mD;
        }
        ~Tab() { }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            Tab otherTab = (Tab)obj;
            return this.FilePath == otherTab.FilePath &&
                   this.Content == otherTab.Content &&
                   this.MD == otherTab.MD;
        }

        public override int GetHashCode() {
            int hash = 17;
            hash = hash * 31 + FilePath.GetHashCode();
            hash = hash * 31 + Content.GetHashCode();
            hash = hash * 31 + MD.GetHashCode();
            return hash;
        }
    }
}
