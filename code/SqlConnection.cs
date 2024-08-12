using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inns
{
    public class SqlConnection
    {

        public MySqlConnection connection = new MySqlConnection("server = 192.168.1.37; port = 3306; user = testotel; database = testotel; password = 1234");
        //public MySqlConnection connection = new MySqlConnection("server = 192.168.3.11; port = 3306; database = otel; user = otel; password = otel1234");
        //public MySqlConnection connection = new MySqlConnection("Server=localhost; Database=otel; port=3306; UserId=root; Password=qwer1234");

        public void SqlOpen()
        {
            connection.Open();
        }
        public void SqlClosing()
        {
            connection.Close();
        }
    }
}
