using System;

namespace ReactiveDream.ViewModels
{
    public abstract class SubViewModelBase
        : ViewModelBase
    {
        bool _hasMainVM = false;
        WeakReference<MainViewModel> _mainVM;
        protected bool TryGetMainVM(out MainViewModel mainVM)
        {
            if (_hasMainVM && _mainVM.TryGetTarget(out mainVM))
                return true;

            mainVM = default;
            return false;
        }

        protected MainViewModel MainVM
        {
            get
            {
                if (!_hasMainVM)
                    return null;
                else if (_mainVM.TryGetTarget(out MainViewModel mainVM))
                    return mainVM;
                else
                    return null;
            }
        }


        public void ProvideMainVM(MainViewModel vm)
        {
            _mainVM ??= new(vm);
            _hasMainVM = (_mainVM != null) && _mainVM.TryGetTarget(out MainViewModel t) && (t != null);
            OnReceivedMainVM(vm);
        }


        protected virtual void OnReceivedMainVM(MainViewModel vm)
        {}
    }
}
