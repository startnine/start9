using Start9.Api.Tools;
using Start9.Host.Views;
using Start9.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Start9
{
    class Start9Host : IHost
    {
        Start9Host()
        {
            
        }

        private static readonly Lazy<Start9Host> _host = new Lazy<Start9Host>(() => new Start9Host());
        public static Start9Host Instance => _host.Value;

        public IConfiguration GetGlobalConfiguration() => throw new NotImplementedException();
        public IList<IModule> GetModules() => throw new NotImplementedException();
        public void SaveConfiguration(IModule module) => throw new NotImplementedException();

        public void SendMessage(IMessage message)
        {
            Application.Current.Dispatcher.BeginInvoke((Action) (() =>
            {
                  var paths = ((SettingsWindow)Application.Current.MainWindow).ViewModel.Network.Connections.Where(c => c.SourceConnector.MessageEntry.FriendlyName == message.MessageEntry.FriendlyName);

                foreach (var receiver in paths.Select(p => p.DestConnector.ReceiverEntry))
                {
                    receiver.SendMessage(message);
                }

            }), DispatcherPriority.Send);
        }
    }

    class MessageReceivedEventArgsImpl : MessageReceivedEventArgs
    {
        public MessageReceivedEventArgsImpl(IMessage m)
        {
            Message = m;
        }
        public override IMessage Message { get; }
    }
}
