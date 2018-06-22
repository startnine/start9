using Start9.Api.Contracts;
using Start9.Host.View;
using System.AddIn.Pipeline;
using System.Collections;

namespace Start9.Host.Adapter
{    
    public class IConfigurationViewToContractHostAdapter : ContractBase, IConfigurationContract
    {
        private IConfiguration _view;

        public IConfigurationViewToContractHostAdapter(IConfiguration view) => _view = view;

        public IDictionary Entries => _view.Entries;

        internal IConfiguration GetSourceView() => _view;
    }
}

