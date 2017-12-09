using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Start9.Models
{
	public class WindowEventArgs : EventArgs
	{
		public ProgramWindow Window { get; }

		public WindowEventArgs(ProgramWindow window)
		{
			Window = window;
		}
	}
}
