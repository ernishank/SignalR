using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var hubConnection = new HubConnection("http://localhost:7424/signalr");
            IHubProxy stockTickerHubProxy = hubConnection.CreateHubProxy("SignalRWeather");
            hubConnection.StateChanged += hubConnection_StateChanged;
            stockTickerHubProxy.On<List<Student>>("sendMessage", SendMessage);
            hubConnection.Start();
            Console.Read();
            //ServicePointManager.DefaultConnectionLimit = 10;
        }

        private static void hubConnection_StateChanged(StateChange obj)
        {
            if (obj.NewState == ConnectionState.Connected)
            {

            }
            else if (obj.NewState == ConnectionState.Connecting)
            {

            }
            else if (obj.NewState == ConnectionState.Disconnected)
            {

            }
            else
            {

            }
        }

        private static void SendMessage(List<Student> obj)
        {
            foreach (var i in obj)
            {
                Console.WriteLine("Name: {0}, Id: {1}", i.Name, i.Id);
            }
            Console.WriteLine("===================================================================");
        }






    }
}
