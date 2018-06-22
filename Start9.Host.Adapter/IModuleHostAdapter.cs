using Start9.Api.Contracts;
using Start9.Host.View;
using System.Runtime.Remoting;

namespace Start9.Host.Adapter
{    
    public class IModuleHostAdapter
    {
        internal static IModule ContractToViewAdapter(IModuleContract contract)
        {
            if (contract == null)
            {
                return null;
            }

            if (RemotingServices.IsObjectOutOfAppDomain(contract) != true && contract is IModuleViewToContractHostAdapter)
            {
                return ((IModuleViewToContractHostAdapter) contract).GetSourceView();
            }
            else
            {
                return new IModuleContractToViewHostAdapter(contract);
            }
        }

        internal static IModuleContract ViewToContractAdapter(IModule view)
        {
            if (view == null)
            {
                return null;
            }
            if (view is IModuleContractToViewHostAdapter)
            {
                return ((IModuleContractToViewHostAdapter) view).GetSourceContract();
            }
            else
            {
                return new IModuleViewToContractHostAdapter(view);
            }
        }
    }
}

