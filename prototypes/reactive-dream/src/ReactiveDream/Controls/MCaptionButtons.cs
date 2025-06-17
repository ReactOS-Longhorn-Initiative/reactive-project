using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Controls.Primitives;

namespace ReactiveDream.Controls
{
    public class MCaptionButtons
        : CaptionButtons
    {
        private const string PART_CloseButton = "PART_CloseButton";
        private const string PART_RestoreButton = "PART_RestoreButton";
        private const string PART_MinimizeButton = "PART_MinimizeButton";
        private const string PART_FullScreenButton = "PART_FullScreenButton";


        public static readonly StyledProperty<bool> UseUniformButtonSizingProperty =
            AvaloniaProperty.Register<MCaptionButtons, bool>(nameof(UseUniformButtonSizing), false);
        public bool UseUniformButtonSizing
        {
            get => GetValue(UseUniformButtonSizingProperty);
            set => SetValue(UseUniformButtonSizingProperty, value);
        }

        public static readonly StyledProperty<double> UniformButtonWidthProperty =
            AvaloniaProperty.Register<MCaptionButtons, double>(nameof(UniformButtonWidth));
        public double UniformButtonWidth
        {
            get => GetValue(UniformButtonWidthProperty);
            set => SetValue(UniformButtonWidthProperty, value);
        }

        public static readonly StyledProperty<double> UniformButtonHeightProperty =
            AvaloniaProperty.Register<MCaptionButtons, double>(nameof(UniformButtonHeight));
        public double UniformButtonHeight
        {
            get => GetValue(UniformButtonHeightProperty);
            set => SetValue(UniformButtonHeightProperty, value);
        }
        
        static MCaptionButtons()
        {
            AffectsMeasure<MCaptionButtons>(
                UseUniformButtonSizingProperty
                , UniformButtonWidthProperty
                , UniformButtonHeightProperty
            );

            UniformButtonWidthProperty.Changed.AddClassHandler<MCaptionButtons>((s, e) => Debug.WriteLine($"UniformButtonWidth: {e.NewValue}"));
            UniformButtonHeightProperty.Changed.AddClassHandler<MCaptionButtons>((s, e) => Debug.WriteLine($"UniformButtonHeight: {e.NewValue}"));
        }


        /*protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnAttachedToLogicalTree(e);
            e
        }*/
        MWindow _hostWindow = null;
        public virtual void Attach(MWindow hostWindow)
            => _hostWindow = hostWindow;
        


        public override void Attach(Window hostWindow)
        {
            /*if (_disposables == null)
            {
                HostWindow = hostWindow;

                _disposables = new CompositeDisposable
                {
                    HostWindow.GetObservable(Window.CanResizeProperty)
                        .Subscribe(x =>
                        {
                            if (_restoreButton is not null)
                                _restoreButton.IsEnabled = x;
                        }),
                    HostWindow.GetObservable(Window.WindowStateProperty)
                        .Subscribe(x =>
                        {
                            PseudoClasses.Set(":minimized", x == WindowState.Minimized);
                            PseudoClasses.Set(":normal", x == WindowState.Normal);
                            PseudoClasses.Set(":maximized", x == WindowState.Maximized);
                            PseudoClasses.Set(":fullscreen", x == WindowState.FullScreen);
                        }),
                };
            }*/
        }

        public override void Detach()
        {
            /*if (_disposables != null)
            {
                _disposables.Dispose();
                _disposables = null;

                HostWindow = null;
            }*/
        }
        protected bool TryEnsureCommand(
#nullable enable
            ICommand? inCommand
#nullable restore
            , out ICommand command)
        {
            command = null;

            if (_hostWindow == null)
                return false;
                
            if (inCommand?.CanExecute(null) == true)
            {
                command = inCommand;
                return true;
            }
            
            return false;
        }
        protected override void OnClose()
        {
            if (_hostWindow != null)
                _hostWindow.AnimationState = MWindowAnimationState.Closing;

            if (TryEnsureCommand(_hostWindow?.CloseCommand, out ICommand cmd))
                cmd.Execute(null);
            //HostWindow?.Close();
        }

        protected override void OnRestore()
        {
            if (_hostWindow != null)
                _hostWindow.IsMaximized = !_hostWindow.IsMaximized;

            if (TryEnsureCommand(_hostWindow?.MaximizeCommand, out ICommand cmd))
                cmd.Execute(null);
            /*if (HostWindow != null)
            {
                HostWindow.WindowState = HostWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }*/
        }

        protected override void OnMinimize()
        {
            if (TryEnsureCommand(_hostWindow?.MinimizeCommand, out ICommand cmd))
                cmd.Execute(null);
            /*if (HostWindow != null)
            {
                HostWindow.WindowState = WindowState.Minimized;
            }*/
        }

        protected override void OnToggleFullScreen()
        {
            /*if (HostWindow != null)
            {
                HostWindow.WindowState = HostWindow.WindowState == WindowState.FullScreen
                    ? WindowState.Normal
                    : WindowState.FullScreen;
            }*/
        }

        const string _BTNSIZE_CLASS = "Window";
        const string _BTNSIZE_PART = "CloseButton";
        const string _BTNSIZE_STATE = "Normal";
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            if (UseUniformButtonSizing)
            {
                if (EnsureButtonWidth())
                    return;
            }

            UniformButtonWidth = 0;
            UniformButtonHeight = 0;
        }

        bool EnsureButtonWidth()
        {
#if NO
            var currentVS = MsStyles.Current;
            if (currentVS == null)
                return false;
            
            if (!currentVS.TryGetEnum(null, _BTNSIZE_CLASS, _BTNSIZE_PART, _BTNSIZE_STATE, "BgType", out BGTYPE bgType))
                return false;
            if (bgType != BGTYPE.IMAGEFILE)
                return false;

            /*if (!currentVS.TryGetEnum(null, _BTNSIZE_CLASS, _BTNSIZE_PART, _BTNSIZE_STATE, "SizingType", out SIZINGTYPE sizingType))
                return false;
            if (sizingType == SIZINGTYPE.TRUESIZE)
                return false;*/
            
            if (!currentVS.TryGetImage(null, _BTNSIZE_CLASS, _BTNSIZE_PART, _BTNSIZE_STATE, "ImageFile", out IImage image))
                return false;
            if (image == null)
                return false;
            
            var imgSize = image.Size;
#endif //NO

            Size imgSize = new Size(17, 136);
#if NO
            int imgCount = 1;
            if (currentVS.TryGetValue(null, _BTNSIZE_CLASS, _BTNSIZE_PART, _BTNSIZE_STATE, "ImageCount", out int imageCount))
            {
                if (imageCount > 0)
                    imgCount = imageCount;
                else
                    return false;
            }
#else
            int imgCount = 8;
#endif //NO
            
            var imgWidth = imgSize.Width;
            if (imgWidth <= 0)
                return false;

            var imgHeight = imgSize.Height / imgCount;
            if (imgHeight <= 0)
                return false;
            Debug.WriteLine($"imgSize: {imgWidth}, {imgHeight}");
            
#if NO
            currentVS.TryGetValue(null, "SysMetrics", null, null, "CaptionBarHeight", out int heightFac);
#else
            int heightFac = 1;
#endif //NO
            UniformButtonHeight = heightFac - 4;
            UniformButtonWidth = ((imgWidth / imgHeight) * heightFac) - 4;
            return true;
        }
        /*protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            var closeButton = e.NameScope.Get<Button>(PART_CloseButton);
            var restoreButton = e.NameScope.Get<Button>(PART_RestoreButton);
            var minimizeButton = e.NameScope.Get<Button>(PART_MinimizeButton);
            var fullScreenButton = e.NameScope.Get<Button>(PART_FullScreenButton);

            closeButton.Click += (sender, e) => OnClose();
            restoreButton.Click += (sender, e) => OnRestore();
            minimizeButton.Click += (sender, e) => OnMinimize();
            fullScreenButton.Click += (sender, e) => OnToggleFullScreen();

            restoreButton.IsEnabled = HostWindow?.CanResize ?? true;
            _restoreButton = restoreButton;
        }*/
    }
}