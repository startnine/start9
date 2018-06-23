namespace Start9.Host.View
{    
    public interface IModule
    {
        IMessage SendMessage(IMessage message);
        IConfiguration Configuration
        {
            get;
        }
        void HostReceived(IHost Host);
    }
}

