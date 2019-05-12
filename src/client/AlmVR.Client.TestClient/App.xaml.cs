using AlmVR.Client.Core;
using AlmVR.Client.TestClient.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AlmVR.Client.TestClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            var moduleTypes = from t in typeof(App).Assembly.GetTypes()
                              where t.GetInterfaces().Contains(typeof(IModule))
                              select t;

            foreach (var moduleType in moduleTypes)
            {
                moduleCatalog.AddModule(moduleType);
            }

            base.ConfigureModuleCatalog(moduleCatalog);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override async void RegisterTypes(IContainerRegistry containerRegistry)
        {
            await RegisterClientsAsync(containerRegistry);
        }

        private async Task RegisterClientsAsync(IContainerRegistry containerRegistry)
        {
            var hostname = "localhost";
            var port = 50405;

            var boardClient = ClientFactory.GetInstance<IBoardClient>();
            var cardClient = ClientFactory.GetInstance<ICardClient>();

            containerRegistry.RegisterInstance(boardClient);
            containerRegistry.RegisterInstance(cardClient);

            await Task.WhenAll(boardClient.ConnectAsync(hostname, port), cardClient.ConnectAsync(hostname, port));
        }
    }
}
