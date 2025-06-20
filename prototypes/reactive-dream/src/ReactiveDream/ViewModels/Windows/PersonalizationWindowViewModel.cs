using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia;

using IImage = Avalonia.Media.IImage;
using WallpaperCollection = System.Collections.ObjectModel.ObservableCollection<ReactiveDream.ViewModels.WallpaperViewModel>;
using WallpaperImagePosition = ReactiveDream.Controls.WallpaperImagePosition;

namespace ReactiveDream.ViewModels
{
    public class PersonalizationWindowViewModel
        : WindowViewModelBase
    {
        const string _WALLPAPER_PREFIX = "wallpapers/";
        static readonly Assembly _WALLPAPERS_ASSEMBLY = typeof(WallpaperHelper).Assembly;


        static IReadOnlyCollection<WallpaperViewModel> GetDefaultWallpapers()
        {
            List<string> wallpaperNames = new();
            string[] resNames = _WALLPAPERS_ASSEMBLY.GetManifestResourceNames();
            foreach (string resName in resNames)
            {

                if (!resName.Contains('/'))
                    continue;
                if (!resName.StartsWith(_WALLPAPER_PREFIX))
                    continue;

                string subPath = resName
                    .Substring(0, resName.LastIndexOf('/'))
                    .Substring(_WALLPAPER_PREFIX.Length)
                ;

                if (wallpaperNames.Contains(subPath))
                    continue;

                wallpaperNames.Add(subPath);
            }


            List<WallpaperViewModel> wallpapers = new();
            foreach (string wallpaperName in wallpaperNames)
            {
                string imagePrefix = $"{_WALLPAPER_PREFIX}{wallpaperName}";

                List<string> imageNames = resNames.Where(x => x.StartsWith(imagePrefix)).ToList();

                List<IImage> images = new();
                foreach (string imageName in imageNames)
                {
                    using Stream imageStream = _WALLPAPERS_ASSEMBLY.GetManifestResourceStream(imageName);
                    var image = WallpaperHelper.ImageFromStream(imageStream);
                    images.Add(image);
                }
                
                WallpaperViewModel wallpaperVM = new(wallpaperName, images);
                wallpapers.Add(wallpaperVM);
            }

            return wallpapers.AsReadOnly();
        }

        public static readonly IReadOnlyCollection<WallpaperViewModel> WALLPAPERS = GetDefaultWallpapers();




        WallpaperCollection _wallpapers = new(WALLPAPERS);
        public WallpaperCollection Wallpapers
        {
            get => _wallpapers;
            protected set => RASIC(ref _wallpapers, value);
        }




        WallpaperViewModel _selectedWallpaper = null;
        public WallpaperViewModel SelectedWallpaper
        {
            get => _selectedWallpaper;
            protected set => RASIC(ref _selectedWallpaper, value);
        }




        WallpaperImagePosition _wallpaperPosition = default;
        public WallpaperImagePosition WallpaperPosition
        {
            get => _wallpaperPosition;
            protected set => RASIC(ref _wallpaperPosition, value);
        }




        bool _isCompositionActive = true;
        public bool IsCompositionActive
        {
            get => _isCompositionActive;
            set => RASIC(ref _isCompositionActive, value);
        }




        int _selectedTabIndex = 1;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => RASIC(ref _selectedTabIndex, value);
        }




        protected override Rect CreateDefaultBounds()
            => new(40, 40, 640, 480);




        protected override void OnReceivedMainVM()
        {
            base.OnReceivedMainVM();
            var mainVM = MainVM;
            SelectedWallpaper = mainVM.Wallpaper;
            WallpaperPosition = mainVM.WallpaperPosition;
            IsCompositionActive = mainVM.IsCompositionActive;
        }


        public void ApplyCommand(object parameter)
        {
            Apply();


            bool exit;
            if (parameter is bool paramBool)
                exit = paramBool;
            else if (parameter == null)
                return;
            else if (!bool.TryParse(parameter.ToString(), out exit))
                return;


            if (exit)
                Close();
        }


        public void Apply()
        {
            var mainVM = MainVM;

            var selectedWallpaper = SelectedWallpaper;
            if (selectedWallpaper != null)
                mainVM.Wallpaper = selectedWallpaper;

            mainVM.WallpaperPosition = WallpaperPosition;
            mainVM.IsCompositionActive = IsCompositionActive;
        }
    }
}