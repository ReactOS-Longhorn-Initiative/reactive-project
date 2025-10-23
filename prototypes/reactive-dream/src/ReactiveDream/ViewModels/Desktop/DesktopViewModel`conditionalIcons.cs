using System;
using System.Collections.Generic;
using System.ComponentModel;
using ConditionFunc = System.Func<bool>;

namespace ReactiveDream.ViewModels
{
    public partial class DesktopViewModel
        : SubViewModelBase
    {
        const string _DWM_CONTROL = "mock DWM";
        static readonly string _DWM_ENABLE = $"Enable {_DWM_CONTROL}";
        static readonly string _DWM_DISABLE = $"Disable {_DWM_CONTROL}";


        const string _FULLSCREEN_CONTROL = "Fullscreen";
        static readonly string _FULLSCREEN_ENABLE = $"Enter {_FULLSCREEN_CONTROL}";
        static readonly string _FULLSCREEN_DISABLE = $"Exit {_FULLSCREEN_CONTROL}";


        readonly List<ConditionalIconInfo> _conditionalDesktopIcons = new();


        DesktopIconViewModel _exitIcon = null;
        ConditionalIconInfo _exitIconInfo = null;


        ToggleDesktopIconViewModel _toggleDwmIcon = null;


        ToggleDesktopIconViewModel _toggleFullscreenIcon = null;
        ConditionalIconInfo _toggleFullscreenIconInfo = null;


        int _conditionalDesktopIconInsertionStartIndex = 0;
        void CreateConditionalIcons(MainViewModel vm)
        {
            _toggleDwmIcon = new(_DWM_DISABLE, _DWM_ENABLE, () => vm.IsCompositionActive, v => vm.IsCompositionActive = v);
            _desktopIcons.Insert(0, _toggleDwmIcon);
        
            
            _exitIcon = new("Exit")
            {
                Command = () => System.Diagnostics.Process.GetCurrentProcess().Kill(),
            };
            _exitIconInfo = new(_exitIcon, () => RuntimeHelper.IsDesktop);
            _conditionalDesktopIcons.Add(_exitIconInfo);


            _toggleFullscreenIcon = new(_FULLSCREEN_DISABLE, _FULLSCREEN_ENABLE, () => vm.IsFullScreen, v => vm.IsFullScreen = v);
            _toggleFullscreenIconInfo = new(_toggleFullscreenIcon, () => _isFullScreenCapable);
            _conditionalDesktopIcons.Add(_toggleFullscreenIconInfo);


            foreach (var iconInfo in _conditionalDesktopIcons)
            {
                UpdateIconPresence(iconInfo);
            }
        }



        void UpdateIcons(MainViewModel vm, PropertyChangedEventArgs e)
        {
            string propName = e.PropertyName;
            
            if (propName == nameof(MainViewModel.IsCompositionActive))
            {
                _toggleDwmIcon.UpdateState();
            }
            else if (propName == nameof(MainViewModel.IsFullScreen))
            {
                if (UpdateIconPresence(_toggleFullscreenIconInfo))
                    _toggleFullscreenIcon.UpdateState();
            }
        }


        bool UpdateIconPresence(ConditionalIconInfo iconInfo)
        {
            DesktopIconViewModel icon = iconInfo.IconVM;
            bool wasPresent = _desktopIcons.Contains(icon);
            bool shouldBePresent = iconInfo.Condition;
            if (wasPresent == shouldBePresent)
                return wasPresent;


            if (shouldBePresent)
            {
                int insertionIndex = _conditionalDesktopIconInsertionStartIndex + _conditionalDesktopIcons.IndexOf(iconInfo);
                DesktopIcons.Insert(insertionIndex, icon);
                return true;
            }
            else
            {
                DesktopIcons.Remove(icon);
                return false;
            }
        }


        void UpdateIconName(DesktopIconViewModel vm, bool value, string falseText, string trueText)
        {
            if (vm == null)
                return;

            vm.Name = value
                ? trueText
                : falseText
            ;
        }




        sealed class ConditionalIconInfo
        {
            public readonly DesktopIconViewModel IconVM;
            public readonly ConditionFunc ConditionFunc;
            public bool Condition
            {
                get => ConditionFunc();
            }
            public ConditionalIconInfo(DesktopIconViewModel iconVM, ConditionFunc conditionFunc)
            : base()
            {
                IconVM = iconVM;
                ConditionFunc = conditionFunc;
            }
        }
    }
}