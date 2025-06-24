using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace ReactiveDream
{
    public static class WallpaperHelper
    {
        public static IImage Resize(this IImage image, Size size, Vector? dpi = null)
#if WallpaperHelper_Resize_WIP
        {
            Vector rtDpi = dpi.ValueOr(_DEFAULT_DPI);
            //PixelSize pxSrcSize = PixelSize.FromSizeWithDpi(image.Size, rtDpi);
            PixelSize pxDestSize = PixelSize.FromSizeWithDpi(size, rtDpi);


            RenderTargetBitmap rtBmp = new(pxDestSize);
            using (var ctx = rtBmp.CreateDrawingContext())
            {
                Rect srcRect = new(_POINT_ZERO, image.Size);
                Rect destRect = new(_POINT_ZERO, size);
                ctx.DrawImage(image, srcRect);
            }
            return rtBmp;
        }


        static readonly Point _POINT_ZERO = new(0, 0);
        const double _DEFAULT_DPI_AXIS = 96d;
        static readonly Vector _DEFAULT_DPI = new(_DEFAULT_DPI_AXIS, _DEFAULT_DPI_AXIS); //new RenderTargetBitmap(new(1, 1)).Dpi;
#else
        {
            return image;
        }
#endif




        public static IImage ImageFromStream(Stream imageStream)
        {
            using MemoryStream memStream = new();
            var imageStreamPos = imageStream.Position;
            imageStream.Position = 0;
            imageStream.CopyTo(memStream);
            imageStream.Position = imageStreamPos;

            memStream.Position = 0;
            return new Bitmap(memStream);
        }




        public static IImage GetSmallestImage(this IDictionary<Size, IImage> imageResolutions)
        {
            GetInitParamsForSmallestImage(
                out double smallestSizeW,       out double smallestSizeH,
                out double smallestSizeArea,    out IImage smallestImage
            );

            foreach (var pair in imageResolutions)
            {
                CompareSizeForSmallestImage(pair.Value, pair.Key,
                    ref smallestSizeW,       ref smallestSizeH,
                    ref smallestSizeArea,    ref smallestImage
                );
            }

            return smallestImage;
        }

        public static IImage GetSmallestImage(this IReadOnlyDictionary<Size, IImage> imageResolutions)
        {
            GetInitParamsForSmallestImage(
                out double smallestSizeW,       out double smallestSizeH,
                out double smallestSizeArea,    out IImage smallestImage
            );

            foreach (var pair in imageResolutions)
            {
                CompareSizeForSmallestImage(pair.Value, pair.Key,
                    ref smallestSizeW,       ref smallestSizeH,
                    ref smallestSizeArea,    ref smallestImage
                );
            }

            return smallestImage;
        }

        public static IImage GetSmallestImage(this IEnumerable<IImage> images)
        {
            GetInitParamsForSmallestImage(
                out double smallestSizeW,       out double smallestSizeH,
                out double smallestSizeArea,    out IImage smallestImage
            );

            foreach (IImage image in images)
            {
                CompareSizeForSmallestImage(image, image.Size,
                    ref smallestSizeW,       ref smallestSizeH,
                    ref smallestSizeArea,    ref smallestImage
                );
            }

            return smallestImage;
        }

        static void GetInitParamsForSmallestImage(
            out double smallestSizeW,       out double smallestSizeH,
            out double smallestSizeArea,    out IImage smallestImage
        )
        {
            smallestSizeW = double.MaxValue;
            smallestSizeH = double.MaxValue;
            smallestSizeArea = double.MaxValue;
            smallestImage = null;
        }

        static void CompareSizeForSmallestImage(IImage image, Size size,
            ref double smallestSizeW,       ref double smallestSizeH,
            ref double smallestSizeArea,    ref IImage smallestImage
        )
        {
            double sizeW = size.Width;
            if (sizeW > smallestSizeW)
                return;

            double sizeH = size.Height;
            if (sizeH > smallestSizeH)
                return;

            double sizeArea = sizeW * sizeH;
            if (sizeArea < smallestSizeArea)
            {
                smallestSizeW = sizeW;
                smallestSizeH = sizeH;
                smallestSizeArea = sizeArea;
                smallestImage = image;
            }
        }
    }
}