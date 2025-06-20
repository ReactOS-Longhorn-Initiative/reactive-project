using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ReactiveDream.Views
{
    public partial class NotepadWindowView
        : UserControl
    {
        public NotepadWindowView()
        {
            InitializeComponent();
        }

        void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}