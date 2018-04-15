using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Start9.Host.View
{
    public interface ICalculator
    {
        Double Add(Double a, Double b);
        Double Subtract(Double a, Double b);
        Double Multiply(Double a, Double b);
        Double Divide(Double a, Double b);
    }
}
