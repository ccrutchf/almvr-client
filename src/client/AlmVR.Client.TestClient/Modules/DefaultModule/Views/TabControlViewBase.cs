using AlmVR.Client.TestClient.Modules.DefaultModule.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AlmVR.Client.TestClient.Modules.DefaultModule.Views
{
    public abstract class TabControlViewBase : UserControl
    {
        private readonly TabControlBindableBase viewModel;

        protected TabControlViewBase(TabControlBindableBase viewModel)
        {
            this.viewModel = viewModel;
            this.DataContext = viewModel;
        }

        public string Header => viewModel.Header;
    }
}
