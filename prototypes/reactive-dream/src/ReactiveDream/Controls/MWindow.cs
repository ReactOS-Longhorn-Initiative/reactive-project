using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Timers;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace ReactiveDream.Controls
{
    public enum MWindowAnimationState
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
    

    public class MWindow : HeaderedContentControl
    {
        static MWindow()
        {
            HorizontalAlignmentProperty.OverrideDefaultValue<MWindow>(HorizontalAlignment.Left);
            VerticalAlignmentProperty.OverrideDefaultValue<MWindow>(VerticalAlignment.Top);

            AvaloniaProperty[] props =
            {
                LeftProperty,
                TopProperty,
                WinWidthProperty,
                WinHeightProperty,
                IsMaximizedProperty,
                AnimationStateProperty,
            };
            
            AffectsMeasure<MWindow>(props);
            AffectsArrange<MWindow>(props);
            AffectsRender<MWindow>(props);

            IsMaximizedProperty.Changed.AddClassHandler<MWindow>((s, e) =>
            {
                s.RenderTransformOrigin = s.MaximizeRestoreTransitionPoint();
                s.AnimationState = e.GetNewValue<bool>()
                    ? MWindowAnimationState.Maximizing
                    : MWindowAnimationState.RestoringFromMaximize
                ;
            });
            AnimationStateProperty.Changed.AddClassHandler<MWindow>((s, e) =>
            {
                var newState = e.GetNewValue<MWindowAnimationState>();
                if (newState == MWindowAnimationState.Opened)
                    return;
                if (newState == MWindowAnimationState.Hidden)
                    return;
                s.DoRevertAnimationStateTimer();
            });
            
            AffectsRender<MWindow>(TransformAngleXProperty, TransformAngleYProperty, TransformAngleZProperty);
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
            AvaloniaProperty.Register<MWindow, double>(nameof(Left), 0);
        public double Left
        {
            get => GetValue(LeftProperty);
            set => SetValue(LeftProperty, value);
        }

        /// <summary>
        /// Defines the Top property.
        /// </summary>
        public static readonly StyledProperty<double> TopProperty =
            AvaloniaProperty.Register<MWindow, double>(nameof(Top), 0);
        public double Top
        {
            get => GetValue(TopProperty);
            set => SetValue(TopProperty, value);
        }


        /// <summary>
        /// Defines the WinWidth property.
        /// </summary>
        public static readonly StyledProperty<double> WinWidthProperty =
            AvaloniaProperty.Register<MWindow, double>(nameof(WinWidth), 0);
        public double WinWidth
        {
            get => GetValue(WinWidthProperty);
            set => SetValue(WinWidthProperty, value);
        }


        /// <summary>
        /// Defines the WinHeight property.
        /// </summary>
        public static readonly StyledProperty<double> WinHeightProperty =
            AvaloniaProperty.Register<MWindow, double>(nameof(WinHeight), 0);
        public double WinHeight
        {
            get => GetValue(WinHeightProperty);
            set => SetValue(WinHeightProperty, value);
        }

        public static readonly StyledProperty<bool> IsMaximizedProperty =
            AvaloniaProperty.Register<MWindow, bool>(nameof(IsMaximized), false);
        public bool IsMaximized
        {
            get => GetValue(IsMaximizedProperty);
            set => SetValue(IsMaximizedProperty, value);
        }

        public static readonly StyledProperty<bool> CanResizeProperty =
            AvaloniaProperty.Register<MWindow, bool>(nameof(CanResize), true);
        public bool CanResize
        {
            get => GetValue(CanResizeProperty);
            set => SetValue(CanResizeProperty, value);
        }

        public static readonly StyledProperty<bool> IsActiveProperty =
            AvaloniaProperty.Register<MWindow, bool>(nameof(IsActive), false);
        public bool IsActive
        {
            get => GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }


        /*
        public static readonly StyledProperty<bool> IsOpenedProperty =
            AvaloniaProperty.Register<MWindow, bool>(nameof(IsOpened), false);
        public bool IsOpened
        {
            get => GetValue(IsOpenedProperty);
            set => SetValue(IsOpenedProperty, value);
        }


        public static readonly StyledProperty<bool> IsClosingProperty =
            AvaloniaProperty.Register<MWindow, bool>(nameof(IsClosing), false);
        public bool IsClosing
        {
            get => GetValue(IsClosingProperty);
            set => SetValue(IsClosingProperty, value);
        }
        */
        public static readonly StyledProperty<MWindowAnimationState> AnimationStateProperty =
            AvaloniaProperty.Register<MWindow, MWindowAnimationState>(nameof(AnimationState), MWindowAnimationState.Hidden);
        public MWindowAnimationState AnimationState
        {
            get => GetValue(AnimationStateProperty);
            set => SetValue(AnimationStateProperty, value);
        }


#nullable enable
        /// <summary>
        /// Defines the <see cref="MinimizeCommand"/> property.
        /// </summary>
        public static readonly StyledProperty<ICommand?> MinimizeCommandProperty =
            AvaloniaProperty.Register<MWindow, ICommand?>(nameof(MinimizeCommand), enableDataValidation: true);

        /// <summary>
        /// Gets or sets an <see cref="ICommand"/> to be invoked when the Minimize button is clicked.
        /// </summary>
        public ICommand? MinimizeCommand
        {
            get => GetValue(MinimizeCommandProperty);
            set => SetValue(MinimizeCommandProperty, value);
        }

        /// <summary>
        /// Defines the <see cref="MaximizeCommand"/> property.
        /// </summary>
        public static readonly StyledProperty<ICommand?> MaximizeCommandProperty =
            AvaloniaProperty.Register<MWindow, ICommand?>(nameof(MaximizeCommand), enableDataValidation: true);

        /// <summary>
        /// Gets or sets an <see cref="ICommand"/> to be invoked when the Maximize button is clicked.
        /// </summary>
        public ICommand? MaximizeCommand
        {
            get => GetValue(MaximizeCommandProperty);
            set => SetValue(MaximizeCommandProperty, value);
        }

        /// <summary>
        /// Defines the <see cref="CloseCommand"/> property.
        /// </summary>
        public static readonly StyledProperty<ICommand?> CloseCommandProperty =
            AvaloniaProperty.Register<MWindow, ICommand?>(nameof(CloseCommand), enableDataValidation: true);

        /// <summary>
        /// Gets or sets an <see cref="ICommand"/> to be invoked when the Close button is clicked.
        /// </summary>
        public ICommand? CloseCommand
        {
            get => GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        /// <summary>
        /// Defines the <see cref="ActivateCommand"/> property.
        /// </summary>
        public static readonly StyledProperty<ICommand?> ActivateCommandProperty =
            AvaloniaProperty.Register<MWindow, ICommand?>(nameof(ActivateCommand), enableDataValidation: true);

        /// <summary>
        /// Gets or sets an <see cref="ICommand"/> to be invoked when the window should be activated.
        /// </summary>
        public ICommand? ActivateCommand
        {
            get => GetValue(ActivateCommandProperty);
            set => SetValue(ActivateCommandProperty, value);
        }
#nullable restore


        public static readonly StyledProperty<double> TransformAngleXProperty =
            AvaloniaProperty.Register<MWindow, double>(nameof(TransformAngleX), 0);
        public double TransformAngleX
        {
            get => GetValue(TransformAngleXProperty);
            set => SetValue(TransformAngleXProperty, value);
        }
        public static readonly StyledProperty<double> TransformAngleYProperty =
            AvaloniaProperty.Register<MWindow, double>(nameof(TransformAngleY), 0);
        public double TransformAngleY
        {
            get => GetValue(TransformAngleYProperty);
            set => SetValue(TransformAngleYProperty, value);
        }

        public static readonly StyledProperty<double> TransformAngleZProperty =
            AvaloniaProperty.Register<MWindow, double>(nameof(TransformAngleZ), 0);
        public double TransformAngleZ
        {
            get => GetValue(TransformAngleZProperty);
            set => SetValue(TransformAngleZProperty, value);
        }



        bool _openPending = true;
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            
            if (!_openPending)
                return;
            
            _openPending = false;
            //AddPointerPressedHandler(this);
            if (DataContext is ViewModels.WindowViewModelBase winVM) //HACK: NOOOOOOOOOOOOOOO
            {
                if (winVM.IsOpened)
                {
                    AnimationState = MWindowAnimationState.Opened;
                    return;
                }
                
                winVM.IsOpened = true;
            }
            AnimationState = MWindowAnimationState.Opening;
        }

        void AddPointerPressedHandler(InputElement addTo, bool tunnel = false)
        {
            if (tunnel)
                addTo.AddHandler(PointerPressedEvent, This_PreviewPointerPressed, RoutingStrategies.Tunnel);
            else
                addTo.PointerPressed += This_PreviewPointerPressed;
        }

        void DoRevertAnimationStateTimer(MWindowAnimationState animState = MWindowAnimationState.Opened)
        {
            Timer timer = new Timer(1000);
            bool hasTimerFinished = false;
            timer.Elapsed += (s, e) =>
            {
                if (hasTimerFinished)
                {
                    Dispatcher.UIThread.Post(() => AnimationState = animState);
                    timer.Stop();
                }
                else
                {
                    hasTimerFinished = true;
                }
            };
            timer.Start();
        }

        
        List<Visual> _pointerTestVisuals = new();
        static Point GetTopLeftOf(Visual vis)
        {
            Rect bounds = vis.GetTransformedBounds()
                ?.Bounds
                ?? vis.Bounds
            ;
            return bounds.TopLeft;
        }
        public bool IsPointWithinOrWithinNC(Point pt)
        {
            if (Bounds.Contains(pt - GetTopLeftOf(this)))
                return true;
            
            //var ptTransformed =  (Parent as Visual).transform
            
            foreach (var visual in _pointerTestVisuals)
            {
                if (visual.Bounds.Contains(pt - GetTopLeftOf(visual)))
                    return true;
            }
            return false;
        }
        T FindAndAdd<T>(INameScope nameScope, string name)
            where T : Visual
        {
            var match = nameScope.Find<T>(name);
            if (match != null)
                _pointerTestVisuals.Add(match);
            return match;
        }
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _pointerTestVisuals.Clear();
            _pointerTestVisuals.Add(this);
            var nameScope = e.NameScope;
            var captionButtons = FindAndAdd<MCaptionButtons>(nameScope, "PART_CaptionButtons");
            //AddPointerPressedHandler(captionButtons);
            captionButtons.Attach(this);


            var titleBar = FindAndAdd<Thumb>(nameScope, "PART_DragTitleBar");
            //AddPointerPressedHandler(titleBar);
            titleBar.DragDelta += (s, e) =>
            {
                if (true) //IsActive)
                {
                    var vec = e.Vector;
                    MoveX(vec.X);
                    MoveY(vec.Y);
                }
                else
                {
                    Activate();
                }
                
            };


            string p = "PART_DragResize";
            
            FindAndAdd<Thumb>(nameScope, $"{p}TL").DragDelta += (s, e) =>
            {
                var vec = e.Vector;
                ResizeMoveX(vec.X);
                ResizeMoveY(vec.Y);
            };
            FindAndAdd<Thumb>(nameScope, $"{p}T").DragDelta += (s, e) => ResizeMoveY(e.Vector.Y);
            FindAndAdd<Thumb>(nameScope, $"{p}TR").DragDelta += (s, e) =>
            {
                var vec = e.Vector;
                ResizeX(vec.X);
                ResizeMoveY(vec.Y);
            };
            
            FindAndAdd<Thumb>(nameScope, $"{p}L").DragDelta += (s, e) => ResizeMoveX(e.Vector.X);
            FindAndAdd<Thumb>(nameScope, $"{p}R").DragDelta += (s, e) => ResizeX(e.Vector.X);
            
            FindAndAdd<Thumb>(nameScope, $"{p}BL").DragDelta += (s, e) =>
            {
                var vec = e.Vector;
                ResizeMoveX(vec.X);
                ResizeY(vec.Y);
            };
            FindAndAdd<Thumb>(nameScope, $"{p}B").DragDelta += (s, e) => ResizeY(e.Vector.Y);
            FindAndAdd<Thumb>(nameScope, $"{p}BR").DragDelta += (s, e) =>
            {
                var vec = e.Vector;
                ResizeX(vec.X);
                ResizeY(vec.Y);
            };


            var root3D = e.NameScope.Find<Visual>("PART_3DRoot");
            //AddPointerPressedHandler((InputElement)root3D);
            root3D.RenderTransform = new Rotate3DTransform()
            {
                [!Rotate3DTransform.AngleXProperty] = this[!TransformAngleXProperty],
                [!Rotate3DTransform.AngleYProperty] = this[!TransformAngleYProperty],
                [!Rotate3DTransform.AngleZProperty] = this[!TransformAngleZProperty],
                CenterY = -188,
                Depth = 800
            };
        }

        void MoveX(double x)
            => Left += x;
        /*{
            var thisP = this;
            Canvas.SetLeft(thisP, Canvas.GetLeft(thisP) + x);
        }*/
        void MoveY(double y)
            => Top += y;
        /*{
            var thisP = this;
            Canvas.SetTop(thisP, Canvas.GetTop(thisP) + y);
        }*/
        void ResizeMoveX(double x)
        {
            MoveX(x);
            ResizeX(-x);
        }
        void ResizeMoveY(double y)
        {
            MoveY(y);
            ResizeY(-y);
        }
        void ResizeX(double x)
            => WinWidth += x;
        void ResizeY(double y)
            => WinHeight += y;

        void zOnApplyTemplate(TemplateAppliedEventArgs e)
        {
            var resizeEdgesGrid = e.NameScope.Find<Grid>("PART_DragResizeEdges");
            var resizeEdges = resizeEdgesGrid.Children.OfType<Thumb>().ToList();
            for (int r = 0; r < 2; r++)
            {
                for (int c = 0; c < 2; c++)
                {
                    /*if ((r == 1) && (c == 1))
                        continue;*/
                    
                    var edge = resizeEdges.First(x => MatchDragHandler(x, r, c));
                    if (edge != null)
                        AttachDragHandler(edge, r, c);
                }
            }
        }

        bool MatchDragHandler(Thumb thumb, int r, int c)
        {
            var row = Grid.GetRow(thumb);
            var column = Grid.GetColumn(thumb);
            return (r == row) && (c == column);
        }

        void AttachDragHandler(Thumb thumb, int c, int r)
        {
            Action<double> xAct = null;
            if (c == 0)
                xAct = (x => Canvas.SetLeft(this, Canvas.GetLeft(this) + x));
            else if (c == 2)
                xAct = (x => WinWidth += x);
            
            Action<double> yAct = null;
            if (r == 0)
                yAct = (y => Canvas.SetTop(this, Canvas.GetTop(this) + y));
            else if (r == 2)
                yAct = (y => WinHeight += y);
            
            if (xAct == null)
                thumb.DragDelta += (s, e) => yAct(e.Vector.Y);
            else if (yAct == null)
                thumb.DragDelta += (s, e) => xAct(e.Vector.X);
            else
            {
                thumb.DragDelta += (s, e) =>
                {
                    var vec = e.Vector;
                    xAct(vec.X);
                    yAct(vec.Y);
                };
            }
        }

        void This_PreviewPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (!e.Handled)
                Activate();
        }
        void Activate()
        {
            if (!IsActive)
                ActivateCommand?.Execute(null);
        }
        /*
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (!IsActive)
                ActivateCommand?.Execute(null);
        }
        */


        /*protected override Size MeasureOverride(Size availableSize)
        {
            if (IsMaximized && (Parent is Layoutable lyt))
                return lyt.Bounds.Size;
            else
                return base.MeasureOverride(availableSize);
        }

        protected override void ArrangeCore(Rect finalRect)
        {
            if (IsMaximized && (Parent is Layoutable lyt))
                base.ArrangeCore(lyt.Bounds);
            else
                base.ArrangeCore(finalRect);
        }*/
    }
}