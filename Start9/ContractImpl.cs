using Start9.Host.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Start9
{
    class Start9Host : IHost
    {
        public IConfiguration GetGlobalConfiguration() => throw new NotImplementedException();
        public IList<IModule> GetModules() => throw new NotImplementedException();
        public void SaveConfiguration(IModule module) => throw new NotImplementedException();
        public void SendMessage(IMessage message) => throw new NotImplementedException();
    }
}
