using System.Drawing;

namespace Start9.Api.Programs
{
	public interface IProgramItem
	{
		string Name { get; }
		Icon Icon { get; }

		void Open();
	}
}