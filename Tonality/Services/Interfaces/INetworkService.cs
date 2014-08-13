using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tonality.Services.Interfaces
{
    public interface INetworkService
    {
        bool IsConnectionAvailable { get; }
    }
}
