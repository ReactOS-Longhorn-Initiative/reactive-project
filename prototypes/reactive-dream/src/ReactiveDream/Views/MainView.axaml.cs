using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ReactiveDream.Controls;
using ReactiveDream.ViewModels;
using DBitmap = System.Drawing.Bitmap;

namespace ReactiveDream.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            _windowsRoot = this.Find<ItemsControl>("WindowsRoot");
        }


        void Windows_PanelAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            //((Panel)sender).AttachedToVisualTree += Windows_PanelAttachedToVisualTree;
            Panel panel = (Panel)sender;
            panel.AddHandler(PointerPressedEvent, Windows_PointerPressed, RoutingStrategies.Direct | RoutingStrategies.Bubble | RoutingStrategies.Tunnel, true);
        }


        ItemsControl _windowsRoot = null;
        public void Windows_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            var windows = ((MainViewModel)DataContext).Windows.Reverse().ToList();
            var pointerPos = e.GetPosition((Visual)sender);
            foreach (var window in windows)
            {
                var container = _windowsRoot.ContainerFromItem(window);
                if (
                    //winCtl.IsPointWithinOrWithinNC(pointerPos)
                    //container.Bounds.Contains(pointerPos)
                    container.IsPointerOver
                )
                {
                    ((MainViewModel)DataContext).ActiveWindow = window;
                    break;
                }
            }
            e.Handled = false;
        }
    }
}