using System;

namespace ReactiveDream.ViewModels
{
    public class DesktopIconViewModel
        : ViewModelBase
    {
        string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => RASIC(ref _name, value);
        }


        Action _command = null;
        public Action Command
        {
            get => _command;
            set => RASIC(ref _command, value);
        }


        public DesktopIconViewModel()
            : base()
        {}
        public DesktopIconViewModel(string name)
            : this()
        {
            Name = name;
        }


        public void ExecuteCommand()
            => ExecuteCommandCore();
        protected virtual bool ExecuteCommandCore()
        {
            var command = Command;
            if (command == null)
                return false;

            command();
            return true;
        }
    }
}