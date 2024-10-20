﻿using BetterEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BetterEditor.Models {
    internal class DataManager : BaseViewModel {
        public Settings Settings { get; set; }
        public List<ViewMode> ViewModes { get; set; }
        public List<Tab> Tabs { get; set; }
        private static string _folderPath = GetFolderPath();
        public static string FilePath = GetFilePath();

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
                if (File.Exists(FilePath)) {
                    string jsonData = File.ReadAllText(FilePath);
                    DataManager dataManager = JsonConvert.DeserializeObject<DataManager>(jsonData);
                    return dataManager;
                } else {
                    return CreateDefaultAppData(FilePath);
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


        /// <summary>
        /// Writes data in file
        /// </summary>
        /// <param name="dataManager"></param>
        /// <returns>if writing was successful</returns>
        public static bool WriteData(DataManager dataManager) {
            try {
                string json = JsonConvert.SerializeObject(dataManager, Formatting.Indented);
                File.WriteAllText(FilePath, json);
                return true;
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
                return false;
            }
        }


        /// <summary>
        /// Writes settings in file
        /// </summary>
        /// <param name="settings"></param>
        /// <returns>if writing was successful</returns>
        public static bool WriteSettings(Settings settings) {
            try {
                DataManager dataManager = DataManager.GetData();
                dataManager.Settings = settings;
                DataManager.WriteData(dataManager);
                return true;
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
                return false;
            }
        }


        /// <summary>
        /// Writes tabs in file
        /// </summary>
        /// <param name="settings"></param>
        /// <returns>if writing was successful</returns>
        public static bool WriteTabs(List<Tab> tabs) {
            try {
                DataManager dataManager = DataManager.GetData();
                dataManager.Tabs = tabs;
                DataManager.WriteData(dataManager);
                return true;
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
                return false;
            }
        }

        /// <summary>
        /// Gets settings
        /// </summary>
        /// <returns>returns settings instance</returns>
        public static Settings GetSettings() {
            return GetData().Settings;
        }


        /// <summary>
        /// Gets all viewmodes
        /// </summary>
        /// <returns>List of viewmode</returns>
        public static List<ViewMode> GetViewModes() {
            return GetData().ViewModes;
        }

        /// <summary>
        /// Gets all tabs
        /// </summary>
        /// <returns>List of tab</returns>
        public static List<Tab> GetTabs() {
            return GetData().Tabs;
        }

        /// <summary>
        /// Gets content of JSON file
        /// </summary>
        /// <returns>JSON content as string</returns>
        public static string GetPlainJSONFile() {
            try {
                return File.ReadAllText(FilePath);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
                return "";
            }
        }

        /// <summary>
        /// Writes string in json file
        /// </summary>
        /// <param name="content"></param>
        /// <returns>if writing was successful</returns>
        public static bool WritePlainTextInJSONFile(string content) {
            try {
                DataManager dataManager = JsonConvert.DeserializeObject<DataManager>(content);
                return WriteData(dataManager, FilePath);
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
                return false;
            }
        }

        /// <summary>
        /// Gets folder path
        /// </summary>
        /// <returns>folder path as a string</returns>
        public static string GetFolderPath() {
            try {
                string currDirectory = Directory.GetCurrentDirectory();
                int indexFolder = currDirectory.LastIndexOf("bin");
                string folderPath = currDirectory.Substring(0, indexFolder);
                return folderPath;
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
                return "";
            }
        }

        /// <summary>
        /// Gets file path of AppData.json
        /// </summary>
        /// <returns>file path as a string</returns>
        public static string GetFilePath() {
            try {
                string filePath = _folderPath + "AppData.json";

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

                Settings settings = new Settings(new Tab("", "", new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day)), viewModes[1], true, false, 14);
                List<Tab> tabs = new List<Tab>();
                DataManager dataManager = new DataManager(settings, viewModes, tabs);

                WriteData(dataManager, filePath);
                return dataManager;
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
                return null;
            }
        }
        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="tab"></param>
        /// <returns>if deleting was successful</returns>
        public static bool DeleteFile(string filePath) {
            try {
                string licensePath = FilePath.Substring(0, FilePath.Length - "BetterEditor".Length * 2 - 1) + "LICENSE.txt";
                string readmePath = FilePath.Substring(0, FilePath.Length - "BetterEditor".Length * 2 - 1) + "README.md";
                if (File.Exists(filePath) && filePath != FilePath && filePath != licensePath && filePath != readmePath) {
                    File.Delete(filePath);
                }
                return true;
            } catch (Exception e) {
                BaseViewModel.ShowErrorMessage(e);
            }
            return false;
        }

        /// <summary>
        /// Renames a file
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="newName"></param>
        /// <returns>if renaming was successful</returns>
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
