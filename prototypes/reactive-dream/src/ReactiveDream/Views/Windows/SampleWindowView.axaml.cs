using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ReactiveDream.Views
{
    public partial class SampleWindowView
        : UserControl
    {
        public SampleWindowView()
        {
            InitializeComponent();
        }

        void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}