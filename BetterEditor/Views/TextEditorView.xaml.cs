﻿using BetterEditor.ViewModels;
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
            textbox.Focus();
            textbox.Select(selectionStart, selectionLength);
        }
    }
}
