using Start9.Api.Contracts;
using Start9.Host.View;
using System.AddIn.Pipeline;
using System;

namespace Start9.Host.Adapter
{ 
    public class IMessageContractToViewHostAdapter : IMessage
    {
        private IMessageContract _contract;
        private ContractHandle _handle;

        static IMessageContractToViewHostAdapter()
        {
        }

        public IMessageContractToViewHostAdapter(IMessageContract contract)
        {
            _contract = contract;
            _handle = new ContractHandle(contract);
        }

        public String Text => _contract.Text;

        public Object Object => _contract.Object;

        internal IMessageContract GetSourceContract() => _contract;
    }
}

