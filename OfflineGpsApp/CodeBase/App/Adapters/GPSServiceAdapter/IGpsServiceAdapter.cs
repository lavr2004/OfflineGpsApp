namespace OfflineGpsApp.CodeBase.App.Adapters.GPSServiceAdapter
{
    public interface IGpsServiceAdapter
    {
        event EventHandler<GpsServiceAdapterEventArgs> LocationChanged;
        void StartListening();
        void StopListening();
    }
}
