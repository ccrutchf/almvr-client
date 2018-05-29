using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlmVR.Client.Core
{
    public interface IClient : IDisposable
    {
        Task ConnectAsync(string hostName, int port);
    }
}
