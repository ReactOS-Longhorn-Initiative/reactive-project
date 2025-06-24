using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;

using MWindow = ReactiveDream.Controls.MWindow;

namespace ReactiveDream.ViewModels
{
    public class MainViewModel
        : ViewModelBase
    {
        Size _workingAreaSize = new();
        public Size WorkingAreaSize
        {
            get => _workingAreaSize;
            set => RASIC(ref _workingAreaSize, value);
        }


        DesktopViewModel _desktopVM = null;
        public DesktopViewModel DesktopVM
        {
            /*
            get
            {
                if (_desktopVM == null)
                    _desktopVM = new(this);

                return _desktopVM;
            }
            */
            get => _desktopVM;
            private set => RASIC(ref _desktopVM, value);
        }


        bool _isCompositionActive = true;
        public bool IsCompositionActive
        {
            get => _isCompositionActive;
            set => RASIC(ref _isCompositionActive, value);
        }


        bool _isFullScreen = false;
        public bool IsFullScreen
        {
            get => _isFullScreen;
            set => RASIC(ref _isFullScreen, value);
        }


        readonly ObservableCollection<WindowViewModelBase> _windows = new();
        public ObservableCollection<WindowViewModelBase> Windows
        {
            get => _windows;
        }


        public WindowViewModelBase ActiveWindow
        {
            get => Windows.FirstOrDefault(x => x.IsActive);
            set
            {
                var prev = ActiveWindow;
                if (prev == value)
                    return;

                if (prev != null)
                    prev.IsActive = false;

                if (value == null)
                    return;

                if (Windows.Contains(value))
                    Windows.Move(Windows.IndexOf(value), Windows.Count - 1);

                value.IsActive = true;
            }
        }


        public MainViewModel()
        {
            DesktopVM = new DesktopViewModel().WithMainVM(this);


            MWindow.WindowClosed += MWindow_WindowClosed; //HACK: NOOOOOOOOOOOOOOO

            Windows.CollectionChanged += (s, e) =>
            {
                if (e.Action != NotifyCollectionChangedAction.Remove)
                    return;
                var removedWindows = e.OldItems
                    .OfType<WindowViewModelBase>()
                    .ToArray()
                ;
            };

            WindowViewModelBase.WindowActivated += (s, e) => ActiveWindow = e.WindowVM;
        }


        


        public void SetFullScreenCapability(bool enable)
        {
            bool switchToFullscreen = IsFullScreen;
            DesktopVM.SetFullScreenCapability(enable, ref switchToFullscreen);

            if (switchToFullscreen)
                IsFullScreen = true;
        }




        void MWindow_WindowClosed(object sender, EventArgs e)
        {
            MWindow window = (MWindow)sender;
            WindowViewModelBase winVM = (WindowViewModelBase)window.DataContext;
            if (winVM == null)
                return;


            bool wasActive = winVM.IsActive || (ActiveWindow == winVM);

            Console.WriteLine($"{nameof(Windows)}.{nameof(Windows.Remove)}({winVM});");
            Windows.Remove(winVM);

            if (wasActive)
                ActiveWindow = Windows.Where(x => x != winVM).LastOrDefault();
        }

        List<WindowViewModelBase> _windowsTemp = new();
        public void RefreshHackPrepare()
        {
            _windowsTemp = Windows.ToList();
            int windowsCount = Windows.Count;
            for (int i = 0; i < windowsCount; i++)
            {
                Windows.RemoveAt(0);
            }
        }
        public void RefreshHackConclude()
        {
            int windowsCount = _windowsTemp.Count;
            for (int i = 0; i < windowsCount; i++)
            {
                Windows.Add(_windowsTemp[0]);
                _windowsTemp.RemoveAt(0);
            }
        }


        public void AddWindow<TWindowVM>(string title = null
        , double x = double.NaN, double y = double.NaN, double width = double.NaN, double height = double.NaN
        )
            where TWindowVM : WindowViewModelBase, new()
        {
            TWindowVM winVM = new TWindowVM().WithMainVM<TWindowVM>(this);

            if (!string.IsNullOrWhiteSpace(title))
                winVM.Title = title;


            WindowBoundsAxis vmX = new(winVM.X, winVM.Width);
            WindowBoundsAxis vmY = new(winVM.Y, winVM.Height);

            WindowBoundsAxis paramsX = new(x, width);
            WindowBoundsAxis paramsY = new(y, height);


            Size workingAreaSize = WorkingAreaSize;

            WindowBoundsAxis finalX = WindowPositioningHelper.GetValidWindowPosForAxis(vmX, paramsX, workingAreaSize.Width);
            WindowBoundsAxis finalY = WindowPositioningHelper.GetValidWindowPosForAxis(vmY, paramsY, workingAreaSize.Height);


            winVM.SetBoundsFromAxes(finalX, finalY);
            Windows.Add(winVM);
            ActiveWindow = winVM;
        }
    }
}