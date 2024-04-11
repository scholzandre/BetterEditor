using BetterEditor.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace BetterEditor {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        MainViewModel _mainViewModel;
        private string _overlap = "🗗";
        private string _maximize = "🗖";

        public MainWindow() {
            InitializeComponent();
            _mainViewModel = (MainViewModel)DataContext;
            this.SizeChanged += MainWindowSizeChanged;
        }

        private void MainWindowSizeChanged(object sender, SizeChangedEventArgs e) {
            _mainViewModel.ChangeSizeButton = (this.WindowState == WindowState.Maximized)? _overlap : _maximize;
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void ChangeWindowSize(object sender, RoutedEventArgs e) {
            if (WindowState == WindowState.Maximized) {
                WindowState = WindowState.Normal;
            } else { 
                WindowState = WindowState.Maximized;
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e) {
            Close();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                if (WindowState == WindowState.Maximized && e.ClickCount == 1) {
                    WindowState = WindowState.Normal;
                    Top = 0;
                    Left = Mouse.GetPosition(this).X - Width / 2;
                }
                DragMove();
            }
        }

        private void DoubleClick(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                if (WindowState == WindowState.Maximized) {
                    WindowState = WindowState.Normal;
                } else {
                    WindowState = WindowState.Maximized;
                }
            }
        }
    }
}