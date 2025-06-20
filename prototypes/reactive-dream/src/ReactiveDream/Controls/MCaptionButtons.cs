using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;

namespace ReactiveDream.Controls
{
    public class MCaptionButtons
        : CaptionButtons
    {
        MWindow _hostWindow = null;
        public virtual void Attach(MWindowBase hostWindow)
        {
            _hostWindow = (hostWindow is MWindow hostMWindow)
                ? hostMWindow
                : null
            ;
        }




        protected override void OnClose()
            => _hostWindow?.CloseWindow();


        protected override void OnRestore()
        {
            if (_hostWindow != null)
                _hostWindow.IsMaximized = !_hostWindow.IsMaximized;

            TryExecuteCommand(_hostWindow?.MaximizeCommand);
        }

        protected override void OnMinimize()
            => TryExecuteCommand(_hostWindow?.MinimizeCommand);


        protected static bool TryExecuteCommand(
#nullable enable
            ICommand? command
#nullable restore
            , object parameter = null)
        {
            if (command == null)
                return false;

            if (!command.CanExecute(parameter))
                return false;

            command.Execute(parameter);
            return true;
        }




        public override void Attach(Window hostWindow)
        {}

        public override void Detach()
        {}

        protected override void OnToggleFullScreen()
        {}
    }
}