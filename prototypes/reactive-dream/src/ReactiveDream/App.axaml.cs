using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using ReactiveDream.ViewModels;
using ReactiveDream.Views;

namespace ReactiveDream
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        MainView _mainView = null;
        public MainView MainView
        {
            get => _mainView;
            protected set => _mainView = value;
        }

        MainViewModel _mainVM = null;
        public MainViewModel MainVM
        {
            get => _mainVM;
            protected set => _mainVM = value;
        }

        const string _DWM_INCLUDE_KEY = "DWMStyles";
        IStyle _dwmInclude = null;
        public override void OnFrameworkInitializationCompleted()
        {
            string sourceThemesDir = string.Empty;

            MainVM = new MainViewModel();

            
            /*ResourceInclude dwmInclude = 
                /*new ResourceInclude()
                {
                    Source = new Uri("avares://ReactiveDream/Styles/DWMStyles.axaml", UriKind.RelativeOrAbsolute)
                }* /
                (IStyle)Resources[_DWM_INCLUDE_KEY]!
            ;*/
            if (Resources.TryGetValue(_DWM_INCLUDE_KEY, out object dwmIncludeO) && (dwmIncludeO is IStyle dwmInclude))
            {
                _dwmInclude = dwmInclude;
                Resources.Remove(_DWM_INCLUDE_KEY);
                
                MainVM.PropertyChanged += (s, e) =>
                {
                    Debug.WriteLine("MainVM.PropertyChanged");

                    if (e.PropertyName != nameof(MainViewModel.IsCompositionActive))
                        return;

                    MainVM.RefreshHackPrepare();
                    DwmEnableComposition(MainVM.IsCompositionActive);
                    MainVM.RefreshHackConclude();
                };
            }
            DwmEnableComposition(MainVM.IsCompositionActive);
            MainView = new()
            {
                DataContext = MainVM,
            };
            
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    Content = MainView,
                    [!DataContextProperty] = MainView[!DataContextProperty]
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        void DwmEnableComposition(bool enable)
        {
            /*if (_dwmInclude == null)
                return;*/
            //MainView?.UprootHack();

            bool areDwmStylesMerged = Styles.Contains(_dwmInclude); //Resources.MergedDictionaries.Contains(_dwmInclude);
            if (enable == areDwmStylesMerged)
                return;
            
            
            var prevStyles = Styles.Where(x => x != _dwmInclude).ToList();
            int styleCount = prevStyles.Count;
            //foreach (var style in styles)
            /*for (int i = 0; i < styleCount; i++)
            {
                Styles.RemoveAt(0);
            }*/

            if (enable)
                Styles.Add(_dwmInclude); //Resources.MergedDictionaries.Add(_dwmInclude);
            else
                Styles.Remove(_dwmInclude); //Resources.MergedDictionaries.Remove(_dwmInclude);
            
            
            /*for (int i = 0; i < styleCount; i++)
            {
                Styles.Insert(i, prevStyles[i]);
            }*/
            //Styles.Owner
            //MainView.ReinsertHack();
        }
    }
}