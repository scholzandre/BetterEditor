using BetterEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterEditor.Models {
    internal class DataManager {
        public Settings Settings { get; set; }
        public List<ViewMode> ViewModes { get; set; }
        public List<Tab> Tabs { get; set; }
        private string _filePath = "";

        public DataManager(Settings settings, List<ViewMode> viewModes, List<Tab> tabs) {
            Settings = settings;
            ViewModes = viewModes;
            Tabs = tabs;
        }
        public DataManager() { }

        public static DataManager GetData() {
            throw new NotImplementedException();
        }

        public static bool WriteData() {
            throw new NotImplementedException();
        }

        public static Settings GetSettings() {
            throw new NotImplementedException();
        }

        public static List<ViewMode> GetViewModes() {
            throw new NotImplementedException();
        }

        public static List<Tab> GetTabs() {
            throw new NotImplementedException();
        }

        public static string GetFilePath() {
            throw new NotImplementedException();
        }
    }
}
