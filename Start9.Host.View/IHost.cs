using System.Collections;
using System.Collections.Generic;

namespace Start9.Host.View
{
    public interface IHost
    {
        void SendGlobalMessage(IMessage message);
        IList<IModule> GetModules();
        IConfiguration GetConfiguration(IModule module);
    }
}

