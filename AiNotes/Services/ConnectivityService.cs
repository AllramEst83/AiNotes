namespace AiNotes.Services
{
    public interface IConnectivityService
    {
        bool IsConnected();
    }
    public class ConnectivityService : IConnectivityService
    {
        public bool IsConnected()
        {
            bool isConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
            if (isConnected)
            {
                return true;
            }

            return false;
        }
    }
}
