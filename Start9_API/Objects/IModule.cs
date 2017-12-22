using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Start9.Api.Objects
{
	public static class ModuleManager
	{
		public static List<IFixedModule> InstalledFixedModules { get; } = new List<IFixedModule>();
		public static List<ITransientModule> InstalledTransientModules { get; } = new List<ITransientModule>();

		public static void Test()
		{
			if (!Directory.Exists(Environment.ExpandEnvironmentVariables(@"%appdata%\Start9\Modules"))) return;
			foreach (var f in Directory.EnumerateFiles(Environment.ExpandEnvironmentVariables(@"%appdata%\Start9\Modules")))
				if (Path.GetExtension(f) == ".dll")
				{
					var an = AssemblyName.GetAssemblyName(f);
					var assembly = Assembly.Load(an);
					if (assembly == null) continue;

                    foreach (var type in assembly.GetTypes())
                    {
                        var theseInterfaces = type.GetInterfaces().ToList();

                        if (theseInterfaces.Contains(typeof(IFixedModule)))
                        {
                            var module = (IFixedModule)Activator.CreateInstance(type);
                            InstalledFixedModules.Add(module);
                            Debug.WriteLine(module + " ENABLED! (FIXED)");
                            module.ModuleEnabled();
                        }

                        if (theseInterfaces.Contains(typeof(ITransientModule)))
                        {
                            var module = (ITransientModule)Activator.CreateInstance(type);
                            InstalledTransientModules.Add(module);
                            Debug.WriteLine(module + " ENABLED! (TRANSIENT)");
                            module.ModuleEnabled();
                        }
                    }
				}
		}

		public static void FireThisCommand(ModuleCommand command)
		{
			if (command.CommandName == "Start Button Command")
				InstalledTransientModules[0].HandleCommand(command);
		}
	}

	public interface IModule
	{
		string ModuleName { get; }
		void ModuleEnabled();
		void ModuleDisabled();
	}

	public interface IFixedModule : IModule
	{
		List<ModuleCommand> Commands { get; }
	}

	public interface ITransientModule : IModule
	{
		void HandleCommand(ModuleCommand sender);
	}

	public class ModuleCommand
	{
		public string CommandName { get; set; }

		public CommandStatus Status = CommandStatus.Idle;

		public void FireCommand()
		{
			Debug.WriteLine(CommandName + " FIRED!");
			ModuleManager.FireThisCommand(this);
		}

		public static ModuleCommand GetCommandByName(string commandName, IEnumerable<ModuleCommand> source)
		{
			ModuleCommand c = null;
			foreach (var m in source)
				if (m.CommandName == commandName)
					c = m;
			return c;
		}

		public enum CommandStatus
		{
			Idle,
			Active
		}
	}
}