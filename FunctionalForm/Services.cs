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

namespace Inns.UserForm
{
    public partial class Services : Form
    {
        public Services()
        {
            InitializeComponent();
        }
        SqlConnection connect;
        private void Services_Load(object sender, EventArgs e)
        {
            connect = new SqlConnection();
            Update();
        }

        private void Update()
        {
            connect.SqlOpen();
            MySqlCommand mySqlCommand = new MySqlCommand("CALL Select_services", connect.connection);
            MySqlDataAdapter my = new MySqlDataAdapter(mySqlCommand);
            DataTable dt = new DataTable();
            my.Fill(dt);
            dt.Columns[0].ColumnName = "Код";
            dt.Columns[1].ColumnName = "Наименование";
            dt.Columns[2].ColumnName = "Описание";
            dt.Columns[3].ColumnName = "Цена";
            dataGridViewServices.DataSource = dt;
            connect.SqlClosing();
        }

        //добавляет запись в таблицу
        private void buttonAddNomer_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Добавить запись?", "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if(textBox1.Text != "" & textBox2.Text != "" & textBox3.Text != "")
                {
                    if (double.TryParse(textBox3.Text, out double price))
                    {
                        connect.SqlOpen();
                        string name = textBox1.Text;
                        string description = textBox2.Text;
                        MySqlCommand mySqlCommand = new MySqlCommand("CALL Insert_services (\"" + name + "\", \"" + description + "\", \"" + price + "\")", connect.connection);
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
                if (textBox4.Text != "" & textBox5.Text != "" & textBox6.Text != "" & textBox7.Text != "")
                {
                    if (int.TryParse(textBox4.Text, out int id) & double.TryParse(textBox7.Text, out double price))
                    {
                        connect.SqlOpen();
                        string name = textBox5.Text;
                        string description = textBox6.Text;
                        MySqlCommand mySqlCommand = new MySqlCommand("CALL Update_services (\"" + id + "\", \"" + name + "\", \"" + description + "\", \"" + price + "\")", connect.connection);
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
                if (textBox8.Text != "")
                {
                    if (int.TryParse(textBox8.Text, out int id))
                    {
                        connect.SqlOpen();
                        MySqlCommand mySqlCommand = new MySqlCommand("CALL Delete_services (\"" + id + "\")", connect.connection);
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

        private void dataGridViewServices_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = dataGridViewServices.CurrentCell.RowIndex;

            textBox1.Text = dataGridViewServices[1, index].Value.ToString();
            textBox2.Text = dataGridViewServices[2, index].Value.ToString(); 
            textBox3.Text = dataGridViewServices[3, index].Value.ToString();

            textBox4.Text = dataGridViewServices[0, index].Value.ToString();
            textBox5.Text = dataGridViewServices[1, index].Value.ToString();
            textBox6.Text = dataGridViewServices[2, index].Value.ToString();
            textBox7.Text = dataGridViewServices[3, index].Value.ToString();

            textBox8.Text = dataGridViewServices[0, index].Value.ToString();
        }

        private void Services_FormClosing(object sender, FormClosingEventArgs e)
        {
            Manager manager = Application.OpenForms.OfType<Manager>().FirstOrDefault(); ;
            manager.parametersPanel();
        }
    }
}
