using System;
using Avalonia;

namespace ReactiveDream.ViewModels
{
    public class RunWindowViewModel
        : WindowViewModelBase
    {
        public RunWindowViewModel()
            : base()
        {
            Title = "Run";
            CanResize = false;
        }


        protected override Rect CreateDefaultBounds()
            => new(16, -186, 341, 154);
    }
}