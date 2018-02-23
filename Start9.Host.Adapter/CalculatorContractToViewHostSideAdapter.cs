using System;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Start9.Api.Contracts;
using Start9.Host.AddInView;

namespace Start9.Host.Adapter
{
	[HostAdapter]
	public class CalculatorContractToViewHostSideAdapter : ICalculator
	{
		private readonly ICalculatorContract _contract;
		private ContractHandle _handle;

		public CalculatorContractToViewHostSideAdapter(ICalculatorContract contract)
		{
			_contract = contract;
			_handle = new ContractHandle(contract);
		}

		public double Add(double a, double b)
		{
			return _contract.Add(a, b);
		}

		public double Subtract(double a, double b)
		{
			return _contract.Subtract(a, b);
		}

		public double Multiply(double a, double b)
		{
			return _contract.Multiply(a, b);
		}

		public double Divide(double a, double b)
		{
			return _contract.Divide(a, b);
		}
	}
}
