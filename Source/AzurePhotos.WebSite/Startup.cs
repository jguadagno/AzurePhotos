using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AzurePhotos.WebSite.Startup))]
namespace AzurePhotos.WebSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
        }
    }
}
