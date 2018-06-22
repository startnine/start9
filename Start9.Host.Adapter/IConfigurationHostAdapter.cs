using Start9.Api.Contracts;
using Start9.Host.View;
using System.Runtime.Remoting;

namespace Start9.Host.Adapter
{    
    public class IConfigurationHostAdapter
    {
        internal static IConfiguration ContractToViewAdapter(IConfigurationContract contract)
        {
            if (contract == null)
            {
                return null;
            }

            if (RemotingServices.IsObjectOutOfAppDomain(contract) != true && contract is IConfigurationViewToContractHostAdapter)
            {
                return ((IConfigurationViewToContractHostAdapter) contract).GetSourceView();
            }
            else
            {
                return new IConfigurationContractToViewHostAdapter(contract);
            }
        }

        internal static IConfigurationContract ViewToContractAdapter(IConfiguration view)
        {
            if (view == null)
            {
                return null;
            }

            if (view is IConfigurationContractToViewHostAdapter)
            {
                return ((IConfigurationContractToViewHostAdapter) view).GetSourceContract();
            }
            else
            {
                return new IConfigurationViewToContractHostAdapter(view);
            }
        }
    }
}

