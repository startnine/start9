using Start9.Host.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Start9
{
    struct Message : IMessage
    {
        public Message(String message = "", Object o = null)
        {
            Text = message;
            Object = o;
        }

        public static Message Empty { get; } = new Message();

        public String Text { get; }
        public Object Object { get; }
    }

    public class Start9Host : IHost
    {
        public IConfiguration GetConfiguration(IModule module) => null;
        public IList<IModule> GetModules() => new IModule[] { };
        public void SendGlobalMessage(IMessage message)
        {
            foreach(var module in GetModules())
            {
                module.SendMessage(message);
            }
        }
    }
}
