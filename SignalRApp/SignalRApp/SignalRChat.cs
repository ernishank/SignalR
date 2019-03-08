using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Timers;
using System.Text;
using SignalRApp.Logs;
using System.Data.OleDb;
using System.Data;

namespace SignalRApp
{
    [HubName("SignalRDataApp")]
    public class SignalRChat : Hub
    {
        public static List<UserConnection> _listUserConnetion = new List<UserConnection>();
        public bool _firstInit = true;

        public SignalRChat()
        {
            Timer aTimer = new Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 5000;
            aTimer.Enabled = true;
            _firstInit = false;
            aTimer.Start();
        }

        #region TimerEvent
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (!_firstInit)
                SendData(Context.ConnectionId);
        }
        #endregion TimerEvent

        #region SignalR LifeCycle
        public override Task OnConnected()
        {
            var us = new UserConnection();
            us.UserName = Context.Request.QueryString["UserName"].ToString();
            us.ConnectionID = Context.ConnectionId;
            _listUserConnetion.Add(us);
            if (_listUserConnetion != null)
            {
                WriteLogFile.WriteLog(string.Format("SignalR Connected for Username: {0}, ConntectionId: {1}, User count: {2}", us.UserName, us.ConnectionID, _listUserConnetion.Count));
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
                WriteLogFile.WriteLog(string.Format("SignalR disconneted for Username: {0}, ConntectionId: {1}, User count: {2}", userToBeRemove.UserName, userToBeRemove.ConnectionID, _listUserConnetion.Count));
            }
            return base.OnDisconnected(false);
        }

        public override Task OnReconnected()
        {
            var checkUserIsExists = _listUserConnetion.Find(o => o.ConnectionID == Context.ConnectionId);
            if (checkUserIsExists != null)
            {
                WriteLogFile.WriteLog(string.Format("Trying to reconnect SignalR for Username: {0}, ConntectionId: {1}, User count: {2}", checkUserIsExists.UserName, checkUserIsExists.ConnectionID, _listUserConnetion.Count));
                SendData(checkUserIsExists.ConnectionID);
            }
            return base.OnReconnected();
        }
        #endregion SignalR LifeCycle

        public void SendData(string ConnectionId)
        {
            var user = _listUserConnetion.Where(o => o.ConnectionID == ConnectionId);
            if (user.Any())
            {
                //Using the Clients property of IHubCallerConnectionContext from hub abstract clas
                Clients.Client(user.First().ConnectionID).sendMessage(FetchData());
            }
        }

        #region Bind Data
        public List<Student> FetchData()
        {
            OleDbConnection conn = null;
            List<Student> listStudent = new List<Student>();
            string FilePath = @"C:\Users\n.maraiya\Documents\visual studio 2015\Projects\Excel\ExcelData.xlsx";
            string ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FilePath + ";Extended Properties=\"Excel 12.0;ReadOnly=False;HDR=Yes;\"";
            string Command = "SELECT TOP 4 ID, Name FROM [Sheet1$] ORDER BY ID Desc";
            try
            {
                using (conn = new OleDbConnection(ConnectionString))
                {
                    using (OleDbCommand cmd = new OleDbCommand(Command, conn))
                    {
                        conn.Open();
                        using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                        {
                            DataSet id = new DataSet();
                            da.Fill(id);

                            DataTable idtable = id.Tables[0];

                            listStudent = (from s in idtable.AsEnumerable()
                                           select new Student
                                           {
                                               Id = Convert.ToInt32(s["ID"].ToString()),
                                               Name = s["Name"].ToString()
                                           }).ToList();

                            return listStudent;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLogFile.WriteLog(ex.Message);
            }
            finally
            {
                if (conn != null || conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return listStudent;
        }
        #endregion Bind Data
    }



}