using Start9.Api.Contracts;
using Start9.Host.View;
using System.AddIn.Pipeline;
using System.Collections;

namespace Start9.Host.Adapter
{
    public class IHostViewToContractHostAdapter : ContractBase, IHostContract
    {
        private IHost _view;
        public IHostViewToContractHostAdapter(IHost view)
        {
            _view = view;
        }
        public virtual void SendGlobalMessage(IMessageContract message)
        {
            _view.SendGlobalMessage(IMessageHostAdapter.ContractToViewAdapter(message));
        }
        public virtual System.AddIn.Contract.IListContract<IModuleContract> GetModules()
        {
            return CollectionAdapters.ToIListContract(_view.GetModules(), IModuleHostAdapter.ViewToContractAdapter, IModuleHostAdapter.ContractToViewAdapter);
        }
        public virtual IConfigurationContract GetConfiguration(IModuleContract module)
        {
            return IConfigurationHostAdapter.ViewToContractAdapter(_view.GetConfiguration(IModuleHostAdapter.ContractToViewAdapter(module)));
        }
        internal IHost GetSourceView()
        {
            return _view;
        }
    }
}

