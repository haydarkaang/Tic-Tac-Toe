using System;
using System.Threading.Tasks;

namespace Service
{
    public abstract class ServiceManagerBase : IDisposable
    {
        public abstract Task<ServiceInfo> Up();
        public abstract void Dispose();
    }
}
