using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using WallpaperImagePosition = ReactiveDream.Controls.WallpaperImagePosition;
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


        WallpaperViewModel _wallpaper = PersonalizationWindowViewModel.WALLPAPERS.Last();
        public WallpaperViewModel Wallpaper
        {
            get => _wallpaper;
            set => RASIC(ref _wallpaper, value);
        }




        WallpaperImagePosition _wallpaperPosition = WallpaperImagePosition.Fill;
        public WallpaperImagePosition WallpaperPosition
        {
            get => _wallpaperPosition;
            set => RASIC(ref _wallpaperPosition, value);
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

        
        readonly ObservableCollection<DesktopIconViewModel> _desktopIcons = new();
        public ObservableCollection<DesktopIconViewModel> DesktopIcons
        {
            get => _desktopIcons;
        }

        double _desktopIconSize = 32;
        public double DesktopIconSize
        {
            get => _desktopIconSize;
            set => RASIC(ref _desktopIconSize, value);
        }


        const string _DWM_CONTROL = "mock DWM";
        static readonly string _DWM_ENABLE = $"Enable {_DWM_CONTROL}";
        static readonly string _DWM_DISABLE = $"Disable {_DWM_CONTROL}";


        const string _FULLSCREEN_CONTROL = "Fullscreen";
        static readonly string _FULLSCREEN_ENABLE = $"Enter {_FULLSCREEN_CONTROL}";
        static readonly string _FULLSCREEN_DISABLE = $"Exit {_FULLSCREEN_CONTROL}";


        bool _hasSetFullScreenCapability = false;
        bool _isFullScreenEnabled = false;
        public void SetFullScreenCapability(bool enable)
        {
            _isFullScreenEnabled = enable;
            if (!_hasSetFullScreenCapability)
            {
                _hasSetFullScreenCapability = true;

                if (_isFullScreenEnabled)
                    IsFullScreen = true;
            }


            if (!_hasDesktopIcons)
                return;

            
            bool wasFullScreenEnabled = _desktopIcons.Contains(_toggleFullscreenIcon);
            if (_isFullScreenEnabled == wasFullScreenEnabled)
                return;

            else if (_isFullScreenEnabled)
                _desktopIcons.Insert(0, _toggleFullscreenIcon);
            else
                _desktopIcons.Remove(_toggleFullscreenIcon);

            UpdateFullScreenIconName();
        }


        bool _hasDesktopIcons = false;
        DesktopIconViewModel _toggleFullscreenIcon = null;
        public MainViewModel()
        {
            _toggleFullscreenIcon = new()
            {
                Name = _FULLSCREEN_DISABLE,
                Command = () =>
                {
                    bool enable = !IsFullScreen;
                    IsFullScreen = enable;
                    UpdateFullScreenIconName();
                },
            };



            _desktopIcons = new()
            {
                new("Personalization")
                {
                    Command = () => AddWindow<PersonalizationWindowViewModel>(),
                },
                new("Notepad")
                {
                    Command = () => AddWindow<NotepadWindowViewModel>(),
                },
                new("Run...")
                {
                    Command = () => AddWindow<RunWindowViewModel>(),
                },
                /*
                new("Internet Browser")
                {
                    Command = () => AddWindow("Wine Internet Explorer"),
                },
                CreateIconForOpenDummyWindow("My Computer"),
                CreateIconForOpenDummyWindow("My Documents"),
                CreateIconForOpenDummyWindow("My Network Places"),
                CreateIconForOpenDummyWindow("Recycle Bin"),
                CreateIconForOpenDummyWindow("Applications Manager"),
                CreateIconForOpenDummyWindow("Command Prompt"),
                CreateIconForOpenDummyWindow("Read Me"),
                */
            };
            if (_isFullScreenEnabled)
            {
                _desktopIcons.Insert(0, _toggleFullscreenIcon);
                UpdateFullScreenIconName();
            }

            _hasDesktopIcons = true;


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
        void UpdateFullScreenIconName()
            => _toggleFullscreenIcon.Name = IsFullScreen
                ? _FULLSCREEN_DISABLE
                : _FULLSCREEN_ENABLE
            ;

        void MWindow_WindowClosed(object sender, EventArgs e)
        {
            MWindow window = (MWindow)sender;
            WindowViewModelBase winVM = (WindowViewModelBase)window.DataContext;
            if (winVM == null)
                return;


            bool wasActive = winVM.IsActive || (ActiveWindow == winVM);

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


        void AddWindow<TWindowVM>(string title = null
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