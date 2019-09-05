using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace SimpleCodeExample
{
    class Program
    {
        
            static void Main(string[] args)
            {
                // Simple code example. ( j. phillips ) - This code has not been tested. I banged it out quickly for an example.

                // No ORM or tiered architecture ( data layer , mvc ) etc. 
                // 
                // This sample takes rows from a table at night ( service app ) 
                // changes processed rows with todays date to TRUE.
                //
                // Normally the GetTodaysOrders, ProcessOrders would be in a WebService, Datalayer( mvc or n-tier )
                // which would support Readability and Scalability. 

                List<OrderTable> lstOrders = new List<OrderTable>();

                lstOrders = GetTodaysOrders();
                foreach (OrderTable ot in lstOrders)
                {
                    if (ot.OrderDate.Equals(DateTime.Now)) // <-- DateTime Issue ( timestamp ).
                    {
                        ProcessOrders(ot);
                    }
                }

            }


            public static List<OrderTable> GetTodaysOrders()
            {
                // This is the generic way to access 
                // a database. Usually this code would be placed in a tier by itself.
                // ( ORM, mvc etc for example )

                List<OrderTable> lstCurrentOrders = new List<OrderTable>();
                using (SqlConnection conn = new SqlConnection("somedb;password;"))
                {
                    StringBuilder strSql = new StringBuilder();
                    OrderTable ot = new OrderTable();
                    strSql.Append("SELECT * from OrderTbl WHERE OrderDate=");
                    strSql.Append(DateTime.Now.ToString());

                    conn.Open();
                    SqlCommand cmd = new SqlCommand(strSql.ToString(), conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ot = new OrderTable();
                        ot.OrderID = Convert.ToInt32(reader["OrderID"]);
                        ot.AccountID = Convert.ToInt32(reader["AccountID"]);
                        ot.OrderDate = Convert.ToDateTime(reader["OrderDate"]);
                        ot.Processed = Convert.ToBoolean(reader["Processed"]);
                        lstCurrentOrders.Add(ot);
                    }
                    conn.Close();
                }
                return lstCurrentOrders;
            }

            public static void ProcessOrders(OrderTable ot)
            {
                // Updates Order table ( Usually this would be in a tier ( n-tier or mvc = datalayer )

                // More order processing here //

                // Update Order status ( Usually this would be OrderStatus field but for readability )
                using (SqlConnection conn = new SqlConnection("somedb;password;")) // <-- Using Stmt SCOPE ..Connection to garbage.
                {
                    SqlCommand cmd = new SqlCommand("Update OrderTbl Set Processed=@parmProcessed where OrderID=@parmOrderID");
                    cmd.Parameters.Add("@parmProcessed", SqlDbType.Bit).Value = true;
                    cmd.Parameters.Add("@parmOrderID", SqlDbType.Int).Value = ot.OrderID;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public class OrderTable
        {
            // OrderID identity column 
            // A better example would be to have private variables and accessors to 
            // support encapsulation ( Since this is an example I'm not implementing 
            // that .. makes it easier to browse example ).

            public int OrderID { get; set; }
            public int AccountID { get; set; }
            public DateTime OrderDate { get; set; }
            public bool Processed { get; set; }

        }
    
}
