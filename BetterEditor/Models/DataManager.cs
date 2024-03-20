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

        /// <summary>
        /// Gets data from AppData.json
        /// </summary>
        /// <returns>DataManager instance</returns>
        public static DataManager GetData() {
            try {
                if (File.Exists(_filePath)) {
                    string jsonData = File.ReadAllText(_filePath);
                    DataManager dataManager = JsonConvert.DeserializeObject<DataManager>(jsonData);
                    return dataManager;
                } else {
                    return CreateDefaultAppData(_filePath);
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
                return null;
            }
        }


        /// <summary>
        /// Writes data in file
        /// </summary>
        /// <param name="dataManager"></param>
        /// <param name="filePath"></param>
        /// <returns>if writing was successful</returns>
        public static bool WriteData(DataManager dataManager, string filePath) {
            try {
                string json = JsonConvert.SerializeObject(dataManager, Formatting.Indented);
                File.WriteAllText(filePath, json);
                return true;
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
                return false;
            }
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


        /// <summary>
        /// Gets file path of AppData.json
        /// </summary>
        /// <returns>file path as a string</returns>
        public static string GetFilePath() {
            try {
                string currDirectory = Directory.GetCurrentDirectory();
                int indexFolder = currDirectory.LastIndexOf("bin");
                string folderPath = currDirectory.Substring(0, indexFolder);
                string filePath = folderPath + "AppData.json";

                if (!File.Exists(filePath)) {
                    CreateDefaultAppData(filePath);
                }
                return filePath;
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
                return "";
            }
        }

        /// <summary>
        /// Creates DataManager with default data
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>DataManager instance</returns>
        private static DataManager CreateDefaultAppData(string filePath) {
            try {
                List<ViewMode> viewModes = new List<ViewMode>() {
                    new ViewMode("Lightmode", "#FFFFFF", "#F5F5F5", "#000000"),
                    new ViewMode("Darkmode", "#525252", "#7F7D8A", "#FFFFFF")
                };

                Settings settings = new Settings(new Tab(), viewModes[0], true, true);
                List<Tab> tabs = new List<Tab>();
                DataManager dataManager = new DataManager(settings, viewModes, tabs);

                WriteData(dataManager, filePath);
                return dataManager;
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
                return null;
            }
        }

        public static bool DeleteFile(Tab tab) {
            try {
                if (File.Exists(tab.FilePath)) {
                    File.Delete(tab.FilePath);
                    return true;
                }
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
            return false;
        }

        public static bool RenameFile(Tab tab, string newName) {
            try {
                if (File.Exists(tab.FilePath)) {
                    int indexBackslash = tab.FilePath.LastIndexOf("\\") + 1;
                    string destinationFilePath = tab.FilePath.Substring(0, indexBackslash) + newName;
                    File.Move(tab.FilePath, destinationFilePath);
                    return true;
                } else
                    throw new Exception("This tab is not linked to a file");
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
            return false;
        }
    }
}
