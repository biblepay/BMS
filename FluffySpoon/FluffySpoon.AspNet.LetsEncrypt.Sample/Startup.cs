using System;
using System.IO;
using Certes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace FluffySpoon.AspNet.LetsEncrypt.Sample
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddFluffySpoonLetsEncryptRenewalService(new LetsEncryptOptions()
			{
				Email = "rob@biblepay.org",
				UseStaging = false,
				Domains = new[] { Program.DomainToUse },
				TimeUntilExpiryBeforeRenewal = TimeSpan.FromDays(30),
				CertificateSigningRequest = new CsrInfo()
				{
					CountryName = "USA",
					Locality = "Dallas",
					Organization = "BiblePay",
					OrganizationUnit = "DSQL",
					State = "TX"
				}
			});

			services.AddFluffySpoonLetsEncryptFileCertificatePersistence();
			services.AddFluffySpoonLetsEncryptFileChallengePersistence();
            services.AddFluffySpoonLetsEncryptMemoryChallengePersistence();

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseFluffySpoonLetsEncryptChallengeApprovalMiddleware();
            /*
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @".well-known")),
                RequestPath = new PathString("/.well-known"),
                ServeUnknownFileTypes = true // serve extensionless file
            });
            */

            app.Run(async (context) =>
			{
                string resp = "certificate-generator";

                await context.Response.WriteAsync(resp);
			});
		}
	}
}
