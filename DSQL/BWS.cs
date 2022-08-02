using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BiblePay.BMS.Data;
using BiblePay.BMS.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
//using Swashbuckle.AspNetCore.Swagger;
using static BiblePay.BMS.Common;
using static BMSCommon.Common;

namespace BiblePay.BMS
{
    
    public class BWS
    {

        public class ViewBagActionFilter : ActionFilterAttribute
        {

            
            public override void OnResultExecuting(ResultExecutingContext context)
            {
                // for razor pages
                if (context.Controller is PageModel)
                {
                    var controller = context.Controller as PageModel;
                    controller.ViewData.Add("b1", $"~/avatar/empty.png");
                }
                // for Razor Views
                if (context.Controller is Controller)
                {
                    var controller = context.Controller as Controller;
                    //controller.ViewData.Add("b", $"~/avatar/empty.png");
                    
                    controller.ViewBag.Chain = DSQL.UI.GetChain(controller.HttpContext);
                    controller.ViewBag.ChainColor = DSQL.UI.GetChainColor(controller.HttpContext);
                    controller.ViewBag.LoginStatus = DSQL.UI.GetLogInStatus(controller.HttpContext);
                    controller.ViewBag.BioURL = DSQL.UI.GetBioURL(controller.HttpContext);
                    controller.ViewBag.Balance = DSQL.UI.GetAvatarBalance(controller.HttpContext, false);
                    controller.ViewBag.NotificationCountHR = DSQL.UI.GetNotificationCountHR(controller.HttpContext);
                    controller.ViewBag.NotificationCount = DSQL.UI.GetNotificationCount(controller.HttpContext);
                    controller.ViewBag.LoginAction = DSQL.UI.GetLogInAction(controller.HttpContext);

                    BMSCommon.CryptoUtils.User u = DSQL.UI.GetUser(controller.HttpContext);
                    controller.ViewBag.NickName = u.NickName;
                    controller.ViewBag.Notifications = DSQL.UI.GetNotifications(controller.HttpContext, DSQL.UI.GetUser(controller.HttpContext).ERC20Address);
                    //also you have access to the httpcontext & route in controller.HttpContext & controller.RouteData
                }

                base.OnResultExecuting(context);
            }
        }


        public IConfigurationRoot Configuration { get; }

        public BWS(IHostingEnvironment env)
        {
            //string sContentRoot = BMSCommon.Common.GetFolder("wwwroot");

            BMSCommon.Database.msContentRootPath = env.ContentRootPath;

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {


            //session info
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60*8);//We set Time here 
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            // end of session area
            services.Configure<SmartSettings>(Configuration.GetSection(SmartSettings.SectionName));
            services.AddSingleton(s => s.GetRequiredService<IOptions<SmartSettings>>().Value);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given 
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTransient<IEmailSender, EmailSender>();

            //services.AddControllersWithViews();
            services.AddControllersWithViews(options => {
                options.Filters.Add<BiblePay.BMS.BWS.ViewBagActionFilter>();
            });

            //services.AddRazorPages();
            //services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddRazorPages(); //1
            services.AddServerSideBlazor();//2
            
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });


            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));


            services.AddMvc(options =>
            {
                ServiceProvider serviceProvider = services.BuildServiceProvider();
            });


            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder.WithOrigins(new string[] { "https://dec.app", "https://hitch.social", "https://www.hitch.social",
                    "https://social.biblepay.org", "http://social.biblepay.org", "https://localhost:8443", "https://*", "http://localhost", "https://localhost" })
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
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

            //SmartWeb changes:
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseStaticFiles();
            app.UseRouting();
          
            app.UseCors("CorsPolicy");

            app.UseSession();

            /*
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            */

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=BMS}/{action=Status}");
                endpoints.MapRazorPages();
                endpoints.MapBlazorHub(); //3
            });


            app.UseHalfordFileProvider();
            try
            {
                var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
                string address = GetFirst(serverAddressesFeature.Addresses).ToString();
                int iPort = GetPort(address);

                string sWWWRoot = BMSCommon.Common.GetFolder("video");
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(sWWWRoot),
                    RequestPath = "/video"
                });
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(BMSCommon.Common.GetFolder("sql")),
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
            catch (Exception)
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
            string Sourcepath = Path.Combine(GetFolder(""), sReqPath);

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



        private static long GetFileSizeFromRemoteURL(string sCDNURL, string sResource)
        {
            // This function only works on our nodes...
            string sFull = sCDNURL + "/BMS/GetFileSize?name=" + sResource;
            long nSz = (long)DSQL.Sync.GetShardSize(sFull);
            return nSz;
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

                if (sourcepath.Contains("/BMS/StaticVideoPlayer"))
                {
                    string sSource = context.Request.Query["id"];
                    string sPage = "https://globalcdn.biblepay.org:8443/video/staticplayer/bbp/staticplayer.htm";
                    string html = BMSCommon.Common.ExecuteMVCCommand(sPage);
                    string sBindURL = BMSCommon.Common.GetConfigurationKeyValue("bindurl");
                    string sURL = sBindURL + "/video/" + sSource + "/1.m3u8";
                    html = html.Replace("{thesource}", sURL);
                    await context.Response.WriteAsync(html);
                    return;
                }
                else if (sourcepath.Contains("/BMS/GetDirectoryContents"))
                {
                    try
                    {
                        string sKey = context.Request.Query["key"].ToString();
                        string sNN = "";
                        string sUID = BBPTestHarness.Service.ValidateKey(sKey, out sNN);
                        if (sUID == "")
                        {
                            throw new Exception("API Key invalid.  To obtain a key, go to unchained.biblepay.org | Wallet.");
                        }
                        List<string> s = BMSCommon.DSQL.QueryIPFSFolderContents(BiblePay.BMS.DSQL.UI.IsTestNet(context),"", "", sKey);
                        string sJson3 = Newtonsoft.Json.JsonConvert.SerializeObject(s);
                        //mission critical: test the video display with await
                        await context.Response.WriteAsync(sJson3);
                    }
                    catch (Exception ex)
                    {
                        Log("gdi" + ex.Message);
                        await context.Response.WriteAsync("ERROR");
                    }
                }
                else if (sourcepath.Contains("/BMS/TestFile"))
                {
                    string sPath = "c:\\code\\testbed\\90.mp4";
                    context.Response.ContentType = "video/mp4";
                    await context.Response.SendFileAsync(sPath);
                    return;
                }
                else if (sourcepath.Contains("/BMS/ValidateKey"))
                {
                    try
                    {
                        string sKey = context.Request.Query["key"].ToString();
                        string sNN = "";
                        string sUID = BBPTestHarness.Service.ValidateKey(sKey, out sNN);
                        if (sUID == "")
                        {
                            throw new Exception("API Key invalid.  To obtain a key, go to unchained.biblepay.org | Wallet.");
                        }
                        BiblePay.BMS.Controllers.BMSController.UnchainedReply u = new Controllers.BMSController.UnchainedReply();
                        u.userid = sUID;
                        string sJson3 = Newtonsoft.Json.JsonConvert.SerializeObject(u);
                        await context.Response.WriteAsync(sJson3);
                    }
                    catch (Exception ex)
                    {
                        Log("ValidateKey" + ex.Message);
                        await context.Response.WriteAsync("ERROR");
                    }

                }
                else if (sourcepath.Contains("/BMS/PlayVideos"))
                {
                    string sFolder = GetFolder("video");

                    DirectoryInfo d = new DirectoryInfo(sFolder);
                    DirectoryInfo[] dis = d.GetDirectories();
                    string sBindURL = BMSCommon.Common.GetConfigurationKeyValue("bindurl");
                    string sHTML = "<html><h3>Sanctuary Videos</h3><br><br>";
                    foreach (DirectoryInfo di in dis)
                    {
                        string sSubFolder = di.Name;
                        if (sSubFolder.Length > 8)
                        {
                            string sMainFile = di.FullName + "/1.m3u8";
                            if (System.IO.File.Exists(sMainFile))
                            {
                                string sID = sSubFolder;
                                string sURL = sBindURL + "/BMS/StaticVideoPlayer?id=" + sID;
                                string sRow = "<a href='" + sURL + "'>Play " + sID + "</a><br>\r\n";
                                sHTML += sRow;
                            }
                        }
                    }
                    sHTML += "</html>";
                    await context.Response.WriteAsync(sHTML);
                    return;
                }
                else if (sourcepath.Contains("/video") || sourcepath.Contains("/upload") || sourcepath.Contains("/wwwroot") || sourcepath.Contains("/shard") || sourcepath.Contains("/database") || sourcepath.Contains("/broadcast"))
                {
                    // Case 1: A web resource
                    FileInfo fi = GetFileInfo(sourcepath);

                    if (fi == null)
                    {
                        if (true)
                        {
                            for (int i = 0; i < BMSCommon.API.mNodes.Count; i++)
                            {
                                BMSCommon.API.Node n = BMSCommon.API.mNodes[i];
                                if (!n.IsMine && n.FullyQualified)
                                {
                                    string sURL = BMSCommon.Common.NormalizeURL(n.FullyQualifiedDomainName + "/" + sourcepath);
                                    long iSz = GetFileSizeFromRemoteURL(n.FullyQualifiedDomainName, sourcepath);
                                    if (iSz > 0)
                                    {
                                        Log("Redirecting to sanc " + n.FullyQualifiedDomainName + " for " + sourcepath);
                                        context.Response.Redirect(sURL);
                                        return;
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
                        }
                        // If we reach here, this is our last resort.  This means every sanc is missing the file (or this is really a 404).
                        // Try to get the file from IPFS next:
                        string sNewURL = BMSCommon.Common.NormalizeURL("https://bbpipfs.s3.filebase.com/" + sourcepath);

                        Log("Redir to ipfs " + sourcepath);
                        context.Response.Redirect(sNewURL);
                        string sTargetPath = ReqPathToFilePath(sourcepath);
                        PullDown(sNewURL, sTargetPath);
                        return;
                        // If not, send them a 404:
                        // We dont actually hit this line, because if the file doesnt exist, the last leg will return 404.. Leaving this in for historical reasons.
                        // context.Response.StatusCode = 404;
                    }
                    else
                    {
                        // File actually exists locally:

                        await context.Response.SendFileAsync(fi.FullName);

                    }
                }
                else if (sourcepath == "/")
                {
                    // The web root
                    context.Response.Redirect("/gospel/about");
                }
                else if (sourcepath.Contains("/favicon.ico"))
                {
                    context.Response.StatusCode = 404;
                    return;
                    /*
                    string sReqPath = GetFolder("images1", "favicon.png");
                    await context.Response.SendFileAsync(sReqPath);
                    */
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
            catch (Exception ex)
            {
                // cancelled task?
                BMSCommon.Common.Log("BWS::WebServer::" + ex.Message);
            }
        }
    }
}


