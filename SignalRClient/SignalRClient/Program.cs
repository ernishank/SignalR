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
        public static string _SignalRUrl = @"http://localhost:7424/";
        static IHubProxy _stockTickerHubProxy = null;
        static void Main(string[] args)
        {
            Console.Write("Please enter username: ");
            string UserName = Console.ReadLine();

            //Register hubproxy for a server side hub
            //pass URL, QueryString parameters and Default URL boolean (/signalr is the default url root value)
            var hubConnection = new HubConnection(_SignalRUrl, string.Format("UserName={0}", UserName), true);

            //Configure the HubName
            _stockTickerHubProxy = hubConnection.CreateHubProxy("SignalRDataApp");

            //Bind statechange event with hub connection
            hubConnection.StateChanged += hubConnection_StateChanged;

            //Invoke the SignalR server side method
            _stockTickerHubProxy.On<List<StockExchange>>("sendMessage", SendMessage);

            //start the hubconnection
            hubConnection.Start();
            Console.Read();
        }

        private static void hubConnection_StateChanged(StateChange obj)
        {
            if (obj.NewState == ConnectionState.Connected)
            {
                Console.WriteLine("Connected..");

            }
            else if (obj.NewState == ConnectionState.Connecting)
            {
                Console.WriteLine("Connecting..");
            }
            else if (obj.NewState == ConnectionState.Disconnected)
            {
                Console.WriteLine("Disconnected");
            }
            else
            {
                Console.WriteLine("Reconnecting");
            }
        }

        private static void SendMessage(List<StockExchange> obj)
        {
            if (obj != null && obj.Count > 0)
            {
                foreach (var i in obj)
                {
                    Console.WriteLine("Id: {0}, Company Name: {1}, Price: {2}", i.Id, i.Name, i.Price);
                }
                Console.WriteLine("===================================================================");
            }
        }






    }
}
