using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace FluffySpoon.AspNet.LetsEncrypt.Sample
{
	public class Program
	{
		public const string DomainToUse = "*.cdn.biblepay.org";
		public const string DomainToListen = "a.cdb.biblepay.org";

		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.ConfigureLogging(l => l.AddConsole(x => x.IncludeScopes = true))
				.UseKestrel(kestrelOptions =>
				{
					kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
					{
						httpsOptions.ServerCertificateSelector = (c, s) => LetsEncryptRenewalService.Certificate;
					});
				})
				.UseUrls(
					"http://" + DomainToListen,
					"https://" + DomainToListen)
				.UseStartup<Startup>();
	}
}
