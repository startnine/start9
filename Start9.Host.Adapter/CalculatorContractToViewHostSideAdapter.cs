using Start9.Api.Contracts;
using Start9.Host.View;
using System;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Start9.Host.Adapter
{
    [HostAdapter]
    public class CalculatorContractToViewHostSideAdapter : ICalculator
    {
        private ICalc1Contract _contract;
        private ContractHandle _handle;

        public CalculatorContractToViewHostSideAdapter(ICalc1Contract contract)
        {
            _contract = contract;
            _handle = new ContractHandle(contract);
        }

        public Double Add(Double a, Double b)
        {
            return _contract.Add(a, b);
        }

        public Double Subtract(Double a, Double b)
        {
            return _contract.Subtract(a, b);
        }

        public Double Multiply(Double a, Double b)
        {
            return _contract.Multiply(a, b);
        }

        public Double Divide(Double a, Double b)
        {
            return _contract.Divide(a, b);
        }
    }
}
