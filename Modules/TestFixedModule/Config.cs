using System.Collections.Generic;
using System.Windows;
using Start9.Api.Objects;

namespace TestModule
{
	public class SuperbarModule : IFixedModule
	{
		public List<ModuleCommand> TaskbarCommands = new List<ModuleCommand>
		{
			new ModuleCommand
			{
				CommandName = "Start Button Command"
			},
			new ModuleCommand
			{
				CommandName = "Search Button Command"
			},
			new ModuleCommand
			{
				CommandName = "Task View Button Command"
			},
			new ModuleCommand
			{
				CommandName = "Action Center Button Command"
			}
		};

		public List<Taskbar> Taskbars = new List<Taskbar>();

		public List<ModuleCommand> Commands => TaskbarCommands;

		public string ModuleName => "Test Module";

		public void ModuleEnabled()
		{
			var taskbar = new Taskbar();
			Taskbars.Add(taskbar);
			taskbar.Visibility = Visibility.Visible;
		}

		public void ModuleDisabled()
		{
			Taskbars[0].Close();
		}
	}

	public static class Config
	{
		public static Taskbar Taskbar;

		public static TaskbarGroupingMode GroupingMode = TaskbarGroupingMode.Combine;
	}
}