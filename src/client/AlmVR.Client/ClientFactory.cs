using AlmVR.Client.Core;
using AlmVR.Client.Providers.SignalR;
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlmVR.Client
{
    public static class ClientFactory
    {
        private static IContainer container;

        static ClientFactory()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<BoardClientSignalRProvider>().As<IBoardClient>();
            builder.RegisterType<CardClientSignalRProvider>().As<ICardClient>();

            container = builder.Build();
        }

        public static T GetInstance<T>()
        {
            return container.Resolve<T>();
        }
    }
}
