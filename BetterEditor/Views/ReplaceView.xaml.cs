using System.Windows;
using System.Windows.Controls;

namespace BetterEditor.Views {
    /// <summary>
    /// Interaction logic for ReplaceView.xaml
    /// </summary>
    public partial class ReplaceView : UserControl {
        public ReplaceView() {
            InitializeComponent();
        }

        private void ViewLoaded(object sender, RoutedEventArgs e) {
            var textBox = sender as TextBox;
            if (textBox != null) {
                textBox.Focus();
            }
        }
    }
}
