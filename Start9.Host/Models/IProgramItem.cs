using System.Drawing;

namespace Start9.Host.Models
{
    public interface IProgramItem
    {
        string Name { get; set; }
        Icon Icon { get; }

        void Open();
    }
	
}