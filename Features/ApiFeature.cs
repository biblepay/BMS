using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Stratis.Bitcoin.Features.Api.Unknown
{
    /// <summary>
    /// Provides an Api to the full node
    /// </summary>
    /// change to sealed when done coding
    public class ApiFeature 
    {
        /// <summary>How long we are willing to wait for the API to stop.</summary>
        private const int ApiStopTimeoutSeconds = 10;
        
        private readonly ApiSettings apiSettings;

        private readonly ApiFeatureOptions apiFeatureOptions;

        private readonly ILogger logger;

        private IWebHost webHost;

        public ApiFeature(
            ApiFeatureOptions apiFeatureOptions,
            ApiSettings apiSettings,
            ILoggerFactory loggerFactory)
        {
            this.apiFeatureOptions = apiFeatureOptions;
            this.apiSettings = apiSettings;
           // this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        public void Initialize()
        {
            this.logger.LogInformation("API starting on URL '{0}'.", this.apiSettings.ApiUri);
            //this.webHost = Program.Initialize(this.fullNodeBuilder.Services, this.fullNode, this.apiSettings);

            // Start the keepalive timer, if set.
            // If the timer expires, the node will shut down.
            if (this.apiSettings.KeepaliveTimer != null)
            {
                this.apiSettings.KeepaliveTimer.Elapsed += (sender, args) =>
                {
                    this.logger.LogInformation($"The application will shut down because the keepalive timer has elapsed.");

                    this.apiSettings.KeepaliveTimer.Stop();
                    this.apiSettings.KeepaliveTimer.Enabled = false;
                    
                };

                this.apiSettings.KeepaliveTimer.Start();
            }
        }

        /// <summary>
        /// Prints command-line help.
        /// </summary>
        /// <param name="network">The network to extract values from.</param>
        

        /// <summary>
        /// Get the default configuration.
        /// </summary>
        /// <param name="builder">The string builder to add the settings to.</param>
        /// <param name="network">The network to base the defaults off.</param>
       

        /// <inheritdoc />
        public void Dispose()
        {
            // Make sure the timer is stopped and disposed.
            if (this.apiSettings.KeepaliveTimer != null)
            {
                this.apiSettings.KeepaliveTimer.Stop();
                this.apiSettings.KeepaliveTimer.Enabled = false;
                this.apiSettings.KeepaliveTimer.Dispose();
            }

            // Make sure we are releasing the listening ip address / port.
            if (this.webHost != null)
            {
                this.logger.LogInformation("API stopping on URL '{0}'.", this.apiSettings.ApiUri);
                this.webHost.StopAsync(TimeSpan.FromSeconds(ApiStopTimeoutSeconds)).Wait();
                this.webHost = null;
            }
        }
    }

    public sealed class ApiFeatureOptions
    {
    }

    /// <summary>
    /// A class providing extension methods for <see cref="IFullNodeBuilder"/>.
    /// </summary>
    public static class ApiFeatureExtension
    {
        
    }
}
