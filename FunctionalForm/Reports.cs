using Inns.FunctionalForm;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inns.UserForm
{
    public partial class Reports : Form
    {
        public Reports()
        {
            InitializeComponent();
        }
        SqlConnection connect;
        private void Reports_Load(object sender, EventArgs e)
        {
            connect = new SqlConnection();
        }

        //выводит график
        private void buttonBuild_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            DateTime dateStart = dateTimePicker1.Value;
            DateTime dateOut = dateTimePicker2.Value;
            connect.SqlOpen();
            MySqlCommand mySqlCommand = new MySqlCommand("CALL Booking_schedule", connect.connection);
            MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
            DataTable dt = new DataTable();
            mySqlDataAdapter.Fill(dt);
            connect.SqlClosing();
            //список введённых дат
            List<DateTime> listTextBox = new List<DateTime>();
            TimeSpan difference = dateOut.Date.Subtract(dateStart.Date);
            int colDate = difference.Days;
            for (int i = 0; i <= colDate; i++)
            {
                listTextBox.Add(dateStart.AddDays(i));
            }
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            //список дат из БД
            List<DateTime> dateChart = new List<DateTime>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sd = dt.Rows[i][0].ToString();
                string ed = dt.Rows[i][1].ToString();
                startDate = DateTime.Parse(sd);
                endDate = DateTime.Parse(ed);

                for (DateTime date2 = startDate; date2 <= endDate; date2 = date2.AddDays(1))
                {
                    dateChart.Add(date2);
                }
            }
            //Цикл дат для графика
            for (int i = 0; i < listTextBox.Count; i++)
            {
                int reservation = 0;
                for (int j = 0; j < dateChart.Count; j++)
                {
;                    if (listTextBox.ElementAt(i).ToString("dd.MM.yy") == dateChart.ElementAt(j).ToString("dd.MM.yy"))
                     {
                         reservation += 1;
                     }
                }
                chart1.Series[0].Points.AddXY(listTextBox.ElementAt(i).ToString("dd.MM.yy"), reservation);
            }
            chart1.Invalidate();
        }

        //выводит таблицу заказы
        private void buttonBringOut_Click(object sender, EventArgs e)
        {
            connect.SqlOpen();
            DateTime startDate = dateTimePicker3.Value;
            string date = startDate.ToString("yyyy-MM-dd");

            DateTime expirationDate = dateTimePicker4.Value;
            string date2 = expirationDate.ToString("yyyy-MM-dd");

            MySqlCommand command = new MySqlCommand("CALL Orders_in_range (\"" + date + "\", \"" + date2 +"\")", connect.connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dt.Columns[0].ColumnName = "Код";
            dt.Columns[1].ColumnName = "Код постояльца";
            dt.Columns[2].ColumnName = "Код номера";
            dt.Columns[3].ColumnName = "Дата и время заезда";
            dt.Columns[4].ColumnName = "Дата и время выезда";
            dt.Columns[5].ColumnName = "Код услуги";
            dt.Columns[6].ColumnName = "Цена";
            dataGridViewReportOrders.DataSource = dt;
            connect.SqlClosing();
        }
    }
}
