using System;
using System.Text;
using System.Timers;
using Microsoft.Extensions.Logging;

namespace BiblePay.BMS
{
    /// <summary>
    /// Configuration related to the API interface.
    /// </summary>
    public class ApiSettings
    {
        private readonly ILogger logger;
        public Uri ApiUri { get; set; }
        public int ApiPort { get; set; }
        public Timer KeepaliveTimer { get; private set; }
        /// <param name="nodeSettings">The node configuration.</param>
        public ApiSettings()
        {
            this.logger.LogTrace("(-)");
        }
    }
}
