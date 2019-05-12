using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlmVR.Client.TestClient.Modules.DefaultModule.ViewModels
{
    public abstract class TabControlBindableBase : BindableBase
    {
        private string header;

        protected TabControlBindableBase(string header)
        {
            this.header = header;
        }

        public string Header
        {
            get => header;
            set => SetProperty(ref header, value);
        }
    }
}
