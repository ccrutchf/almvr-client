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

        public static Action<string> Log { get; set; }

        public static T GetInstance<T>()
        {
            if (container == null)
            {
                var builder = new ContainerBuilder();

                builder.RegisterType<BoardClientSignalRProvider>().As<IBoardClient>();
                builder.RegisterType<CardClientSignalRProvider>().As<ICardClient>();
                builder.RegisterInstance(Log).As<Action<string>>();

                container = builder.Build();
            }

            return container.Resolve<T>();
        }
    }
}
