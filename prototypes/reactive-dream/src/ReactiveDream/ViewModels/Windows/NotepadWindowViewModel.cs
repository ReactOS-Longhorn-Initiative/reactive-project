using System;
using Avalonia;

namespace ReactiveDream.ViewModels
{
    public class NotepadWindowViewModel
        : WindowViewModelBase
    {
        string _documentText = string.Empty;
        public string DocumentText
        {
            get => _documentText;
            set => RASIC(ref _documentText, value);
        }


        
        bool _showStatusBar = true;
        public bool ShowStatusBar
        {
            get => _showStatusBar;
            set => RASIC(ref _showStatusBar, value);
        }


        public NotepadWindowViewModel()
            : base()
        {
            Title = "Untitled - Notepad";
        }


        protected override Rect CreateDefaultBounds()
            => new(80, 80, 592, 397);
    }
}