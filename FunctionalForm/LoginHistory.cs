using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inns.FunctionalForm
{
    public partial class LoginHistory : Form
    {
        public LoginHistory()
        {
            InitializeComponent();
        }
        SqlConnection connect;
    
        private void LoginHistory_Load(object sender, EventArgs e)
        {
            connect = new SqlConnection();
            SelectLogHis();
        }
         
        private void SelectLogHis()
        {
            connect.SqlOpen();
            MySqlCommand mySqlCommand = new MySqlCommand("CALL Select_LogHis", connect.connection);
            mySqlCommand.ExecuteNonQuery();
            MySqlDataAdapter my = new MySqlDataAdapter(mySqlCommand);
            DataTable dt = new DataTable();
            my.Fill(dt);
            connect.SqlClosing();
            Update(dt);
        }

        //метод загружающий данные истории входа из БД в таблицу
        private void Update(DataTable dt)
        {
            dt.Columns[0].ColumnName = "Код";
            dt.Columns[1].ColumnName = "Должность";
            dt.Columns[2].ColumnName = "Фамилия";
            dt.Columns[3].ColumnName = "Имя";
            dt.Columns[4].ColumnName = "Отчество";
            dt.Columns[5].ColumnName = "Дата и время входа";
            dataGridViewLogHis.DataSource = dt;
        }

        //сортирует записи по должностям
        private void buttonFindPosition_Click(object sender, EventArgs e)
        {
            connect.SqlOpen();
            string dolhnost = textBox1.Text;
            MySqlCommand mySqlCommand = new MySqlCommand("CALL Find_LogHis_Position (\"" + dolhnost + "\")", connect.connection);
            MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
            DataTable dt = new DataTable();
            mySqlDataAdapter.Fill(dt);
            connect.SqlClosing();
            Update(dt);
        }

        //сортирует записи по фамилии
        private void buttonFindSurname_Click(object sender, EventArgs e)
        {
            connect.SqlOpen();
            string surname = textBox2.Text;
            MySqlCommand mySqlCommand = new MySqlCommand("CALL Find_LogHis_Surname (\"" + surname + "\")", connect.connection);
            MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
            DataTable dt = new DataTable();
            mySqlDataAdapter.Fill(dt);
            connect.SqlClosing();
            Update(dt);
        }

        //сортирует записи по датам
        private void buttonFindDate_Click(object sender, EventArgs e)
        {
            connect.SqlOpen();
            DateTime currentDate = dateTimePicker1.Value;
            string date = currentDate.ToString("yyyy-MM-dd");
          
            MySqlCommand mySqlCommand = new MySqlCommand("CALL Find_LogHis_Date (\"" + date + "\")", connect.connection);
            MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
            DataTable dt = new DataTable();
            mySqlDataAdapter.Fill(dt);
            connect.SqlClosing();
            Update(dt);
        }

        //возвращает таблицу к изначальному состоянию 
        private void buttonReset_Click(object sender, EventArgs e)
        {
            SelectLogHis();
        }
    }
}
