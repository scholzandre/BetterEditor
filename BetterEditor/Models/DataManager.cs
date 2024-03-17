using BetterEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BetterEditor.Models {
    internal class DataManager : BaseViewModel {
        public Settings Settings { get; set; }
        public List<ViewMode> ViewModes { get; set; }
        public List<Tab> Tabs { get; set; }
        private static string _filePath = GetFilePath();

        public DataManager(Settings settings, List<ViewMode> viewModes, List<Tab> tabs) {
            Settings = settings;
            ViewModes = viewModes;
            Tabs = tabs;
        }
        public DataManager() { }

        public static DataManager GetData() {
            try {
                if (File.Exists(_filePath)) {
                    string jsonData = File.ReadAllText(_filePath);
                    DataManager dataManager = JsonConvert.DeserializeObject<DataManager>(jsonData);
                    return dataManager;
                } else {
                    throw new NotImplementedException("Create standard file is missing");
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
                return null;
            }
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
            try {
                string currDirectory = Directory.GetCurrentDirectory();
                int indexFolder = currDirectory.LastIndexOf("bin");
                string folderPath = currDirectory.Substring(0, indexFolder);
                string filePath = folderPath + "AppData.json";

                if (!File.Exists(filePath)) {
                    File.Create(filePath);
                } else {
                    throw new NotImplementedException("Create standard file is missing");
                }
                return filePath;
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
                return "";
            }
        }
    }
}
