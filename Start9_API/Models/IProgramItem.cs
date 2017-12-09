using System.Drawing;

namespace Start9API.Models
{
    public interface IProgramItem
    {
        string Name { get; set; }
        Icon Icon { get; }

        void Open();
    }
	
}