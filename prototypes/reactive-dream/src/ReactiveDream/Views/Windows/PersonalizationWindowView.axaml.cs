using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ReactiveDream.Styles;
using ReactiveDream.ViewModels;

namespace ReactiveDream.Views
{
    public partial class PersonalizationWindowView
        : UserControl
    {
        public PersonalizationWindowView()
        {
            InitializeComponent();
        }


        ReactiveTheme _theme = null;
        ContentPresenter _appearancePreview = null;
        ComboBox _variantComboBox = null;
        ComboBoxItem _reactiveDWMItem = null;
        void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            _appearancePreview = this.Find<ContentPresenter>("AppearancePreview");

            _theme = _appearancePreview.Styles.OfType<ReactiveTheme>().First();
            _reactiveDWMItem = this.Find<ComboBoxItem>("ReactiveDWMItem");


            _variantComboBox = this.Find<ComboBox>("VariantComboBox");
            _variantComboBox.SelectionChanged += VariantComboBox_SelectionChanged;

            DispatcherPriority priority = DispatcherPriority.ApplicationIdle;

            Dispatcher.UIThread.Post(() =>
            {
                _variantComboBox.IsDropDownOpen = true;
                Dispatcher.UIThread.Post(() => 
                    _variantComboBox.IsDropDownOpen = false
                , priority);
            }, priority);
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            _variantComboBox.SelectionChanged -= VariantComboBox_SelectionChanged;
        }


        void VariantComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => Dispatcher.UIThread.Post(UpdateVSPreview, DispatcherPriority.ApplicationIdle);

        void UpdateVSPreview()
        {
            if (!(DataContext is PersonalizationWindowViewModel vm))
                return;

            var content = _appearancePreview.Content;
            var contentTemplate = _appearancePreview.ContentTemplate;
            _appearancePreview.ContentTemplate = null;
            _appearancePreview.Content = null;
            _theme.IsCompositionActive = vm.IsCompositionActive; //e.AddedItems?.Contains(_reactiveDWMItem) ?? false;
            Dispatcher.UIThread.Post(() =>
            {
                _appearancePreview.Content = content;
                _appearancePreview.ContentTemplate = contentTemplate;
            }, DispatcherPriority.ApplicationIdle);
        }
    }
}