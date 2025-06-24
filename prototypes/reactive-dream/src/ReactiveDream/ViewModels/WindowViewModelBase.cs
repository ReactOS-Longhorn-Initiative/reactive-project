using System;
using System.Timers;
using Avalonia;

namespace ReactiveDream.ViewModels
{
    public abstract class WindowViewModelBase
        : SubViewModelBase
    {
        string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => RASIC(ref _title, value);
        }


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


        public Rect Bounds
        {
            get => new(X, Y, Width, Height);
            set
            {
                Rect _ = Bounds;
                X = value.Left;
                Y = value.Top;
                Width = value.Width;
                Height = value.Height;
                RASIC(ref _, value);
            }
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




        public WindowViewModelBase()
            : base()
        {
            Rect defaultBounds = CreateDefaultBounds();

            X = defaultBounds.X;
            Y = defaultBounds.Y;
            Width = defaultBounds.Width;
            Height = defaultBounds.Height;
        }


        public void SetBoundsFromAxes(WindowBoundsAxis xAxis, WindowBoundsAxis yAxis)
        {
            X = xAxis.Position;
            Y = yAxis.Position;
            Width = xAxis.Size;
            Height = yAxis.Size;
        }


        protected virtual Rect CreateDefaultBounds()
            => new(40, 40, 320, 240);




        
        public void CloseCommand(object _)
            => Close();

        public void Close()
            => WindowClosing?.Invoke(this, new());


        public void Activate()
            => WindowActivated?.Invoke(this, new(this));

        public event EventHandler<EventArgs> WindowClosing;
        public static event EventHandler<WindowActionEventArgs> WindowActivated;





        public string ToString(bool useTypeFullName)
        {
            Type type = GetType();
            string typeName = useTypeFullName
                ? type.FullName
                : type.Name
            ;
            return $"({typeName} titled '{Title}' at '{Bounds}')";
        }
        public override string ToString()
            => ToString(false);
    }
}