using System;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Embedding.Offscreen;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;


namespace ReactiveDream.Controls
{
    public enum WindowAnimationState
        : int
    {
        Hidden,
        Opening,
        Opened,
        Maximizing,
        RestoringFromMaximize,
        Minimizing,
        Closing,
    }
    

    public class MWindowBase
        : HeaderedContentControl
    {
        static MWindowBase()
        {
            HorizontalAlignmentProperty.OverrideDefaultValue<MWindowBase>(HorizontalAlignment.Left);
            VerticalAlignmentProperty.OverrideDefaultValue<MWindowBase>(VerticalAlignment.Top);

            AvaloniaProperty[] props =
            {
                LeftProperty,
                TopProperty,
                WinWidthProperty,
                WinHeightProperty,
                IsMaximizedProperty,
                AnimationStateProperty,
            };
            
            AffectsMeasure<MWindowBase>(props);
            AffectsArrange<MWindowBase>(props);
            AffectsRender<MWindowBase>(props);

            IsMaximizedProperty.Changed.AddClassHandler<MWindowBase>(IsMaximizedProperty_Changed);
            AnimationStateProperty.Changed.AddClassHandler<MWindowBase>(AnimationStateProperty_Changed);
            
            AffectsRender<MWindowBase>(TransformAngleXProperty, TransformAngleYProperty, TransformAngleZProperty);
        }


        static void IsMaximizedProperty_Changed(MWindowBase sender, AvaloniaPropertyChangedEventArgs e)
        {
                sender.RenderTransformOrigin = sender.MaximizeRestoreTransitionPoint();
                sender.AnimationState = e.GetNewValue<bool>()
                    ? WindowAnimationState.Maximizing
                    : WindowAnimationState.RestoringFromMaximize
                ;
        }


        static void AnimationStateProperty_Changed(MWindowBase sender, AvaloniaPropertyChangedEventArgs e)
            => sender.OnAnimationStateChanged(e);
        protected virtual void OnAnimationStateChanged(AvaloniaPropertyChangedEventArgs e)
        {
            (WindowAnimationState oldState, WindowAnimationState newState) = e.GetOldAndNewValue<WindowAnimationState>();
            if (oldState == WindowAnimationState.Closing)
            {
                if (newState != WindowAnimationState.Closing)
                    AnimationState = WindowAnimationState.Closing;
            }

            if (newState == WindowAnimationState.Opened)
                return;
            if (newState == WindowAnimationState.Hidden)
                return;
            DoRevertAnimationStateTimer();
        }




        RelativePoint MaximizeRestoreTransitionPoint()
            => new RelativePoint(
                Left + (WinWidth / 2),
                Top + (WinHeight / 2)
                , RelativeUnit.Absolute
            );


        /// <summary>
        /// Defines the Left property.
        /// </summary>
        public static readonly StyledProperty<double> LeftProperty =
            AvaloniaProperty.Register<MWindowBase, double>(nameof(Left), 0);
        public double Left
        {
            get => GetValue(LeftProperty);
            set => SetValue(LeftProperty, value);
        }


        /// <summary>
        /// Defines the Top property.
        /// </summary>
        public static readonly StyledProperty<double> TopProperty =
            AvaloniaProperty.Register<MWindowBase, double>(nameof(Top), 0);
        public double Top
        {
            get => GetValue(TopProperty);
            set => SetValue(TopProperty, value);
        }


        /// <summary>
        /// Defines the WinWidth property.
        /// </summary>
        public static readonly StyledProperty<double> WinWidthProperty =
            AvaloniaProperty.Register<MWindowBase, double>(nameof(WinWidth), 0);
        public double WinWidth
        {
            get => GetValue(WinWidthProperty);
            set => SetValue(WinWidthProperty, value);
        }


        /// <summary>
        /// Defines the WinHeight property.
        /// </summary>
        public static readonly StyledProperty<double> WinHeightProperty =
            AvaloniaProperty.Register<MWindowBase, double>(nameof(WinHeight), 0);
        public double WinHeight
        {
            get => GetValue(WinHeightProperty);
            set => SetValue(WinHeightProperty, value);
        }


        public static readonly StyledProperty<bool> IsMaximizedProperty =
            AvaloniaProperty.Register<MWindowBase, bool>(nameof(IsMaximized), false);
        public bool IsMaximized
        {
            get => GetValue(IsMaximizedProperty);
            set => SetValue(IsMaximizedProperty, value);
        }


        public static readonly StyledProperty<bool> CanResizeProperty =
            AvaloniaProperty.Register<MWindowBase, bool>(nameof(CanResize), true);
        public bool CanResize
        {
            get => GetValue(CanResizeProperty);
            set => SetValue(CanResizeProperty, value);
        }


        public static readonly StyledProperty<bool> IsActiveProperty =
            AvaloniaProperty.Register<MWindowBase, bool>(nameof(IsActive), true);
        public bool IsActive
        {
            get => GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }


        public static readonly StyledProperty<WindowAnimationState> AnimationStateProperty =
            AvaloniaProperty.Register<MWindowBase, WindowAnimationState>(nameof(AnimationState), WindowAnimationState.Opened);
        public WindowAnimationState AnimationState
        {
            get => GetValue(AnimationStateProperty);
            set => SetValue(AnimationStateProperty, value);
        }




        public static readonly StyledProperty<double> TransformAngleXProperty =
            AvaloniaProperty.Register<MWindowBase, double>(nameof(TransformAngleX), 0);
        public double TransformAngleX
        {
            get => GetValue(TransformAngleXProperty);
            set => SetValue(TransformAngleXProperty, value);
        }


        public static readonly StyledProperty<double> TransformAngleYProperty =
            AvaloniaProperty.Register<MWindowBase, double>(nameof(TransformAngleY), 0);
        public double TransformAngleY
        {
            get => GetValue(TransformAngleYProperty);
            set => SetValue(TransformAngleYProperty, value);
        }


        public static readonly StyledProperty<double> TransformAngleZProperty =
            AvaloniaProperty.Register<MWindowBase, double>(nameof(TransformAngleZ), 0);
        public double TransformAngleZ
        {
            get => GetValue(TransformAngleZProperty);
            set => SetValue(TransformAngleZProperty, value);
        }


        public static readonly DirectProperty<MWindowBase, Rect> WinBoundsProperty =
            AvaloniaProperty.RegisterDirect<MWindowBase, Rect>(nameof(Bounds), o => o.WinBounds, (o, v) => o.WinBounds = v);
        public Rect WinBounds
        {
            get => new(Left, Top, WinWidth, WinHeight);
            set
            {
                Left = value.Left;
                Top = value.Top;
                WinWidth = value.Width;
                WinHeight = value.Height;
            }
        }


        public string Title
        {
            get
            {
                var header = Header;
                if (header == null)
                    return "{x:Null}";
                else if (header is string titleText)
                    return titleText;
                else
                    return header.ToString();
            }
        }




        void DoRevertAnimationStateTimer(WindowAnimationState animState = WindowAnimationState.Opened)
        {
            Timer timer = new Timer(1000);
            bool hasTimerFinished = false;
            timer.Elapsed += (s, e) =>
            {
                if (hasTimerFinished)
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        if (AnimationState != WindowAnimationState.Closing)
                            AnimationState = animState;
                    });
                    timer.Stop();
                }
                else
                {
                    hasTimerFinished = true;
                }
            };
            timer.Start();
        }


        protected virtual T FindAndAdd<T>(INameScope nameScope, string name)
            where T : Visual
            => nameScope.Find<T>(name);


        MCaptionButtons _captionButtons = null;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            var nameScope = e.NameScope;

            _captionButtons = FindAndAdd<MCaptionButtons>(nameScope, "PART_CaptionButtons");
            _captionButtons.Attach(this);

            var root3D = e.NameScope.Find<Visual>("PART_3DRoot");
            root3D.RenderTransform = new Rotate3DTransform()
            {
                [!Rotate3DTransform.AngleXProperty] = this[!TransformAngleXProperty],
                [!Rotate3DTransform.AngleYProperty] = this[!TransformAngleYProperty],
                [!Rotate3DTransform.AngleZProperty] = this[!TransformAngleZProperty],
                CenterY = -188,
                Depth = 800
            };
        }


        void This_PreviewPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (!e.Handled)
                Activate();
        }


        protected virtual void Activate()
        {}




        const string _TOSTRING_PREFIX = nameof(MWindow);
        public string ToString(bool includeTypeName, bool useTypeFullName = false)
        {
            string typeName;
            if (includeTypeName)
            {
                Type type = GetType();
                if (useTypeFullName)
                    typeName = type.FullName;
                else
                    typeName = type.Name;
            }
            else
                typeName = _TOSTRING_PREFIX;

            return $"({typeName} titled '{Title}' at '{WinBounds}')";
        }
        public override string ToString()
            => ToString(true);
    }
}