﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Ethereal.WebAPI.Settings;
using Flurl.Http;
using NLog;

namespace Ethereal.WebAPI
{
    public class KeepMeAlive
    {
        static readonly Logger InternalLogger = LogManager.GetCurrentClassLogger();
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
                        InternalLogger.Info(response);
                    }
                    catch (Exception e)
                    {
                        InternalLogger.Error(e);
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