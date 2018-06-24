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
    
    public class IMessageEventHandlerViewToContractHostAdapter : System.AddIn.Pipeline.ContractBase, Start9.Api.Contracts.IMessageEventHandlerContract
    {
        private object _view;
        private System.Reflection.MethodInfo _event;
        public IMessageEventHandlerViewToContractHostAdapter(object view, System.Reflection.MethodInfo eventProp)
        {
            _view = view;
            _event = eventProp;
        }
        public void Handler(Start9.Api.Contracts.IMessageReceivedEventArgsContract args)
        {
            Start9.Host.Views.MessageReceivedEventArgs adaptedArgs;
            adaptedArgs = Start9.Host.Adapters.MessageReceivedEventArgsHostAdapter.ContractToViewAdapter(args);
            object[] argsArray = new object[1];
            argsArray[0] = adaptedArgs;
            _event.Invoke(_view, argsArray);
        }
        internal object GetSourceView()
        {
            return _view;
        }
    }
}

