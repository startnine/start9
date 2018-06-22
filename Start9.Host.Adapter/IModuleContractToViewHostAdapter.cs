using Start9.Api.Contracts;
using Start9.Host.View;
using System.AddIn.Pipeline;

namespace Start9.Host.Adapter
{
    [HostAdapter]
    public class IModuleContractToViewHostAdapter : IModule
    {
        private IModuleContract _contract;
        private ContractHandle _handle;

        static IModuleContractToViewHostAdapter()
        {
        }

        public IModuleContractToViewHostAdapter(IModuleContract contract)
        {
            _contract = contract;
            _handle = new ContractHandle(contract);
        }

        public IConfiguration Configuration
        {
            get
            {
                return IConfigurationHostAdapter.ContractToViewAdapter(_contract.Configuration);
            }
        }

        public void HostReceived(IHost host)
        {
            _contract.HostReceived(IHostHostAdapter.ViewToContractAdapter(host));
        }

        public IMessage SendMessage(IMessage message) => IMessageHostAdapter.ContractToViewAdapter(_contract.SendMessage(IMessageHostAdapter.ViewToContractAdapter(message)));

        internal IModuleContract GetSourceContract() => _contract;
    }
}

