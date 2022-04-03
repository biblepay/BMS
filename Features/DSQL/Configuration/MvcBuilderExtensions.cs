using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BiblePay.BMS
{
    public static class MvcBuilderExtensions
    {
        /// Finds all the types that are <see cref="Controller"/> or <see cref="FeatureController"/>and add them to the Api as services.
        /// <returns>The Mvc builder</returns>
        public static IMvcBuilder AddControllers(this IMvcBuilder builder, IServiceCollection services)
        {
            // Adds Controllers with API endpoints
            System.Collections.Generic.IEnumerable<ServiceDescriptor> controllerTypes = services.Where(s => s.ServiceType.GetTypeInfo().BaseType == typeof(Controller));
            foreach (ServiceDescriptor controllerType in controllerTypes)
            {
                builder.AddApplicationPart(controllerType.ServiceType.GetTypeInfo().Assembly);
            }
      
            //builder.AddApplicationPart(typeof(Controllers.NodeController).Assembly);
            builder.AddControllersAsServices();
            return builder;
        }
    }
}
