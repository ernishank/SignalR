using System;
using System.Data.OleDb;
using System.Timers;

namespace ExcelOperations
{
    class Program
    {
        //Excel sheet file path
        static string _FilePath = @"D:\Auth\SignalR\SignalR\Excel\ExcelData.xlsx";
        //ConnectionString for OleDB connection purpose
        static string _ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + _FilePath + ";Extended Properties=\"Excel 12.0;ReadOnly=False;HDR=Yes;\"";

        /// <summary>
        /// Default constructor
        /// </summary>
        Program()
        {
            //Initializing timer 
            Timer aTimer = new Timer();
            //Binding timer event on elapse
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            //timer interval
            //will tick in every second
            aTimer.Interval = 1000;
            //starting timer
            aTimer.Start();
        }


     
        static void Main(string[] args)
        {
            //Creating object to execute/call default constructor
            Program objProgram = new Program();
            Console.Read();
        }

        //In every one second we will insert the data
        //in Excel Sheet
        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            InsertData();
        }

        public static void InsertData()
        {
            try
            {
                //making query    
                string query = "INSERT INTO [Sheet1$] ([Id], [CompanyName], [Price]) VALUES(" + Id() + ",'" + CompanyName() + "'," + Price() + ")";
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
                Console.WriteLine(ex.Message);
                Console.WriteLine("====================================================");
            }
        }

        //Generating Id based on the maximum Id in ExcelSheet
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
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("====================================================");
                Console.WriteLine(ex.Message);
                Console.WriteLine("====================================================");
            }
            return Id + 1;
        }

        //Initialing an array of Company
        //Based on this array, will return random Company Name
        //to insert in Excel Sheet
        public static string CompanyName()
        {
            // A array of Company Name  
            string[] companyName = { "Apple", "Microsoft", "Alphabet", "GE", "Reliance", "Facebook", "Twitter", "Goldman Sachs", "Netflix", "Amazon", "Nescafe", "Tata", "TCS", "Infosys", "Wipro", "Prowareness" };
            int index = 0;
            try
            {
                // Create a Random object  
                Random rand = new Random();
                // Generate a random index less than the size of the array.  
                index = rand.Next(companyName.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            // Return company name.  
            return companyName[index];
        }

        //Generating random price number
        private static double Price()
        {
            double minValue = 100;
            double maxValue = 1000;
            // Create a Random object  
            Random rand = new Random();
            var next = rand.NextDouble();

            return Math.Round(minValue + (next * (maxValue - minValue)));
        }
    }
}
