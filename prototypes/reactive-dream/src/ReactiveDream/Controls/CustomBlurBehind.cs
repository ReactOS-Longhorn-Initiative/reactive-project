// https://gist.github.com/kekekeks/ac06098a74fe87d49a9ff9ea37fa67bc
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace ReactiveDream.Controls
{
    public class CustomBlurBehind
        : Decorator
    {
        public static readonly StyledProperty<double> BlurRadiusProperty =
            AvaloniaProperty.Register<CustomBlurBehind, double>(nameof(BlurRadius));

        public double BlurRadius
        {
            get => GetValue(BlurRadiusProperty);
            set => SetValue(BlurRadiusProperty, value);
        }

        public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
            Border.CornerRadiusProperty.AddOwner<CustomBlurBehind>();
        
        public CornerRadius CornerRadius
        {
            get => GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }


        static CustomBlurBehind()
        {
            AffectsRender<CustomBlurBehind>(BlurRadiusProperty);
            AffectsRender<CustomBlurBehind>(CornerRadiusProperty);
        }


        class BlurBehindRenderOperation
            : ICustomDrawOperation
        {
            readonly double _blurRadius;
            readonly Rect _bounds;

            public BlurBehindRenderOperation(double blurRadius, Rect bounds)
            {
                _blurRadius = blurRadius;
                _bounds = bounds;
            }
            
            public void Dispose()
            {
                
            }

            public bool HitTest(Point p) => _bounds.Contains(p);

            
            public void Render(ImmediateDrawingContext context)
            {
                // https://github.com/AvaloniaUI/Avalonia/issues/8615#issuecomment-1221449455
                if (!context.TryGetFeature(out ISkiaSharpApiLeaseFeature leaseFeature))
                    return;
                using var skia = leaseFeature.Lease();


                if (!skia.SkCanvas.TotalMatrix.TryInvert(out var currentInvertedTransform))
                    return;

                using var backgroundSnapshot = skia.SkSurface.Snapshot();
                using var backdropShader = SKShader.CreateImage(backgroundSnapshot, SKShaderTileMode.Clamp,
                    SKShaderTileMode.Clamp, currentInvertedTransform);

                using var blurred = SKSurface.Create(skia.GrContext, false, new SKImageInfo(
                    (int)Math.Ceiling(_bounds.Width),
                    (int)Math.Ceiling(_bounds.Height), SKImageInfo.PlatformColorType, SKAlphaType.Premul));
                using var filter = SKImageFilter.CreateBlur(3, 3, SKShaderTileMode.Clamp);
                using SKPaint blurPaint = new()
                {
                    Shader = backdropShader,
                    ImageFilter = filter
                };
                blurred.Canvas.DrawRect(0, 0, (float)_bounds.Width, (float)_bounds.Height, blurPaint);
                using var blurSnap = blurred.Snapshot();
                using var blurSnapShader = SKShader.CreateImage(blurSnap);
                using SKPaint blurSnapPaint = new()
                {
                    Shader = blurSnapShader,
                    IsAntialias = true
                };
                skia.SkCanvas.DrawRect(0, 0, (float)_bounds.Width, (float)_bounds.Height, blurSnapPaint);
            }

            public Rect Bounds => _bounds.Inflate(4);
            public bool Equals(ICustomDrawOperation other)
            {
                if (!(other is BlurBehindRenderOperation op))
                    return false;
                
                if (op._bounds != _bounds)
                    return false;
                
                if (!op._blurRadius.Equals(_blurRadius))
                    return false;
                
                return true;
            }
        }
        
        
        public override void Render(DrawingContext context)
        {
            //base.Render(context);

            context.Custom(
                new BlurBehindRenderOperation(
                    BlurRadius
                    , new Rect(default, Bounds.Size)
                )
            );
        }
    }
}