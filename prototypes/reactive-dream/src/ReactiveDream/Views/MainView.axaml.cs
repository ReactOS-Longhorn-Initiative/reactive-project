using System;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ReactiveDream.ViewModels;

namespace ReactiveDream.Views
{
    public partial class MainView
        : UserControl
    {
        WindowState _prevState = WindowState.Normal;
        public bool IsFullScreen
        {
            get => TryGetWindow(out Window window) && (window.WindowState == WindowState.FullScreen);
            set
            {
                if (TryGetWindow(out Window window))
                    SetWindowFullScreen(window, value, ref _prevState);
            }
        }



        static void SetWindowFullScreen(Window window, bool enable, ref WindowState prevStateCache)
        {
            if (!enable)
            {
                window.WindowState = prevStateCache;
                return;
            }


            var winState = window.WindowState;
            prevStateCache = ((winState == WindowState.Minimized) || (winState == WindowState.FullScreen))
                ? WindowState.Normal
                : winState
            ;

            window.WindowState = WindowState.FullScreen;
        }




        ItemsControl _windowsRoot = null;


        public MainView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            _windowsRoot = this.Find<ItemsControl>("WindowsRoot");
            _windowsRoot.SizeChanged += WindowsRoot_SizeChanged;

            Dispatcher.UIThread.Post(AttachToDataContext, DispatcherPriority.Loaded);
        }


        void AttachToDataContext()
        {
            var vm = (MainViewModel)DataContext;

            bool shouldBecomeFullScreen = IsFullScreen || vm.IsFullScreen;
            vm.IsFullScreen = shouldBecomeFullScreen;
            vm.PropertyChanged += VM_PropertyChanged;

            Dispatcher.UIThread.Post(() => InitFullScreen(shouldBecomeFullScreen), DispatcherPriority.ApplicationIdle);
        }


        void InitFullScreen(bool shouldBecomeFullScreen)
        {
            IsFullScreen = shouldBecomeFullScreen;
            Dispatcher.UIThread.Post(() => UpdateWorkingAreaSize(_windowsRoot.Bounds.Size), DispatcherPriority.Loaded);
        }


        void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(DataContext is MainViewModel mainVM))
                return;
            if (e.PropertyName != nameof(MainViewModel.IsFullScreen))
                return;

            IsFullScreen = mainVM.IsFullScreen;
        }


        void WindowsRoot_SizeChanged(object sender, SizeChangedEventArgs e)
            => UpdateWorkingAreaSize(e.NewSize);
        void UpdateWorkingAreaSize(Size size)
            => ((MainViewModel)DataContext).WorkingAreaSize = size;


        void Windows_PanelAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            Panel panel = (Panel)sender;
            panel.AddHandler(PointerPressedEvent, Windows_PointerPressed, RoutingStrategies.Direct | RoutingStrategies.Bubble | RoutingStrategies.Tunnel, true);
        }


        public void Windows_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            var windows = ((MainViewModel)DataContext).Windows.Reverse().ToList();
            foreach (var window in windows)
            {
                var container = _windowsRoot.ContainerFromItem(window);
                if (container.IsPointerOver)
                {
                    ((MainViewModel)DataContext).ActiveWindow = window;
                    break;
                }
            }
            e.Handled = false;
        }


        bool TryGetWindow(out Window window)
        {
            if (TopLevel.GetTopLevel(this) is Window win)
            {
                window = win;
                return window != null;
            }
            else
            {
                window = null;
                return false;
            }
        }
    }
}