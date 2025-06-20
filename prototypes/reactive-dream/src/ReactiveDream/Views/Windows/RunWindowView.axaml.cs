using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ReactiveDream.Views
{
    public partial class RunWindowView
        : UserControl
    {
        public RunWindowView()
        {
            InitializeComponent();
        }

        void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}