// https://gist.github.com/kekekeks/ac06098a74fe87d49a9ff9ea37fa67bc
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace ReactiveDream.Controls
{
    public class BlurBehindDecorator
        : Decorator
    {
        public static readonly StyledProperty<double> BlurRadiusProperty =
            AvaloniaProperty.Register<BlurBehindDecorator, double>(nameof(BlurRadius));

        public double BlurRadius
        {
            get => GetValue(BlurRadiusProperty);
            set => SetValue(BlurRadiusProperty, value);
        }


        public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
            Border.CornerRadiusProperty.AddOwner<BlurBehindDecorator>();
        
        public CornerRadius CornerRadius
        {
            get => GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }




        static BlurBehindDecorator()
        {
            AffectsRender<BlurBehindDecorator>(BlurRadiusProperty);
            AffectsRender<BlurBehindDecorator>(CornerRadiusProperty);

            BlurRadiusProperty.Changed.AddClassHandler<BlurBehindDecorator>(BlurRadiusProperty_Changed);
            BoundsProperty.Changed.AddClassHandler<BlurBehindDecorator>(BoundsProperty_Changed);
        }

        static void BlurRadiusProperty_Changed(BlurBehindDecorator sender, AvaloniaPropertyChangedEventArgs e)
            => sender._blurBehindOp.BlurRadius = e.GetNewValue<double>();

        static void BoundsProperty_Changed(BlurBehindDecorator sender, AvaloniaPropertyChangedEventArgs e)
            => sender?.RefreshBlurBounds(e.GetNewValue<Rect>().Size);




        public BlurBehindDecorator()
            : base()
        {
            //Padding
            
        }


        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);
            RefreshBlurBounds(e.NewSize);
        }


        static readonly Point _POINT_ZERO = new(0, 0);
        void RefreshBlurBounds(Size size)
            => _blurBehindOp.BlurBounds = new(_POINT_ZERO, size);




        readonly BlurBehindRenderOperation _blurBehindOp = new();
        public override void Render(DrawingContext context)
        {
            base.Render(context);
            context.Custom(_blurBehindOp);
        }
    }
}