using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.VisualTree;
using IDict_TT = System.Collections.Generic.IDictionary<Avalonia.Size, Avalonia.Media.IImage>;

namespace ReactiveDream.Controls
{
    public class WallpaperPresenter
        : TemplatedControl
    {

        /// <summary>
        /// Defines the <see cref="WallpaperResolutions"/> property.
        /// </summary>
        public static readonly StyledProperty<IDict_TT> WallpaperResolutionsProperty =
            AvaloniaProperty.Register<WallpaperPresenter, IDict_TT>(nameof(WallpaperResolutions));

        /// <summary>
        /// Other stuff TBD
        /// </summary>
        public IDict_TT WallpaperResolutions
        {
            get => GetValue(WallpaperResolutionsProperty);
            set => SetValue(WallpaperResolutionsProperty, value);
        }

        static WallpaperPresenter()
        {
            Action<WallpaperPresenter, AvaloniaPropertyChangedEventArgs> onChanged = (sender, e)
                => sender?.RefreshWallpaperImage();
            
            AvaloniaProperty[] props =
            {
                WallpaperResolutionsProperty,
                BoundsProperty,
            };
            
            AffectsRender<WallpaperPresenter>(props);
            foreach (var prop in props)
            {
                prop.Changed.AddClassHandler(onChanged);
            }
        }

        static void WhenWallpaperResolutionsPropertyChanged(WallpaperPresenter sender, AvaloniaPropertyChangedEventArgs e)
        {
            IDict_TT newValue = (e.NewValue != null)
                ? e.GetNewValue<IDict_TT>()
                : null
            ;
            sender?.RefreshWallpaperImage(); //OnWallpaperResolutionsPropertyChanged(newValue);
        }
        
        Image _img = null;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _img = e.NameScope.Find<Image>("PART_Image");
        }

        IImage _renderImage = null;
        protected void RefreshWallpaperImage()
        {
            if (!this.IsAttachedToVisualTree())
                return;
            
            _renderImage = GetImageForSize(Bounds.Size);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            RefreshWallpaperImage();
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
        }

        IImage GetImageForSize(Size size)
        {
            var wallRes = WallpaperResolutions;
            if (wallRes == null)
                return null;
            
            Dictionary<double, IImage> resDiffs = new();
            foreach (var pair in wallRes)
            {
                Size imgSize = pair.Key;
                
                double diffAvg = GetAverage(new Size(
                    Math.Abs(imgSize.Width - size.Width),
                    Math.Abs(imgSize.Height - size.Height)
                ));
                
                if (resDiffs.TryGetValue(diffAvg, out IImage priorImg))
                {
                    Size priorSize = priorImg.Size;
                    if (GetAverage(priorSize) >= GetAverage(imgSize))
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