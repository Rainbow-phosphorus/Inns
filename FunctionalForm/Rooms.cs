using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Inns.UserForm
{
    public partial class Rooms : Form
    {
        public Rooms()
        {
            InitializeComponent();
        }
        SqlConnection connect;
        private void Rooms_Load(object sender, EventArgs e)
        {
            connect = new SqlConnection();
            StatusNomer();
            Update();
        }

        private void StatusNomer()
        {
            connect.SqlOpen();
            MySqlCommand mySqlCommand = new MySqlCommand("CALL Number_code", connect.connection);
            mySqlCommand.ExecuteNonQuery();
            MySqlDataAdapter my = new MySqlDataAdapter(mySqlCommand);
            DataTable dt = new DataTable();
            my.Fill(dt);

            DateTime thisDay = DateTime.Now;

            for (int i = 0; i<dt.Rows.Count; i++)
            {
                MySqlCommand poisk = new MySqlCommand("CALL Busy_numbers (\"" + dt.Rows[i][0] + "\", \"" + thisDay.ToString("yyyy-MM-dd HH:mm:ss") + "\")", connect.connection);
                object Poisk = poisk.ExecuteScalar();
                string count = Convert.ToString(Poisk);
                if (count != "")
                {
                    MySqlCommand statysNomer = new MySqlCommand("CALL Change_of_status_nomer (\"" + "Занят" + "\", \"" + dt.Rows[i][0] + "\")", connect.connection);
                    statysNomer.ExecuteNonQuery();
                }
                else
                {
                    MySqlCommand statysNomer = new MySqlCommand("CALL Change_of_status_nomer (\"" + "Свободен" + "\", \"" + dt.Rows[i][0] + "\")", connect.connection);
                    statysNomer.ExecuteNonQuery();
                }
                
            }
            connect.SqlClosing();
        }
        //метод загружающий данные номерного фонда из БД в таблицу
        private void Update()
        {
            connect.SqlOpen();
            MySqlCommand mySqlCommand = new MySqlCommand("CALL Select_nomer",connect.connection);
            MySqlDataAdapter my = new MySqlDataAdapter(mySqlCommand);
            DataTable dt = new DataTable();
            my.Fill(dt);
            dt.Columns[0].ColumnName = "Код";
            dt.Columns[1].ColumnName = "Номер комнаты";
            dt.Columns[2].ColumnName = "Наименование";
            dt.Columns[3].ColumnName = "Тип номера";
            dt.Columns[4].ColumnName = "Цена";
            dt.Columns[5].ColumnName = "Площадь (м2)";
            dt.Columns[6].ColumnName = "Этаж";
            dt.Columns[7].ColumnName = "Статус";
            dataGridViewNomer.DataSource = dt;
            connect.SqlClosing();
        }

        //добавляет запись в таблицу
        private void buttonAddNomer_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Добавить запись?", "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if(textBox1.Text != "" & textBox2.Text != "" & textBox3.Text != "" & textBox4.Text != "" & textBox5.Text != "" & textBox6.Text != "")
                {
                    if (int.TryParse(textBox1.Text, out int roomNomer) & double.TryParse(textBox4.Text, out double price) & int.TryParse(textBox5.Text, out int square) & int.TryParse(textBox6.Text, out int floor))
                    {
                        connect.SqlOpen();
                        string name = textBox2.Text;
                        string tipNomer = textBox3.Text;
                        MySqlCommand mySqlCommand = new MySqlCommand("CALL Insert_nomer (\"" + roomNomer + "\", \"" + name + "\", \"" + tipNomer + "\", \"" + price + "\", \"" + square + "\",  \"" + floor + "\", 'Свободен')", connect.connection);
                        mySqlCommand.ExecuteNonQuery();
                        connect.SqlClosing();
                        Update();
                        MessageBox.Show("Запись добавлена!", "TsManager");
                    }
                    else
                    {
                        MessageBox.Show("Неправильные типы данных");
                    }
                }
                else
                {
                    MessageBox.Show("Вы не ввели значения");
                }   
            }
            else if (result == DialogResult.No)
            {
                MessageBox.Show("Действие отмененно!", "TsManager");
            }
        }

        //редактирует запись
        private void buttonEditNomer_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Редактировать запись?", "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (textBox7.Text != "" & textBox8.Text != "" & textBox9.Text != "" & textBox10.Text != "" & textBox11.Text != "" & textBox12.Text != "" & textBox13.Text != "")
                {
                    if(int.TryParse(textBox7.Text, out int id) & int.TryParse(textBox8.Text, out int roomNomer) & double.TryParse(textBox11.Text, out double price) & int.TryParse(textBox12.Text, out int square) & int.TryParse(textBox13.Text, out int floor))
                    {
                        connect.SqlOpen();
                        string name = textBox9.Text;
                        string tipNomer = textBox10.Text;
                        MySqlCommand mySqlCommand = new MySqlCommand("CALL Update_nomer (\"" + id + "\", \"" + roomNomer + "\", \"" + name + "\", \"" + tipNomer + "\", \"" + price + "\", \"" + square + "\",  \"" + floor + "\", 'Свободен')", connect.connection);
                        mySqlCommand.ExecuteNonQuery();
                        connect.SqlClosing();
                        Update();
                        MessageBox.Show("Запись отредактированна!", "TsManager");
                    }
                    else
                    {
                        MessageBox.Show("Неправильные типы данных");
                    }
                }
                else
                {
                    MessageBox.Show("Вы не ввели значения");
                }
            }
            else if (result == DialogResult.No)
            {
                MessageBox.Show("Действие отмененно!", "TsManager");
            }
        }

        //удаляет запись
        private void buttonDeleteNomer_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Удалить запись?", "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if(textBox14.Text != "")
                {
                    if (int.TryParse(textBox14.Text, out int id))
                    {
                        connect.SqlOpen();
                        MySqlCommand mySqlCommand = new MySqlCommand("CALL Delete_nomer (\"" + id + "\")", connect.connection);
                        mySqlCommand.ExecuteNonQuery();
                        connect.SqlClosing();
                        Update();
                        MessageBox.Show("Запись удалена!", "TsManager");
                    }    
                    else
                    {
                        MessageBox.Show("Неправильный тип данных");
                    }
                }
                else
                {
                    MessageBox.Show("Вы не ввели значение");
                }
            }
            else if (result == DialogResult.No)
            {
                MessageBox.Show("Действие отмененно!", "TsManager");
            }
        }

        private void dataGridViewNomer_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = dataGridViewNomer.CurrentCell.RowIndex;

            textBox1.Text = dataGridViewNomer[1, index].Value.ToString();
            textBox2.Text = dataGridViewNomer[2, index].Value.ToString(); ;
            textBox3.Text = dataGridViewNomer[3, index].Value.ToString();
            textBox4.Text = dataGridViewNomer[4, index].Value.ToString();
            textBox5.Text = dataGridViewNomer[5, index].Value.ToString();
            textBox6.Text = dataGridViewNomer[6, index].Value.ToString();

            textBox7.Text = dataGridViewNomer[0, index].Value.ToString();
            textBox8.Text = dataGridViewNomer[1, index].Value.ToString();
            textBox9.Text = dataGridViewNomer[2, index].Value.ToString();
            textBox10.Text = dataGridViewNomer[3, index].Value.ToString();
            textBox11.Text = dataGridViewNomer[4, index].Value.ToString();
            textBox12.Text = dataGridViewNomer[5, index].Value.ToString();
            textBox13.Text = dataGridViewNomer[6, index].Value.ToString();

            textBox14.Text = dataGridViewNomer[0, index].Value.ToString();
        }

        private void Rooms_FormClosing(object sender, FormClosingEventArgs e)
        {
            Manager manager = Application.OpenForms.OfType<Manager>().FirstOrDefault(); ;
            manager.parametersPanel();
        }

        private void buttonDownloadNomer_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Формат|*.csv";
            openFileDialog1.Title = "Выберите ранее записанный график в формате .csv";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(openFileDialog1.FileName);
                string line = sr.ReadLine();

                while (line != null)
                {

                    string[] words = line.Split(';');

                    int col = 0;
                    string[] stroca = new string[6];
                    foreach (var word in words)
                    {
                        stroca[col] = word;
                        if(word != "")
                        {
                            col++;
                        }
                    }
                    if (col == 6)
                    {
                        connect.SqlOpen();
                        MySqlCommand cmd = new MySqlCommand("CALL Insert_nomer (\"" + stroca[0] + "\", \"" + stroca[1] + "\", \"" + stroca[2] + "\", \"" + stroca[3] + "\", \"" + stroca[4] + "\",  \"" + stroca[5] + "\", 'Свободен')", connect.connection);
                        cmd.ExecuteNonQuery();
                        connect.SqlClosing();
                        Update();
                    }
                    else
                    {
                        MessageBox.Show("В одной из записей не соблюденно количетво столбцов, поэтому она будет пропущена");
                    }

                    line = sr.ReadLine();
                }
                sr.Close();
            }
        }
    }
}

//Treshchalina A. V.