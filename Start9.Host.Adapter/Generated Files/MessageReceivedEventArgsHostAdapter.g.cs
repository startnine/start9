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
    
    public class MessageReceivedEventArgsHostAdapter
    {
        internal static Start9.Host.Views.MessageReceivedEventArgs ContractToViewAdapter(Start9.Api.Contracts.IMessageReceivedEventArgsContract contract)
        {
            if ((contract == null))
            {
                return null;
            }
            if (((System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true) 
                        && contract.GetType().Equals(typeof(MessageReceivedEventArgsViewToContractHostAdapter))))
            {
                return ((MessageReceivedEventArgsViewToContractHostAdapter)(contract)).GetSourceView();
            }
            else
            {
                return new MessageReceivedEventArgsContractToViewHostAdapter(contract);
            }
        }
        internal static Start9.Api.Contracts.IMessageReceivedEventArgsContract ViewToContractAdapter(Start9.Host.Views.MessageReceivedEventArgs view)
        {
            if ((view == null))
            {
                return null;
            }
            if (view.GetType().Equals(typeof(MessageReceivedEventArgsContractToViewHostAdapter)))
            {
                return ((MessageReceivedEventArgsContractToViewHostAdapter)(view)).GetSourceContract();
            }
            else
            {
                return new MessageReceivedEventArgsViewToContractHostAdapter(view);
            }
        }
    }
}

