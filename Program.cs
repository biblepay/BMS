using BBPAPI;
using BBPAPI.Model;
using BiblePay.BMS.DSQL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static BMSCommon.Common;

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
        public static async Task Main(string[] args)
        {
            // TODO: When running this as a regular User, ensure we bind to the right port.
            // MISSION CRITICAL
            string sBindURL = GetConfigKeyValue("bindurl");
            if (sBindURL == string.Empty)
            {
                sBindURL = "https://localhost:" + GlobalSettings.DEFAULT_PORT.ToString();
            }
            sBindURL += ";http://0.0.0.0:8080";
            await BBPAPI.ServiceInit.Init();
            CreateHostBuilder(args,sBindURL,null).Build().Run();
            
        }

        public static async Task<bool> Init(string sURL)
        {
            // Primary Entry point for BMS:
            // Set the user, start the loop thread, and initialize the charge
            await BBPAPI.ServiceInit.Init();
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
