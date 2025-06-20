using System;
using System.Globalization;
using Avalonia.Data.Converters;
using ReactiveDream.ViewModels;
using InImageDict = System.Collections.Generic.Dictionary<Avalonia.Size, Avalonia.Media.IImage>;
using OutImageDict = Avalonia.Collections.AvaloniaDictionary<Avalonia.Size, Avalonia.Media.IImage>;

namespace ReactiveDream.Controls
{
    public class WallpaperViewModelToImageResolutionsConverter
        : IValueConverter
    {
        public static WallpaperViewModelToImageResolutionsConverter Instance { get; } = new();
        private WallpaperViewModelToImageResolutionsConverter()
        {}


        bool TryConvert(object value, out OutImageDict imageDictionary)
        {
            if (!(value is WallpaperViewModel wallpaperVM))
                goto fail;

            var images = wallpaperVM.Images;


            InImageDict inImageDict;
            if (images is OutImageDict outImageDict)
            {
                imageDictionary = outImageDict;
                return imageDictionary != null;
            }
            else if (images is InImageDict inImageDictionary)
                inImageDict = inImageDictionary;
            else
                goto fail;


            imageDictionary = new();
            var pairs = inImageDict;
            foreach (var pair in pairs)
            {
                imageDictionary[pair.Key] = pair.Value;
            }
            return true;


            fail:
            imageDictionary = null;
            return false;
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => TryConvert(value, out OutImageDict imageDictionary)
                ? imageDictionary
                : null
            ;


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}