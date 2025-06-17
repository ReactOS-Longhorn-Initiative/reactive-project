using System;
using System.Timers;
using Avalonia;

namespace ReactiveDream.ViewModels
{
    public class WindowViewModelBase
        : ViewModelBase
    {
        string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => RASIC(ref _title, value);
        }

        /*
        object _content = null;
        public object Content
        {
            get => _content;
            set => RASIC(ref _content, value);
        }
        */

        bool _isOpened = false;
        public bool IsOpened
        {
            get => _isOpened;
            set => RASIC(ref _isOpened, value);
        }


        bool _canResize = true;
        public bool CanResize
        {
            get => _canResize;
            set => RASIC(ref _canResize, value);
        }

        
        double _x = 0;
        public double X
        {
            get => _x;
            set => RASIC(ref _x, value);
        }

        double _y = 0;
        public double Y
        {
            get => _y;
            set => RASIC(ref _y, value);
        }

        double _width = 0;
        public double Width
        {
            get => _width;
            set => RASIC(ref _width, value);
        }

        double _height = 0;
        public double Height
        {
            get => _height;
            set => RASIC(ref _height, value);
        }

        bool _isMaximized = false;
        public bool IsMaximized
        {
            get => _isMaximized;
            set => RASIC(ref _isMaximized, value);
        }

        bool _isActive = false;
        public bool IsActive
        {
            get => _isActive;
            set => RASIC(ref _isActive, value);
        }


        WeakReference<MainViewModel> _vm;
        public WindowViewModelBase()
            : base()
        {
            Rect defaultBounds = CreateDefaultBounds();

            if (_x < 0)
                X = defaultBounds.X;

            if (_y < 0)
                Y = defaultBounds.Y;

            if (_width <= 0)
                Width = defaultBounds.Width;

            if (_height <= 0)
                Height = defaultBounds.Height;
        }
        public TWindowVM WithMainVM<TWindowVM>(MainViewModel vm)
        {
            _vm ??= new(vm);
            return (TWindowVM)(object)this;
        }
        /*public WindowViewModel(MainViewModel vm, string title, object content = null, double x = 40, double y = 40, double width = 320, double height = 240)
            : this()
        {
            Title = title;
            Content = content;
            
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }*/


        protected virtual Rect CreateDefaultBounds()
            => new(40, 40, 320, 240);




        public void Close()
        {
            Timer timer = new Timer(1000);
            timer.Elapsed += CloseTimer_Elapsed;
            timer.Start();
        }
        void CloseTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (sender is Timer timer)
            {
                timer.Stop();
                timer.Elapsed -= CloseTimer_Elapsed;
            }
            WindowClosed?.Invoke(this, new(this));
        }

        public void Activate()
            => WindowActivated?.Invoke(this, new(this));

        public static event EventHandler<WindowActionEventArgs> WindowActivated;
        public static event EventHandler<WindowActionEventArgs> WindowClosed;
    }
}