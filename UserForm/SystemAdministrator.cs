using Inns.FunctionalForm;
using Inns.UserForm;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inns
{
    public partial class SystemAdministrator : Form
    {
        public SystemAdministrator()
        {
            InitializeComponent();
            menuStripSysAdmin.Renderer = new MyRenderer();
        }

        SqlConnection connect;

        DataProtection DataProtection = new DataProtection();

        private void SystemAdministrator_Load(object sender, EventArgs e)
        {
            connect = new SqlConnection();
            parametersPanel();
        }

        //выводит информацию о пользователе на форму
        public void userInformation(string sravId)
        {
            SqlConnection connect = new SqlConnection();
            connect.SqlOpen();
            MySqlCommand mySqlCommand = new MySqlCommand("CALL Select_user (\"" + sravId + "\")", connect.connection);
            mySqlCommand.ExecuteNonQuery();
            MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
            DataTable dt = new DataTable();
            mySqlDataAdapter.Fill(dt);
            connect.SqlClosing();
            string[] mas = new string[dt.Columns.Count];
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                mas[i] = DataProtection.Decrypt(dt.Rows[0][i].ToString());
            }
            label4.Text = mas[0];
            label2.Text = mas[1];
            label6.Text = mas[3];
            //DataLogHis(mas[3], mas[0], mas[1], mas[2]);
        }

        //тема и цвета меню
        private class MyRenderer : ToolStripProfessionalRenderer
        {
            public MyRenderer() : base(new MyColors()) { }
        }

        private class MyColors : ProfessionalColorTable
        {
            public override Color MenuItemSelected
            {
                get { return Color.Firebrick; }
            }
            public override Color MenuItemSelectedGradientBegin
            {
                get { return Color.IndianRed; }
            }
            public override Color MenuItemSelectedGradientEnd
            {
                get { return Color.Firebrick; }
            }
            public override Color MenuItemBorder
            {
                get { return Color.Firebrick; }
            }
        }

        //выводит данные на панель с параметрами
        public void parametersPanel()
        {
            SqlConnection co = new SqlConnection();
            co.SqlOpen();
            DateTime day = DateTime.Today;
            MySqlCommand loginHistoryComand = new MySqlCommand("CALL Number_of_inputs (\"" + day.ToString("yyyy-MM-dd") + "\")", co.connection);
            object colDay = loginHistoryComand.ExecuteScalar();
            label8.Text = Convert.ToString(colDay);

            MySqlCommand reservationComand2 = new MySqlCommand("CALL Number_of_employees", co.connection);
            object colSotrudnicov = reservationComand2.ExecuteScalar();
            label10.Text = Convert.ToString(colSotrudnicov);
            co.SqlClosing();
        }

        public void DataLogHis(string mas, string mas1, string mas2, string mas3)
        {
            SqlConnection co = new SqlConnection();
            co.SqlOpen();
            DateTime datetime = DateTime.Now;

            MySqlCommand mySqlCommand = new MySqlCommand("CALL Insert_logHis (\"" + mas + "\", \"" + mas1 + "\", \"" + mas2 + "\", \"" + mas3 + "\", \"" + datetime.ToString("yyyy-MM-dd hh:mm:ss") + "\")", co.connection);
            mySqlCommand.ExecuteNonQuery();
            co.SqlClosing();
        }

        Employees employees;
        LoginHistory loginHistory;
        Reports reports;
        AboutBox aboutBox1;

        private void toolStripMenuItemEmployees_Click(object sender, EventArgs e)
        {
            employees = new Employees();
            employees.Show();
        }

        private void toolStripMenuItemLogHistory_Click(object sender, EventArgs e)
        {
            loginHistory = new LoginHistory();
            loginHistory.Show();
        }

        private void toolStripMenuItemReports_Click(object sender, EventArgs e)
        {
            reports = new Reports();
            reports.Show();
        }

        private void toolStripMenuItemAbout_Click(object sender, EventArgs e)
        {
            aboutBox1 = new AboutBox();
            aboutBox1.ShowDialog();
        }

        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            ClosingForms();
            this.Close();
        }
        //закрывает все открытые пользователем формы
        private void ClosingForms()
        {
            Form [] forms = { employees, loginHistory, reports, aboutBox1 };
            for (int i = 0; i<forms.Length; i++)
            {
                if (forms[i] != null)
                {
                    forms[i].Close();
                }
            }
        }
    }
}
