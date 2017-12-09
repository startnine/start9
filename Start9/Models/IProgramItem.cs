using System.Drawing;

namespace Start9.Models
{
    public interface IProgramItem
    {
        string Name { get; set; }
        Icon Icon { get; }

        void Open();
    }
	
}