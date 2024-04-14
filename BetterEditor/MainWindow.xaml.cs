using BetterEditor.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
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
        private bool _canSetStateNormal = false;

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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                _canSetStateNormal = false;
                if (WindowState == WindowState.Maximized) {
                    WindowState = WindowState.Normal;
                } else {
                    WindowState = WindowState.Maximized;
                }
            }
        }

        private void MouseMoves(object sender, System.Windows.Input.MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                if (WindowState == WindowState.Maximized && _canSetStateNormal) {
                    WindowState = WindowState.Normal;
                    Screen selectedScreen = Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(this).Handle);
                    int screenWidth = selectedScreen.WorkingArea.Width;
                    double leftArea = screenWidth * 0.2;
                    double middle = screenWidth * 0.8;
                    int upperLeftCornor = selectedScreen.Bounds.Left;
                    double mouseXPosition = e.GetPosition(this).X;
                    Top = 0;
                    if (mouseXPosition <= leftArea)
                        Left = upperLeftCornor;
                    else if (mouseXPosition > leftArea && mouseXPosition <= middle)
                        Left = upperLeftCornor + mouseXPosition - Width / 2;
                    else
                        Left = upperLeftCornor + screenWidth - Width;
                }
                DragMove();
            }
        }
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            _canSetStateNormal = true;
        }
    }
}