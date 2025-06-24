using System;
using ReactiveDream.ViewModels;

namespace ReactiveDream
{
    public static class ViewModelHelper
    {
        public static TWindowVM WithMainVM<TWindowVM>(this TWindowVM self, MainViewModel vm)
            where TWindowVM : SubViewModelBase
        {
            self.ProvideMainVM(vm);
            return (TWindowVM)(object)self;
        }
    }
}