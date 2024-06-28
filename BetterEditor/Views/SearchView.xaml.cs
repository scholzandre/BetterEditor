using System.Windows;
using System.Windows.Controls;

namespace BetterEditor.Views {
    /// <summary>
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class SearchView : UserControl {
        public SearchView() {
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
