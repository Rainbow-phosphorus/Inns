﻿using Inns.UserForm;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inns
{
    public partial class Manager : Form
    {
        public Manager()
        {
            InitializeComponent();
            menuStripManager.Renderer = new MyRenderer();
        }

        DataProtection DataProtection = new DataProtection();

        private void Manager_Load(object sender, EventArgs e)
        {
            parametersPanel();
        }

        //выводит информацию о пользователе на форму
        public void userInformation(string sravId)
        {
            SqlConnection co = new SqlConnection();
            co.SqlOpen();
            MySqlCommand mySqlCommand = new MySqlCommand("CALL Select_user (\"" + sravId + "\")", co.connection);
            mySqlCommand.ExecuteNonQuery();
            MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
            DataTable dt = new DataTable();
            mySqlDataAdapter.Fill(dt);
            co.SqlClosing();
            string[] mas = new string[dt.Columns.Count];
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                mas[i] = DataProtection.Decrypt(dt.Rows[0][i].ToString());
            }
            label4.Text = mas[0];
            label2.Text = mas[1];
            label6.Text = mas[3];
            /*
            try
            {
                string executingAssemblyLocation = Assembly.GetExecutingAssembly().Location;
                string relativePathToImage = Path.Combine(Path.GetDirectoryName(executingAssemblyLocation), "Images\\Photo\\" + sravId + ".png");
                using (var stream = File.OpenRead(relativePathToImage))
                {
                    pictureBox1.Image = Image.FromStream(stream);
                }
            }
            catch (Exception ex)
            {
            }
            */
            //DataLogNis(mas[3], mas[0], mas[1], mas[2]);
        }

        public void DataLogHis(string mas, string mas1, string mas2, string mas3)
        {
            SqlConnection co = new SqlConnection();
            co.SqlOpen();
            DateTime datetime = DateTime.Now;

            MySqlCommand mySqlCommand = new MySqlCommand("CALL Insert_logHis (\"" + mas + "\", \"" + mas1 + "\", \"" + mas2 + "\", \"" + mas3 + "\", \"" + datetime.ToString("yyyy-MM-dd hh:mm:ss") +"\")", co.connection);
            mySqlCommand.ExecuteNonQuery();
            co.SqlClosing();
        }

        Clients clients;
        Rooms rooms;
        Services services;
        AboutBox aboutBox1;

        // тема и цвета меню
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
            MySqlCommand reservationComand = new MySqlCommand("CALL Number_of_rooms_busy", co.connection);
            object count1 = reservationComand.ExecuteScalar();
            label8.Text = Convert.ToString(count1);

            MySqlCommand reservationComand2 = new MySqlCommand("CALL Number_of_rooms_free", co.connection);
            object count2 = reservationComand2.ExecuteScalar();
            label10.Text = Convert.ToString(count2);
            co.SqlClosing();
        }
        private void toolStripMenuItemClients_Click(object sender, EventArgs e)
        {
            clients = new Clients();
            clients.Show();
        }

        private void toolStripMenuItemRooms_Click(object sender, EventArgs e)
        {
            rooms = new Rooms();
            rooms.Show();
        }

        private void toolStripMenuItemServices_Click(object sender, EventArgs e)
        {
            services = new Services();
            services.Show();
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
        private void ClosingForms()
        {
            Form[] forms = {clients, rooms, services, aboutBox1 };
            for(int i = 0; i < forms.Length; i++)
            {
                if (forms[i] != null)
                {
                    forms[i].Close();
                }
            }
        }
    }
}
