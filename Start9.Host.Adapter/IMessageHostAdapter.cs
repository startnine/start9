using Start9.Api.Contracts;
using Start9.Host.View;

namespace Start9.Host.Adapter
{
    public class IMessageHostAdapter
    {
        internal static IMessage ContractToViewAdapter(IMessageContract contract)
        {
            if (contract == null)
            {
                return null;
            }

            if (System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true && contract is IMessageViewToContractHostAdapter)
            {
                return ((IMessageViewToContractHostAdapter) contract).GetSourceView();
            }
            else
            {
                return new IMessageContractToViewHostAdapter(contract);
            }
        }

        internal static IMessageContract ViewToContractAdapter(IMessage view)
        {
            if (view == null)
            {
                return null;
            }

            if (view is IMessageContractToViewHostAdapter)
            {
                return ((IMessageContractToViewHostAdapter) view).GetSourceContract();
            }
            else
            {
                return new IMessageViewToContractHostAdapter(view);
            }
        }
    }
}

