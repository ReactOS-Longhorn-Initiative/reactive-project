using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace ReactiveDream.Controls
{
    public class TransformAnimator3D
        : Decorator
    {
        static TransformAnimator3D()
        {
            AffectsRender<TransformAnimator3D>(
                TransformAngleXProperty, TransformAngleYProperty, TransformAngleZProperty,
                TransformCenterXProperty, TransformCenterYProperty, TransformCenterZProperty,
                TransformDepthProperty
            );
        }




        public static readonly StyledProperty<double> TransformAngleXProperty =
            AvaloniaProperty.Register<TransformAnimator3D, double>(nameof(TransformAngleX), 0d);
        public double TransformAngleX
        {
            get => GetValue(TransformAngleXProperty);
            set => SetValue(TransformAngleXProperty, value);
        }


        public static readonly StyledProperty<double> TransformAngleYProperty =
            AvaloniaProperty.Register<TransformAnimator3D, double>(nameof(TransformAngleY), 0d);
        public double TransformAngleY
        {
            get => GetValue(TransformAngleYProperty);
            set => SetValue(TransformAngleYProperty, value);
        }


        public static readonly StyledProperty<double> TransformAngleZProperty =
            AvaloniaProperty.Register<TransformAnimator3D, double>(nameof(TransformAngleZ), 0d);
        public double TransformAngleZ
        {
            get => GetValue(TransformAngleZProperty);
            set => SetValue(TransformAngleZProperty, value);
        }


        public static readonly StyledProperty<double> TransformCenterXProperty =
            AvaloniaProperty.Register<TransformAnimator3D, double>(nameof(TransformCenterX), 0d);
        public double TransformCenterX
        {
            get => GetValue(TransformCenterXProperty);
            set => SetValue(TransformCenterXProperty, value);
        }


        public static readonly StyledProperty<double> TransformCenterYProperty =
            AvaloniaProperty.Register<TransformAnimator3D, double>(nameof(TransformCenterY), -188d);
        public double TransformCenterY
        {
            get => GetValue(TransformCenterYProperty);
            set => SetValue(TransformCenterYProperty, value);
        }


        public static readonly StyledProperty<double> TransformCenterZProperty =
            AvaloniaProperty.Register<TransformAnimator3D, double>(nameof(TransformCenterZ), 0d);
        public double TransformCenterZ
        {
            get => GetValue(TransformCenterZProperty);
            set => SetValue(TransformCenterZProperty, value);
        }


        public static readonly StyledProperty<double> TransformDepthProperty =
            AvaloniaProperty.Register<TransformAnimator3D, double>(nameof(TransformDepth), 800d);
        public double TransformDepth
        {
            get => GetValue(TransformDepthProperty);
            set => SetValue(TransformDepthProperty, value);
        }




        public TransformAnimator3D()
            : base()
        {
            RenderTransform = CreateTransform();
        }




        Rotate3DTransform CreateTransform()
            => new()
            {
                [!Rotate3DTransform.AngleXProperty] = this[!TransformAngleXProperty],
                [!Rotate3DTransform.AngleYProperty] = this[!TransformAngleYProperty],
                [!Rotate3DTransform.AngleZProperty] = this[!TransformAngleZProperty],
                [!Rotate3DTransform.CenterXProperty] = this[!TransformCenterXProperty],
                [!Rotate3DTransform.CenterYProperty] = this[!TransformCenterYProperty],
                [!Rotate3DTransform.CenterZProperty] = this[!TransformCenterZProperty],
                [!Rotate3DTransform.DepthProperty] = this[!TransformDepthProperty],
            };
    }
}