using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using static BiblePay.BMS.Common;

namespace BiblePay.BMS
{

    public class BWS
    {
        public IConfigurationRoot Configuration { get; }
   
        public BWS(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            this.Configuration = builder.Build();
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
                    "https://social.biblepay.org", "http://social.biblepay.org", "http://localhost", "https://localhost" })
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

        }


        private StaticFileOptions GetStaticFileConfiguration()
        {
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".exe"] = "application/octect-stream";
            return new StaticFileOptions { ContentTypeProvider = provider };
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
 
        object GetFirst(dynamic oCollection)
        {
            foreach (object o in oCollection)
            {
                return o;
            }
            return null;
        }
        public static int GetPort(string URL)
        {
            string[] pieces = URL.Split(':');
            if (pieces.Length > 1)
            {
                return (int)GetDouble(pieces[2]);
            }
            return 0;
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors("CorsPolicy");
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseStaticFiles();
            app.UseHalfordFileProvider();
            try
            {
                var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
                string address = GetFirst(serverAddressesFeature.Addresses).ToString();
                int iPort = GetPort(address);

                string sWWWRoot = GetFolder("video");
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(sWWWRoot),
                    RequestPath = "/video"
                });
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(GetFolder("sql")),
                    RequestPath = "/sql"
                });
                // End of Biblepay MVC
            }
            catch (Exception ex)
            {
                Log("BWS::" + ex.Message);
            }
        }
    }

    public static class UseMiddlewareExtensions
    {
        public static IApplicationBuilder UseHalfordFileProvider(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HalfordFileProvider>();
        }
    }

    public class HalfordFileProvider
    {
        private readonly RequestDelegate _next;
        private readonly IFileProvider fileProvider;
        
        public HalfordFileProvider(RequestDelegate next, IFileProvider fileProvider)
        {
            this._next = next;
            this.fileProvider = fileProvider;
        }

        public static string StripLeading(string Data, int iLeadingChars)
        {
            if (iLeadingChars > Data.Length)
                return Data;
            string sOut = Data.Substring(iLeadingChars - 1, Data.Length - iLeadingChars + 1);
            return sOut;
        }

        public static bool PullDown(string sURL, string sTempFile)
        {
            // This is used in our CDN if file is missing on all sancs, we pull it from IPFS.
            MyWebClient wc = new MyWebClient();
            try
            {
                Log("Pulling down " + sURL);
                wc.DownloadFile(sURL, sTempFile);
                return true;
            }
            catch(Exception ex)
            {
                Log("Unable to pull down " + sURL);
                return false;
            }
        }

        public static string ReqPathToFilePath(string ContextRequestPath)
        {
            string sOrigReqPath = ContextRequestPath;
            string sReqPath = StripLeading(sOrigReqPath, 2);
            if (IsWindows())
            {
                sReqPath = sReqPath.Replace("/", "\\");
            }
            string Sourcepath = Path.Combine(GetFolder(""), sReqPath);

            System.IO.FileInfo fi = new FileInfo(Sourcepath);
            if (!System.IO.Directory.Exists(fi.Directory.FullName))
            {
                System.IO.Directory.CreateDirectory(fi.Directory.FullName);
            }
            return Sourcepath;
        }

        public FileInfo GetFileInfo(string ContextRequestPath)
        {
            string sOrigReqPath = ContextRequestPath;
            string sReqPath = StripLeading(sOrigReqPath, 2);
            if (IsWindows())
            {
                sReqPath = sReqPath.Replace("/", "\\");
            }
            string Sourcepath =  Path.Combine(GetFolder(""), sReqPath);

            System.IO.FileInfo fi = new FileInfo(Sourcepath);
            if (!System.IO.Directory.Exists(fi.Directory.FullName))
            {
                System.IO.Directory.CreateDirectory(fi.Directory.FullName);
            }

            if (!fi.Exists)
            {
                // This is BiblePay DSQL 404 page:
                Log("Cant find file " + Sourcepath + " from orig req path " + sOrigReqPath);
                return null;
            }
            else
            {
                // This is static content
                return fi;
            }
        }
        // Storage
        

        public static long GetFileSizeFromURL(string url)
        {
            long result = -1;
            // Similar to a range request, this head request confirms Existence of a file only from a web resource
            try
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                req.Method = "HEAD";
                using (System.Net.WebResponse resp = req.GetResponse())
                {
                    if (long.TryParse(resp.Headers.Get("Content-Length"), out long ContentLength))
                    {
                        result = ContentLength;
                    }
                }
                return result;
            }
            catch(Exception ex)
            {
                return 0;
            }
        }

        public static string NormalizeURL(string sURL)
        {
            sURL = sURL.Replace("https://", "{https}");
            sURL = sURL.Replace("///", "/");
            sURL = sURL.Replace("//", "/");
            sURL = sURL.Replace("{https}", "https://");
            return sURL;
        }

        // BiblePay Decentralized Web Server
        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Reserved: When we resolve CDN nickname to URI:
                // string sNN1 = BiblePay.BMS.DSQL.Sync._nicknames[0].nickName;
                // Normal flow.
                string sourcepath = context.Request.Path;
                if (sourcepath.Contains("/video") || sourcepath.Contains("/upload") || sourcepath.Contains("/shard") || sourcepath.Contains("/database") || sourcepath.Contains("/broadcast"))
                {
                    // Case 1: A web resource
                    FileInfo fi = GetFileInfo(sourcepath);
                    if (fi == null)
                    {
                        for (int i = 0; i < BBPTestHarness.IPFS.mNodes.Count; i++)
                        {
                            if (BBPTestHarness.IPFS.mNodes[i].IsMine == false)
                            {
                                string sURL = NormalizeURL(BBPTestHarness.IPFS.mNodes[i].URL + "/" + sourcepath);

                                long iSz = HalfordFileProvider.GetFileSizeFromURL(sURL);
                                if (iSz > 0)
                                {
                                    context.Response.Redirect(sURL);
                                }
                                /*
                                if (false)
                                {
                                    // Reserved.  This is if we want to 'fill in the gaps' on the local node.  For now we let the Replicator do that, and we move to the next sanctuary.
                                    string sTargetPath = ReqPathToFilePath(sourcepath);
                                    PullDown(sNewURL, sTargetPath);
                                    fi = GetFileInfo(sourcepath);
                                }
                                */
                            }
                        }
                        // If we reach here, this is our last resort.  This means every sanc is missing the file (or this is really a 404).
                        // Try to get the file from IPFS next:
                        string sNewURL = "https://bbpipfs.s3.filebase.com" + sourcepath;
                        long iSize = HalfordFileProvider.GetFileSizeFromURL(sNewURL);
                        if (iSize > 0)
                        {
                            context.Response.Redirect(sNewURL);
                            return;
                        }
                        // If not, send them a 404:
                        context.Response.StatusCode = 404;
                        return;
                    }
                    // File actually exists locally:
                    await context.Response.SendFileAsync(fi.FullName);
                }
                else if (sourcepath == "/")
                {
                    // The web root
                    context.Response.Redirect("/BMS/AppServer");
                }
                else if (sourcepath.Contains("/favicon.ico"))
                {
                    string sReqPath = GetFolder("images1", "favicon.png");
                    await context.Response.SendFileAsync(sReqPath);
                }
                else
                {
                    // Reserved for Reverse DNS lookups:
                    // string webPath = GetWebPathByReverseDNS(sourcepath);
                    // bool fExists = System.IO.File.Exists(webPath);
                    // if (webPath == String.Empty || !fExists)
                    // webPath = GetFolder(DEFAULT_PORT, "images1", "404.jpg");
                    // await context.Response.SendFileAsync(webPath);
                }
            }
            catch(Exception ex)
            {
                // cancelled task?
                Log("BWS::WebServer::" + ex.Message);
            }
        }    
    }
}


