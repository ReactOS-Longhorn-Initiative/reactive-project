using System;
using System.Collections.Generic;
using System.ComponentModel;

using Size = Avalonia.Size;
using IImage = Avalonia.Media.IImage;
using IImageDict = System.Collections.Generic.IDictionary<Avalonia.Size, Avalonia.Media.IImage>;

namespace ReactiveDream.ViewModels
{
    public class WallpaperViewModel
        : ViewModelBase
    {
        string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => RASIC(ref _title, value);
        }


        IImage _previewImage = null;
        public IImage PreviewImage
        {
            get => _previewImage;
            protected set => RASIC(ref _previewImage, value);
        }


        IImageDict _images = new Dictionary<Size, IImage>();
        public IImageDict Images
        {
            get => _images;
            set => RASIC(ref _images, value);
        }


        public WallpaperViewModel(string title)
            : base()
        {
            Title = title;
        }
        public WallpaperViewModel(string title, params IImage[] images)
            : this(title, (IEnumerable<IImage>)images)
        {}
        public WallpaperViewModel(string title, IEnumerable<IImage> images)
            : this(title)
        {
            Dictionary<Size, IImage> imageDict = new();
            foreach (var image in images)
            {
                imageDict[image.Size] = image;
            }
            Images = imageDict;
            UpdatePreviewImage(imageDict);
        }


        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(Images))
            {
                var images = Images;
                if (images?.Count <= 0)
                    return;


                var prevPreviewImage = PreviewImage;
                if (prevPreviewImage == null)
                    return;
                else if (!images.Values.Contains(prevPreviewImage))
                    UpdatePreviewImage(images);
            }
        }


        void UpdatePreviewImage(IImageDict images)
        {
            if (images?.Count <= 0)
                return;

            IImage smallest = images.GetSmallestImage();
            PreviewImage = ResizeForPreview(smallest);
        }


        const double _PREVIEW_WIDTH = 160d;
        const double _PREVIEW_HEIGHT = 120d;
        static IImage ResizeForPreview(IImage image)
        {
            Size size = image.Size;
            double sizeW = size.Width;
            double sizeH = size.Height;
            double sizeFac;
            if (sizeH > sizeW)
                sizeFac = sizeH / _PREVIEW_HEIGHT;
            else
                sizeFac = sizeW / _PREVIEW_WIDTH;
            
            size *= sizeFac;
            return image.Resize(size);
        }
    }
}