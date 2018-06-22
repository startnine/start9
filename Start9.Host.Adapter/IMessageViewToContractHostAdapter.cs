using Start9.Api.Contracts;
using Start9.Host.View;
using System;
using System.AddIn.Pipeline;

namespace Start9.Host.Adapter
{
    public class IMessageViewToContractHostAdapter : ContractBase, IMessageContract
    {
        private IMessage _view;

        public IMessageViewToContractHostAdapter(IMessage view) => _view = view;

        public String Text => _view.Text;

        public Object Object => _view.Object;

        internal IMessage GetSourceView() => _view;
    }
}

