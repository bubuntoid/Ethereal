using Ethereal.WebAPI.Client;

namespace Ethereal.WebAPI.AutoTests
{
    public class EtherealClientSettings : IEtherealClientSettings
    {
        public string Endpoint => "http://localhost:5000";
    }
}