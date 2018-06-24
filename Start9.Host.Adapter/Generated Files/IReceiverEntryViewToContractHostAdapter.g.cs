//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Start9.Host.Adapters
{
    
    public class IReceiverEntryViewToContractHostAdapter : System.AddIn.Pipeline.ContractBase, Start9.Api.Contracts.IReceiverEntryContract
    {
        private Start9.Host.Views.IReceiverEntry _view;
        private System.Collections.Generic.Dictionary<Start9.Api.Contracts.IMessageEventHandlerContract, System.EventHandler<Start9.Host.Views.MessageReceivedEventArgs>> MessageReceived_handlers;
        public IReceiverEntryViewToContractHostAdapter(Start9.Host.Views.IReceiverEntry view)
        {
            _view = view;
            MessageReceived_handlers = new System.Collections.Generic.Dictionary<Start9.Api.Contracts.IMessageEventHandlerContract, System.EventHandler<Start9.Host.Views.MessageReceivedEventArgs>>();
        }
        public string FriendlyName
        {
            get
            {
                return _view.FriendlyName;
            }
        }
        public virtual void MessageReceivedEventAdd(Start9.Api.Contracts.IMessageEventHandlerContract handler)
        {
            System.EventHandler<Start9.Host.Views.MessageReceivedEventArgs> adaptedHandler = new System.EventHandler<Start9.Host.Views.MessageReceivedEventArgs>(new Start9.Host.Adapters.IMessageEventHandlerContractToViewHostAdapter(handler).Handler);
            _view.MessageReceived += adaptedHandler;
            MessageReceived_handlers[handler] = adaptedHandler;
        }
        public virtual void MessageReceivedEventRemove(Start9.Api.Contracts.IMessageEventHandlerContract handler)
        {
            System.EventHandler<Start9.Host.Views.MessageReceivedEventArgs> adaptedHandler;
            if (MessageReceived_handlers.TryGetValue(handler, out adaptedHandler))
            {
                MessageReceived_handlers.Remove(handler);
                _view.MessageReceived -= adaptedHandler;
            }
        }
        internal Start9.Host.Views.IReceiverEntry GetSourceView()
        {
            return _view;
        }
    }
}

