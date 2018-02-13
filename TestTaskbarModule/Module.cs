using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Start9.Api.Modules;

namespace TestTaskbarModule
{
	[Serializable]
	class TaskbarModule : Module
	{
		/// <inheritdoc />
		public override string Name => "Test Taskbar Module";
	}
}
