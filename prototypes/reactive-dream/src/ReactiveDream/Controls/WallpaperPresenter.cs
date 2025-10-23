using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.VisualTree;
using ImageDict = Avalonia.Collections.AvaloniaDictionary<Avalonia.Size, Avalonia.Media.IImage>;
using ImagePair = System.Collections.Generic.KeyValuePair<Avalonia.Size, Avalonia.Media.IImage>;

namespace ReactiveDream.Controls
{
    public enum WallpaperImagePosition
        : int
    {
        Fill = 0,
        Fit = 1,
        Stretch = 2,
        Tile = 3,
        Center = 4,
    }


    public class WallpaperPresenter
        : TemplatedControl
    {
        /// <summary>
        /// Defines the <see cref="ImagePosition"/> property.
        /// </summary>
        public static readonly StyledProperty<WallpaperImagePosition> ImagePositionProperty =
            AvaloniaProperty.Register<WallpaperPresenter, WallpaperImagePosition>(nameof(ImagePosition), WallpaperImagePosition.Fill);

        /// <summary>
        /// Other stuff TBD
        /// </summary>
        public WallpaperImagePosition ImagePosition
        {
            get => GetValue(ImagePositionProperty);
            set => SetValue(ImagePositionProperty, value);
        }




        /// <summary>
        /// Defines the <see cref="ImageResolutions"/> property.
        /// </summary>
        public static readonly StyledProperty<ImageDict> ImageResolutionsProperty =
            AvaloniaProperty.Register<WallpaperPresenter, ImageDict>(nameof(ImageResolutions));

        /// <summary>
        /// Other stuff TBD
        /// </summary>
        public ImageDict ImageResolutions
        {
            get => GetValue(ImageResolutionsProperty);
            set => SetValue(ImageResolutionsProperty, value);
        }




        static WallpaperPresenter()
        {
            AvaloniaProperty[] props =
            {
                ImageResolutionsProperty,
                ImagePositionProperty,
                BoundsProperty,
            };


            AffectsRender<WallpaperPresenter>(props);
            foreach (var prop in props)
            {
                prop.Changed.AddClassHandler<WallpaperPresenter>(WhenAnyAffectsRenderPropertyChanged);
            }
        }


        static void WhenAnyAffectsRenderPropertyChanged(WallpaperPresenter sender, AvaloniaPropertyChangedEventArgs e)
            => sender?.RefreshWallpaperImage();




        IImage _image = null;
        protected void RefreshWallpaperImage()
        {
            if (!this.IsAttachedToVisualTree())
                return;
            
            _image = GetImageForSize(Bounds.Size);
            if (_img == null)
                return;

            _img.Source = _image;
            InvalidateVisual();
        }


        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            RefreshWallpaperImage();
        }


        public override void Render(DrawingContext context)
        {
            base.Render(context);


            var bounds = Bounds;
            if (ImagePosition != WallpaperImagePosition.Tile)
            {
                context.FillRectangle(Background, new(0, 0, bounds.Width, bounds.Height));
                return;
            }


            Size rgnSize = bounds.Size;
            Size imgSize = _image.Size;

            double imgWidth = imgSize.Width;
            double imgHeight = imgSize.Height;
            Rect src = new(0, 0, imgWidth, imgHeight);

            double xTileCount = Math.Round(rgnSize.Width / imgWidth, MidpointRounding.AwayFromZero);
            double yTileCount = Math.Round(rgnSize.Height / imgHeight, MidpointRounding.AwayFromZero);

            for (double y = 0; y < yTileCount; y++)
            {
                for (double x = 0; x < xTileCount; x++)
                {
                    Rect dest = new(imgWidth * x, imgHeight * y, imgWidth, imgHeight);
                    context.DrawImage(_image, src, dest);
                }
            }
        }


        Image _img = null;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _img = e.NameScope.Find<Image>("PART_Image");
            _img.Source = _image;
        }


        IImage GetImageForSize(Size targetSize)
        {
            ImageDict imageResolutions = ImageResolutions;
            if (imageResolutions == null)
                return null;


            IImage ret;
            var imagePosition = ImagePosition;
            if ((imagePosition != WallpaperImagePosition.Center) && (imagePosition != WallpaperImagePosition.Tile))
            {
                ret = GetImageForSizeCore(imageResolutions, targetSize);
                if (ret != null)
                    return ret;
            }

            ret = GetSmallestImage(imageResolutions);

            return ret;
        }


        static IImage GetSmallestImage(ImageDict imageResolutions)
        {
            double smallestSizeW = double.MaxValue;
            double smallestSizeH = double.MaxValue;
            double smallestSizeArea = double.MaxValue;
            IImage smallestImage = null;
            foreach (ImagePair pair in imageResolutions)
            {
                Size size = pair.Key;
                IImage bmp = pair.Value;
                double sizeW = size.Width;
                double sizeH = size.Height;
                if ((sizeW <= smallestSizeW) && (sizeH <= smallestSizeH))
                {
                    double sizeArea = sizeW * sizeH;
                    if (sizeArea < smallestSizeArea)
                    {
                        smallestSizeW = sizeW;
                        smallestSizeH = sizeH;
                        smallestSizeArea = sizeArea;
                        smallestImage = bmp;
                    }
                }
            }
            return smallestImage;
        }


        static IImage GetImageForSizeCore(ImageDict imageResolutions, Size targetSize)
        {
            Dictionary<double, IImage> resDiffs = new();
            foreach (var pair in imageResolutions)
            {
                Size size = pair.Key;
                
                double diffAvg = GetAverage(new Size(
                    Math.Abs(size.Width - targetSize.Width),
                    Math.Abs(size.Height - targetSize.Height)
                ));
                
                if (resDiffs.TryGetValue(diffAvg, out IImage priorImg))
                {
                    Size priorSize = priorImg.Size;
                    if (GetAverage(priorSize) >= GetAverage(size))
                        continue;
                }
                
                resDiffs[diffAvg] = pair.Value;
            }

            var keys = resDiffs.Keys;
            if (keys.Count <= 0)
                return null;
            
            return resDiffs[keys.Min()];
        }

        static double GetAverage(Size size)
            => (size.Width + size.Height) / 2.0;
    }
}