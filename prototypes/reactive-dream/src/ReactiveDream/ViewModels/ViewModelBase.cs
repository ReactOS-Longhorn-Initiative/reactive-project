using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace ReactiveDream.ViewModels
{
    public abstract class ViewModelBase
        : ReactiveObject
    {
        protected virtual Type GetViewType()
            => Type.GetType(GetType().FullName.Replace("ViewModel", "View"));


        Type _viewType = null;
        public Type ViewType
        {
            get
            {
                if (_viewType == null)
                    _viewType = GetViewType();
                return _viewType;
            }
        }


        public T RASIC<T>(ref T backingField, T newValue, [CallerMemberName]string propertyName = null) 
            => this.RaiseAndSetIfChanged(ref backingField, newValue, propertyName);




        public ViewModelBase()
        {
            PropertyChanged += This_PropertyChanged;
        }


        void This_PropertyChanged(object sender, PropertyChangedEventArgs e)
            => OnPropertyChanged(e);
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {}
    }
}
