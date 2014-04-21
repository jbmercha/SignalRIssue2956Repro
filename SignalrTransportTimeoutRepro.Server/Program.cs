using System;
using System.Net;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;
using Microsoft.Owin.Cors;

[assembly: OwinStartup(typeof(SignalRSelfHost.Startup))]

namespace SignalRSelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            const string url = "http://localhost:8080";
            using (WebApp.Start(url))
            {
                Console.WriteLine("Server running on {0}", url);
                Console.ReadLine();
            }
        }
    }
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(12);
            app.UseCors(CorsOptions.AllowAll);
            var listener = (HttpListener)app.Properties[typeof(HttpListener).FullName];
            listener.AuthenticationSchemes = AuthenticationSchemes.Ntlm;
            
            app.MapSignalR();
        }
    }
    public class MyHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }
    }
}