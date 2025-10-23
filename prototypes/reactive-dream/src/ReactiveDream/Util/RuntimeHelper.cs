using System;

namespace ReactiveDream
{
    public enum RuntimePlatformType
    {
        Desktop,
        Mobile,
        Browser,
    }


    public static class RuntimeHelper
    {
        static RuntimePlatformType _platformType = default;
        public static RuntimePlatformType PlatformType
        {
            get => _platformType;
            internal set
            {
                if (_platformType != default)
                    return;

                _platformType = value;
            }
        }


        public static bool IsDesktop
        {
            get => _platformType == RuntimePlatformType.Desktop;
        }


        public static bool IsMobile
        {
            get => _platformType == RuntimePlatformType.Mobile;
        }


        public static bool IsBrowser
        {
            get => _platformType == RuntimePlatformType.Browser;
        }
    }
}