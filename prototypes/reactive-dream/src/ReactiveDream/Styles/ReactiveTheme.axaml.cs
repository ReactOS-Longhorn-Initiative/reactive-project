using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;

namespace ReactiveDream.Styles
{
    public partial class ReactiveTheme
        : Avalonia.Styling.Styles
    {
        /// <summary>
        /// Defines the <see cref="IsCompositionActive"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsCompositionActiveProperty =
            AvaloniaProperty.Register<ReactiveTheme, bool>(nameof(IsCompositionActive), true);


        /// <summary>
        /// Other stuff TBD
        /// </summary>
        public bool IsCompositionActive
        {
            get => GetValue(IsCompositionActiveProperty);
            set => SetValue(IsCompositionActiveProperty, value);
        }


        static ReactiveTheme()
        {
            IsCompositionActiveProperty.Changed.AddClassHandler<ReactiveTheme>(IsCompositionActiveProperty_Changed);
        }


        static void IsCompositionActiveProperty_Changed(ReactiveTheme theme, AvaloniaPropertyChangedEventArgs args)
            => theme.OnIsCompositionActiveChanged(args);




        IResourceProvider _dwmInclude = null;
        public ReactiveTheme()
            : base()
        {
            AvaloniaXamlLoader.Load(this);
            ResourceInclude dwmInclude = new(baseUri: null)
            {
                Source = new("avares://ReactiveDream/Styles/DWMResources.axaml")
            };


            _dwmInclude = dwmInclude;
            DwmEnableComposition(IsCompositionActive);
        }


        void OnIsCompositionActiveChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (_dwmInclude == null)
                return;

            bool newValue = e.GetNewValue<bool>();
            DwmEnableComposition(newValue);
        }


        void DwmEnableComposition(bool enable)
        {
            if (enable == Resources.MergedDictionaries.Contains(_dwmInclude))
                return;
            
            
            if (enable)
                Resources.MergedDictionaries.Add(_dwmInclude);
            else
                Resources.MergedDictionaries.Remove(_dwmInclude);

            Owner?.NotifyHostedResourcesChanged(ResourcesChangedEventArgs.Empty);
        }
    }
}