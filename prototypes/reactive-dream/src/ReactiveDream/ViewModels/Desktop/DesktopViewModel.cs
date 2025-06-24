using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using WallpaperImagePosition = ReactiveDream.Controls.WallpaperImagePosition;

namespace ReactiveDream.ViewModels
{
    public partial class DesktopViewModel
        : SubViewModelBase
    {
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




        bool _hasNeverCalledSetFullScreenCapability = true;
        bool _isFullScreenCapable = false;
        public void SetFullScreenCapability(bool enable, ref bool switchToFullscreen)
        {
            _isFullScreenCapable = enable;
            if (_hasNeverCalledSetFullScreenCapability)
            {
                _hasNeverCalledSetFullScreenCapability = false;

                if (_isFullScreenCapable)
                    switchToFullscreen = true;
            }


            if (!_hasDesktopIcons)
                return;


            if (!_isFullScreenCapable)
                return;

            if ((_toggleFullscreenIcon != null) && (_toggleFullscreenIconInfo != null))
            {
                UpdateIconPresence(_toggleFullscreenIconInfo);
                _toggleFullscreenIcon.UpdateState();
            }
        }


        bool _hasDesktopIcons = false;
        public DesktopViewModel()
        {
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
            _hasDesktopIcons = true;
        }
        
        protected override void OnReceivedMainVM(MainViewModel vm)
        {
            base.OnReceivedMainVM(vm);
            CreateConditionalIcons(vm);
            vm.PropertyChanged += MainVM_PropertyChanged;
        }

        void MainVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MainViewModel vm;
            if (sender is MainViewModel mainViewModel)
                vm = mainViewModel;
            else if (!TryGetMainVM(out vm))
                return;

            UpdateIcons(vm, e);
        }


        void AddWindow<TWindowVM>(string title = null
        , double x = double.NaN, double y = double.NaN, double width = double.NaN, double height = double.NaN
        )
            where TWindowVM : WindowViewModelBase, new()
        {
            if (TryGetMainVM(out MainViewModel mainVm))
                mainVm.AddWindow<TWindowVM>(title, x, y, width, height);
        }
    }
}