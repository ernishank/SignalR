using Microsoft.AspNet.SignalR.Hubs;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Timers;
using System.Text;

namespace SignalRApp
{
    [HubName("SignalRWeather")]
    public class SignalRChat : Hub
    {
        public static List<UserConnection> _listUserConnetion = new List<UserConnection>();
        public bool _firstInit = true;

        public SignalRChat()
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1000;
            aTimer.Enabled = true;
            _firstInit = false;
            aTimer.Start();
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (!_firstInit)
                SendData(Context.ConnectionId);
        }
        public override Task OnConnected()
        {
            var us = new UserConnection();
            us.UserName = "Nishank"; // Context.Request.QueryString["UserName"].ToString();
            us.ConnectionID = Context.ConnectionId;
            _listUserConnetion.Add(us);
            if (_listUserConnetion != null)
            {
                SendData(us.ConnectionID);
            }
            return base.OnConnected();
        }

        //Takes near around 35 seconds to call onDisconnect
        public override Task OnDisconnected(bool stopCalled)
        {
            if (!stopCalled)
            {
                var userToBeRemove = _listUserConnetion.Find(o => o.ConnectionID == Context.ConnectionId);
                _listUserConnetion.Remove(userToBeRemove);
            }
            return base.OnDisconnected(false);
        }

        public override Task OnReconnected()
        {
            var checkUserIsExists = _listUserConnetion.Find(o => o.ConnectionID == Context.ConnectionId);
            if (checkUserIsExists != null)
            {
                SendData(checkUserIsExists.ConnectionID);
            }
            return base.OnReconnected();
        }

        public void SendData(string ConnectionId)
        {
            var user = _listUserConnetion.Where(o => o.ConnectionID == ConnectionId);
            if (user.Any())
            {
                Clients.Client(user.First().ConnectionID).sendMessage(FetchData());
            }
        }

        public List<Student> FetchData()
        {
            int randNum = RandomNumber(1, 100);
            string randStr = RandomString(10, true);
            List<Student> studentList = new List<Student>() { 
                new Student(){ Id=randNum+1, Name= randStr+"_A"},
                new Student(){ Id=randNum+2, Name=randStr+"_B"},
                new Student(){ Id=randNum+3, Name=randStr+"_C"},
                new Student(){ Id=randNum+4, Name=randStr+"_D"}
            };
            return studentList;
        }

        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        // Generate a random string with a given size  
        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
    }



}