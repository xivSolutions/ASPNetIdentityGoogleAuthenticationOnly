using Owin;

namespace ASPNetIdentityGoogleAuthenticationOnly
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
