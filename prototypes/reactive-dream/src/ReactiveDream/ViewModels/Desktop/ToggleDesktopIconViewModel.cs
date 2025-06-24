using System;

using BGetter = System.Func<bool>;
using BSetter = System.Action<bool>;

namespace ReactiveDream.ViewModels
{
    public class ToggleDesktopIconViewModel
        : DesktopIconViewModel
    {
        string _trueName = string.Empty;
        public string TrueName
        {
            get => _trueName;
            set => RASIC(ref _trueName, value);
        }


        string _falseName = string.Empty;
        public string FalseName
        {
            get => _falseName;
            set => RASIC(ref _falseName, value);
        }


        BGetter _getter = null;
        public BGetter Getter
        {
            get => _getter;
            set => RASIC(ref _getter, value);
        }


        BSetter _setter = null;
        public BSetter Setter
        {
            get => _setter;
            set => RASIC(ref _setter, value);
        }




        protected override bool ExecuteCommandCore()
        {
            var getter = Getter;
            var setter = Setter;

            if ((getter == null) || (setter == null))
                return base.ExecuteCommandCore();


            bool oldValue = getter();

            if (base.ExecuteCommandCore())
                return true;


            setter(!oldValue);
            return true;
        }


        public ToggleDesktopIconViewModel(string trueName, string falseName, BGetter getter, BSetter setter)
            : base(falseName)
        {
            TrueName = trueName;
            FalseName = falseName;
            Getter = getter;
            Setter = setter;

            Name = GetInitialName(trueName, falseName, getter);
        }


        public void UpdateState()
        {
            var getter = Getter;
            if (getter == null)
                Name = FalseName;
            else
                UpdateName();
        }


        void UpdateName()
            => UpdateName(Getter());
        void UpdateName(bool state)
            => Name = state
                ? TrueName
                : FalseName
            ;


        static string GetInitialName(string trueName, string falseName, BGetter getter)
        {
            bool state;
            if (getter == null)
                throw new ArgumentNullException(nameof(getter));

            try
            {
                state = getter();
            }
            catch (Exception)
            {
                state = false;
            }

            return state
                ? trueName
                : falseName
            ;
        }
    }
}