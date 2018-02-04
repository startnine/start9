using System;
using System.Windows.Controls;
using Start9.Api.Modules;

namespace TestStartMenuModule
{
	[Serializable]
	class MenuModule : Module
	{
		/// <inheritdoc />
		public override string Name => "Test Start Menu Module";

		/// <inheritdoc />
		protected override void MessageRecieved(Message message)
		{
			new Start9.Api.Plex.PlexWindow
			{
				Content = new TextBlock { Text = message.MessageText }
			}.Show();
		}
	}
}
