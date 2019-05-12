using AlmVR.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlmVR.Client.Core
{
    public interface IBoardClient : IClient
    {
        Task<BoardModel> GetBoardAsync();
    }
}
