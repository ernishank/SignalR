using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ExcelOperations
{
    class Program
    {
        Program()
        {
            Timer aTimer = new Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1000;
            aTimer.Enabled = true;
            aTimer.Start();
        }


        static string _FilePath = @"C:\Users\n.maraiya\Documents\visual studio 2015\Projects\Excel\ExcelData.xlsx";
        static string _ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + _FilePath + ";Extended Properties=\"Excel 12.0;ReadOnly=False;HDR=Yes;\"";
        static void Main(string[] args)
        {
            Program p = new Program();
            Console.Read();
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            InsertData();
        }

        public static void InsertData()
        {
            try
            {
                //getting the path of the file     

                //connection string for that file which extantion is .xlsx    
                //making query    

                string query = "INSERT INTO [Sheet1$] ([Id], [Name]) VALUES(" + Id() + ",'" + RandomString(10, true) + "')";
                //Providing connection    
                using (OleDbConnection conn = new OleDbConnection(_ConnectionString))
                {
                    //checking that connection state is closed or not if closed the     
                    //open the connection    
                    if (conn.State == System.Data.ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    //create command object    
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            Console.WriteLine("Data inserted successfully");
                        }
                        else
                        {
                            Console.WriteLine("Data insertion failed");
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("====================================================");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("====================================================");
            }
        }

        public static int Id()
        {

            string Command = "SELECT max(ID) FROM [Sheet1$]";
            int Id = 0;
            try
            {
                using (OleDbConnection conn = new OleDbConnection(_ConnectionString))
                {
                    using (OleDbCommand cmd = new OleDbCommand(Command, conn))
                    {
                        conn.Open();
                        using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                        {
                            Id = Convert.ToInt32(cmd.ExecuteScalar() == DBNull.Value ? 0 : cmd.ExecuteScalar());
                            //DataSet id = new DataSet();
                            //da.Fill(id);

                            //DataTable idtable = id.Tables[0];
                            //idtable.DefaultView.Sort = idtable.Columns[0].ColumnName + " " + "DESC";
                            //idtable = idtable.DefaultView.ToTable();
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("====================================================");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("====================================================");
            }
            return Id + 1;
        }

        public static string RandomString(int size, bool lowerCase)
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
