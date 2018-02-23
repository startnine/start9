using System;
using System.AddIn.Hosting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Start9.Host.AddInView;

namespace Start9
{
	internal static class AddInManager
	{
		public static Collection<AddInToken> LoadAddins()
		{
			// Assume that the current directory is the application folder, 
			// and that it contains the pipeline folder structure.
			string addInRoot = Environment.ExpandEnvironmentVariables("%appdata%") + "\\Start9";

			// Update the cache files of the pipeline segments and add-ins.
			string[] warnings = AddInStore.Update(addInRoot);
			Console.ForegroundColor = ConsoleColor.Yellow;

			foreach (string warning in warnings)
			{
				Console.WriteLine(warning);
			}

			Console.ResetColor();

			// Search for add-ins of type ICalculator (the host view of the add-in).
			return AddInStore.FindAddIns(typeof(ICalculator), addInRoot);

			// Ask the user which add-in they would like to use.
			// AddInToken calcToken = ChooseCalculator(tokens);

			// Activate the selected AddInToken in a new application domain 
			// with the Internet trust level.
			// var calc =
				// calcToken.Activate<ICalculator>(AddInSecurityLevel.Internet);

			// Run the add-in.
			// RunCalculator(calc);
		}

		/* private static AddInToken ChooseCalculator(Collection<AddInToken> tokens)
		{
			if (tokens.Count == 0)
			{
				Console.WriteLine("No calculators are available");
				return null;
			}
			Console.WriteLine("Available Calculators: ");
			// Show the token properties for each token in the AddInToken collection 
			// (tokens), preceded by the add-in number in [] brackets.
			var tokNumber = 1;
			foreach (AddInToken tok in tokens)
			{
				Console.WriteLine(
					$"\t[{tokNumber}]: {tok.Name} - {tok.AddInFullName}\n\t{tok.AssemblyName}\n\t\t {tok.Description}\n\t\t {tok.Version} - {tok.Publisher}");
				tokNumber++;
			}
			Console.WriteLine("Which calculator do you want to use?");
			string line = Console.ReadLine();
			if (int.TryParse(line, out int selection))
			{
				if (selection <= tokens.Count)
				{
					return tokens[selection - 1];
				}
			}
			Console.WriteLine("Invalid selection: {0}. Please choose again.", line);
			return ChooseCalculator(tokens);
		} */

		public static (double result, bool success) RunCalculator(ICalculator calc, string line)
		{

			if (calc == null)
			{
				return (double.NaN, false);
			}

			// The Parser class parses the user's input.
			try
			{
				var c = new Parser(line);
				switch (c.Action)
				{
					case "+":
						return (calc.Add(c.A, c.B), true);
					case "-":
						return (calc.Subtract(c.A, c.B), true);
					case "*":
						return (calc.Multiply(c.A, c.B), true);
					case "/":
						return (calc.Divide(c.A, c.B), true);
					default:
						throw new Exception();
				}
			}
			catch
			{
				// Console.WriteLine("Invalid command: {0}. Commands must be formated: [number] [operation] [number]", line);
				return (double.NaN, false);
			}
		}

		private class Parser
		{
			internal Parser(string line)
			{
				string[] parts = line.Split(' ');
				A = double.Parse(parts[0]);
				Action = parts[1];
				B = double.Parse(parts[2]);
			}

			public double A { get; }

			public double B { get; }

			public string Action { get; }
		}
	}
}
