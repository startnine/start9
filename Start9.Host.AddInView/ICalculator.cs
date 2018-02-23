using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Start9.Host.AddInView
{
	public interface ICalculator
	{
		double Add(double a, double b);
		double Subtract(double a, double b);
		double Multiply(double a, double b);
		double Divide(double a, double b);
	}

}
