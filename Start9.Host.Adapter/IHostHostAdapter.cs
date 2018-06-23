using Start9.Api.Contracts;
using Start9.Host.View;
using System.Runtime.Remoting;

namespace Start9.Host.Adapter
{    
    public class IHostHostAdapter
    {
        internal static IHost ContractToViewAdapter(IHostContract contract)
        {
            if (contract == null)
            {
                return null;
            }

            if (RemotingServices.IsObjectOutOfAppDomain(contract) != true && contract is IHostViewToContractHostAdapter)
            {
                return ((IHostViewToContractHostAdapter) contract).GetSourceView();
            }
            else
            {
                return new IHostContractToViewHostAdapter(contract);
            }
        }

        internal static IHostContract ViewToContractAdapter(IHost view)
        {
            if (view == null)
            {
                return null;
            }

            if (view is IHostContractToViewHostAdapter)
            {
                return ((IHostContractToViewHostAdapter) view).GetSourceContract();
            }
            else
            {
                return new IHostViewToContractHostAdapter(view);
            }
        }
    }
}

