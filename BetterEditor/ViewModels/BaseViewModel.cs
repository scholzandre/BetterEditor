using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace BetterEditor.ViewModels {
    internal class BaseViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// shows error in a MessageBox and writes it in console
        /// </summary>
        /// <param name="exception"></param>
        public static void ShowErrorMessage(Exception exception) {
            string infoText = "An error occured";
            Debug.WriteLine($"{infoText}\n{exception.Message}");    
            MessageBox.Show(exception.Message, infoText);
        }
    }
}
