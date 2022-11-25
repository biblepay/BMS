using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace BiblePay.BMS
{

    public class MyWebClient : System.Net.WebClient
    {
        private int DEFAULT_TIMEOUT = 30000;
        public void SetTimeout(int iTimeout)
        {
            DEFAULT_TIMEOUT = iTimeout * 1000;
        }
        protected override System.Net.WebRequest GetWebRequest(Uri uri)
        {
            System.Net.WebRequest w = base.GetWebRequest(uri);
            w.Timeout = DEFAULT_TIMEOUT;
            return w;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {

            string sBindURL = BMSCommon.Common.GetConfigurationKeyValue("bindurl");
            if (sBindURL == "")
                sBindURL = "https://localhost:" + Common.DEFAULT_PORT.ToString();
            sBindURL += ";http://0.0.0.0:8080";

            Init(sBindURL);
            CreateHostBuilder(args,sBindURL,null).Build().Run();
        }

        public static async Task<bool> Init(string sURL)
        {
            // Primary Entry point for BMS:
            // BWS will spawn one thread per BMS instance dedicated to servicing BIPFS replication
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(BiblePay.BMS.DSQL.Sync.Syncer));
            t.Start(sURL);
            try
            {
                await BBPTestHarness.IPFS.BroadcastNode(BMSCommon.API.GetCDN(), true);
            }
            catch(Exception ex)
            {
                BMSCommon.WebRPC.LogRPCError("BN::" + ex.Message);
                BMSCommon.Common.Log("BN::" + ex.Message);
            }
            System.Threading.Thread m = new System.Threading.Thread(BMSCommon.BitcoinSync2.BCSync);
            m.Start();
            // The billing thread
            System.Threading.Thread q = new System.Threading.Thread(DSQL.QuantBilling.Looper);
            q.Start();
            // The pool
            DSQL.PoolBase.NewPool();
            return true;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IFileProvider>(
                 new PhysicalFileProvider(Directory.GetCurrentDirectory()));
            services.AddMvc(options =>
            {
                ServiceProvider serviceProvider = services.BuildServiceProvider();
            });

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder.WithOrigins(new string[] { "https://dec.app", "https://hitch.social", "https://www.hitch.social",
                    "https://social.biblepay.org", "http://social.biblepay.org", "https://*", "http://localhost", "https://localhost" })
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

        }



        public static IHostBuilder CreateHostBuilder(string[] args, string sBindURL, IEnumerable<ServiceDescriptor> services) =>
            Host.CreateDefaultBuilder(args)
            
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    /*
                    webBuilder.UseKestrel(kestrelOptions =>
                    {
                        kestrelOptions.Limits.MaxRequestHeadersTotalSize = 51000000;
                        //kestrelOptions.Limits.MaxRequestBufferSize = 51000000;
                        kestrelOptions.Limits.RequestHeadersTimeout = new TimeSpan(180000);
                        kestrelOptions.Limits.MaxRequestBodySize = long.MaxValue;

                        //kestrelOptions.ConfigureHttpsDefaults(httpsOptions.ServerCertificate = FluffySpoon.X509.GetSSL();
                        kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
                        {
                            httpsOptions.ServerCertificate = X509.GetSSL();
                        });
                    });.ConfigureKestrel
                    */
                    //.Build();
                    

                    webBuilder.UseKestrel(serverOptions =>
                    {
                        serverOptions.Limits.MaxRequestHeadersTotalSize = 51000000;
                        serverOptions.Limits.MaxRequestBufferSize = 51000000;
                        serverOptions.Limits.RequestHeadersTimeout = new TimeSpan(180000);
                        serverOptions.Limits.MaxRequestBodySize = long.MaxValue;

                        //kestrelOptions.ConfigureHttpsDefaults(httpsOptions.ServerCertificate = FluffySpoon.X509.GetSSL();
                        serverOptions.ConfigureHttpsDefaults(httpsOptions =>
                        {
                            httpsOptions.ServerCertificate = FluffySpoon.X509.GetSSL();
                        });

                    })
                    .ConfigureServices(collection =>
                    {
                        if (services == null)
                        {
                            return;
                        }

                    })
                    .UseIISIntegration().UseKestrel()
                    .UseUrls(sBindURL)
                    .UseStartup<BWS>();


                });
    }
}
