// https://gist.github.com/kekekeks/ac06098a74fe87d49a9ff9ea37fa67bc
using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace ReactiveDream.Controls
{
    public class BlurBehindRenderOperation
        : ICustomDrawOperation
    {
        public double BlurRadius = 0;
        public Rect BlurBounds = new(0, 0, 1, 1);


        public BlurBehindRenderOperation()
            : base()
        {}
        public BlurBehindRenderOperation(double blurRadius)
            : this()
        {
            BlurRadius = blurRadius;
        }
        public BlurBehindRenderOperation(double blurRadius, Rect bounds)
            : this(blurRadius)
        {
            BlurBounds = bounds;
        }
        
        public void Dispose()
        {
            
        }

        public bool HitTest(Point p) => BlurBounds.Contains(p);

        
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
                (int)Math.Ceiling(BlurBounds.Width),
                (int)Math.Ceiling(BlurBounds.Height), SKImageInfo.PlatformColorType, SKAlphaType.Premul));
            using var filter = SKImageFilter.CreateBlur(3, 3, SKShaderTileMode.Clamp);
            using SKPaint blurPaint = new()
            {
                Shader = backdropShader,
                ImageFilter = filter
            };
            blurred.Canvas.DrawRect(0, 0, (float)BlurBounds.Width, (float)BlurBounds.Height, blurPaint);
            using var blurSnap = blurred.Snapshot();
            using var blurSnapShader = SKShader.CreateImage(blurSnap);
            using SKPaint blurSnapPaint = new()
            {
                Shader = blurSnapShader,
                IsAntialias = true
            };
            skia.SkCanvas.DrawRect(0, 0, (float)BlurBounds.Width, (float)BlurBounds.Height, blurSnapPaint);
        }

        public Rect Bounds => BlurBounds.Inflate(4);
        public bool Equals(ICustomDrawOperation other)
        {
            if (!(other is BlurBehindRenderOperation op))
                return false;
            
            if (op.BlurBounds != BlurBounds)
                return false;
            
            if (!op.BlurRadius.Equals(BlurRadius))
                return false;
            
            return true;
        }
    }
}