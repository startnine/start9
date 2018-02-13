using System;
using Start9.Api.Programs;

namespace Start9.Api.Objects
{
	public class WindowEventArgs : EventArgs
	{
		public WindowEventArgs(ProgramWindow window)
		{
			Window = window;
		}

		public ProgramWindow Window { get; }
	}
}