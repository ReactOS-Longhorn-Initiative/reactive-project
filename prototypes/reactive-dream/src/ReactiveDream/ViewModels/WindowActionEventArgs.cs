using System;

namespace ReactiveDream.ViewModels
{
    public sealed class WindowActionEventArgs
        : EventArgs
    {
        public readonly WindowViewModelBase WindowVM;
        public WindowActionEventArgs(WindowViewModelBase vm)
            : base()
        {
            WindowVM = vm;
        }
    }
}