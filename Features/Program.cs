using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using static BiblePay.BMS.Common;

namespace BiblePay.BMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string sBindURL = GetConfigurationKeyValue("bindurl");
            if (sBindURL == "")
                sBindURL = "https://localhost:5000";
            Log("BMS v" + BMS_VERSION.ToString() + " starting up :: BindURL==" + sBindURL);
            Initialize(null, null, sBindURL);
        }
        
        public static IWebHost Initialize(IEnumerable<ServiceDescriptor> services, ApiSettings apiSettings, string apiUri)
        {
            // BMS Entry Point (BMS->BWS)
            // BWS will spawn one thread per BMS instance dedicated to servicing BIPFS replication
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(BiblePay.BMS.DSQL.Sync.Syncer));
            t.Start(apiUri);

            IWebHost host = new WebHostBuilder()
                .UseKestrel(kestrelOptions =>
                {
                    kestrelOptions.Limits.MaxRequestHeadersTotalSize = 50000000;
                    kestrelOptions.Limits.MaxRequestBufferSize = 50000000;
                    kestrelOptions.Limits.RequestHeadersTimeout = new TimeSpan(180000);
                    kestrelOptions.Limits.MaxRequestBodySize = long.MaxValue;
                    
                    //kestrelOptions.ConfigureHttpsDefaults(httpsOptions.ServerCertificate = FluffySpoon.X509.GetSSL();
                    kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
                    {
                        httpsOptions.ServerCertificate = FluffySpoon.X509.GetSSL();
                    });

                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls(apiUri.ToString())
                .ConfigureServices(collection =>
                {
                    if (services == null)
                    {
                        return;
                    }
                  
                })
                .UseStartup<BWS>()
                .Build();
            
            host.Start();

            return host;
        }
    }
}
