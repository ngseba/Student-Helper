using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(studentApp2.Startup))]
namespace studentApp2
{
    public partial class Startup
    {
        private object services;

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
