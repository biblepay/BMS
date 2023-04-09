using BBPAPI.Model;
using BiblePay.BMS.Extensions;
using BiblePay.BMS.Models;
using BMSCommon;
using BMSCommon.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static BiblePay.BMS.DSQL.Chat;
using static BiblePay.BMS.DSQL.SessionHelper;
using static BiblePay.BMS.DSQL.UI;
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
                    controller.ViewBag.Chain = GetChain(controller.HttpContext);
                    controller.ViewBag.ChainColor = GetChainColor(controller.HttpContext);
                    controller.ViewBag.LoginStatus = GetLogInStatus(controller.HttpContext);
                    controller.ViewBag.BioURL = GetBioURL(controller.HttpContext);
                    controller.ViewBag.Balance = GetAvatarBalance(controller.HttpContext, false);
                    controller.ViewBag.NotificationCountHR = GetNotificationCountHR(controller.HttpContext);
                    controller.ViewBag.NotificationCount = GetNotificationCount(controller.HttpContext);
                    controller.ViewBag.LoginAction = GetLogInAction(controller.HttpContext);
                    controller.ViewBag.NickName = GetNickName(controller.HttpContext);
                    User u = controller.HttpContext.GetCurrentUser();
                    controller.ViewBag.Notifications = GetNotifications0(controller.HttpContext, u.ERC20Address);
                    // You have access to the httpcontext & route in controller.HttpContext & controller.RouteData
                }
                base.OnResultExecuting(context);
            }
        }

        public IConfigurationRoot Configuration { get; }

        [Obsolete]
        public BWS(IHostingEnvironment env)
        {
            Global.msContentRootPath = env.ContentRootPath;

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
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(60*8);
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
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

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddControllersWithViews(options => {
                options.Filters.Add<BiblePay.BMS.BWS.ViewBagActionFilter>();
            });
            
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddRazorPages(); 
            
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
                builder.WithOrigins(new string[] { 
                    "https://social.biblepay.org", 
                    "https://localhost:8443", "https://*", "http://localhost", "https://localhost" })
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
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=BMS}/{action=Status}");
                endpoints.MapRazorPages();
            });

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

        // BiblePay Decentralized Web Server
        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Reserved: When we resolve CDN nickname to URI:
                string sourcepath = context.Request.Path;
                if (sourcepath.Contains("/BMS/StaticVideoPlayer"))
                {
                    string sSource = context.Request.Query["id"];
                    string sPage = "https://globalcdn.biblepay.org:8443/video/staticplayer/bbp/staticplayer.htm";
                    string html = ExecuteMVCCommand(sPage);
                    string sBindURL = String.Empty;
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
                        string sNickName = String.Empty;
                        string sUID = "0"; // BBPTestHarness.Service.ValidateKey(sKey, out sNN);
                        if (sUID == "0")
                        {
                            throw new Exception("API Key invalid.  To obtain a key, go to unchained.biblepay.org | Wallet.");
                        }
                        List<string> s = new List<string>();
                        string sJson3 = Newtonsoft.Json.JsonConvert.SerializeObject(s);
                        await context.Response.WriteAsync(sJson3);
                    }
                    catch (Exception ex)
                    {
                        Log("gdi" + ex.Message);
                        await context.Response.WriteAsync("ERROR");
                    }
                }
                else if (sourcepath.Contains("/BMS/ValidateKey"))
                {
                    try
                    {
                        string sKey = context.Request.Query["key"].ToString();
                        string sNickName = String.Empty;
                        string sUID = "0";// BBPTestHarness.Service.ValidateKey(sKey, out sNN);
                        if (sUID == "0")
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
                    string sBindURL = "";//("bindurl");
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
                else if (sourcepath.Contains("/video") || sourcepath.Contains("/upload/") || sourcepath.Contains("/wwwroot") || sourcepath.Contains("/shard") || sourcepath.Contains("/database") || sourcepath.Contains("/broadcast"))
                {
                    // Case 1: A web resource
                    FileInfo fi = GetFileInfo(sourcepath);

                    if (fi == null)
                    {
                        double nConfigType = 1;
                        if (nConfigType == 1)
                        {
                            // Use storj instead
                            sourcepath = System.Web.HttpUtility.UrlDecode(sourcepath);
                            string sSourceKey = "BB2BwSbDCqCqNsfc7FgWFJn4sRgnUt4tsM" + sourcepath;
                            string sTP = ReqPathToFilePath(sourcepath);
                            bool fSucc = await BBPAPI.StorjIO.StorjDownloadLg(sSourceKey, sTP);
                            Log("Finished pulling " + fSucc.ToString() + " " + sourcepath);
                            fi = GetFileInfo(sourcepath);
                        }
                        
                        if (fi != null)
                        {
                            await context.Response.SendFileAsync(fi.FullName);
                        }
                        string sTargetPath = ReqPathToFilePath(sourcepath);
                        return;
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
                Log("BWS::WebServer::" + ex.Message);
            }
        }
    }
}


