using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace ReactiveDream.ViewModels
{
    public class MainViewModel
        : ViewModelBase
    {
        bool _isCompositionActive = true;
        public bool IsCompositionActive
        {
            get => _isCompositionActive;
            set => RASIC(ref _isCompositionActive, value);
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

                if (prev != null)
                    prev.IsActive = false;

                if (value == null)
                    return;

                if (!Windows.Contains(value)) //Windows.Remove(value))
                    return;

                Windows.Move(Windows.IndexOf(value), Windows.Count - 1);
                value.IsActive = true;
                //Windows.Add(value);
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

        /*
        DesktopIconViewModel CreateIconForOpenDummyWindow(string name, object content = null, double x = 40, double y = 40, double width = 320, double height = 240)
            => CreateIconForOpenDummyWindow(name, name, content, x, y, width, height);
        DesktopIconViewModel CreateIconForOpenDummyWindow(string name, string windowTitle, object content = null, double x = 40, double y = 40, double width = 320, double height = 240)
            => new(name)
            {
                Command = () => AddWindow<WindowViewModelBase>(windowTitle, content, x, y, width, height),
                /*{
                    WindowViewModel winVM = new(this)
                    {
                        Title = title,
                        Content = content,
                        X = x,
                        Y = y,
                        Width = width,
                        Height = height,
                    };
                    Windows.Add(winVM);
                },* /
            };
        */
        public MainViewModel()
        {
            DesktopIconViewModel toggleDwm = new()
            {
                Name = _DWM_DISABLE
            };

            toggleDwm.Command = () =>
            {
                bool isEnablingComposition = !IsCompositionActive;
                IsCompositionActive = isEnablingComposition;
                toggleDwm.Name = isEnablingComposition
                    ? _DWM_DISABLE
                    : _DWM_ENABLE
                ;
            };

            _desktopIcons = new()
            {
                toggleDwm,
                new("Create sample window")
                {
                    Command = () => AddWindow<SampleWindowViewModel>("Sample window"),
                },
                new("Run...")
                {
                    Command = () => AddWindow<RunWindowViewModel>(),
                },
                new("Notepad")
                {
                    Command = () => AddWindow<NotepadWindowViewModel>(),
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


            Windows.CollectionChanged += Windows_CollectionChanged;
            WindowViewModelBase.WindowClosed += (s, e) => 
            {
                var win = e.WindowVM;
                if (Windows.Contains(win))
                {
                    var index = Windows.IndexOf(win);
                    Windows.Remove(win);

#pragma warning disable CS0642
                    if (index >= Windows.Count);
#pragma warning restore CS0642
                    else if (index >= 0)
                    {
                        ActiveWindow = Windows[index];
                    }
                }
            };
            WindowViewModelBase.WindowActivated += (s, e) => ActiveWindow = e.WindowVM;
        }

        List<WindowViewModelBase> _windowsTemp = new();
        bool _isRefreshing = false;
        public void RefreshHackPrepare()
        {
            _isRefreshing = true;
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
            _isRefreshing = false;
        }
        void Windows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isRefreshing)
                return;
        }


        void AddWindow<TWindowVM>(string title = null
        , double x = -1, double y = -1, double width = -1, double height = -1
        //, double x = 40, double y = 40, double width = 320, double height = 240
        )
            where TWindowVM : WindowViewModelBase, new()
        {
            TWindowVM winVM = new TWindowVM().WithMainVM<TWindowVM>(this);

            if (!string.IsNullOrWhiteSpace(title))
                winVM.Title = title;

            if (x >= 0)
                winVM.X = x;

            if (y >= 0)
                winVM.Y = y;

            if (width > 0)
                winVM.Width = width;

            if (height > 0)
                winVM.Height = height;

            Windows.Add(winVM);
        }
    }
}