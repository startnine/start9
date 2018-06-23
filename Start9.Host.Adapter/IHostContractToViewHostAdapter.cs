using Start9.Api.Contracts;
using Start9.Host.View;
using System.AddIn.Pipeline;
using System.Collections;

namespace Start9.Host.Adapter
{
    public class IHostContractToViewHostAdapter : IHost
    {
        private IHostContract _contract;
        private ContractHandle _handle;
        static IHostContractToViewHostAdapter()
        {
        }
        public IHostContractToViewHostAdapter(IHostContract contract)
        {
            _contract = contract;
            _handle = new ContractHandle(contract);
        }
        public void SendGlobalMessage(IMessage message)
        {
            _contract.SendGlobalMessage(IMessageHostAdapter.ViewToContractAdapter(message));
        }
        public System.Collections.Generic.IList<IModule> GetModules()
        {
            return CollectionAdapters.ToIList(_contract.GetModules(), IModuleHostAdapter.ContractToViewAdapter, IModuleHostAdapter.ViewToContractAdapter);
        }
        public IConfiguration GetConfiguration(IModule module)
        {
            return IConfigurationHostAdapter.ContractToViewAdapter(_contract.GetConfiguration(IModuleHostAdapter.ViewToContractAdapter(module)));
        }
        internal IHostContract GetSourceContract()
        {
            return _contract;
        }
    }
}

