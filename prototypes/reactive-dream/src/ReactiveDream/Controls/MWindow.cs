using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace ReactiveDream.Controls
{
    public class MWindow
        : MWindowBase
    {
        static MWindow()
        {
            HorizontalAlignmentProperty.OverrideDefaultValue<MWindow>(HorizontalAlignment.Left);
            VerticalAlignmentProperty.OverrideDefaultValue<MWindow>(VerticalAlignment.Top);
            IsActiveProperty.OverrideDefaultValue<MWindow>(false);


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
                    ? WindowAnimationState.Maximizing
                    : WindowAnimationState.RestoringFromMaximize
                ;
            });
            AnimationStateProperty.OverrideDefaultValue<MWindow>(WindowAnimationState.Hidden);

            DataContextProperty.Changed.AddClassHandler<MWindow>(DataContextProperty_Changed);
            IsVisibleProperty.Changed.AddClassHandler<MWindow>(IsVisibleProperty_Changed);
            
            AffectsRender<MWindow>(TransformAngleXProperty, TransformAngleYProperty, TransformAngleZProperty);
        }


        protected override void OnAnimationStateChanged(AvaloniaPropertyChangedEventArgs e)
        {
            base.OnAnimationStateChanged(e);

            if (_winVM == null)
                return;

            if (_winVM.IsOpened)
                return;

            WindowAnimationState newState = e.GetNewValue<WindowAnimationState>();
            if ((newState == WindowAnimationState.Opening) || (newState == WindowAnimationState.Opened))
                _winVM.IsOpened = true;
        }

        static void DataContextProperty_Changed(MWindow sender, AvaloniaPropertyChangedEventArgs e)
        {
            sender.OnDataContextChanged(e);
        }
        static void IsVisibleProperty_Changed(MWindow sender, AvaloniaPropertyChangedEventArgs e)
            => sender.OnIsVisibleChanged(e);


        RelativePoint MaximizeRestoreTransitionPoint()
            => new RelativePoint(
                Left + (WinWidth / 2),
                Top + (WinHeight / 2)
                , RelativeUnit.Absolute
            );


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



        ViewModels.WindowViewModelBase _winVM = null;
        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);

            if (_winVM != null)
            {
                _winVM.WindowClosing -= DataContext_WindowClosing;
                _winVM = null;
            }

            if (DataContext is ViewModels.WindowViewModelBase winVM)
            {
                _winVM = winVM;
                _winVM.WindowClosing += DataContext_WindowClosing;
            }
        }


        bool _hasAttachedToVisualTree = false;
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (_hasAttachedToVisualTree)
                return;

            if (_winVM != null)
            {
                if (_winVM.IsOpened)
                    AnimationState = WindowAnimationState.Opened;
                else
                {
                    AnimationState = WindowAnimationState.Opening;
                    _winVM.IsOpened = true;
                }
            }
            else
            {
                AnimationState = WindowAnimationState.Opening;
            }

            _hasAttachedToVisualTree = true;
        }


        void DataContext_WindowClosing(object sender, EventArgs e)
            => CloseWindow();

        bool _isClosing = false;
        public void CloseWindow()
        {
            if (_isClosing)
                return;

            _isClosing = true;
            AnimationState = WindowAnimationState.Closing;

            var command = CloseCommand;
            if ((command != null) && command.CanExecute(null))
                command.Execute(null);
        }

        void OnIsVisibleChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (!_isClosing)
                return;

            if (e.GetNewValue<bool>())
                return;

            WindowClosed?.Invoke(this, new());
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
            
            foreach (var visual in _pointerTestVisuals)
            {
                if (visual.Bounds.Contains(pt - GetTopLeftOf(visual)))
                    return true;
            }
            return false;
        }
        protected override T FindAndAdd<T>(INameScope nameScope, string name)
        {
            var match = base.FindAndAdd<T>(nameScope, name);

            if (match != null)
                _pointerTestVisuals.Add(match);

            return match;
        }


        MCaptionButtons _captionButtons = null;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _pointerTestVisuals.Clear();
            _pointerTestVisuals.Add(this);
            var nameScope = e.NameScope;


            var titleBar = FindAndAdd<Thumb>(nameScope, "PART_DragTitleBar");
            titleBar.DragDelta += (s, e) =>
            {
                if (IsMaximized)
                    IsMaximized = false;

                /*
                if (!IsActive)
                    return;
                */

                var vec = e.Vector;
                MoveX(vec.X);
                MoveY(vec.Y);
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

        void MoveY(double y)
            => Top += y;

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


        void This_PreviewPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (!e.Handled)
                Activate();
        }
        protected override void Activate()
        {
            if (!IsActive)
                ActivateCommand?.Execute(null);
        }


        public static event EventHandler<EventArgs> WindowClosed;
    }
}