using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace AdminDesktop.Controls.General
{
    public partial class SearchBar : UserControl
    {
        public event Action<string>? SearchTextChanged;

        public SearchBar()
        {
            InitializeComponent();

            SearchTextBox.TextChanged += (_, _) =>
            {
                SearchTextChanged?.Invoke(SearchTextBox.Text ?? string.Empty);
            };
        }
    }
}