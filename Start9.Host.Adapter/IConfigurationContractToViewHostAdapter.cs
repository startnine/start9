using Start9.Api.Contracts;
using Start9.Host.View;
using System.AddIn.Pipeline;
using System.Collections;

namespace Start9.Host.Adapter
{
    public class IConfigurationContractToViewHostAdapter : IConfiguration
    {
        private IConfigurationContract _contract;
        private ContractHandle _handle;

        static IConfigurationContractToViewHostAdapter()
        {
        }

        public IConfigurationContractToViewHostAdapter(IConfigurationContract contract)
        {
            _contract = contract;
            _handle = new ContractHandle(contract);
        }

        public IDictionary Entries => _contract.Entries;

        internal IConfigurationContract GetSourceContract() => _contract;
    }
}

