using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(hunger_solver.App_Start.StartUp))]
namespace hunger_solver.App_Start
{
    public partial class StartUp
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}