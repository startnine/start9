using System.Drawing;

namespace Start9.Api.Objects
{
	public interface IProgramItem
	{
		string Name { get; set; }
		Icon Icon { get; }

		void Open();
	}
}