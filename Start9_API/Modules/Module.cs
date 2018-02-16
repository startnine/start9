using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Start9.Api.Modules
{
	[Serializable]
	public abstract class Module
	{
		static readonly List<Module> _modules = new List<Module>();

		public static ReadOnlyCollection<Module> Modules => _modules.AsReadOnly();

		static Module()
		{

		}

		public static void Poke()
		{
			//this runs the ctor
		}
		
		public abstract string Name { get; }

		public Guid Guid { get; } = Guid.NewGuid();

		protected virtual void MessageRecieved<T>(Message<T> message)
		{
			// TBD
		}

		protected virtual void MessageRecieved(Message message)
		{

		}


		public override string ToString() => $"[{GetType().Assembly}]\"{Name}\":{Guid}";
	}
}