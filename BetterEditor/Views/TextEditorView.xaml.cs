using System.Windows.Controls;

namespace BetterEditor.Views {
    /// <summary>
    /// Interaction logic for TextEditorView.xaml
    /// </summary>
    public partial class TextEditorView : UserControl {
        public TextEditorView() {
            InitializeComponent();
        }

        public void OnSelectText(int selectionStart, int selectionLength) {
            OnFocusTextBox();
            textbox.Select(selectionStart, selectionLength);
        }

        public void OnFocusTextBox() { 
            textbox.Focus();
            textbox.SelectionStart = textbox.Text.Length;
        }

        public void OnUndoChange() {
            textbox.Undo();
        }

        public void OnRedoChange() {
            textbox.Redo();
        }

        public void OnMoveScrollbarLeft() {
            TabsScrollViewer.ScrollToLeftEnd();
        }

        public void OnMoveScrollbarRight() {
            TabsScrollViewer.ScrollToRightEnd();
        }

        public void OnMoveScrollbar(int tabIndex) {
            // one tab is 215 px wide
            TabsScrollViewer.ScrollToHorizontalOffset(tabIndex*215);
        }
    }
}
