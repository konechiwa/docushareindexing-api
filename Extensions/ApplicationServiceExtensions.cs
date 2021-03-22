using DocuShareIndexingAPI.Interface;
using DocuShareIndexingAPI.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace DocuShareIndexingAPI.Extensions
{

    /**
    * @notice the static class for add application services.
    */
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            services.AddScoped<ITokenService, TokenService>();
            
            return services;
        }
        
    }
}