using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveDream.ViewModels;

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


        void HideStatusBar_Click(object sender, RoutedEventArgs e)
            => ((NotepadWindowViewModel)DataContext).ShowStatusBar = false;

        void ShowStatusBar_Click(object sender, RoutedEventArgs e)
            => ((NotepadWindowViewModel)DataContext).ShowStatusBar = true;
    }
}