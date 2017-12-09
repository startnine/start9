using Start9.Api.Objects;

namespace TestTransientModule
{
	class TstTransientModule : ITransientModule
	{
		public StartMenu StartMenu;

		public string ModuleName => "Test Transient Module";

		public void ModuleEnabled()
		{
			StartMenu = new StartMenu();
			StartMenu.Hide();
		}

		public void ModuleDisabled()
		{
		}

		public void HandleCommand(ModuleCommand sender)
		{
			if (sender.Status == ModuleCommand.CommandStatus.Idle)
			{
				StartMenu.ShowMenu();
				sender.Status = ModuleCommand.CommandStatus.Active;
			}
			else
			{
				StartMenu.HideMenu();
				sender.Status = ModuleCommand.CommandStatus.Idle;
			}
		}
	}
}