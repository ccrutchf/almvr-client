using AlmVR.Client.TestClient.Extensions;
using AlmVR.Client.TestClient.Modules.DefaultModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlmVR.Client.TestClient.Modules.DefaultModule
{
    class DefaultModule : IModule
    {
        private readonly IRegionManager regionManager;

        public DefaultModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            regionManager.RegisterViewWithRegion<BoardTestClientView>("TabsRegion");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
