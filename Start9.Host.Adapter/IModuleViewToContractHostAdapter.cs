using Start9.Api.Contracts;
using Start9.Host.View;
using System.AddIn.Pipeline;

namespace Start9.Host.Adapter
{
    public class IModuleViewToContractHostAdapter : ContractBase, IModuleContract
    {
        private IModule _view;

        public IModuleViewToContractHostAdapter(IModule view) => _view = view;

        public virtual IMessageContract SendMessage(IMessageContract message) => IMessageHostAdapter.ViewToContractAdapter(_view.SendMessage(IMessageHostAdapter.ContractToViewAdapter(message)));

        internal IModule GetSourceView() => _view;
    }
}

