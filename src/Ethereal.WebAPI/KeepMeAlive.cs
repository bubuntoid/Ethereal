using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ethereal.WebAPI.Settings;
using Flurl.Http;

namespace Ethereal.WebAPI
{
    public class KeepMeAlive
    {
        private IFlurlClient client;
        
        public KeepMeAlive(SystemSettings systemSettings)
        {
            client = new FlurlClient(systemSettings.Endpoint);
        }

        public void Run()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var response = client.Request("api", "hc").GetStringAsync().GetAwaiter().GetResult();
                        Console.WriteLine(response);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        Thread.Sleep(60 * 1000);
                    }
                }
            });
        }
    }
}