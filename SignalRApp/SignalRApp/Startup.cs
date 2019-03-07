using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SignalRApp.Startup))]

namespace SignalRApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Branch the pipeline here for requests that start with "/signalr"
            app.Map("/signalr", map =>
            {
                map.RunSignalR();
            });
        }
    }
}
