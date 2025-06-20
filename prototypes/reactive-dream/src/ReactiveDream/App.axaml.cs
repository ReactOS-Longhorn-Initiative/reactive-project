using System;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using ReactiveDream.Styles;
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


        ReactiveTheme _theme = null;
        public override void OnFrameworkInitializationCompleted()
        {
            _theme = Styles.OfType<ReactiveTheme>().First();
            _theme.IsCompositionActive = true;

            
            MainVM = new MainViewModel();
            MainVM.PropertyChanged += MainVM_PropertyChanged;


            _theme.IsCompositionActive = MainVM.IsCompositionActive;
            MainView = new()
            {
                DataContext = MainVM,
            };


            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                MainVM.SetFullScreenCapability(true);
                desktop.MainWindow = new MainWindow()
                {
                    Content = MainView,
                    [!DataContextProperty] = MainView[!DataContextProperty]
                };
            }
			else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewLifetime)
			{
                MainVM.SetFullScreenCapability(false);
				singleViewLifetime.MainView = MainView;
			}

            base.OnFrameworkInitializationCompleted();
        }

        void MainVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(MainViewModel.IsCompositionActive))
                return;

            MainVM.RefreshHackPrepare();
            _theme.IsCompositionActive = MainVM.IsCompositionActive;
            MainVM.RefreshHackConclude();
        }
    }
}