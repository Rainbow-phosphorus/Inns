using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inns.UserForm
{
    public partial class Employees : Form
    {
        public Employees()
        {
            InitializeComponent();
        }

        SqlConnection connect;

        DataProtection DataProtection = new DataProtection();

        private void Employees_Load(object sender, EventArgs e)
        {
            connect = new SqlConnection();
            Update();
        }

        //метод загружающий данные работников из БД в таблицу
        private void Update()
        {
            connect.SqlOpen();
            MySqlCommand mySqlCommand = new MySqlCommand("CALL Select_sotrudniki", connect.connection);
            MySqlDataAdapter my = new MySqlDataAdapter(mySqlCommand);
            DataTable dt = new DataTable();
            my.Fill(dt);
            dt.Columns[0].ColumnName = "Код";
            dt.Columns[1].ColumnName = "Фамилия";
            dt.Columns[2].ColumnName = "Имя";
            dt.Columns[3].ColumnName = "Отчество";
            dt.Columns[4].ColumnName = "Должность";
            dt.Columns[5].ColumnName = "Номер телефона";
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    string decryptedString = DataProtection.Decrypt(dt.Rows[j][i].ToString());
                    dt.Rows[j][i] = decryptedString;
                }
            }
            dataGridViewEmployees.DataSource = dt;
            connect.SqlClosing();
        }

        //добавляет запись в таблицу
        private void buttonAddEmployees_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Добавить запись?", "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (textBox1.Text != "" & textBox2.Text != "" & textBox3.Text != "" & textBox4.Text != "" & textBox5.Text != "" & textBox6.Text != "" & textBox7.Text != "")
                {
                    if (double.TryParse(textBox5.Text, out double nomer))
                    {
                        connect.SqlOpen();
                        string famil = DataProtection.Encrypt(textBox1.Text);
                        string name = DataProtection.Encrypt(textBox2.Text);
                        string patronomic = DataProtection.Encrypt(textBox3.Text);
                        string dolhnist = DataProtection.Encrypt(textBox4.Text);
                        MySqlCommand mySqlCommand = new MySqlCommand("CALL Insert_sotrudniki (\"" + famil + "\", \"" + name + "\", \"" + patronomic + "\", \"" + dolhnist + "\", \"" + DataProtection.Encrypt(Convert.ToString(nomer)) + "\", \"" + Convert.ToBase64String(DataProtection.PBKDF2Hash(textBox6.Text)) + "\", \"" + Convert.ToBase64String(DataProtection.PBKDF2Hash(textBox7.Text)) + "\")", connect.connection);
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
        private void buttonEditEmployees_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Редактировать запись?", "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (textBox8.Text != "" & textBox9.Text != "" & textBox10.Text != "" & textBox11.Text != "" & textBox12.Text != "" & textBox13.Text != "" & textBox14.Text != "" & textBox15.Text != "")
                {
                    if (int.TryParse(textBox8.Text, out int id) & double.TryParse(textBox13.Text, out double nomer))
                    {
                        connect.SqlOpen();
                        string famil = DataProtection.Encrypt(textBox9.Text);
                        string name = DataProtection.Encrypt(textBox10.Text);
                        string patronomic = DataProtection.Encrypt(textBox11.Text);
                        string dolhnist = DataProtection.Encrypt(textBox12.Text);
                        MySqlCommand mySqlCommand = new MySqlCommand("CALL Update_sotrudniki (\"" + id + "\", \"" + famil + "\", \"" + name + "\", \"" + patronomic + "\", \"" + dolhnist + "\", \"" + DataProtection.Encrypt(Convert.ToString(nomer)) + "\", \"" + Convert.ToBase64String(DataProtection.PBKDF2Hash(textBox14.Text)) + "\", \"" + Convert.ToBase64String(DataProtection.PBKDF2Hash(textBox15.Text)) + "\")", connect.connection);
                        mySqlCommand.ExecuteNonQuery();
                        connect.SqlClosing();
                        Update();
                        MessageBox.Show("Запись отредактирована!", "TsManager");
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
        private void buttonDeleteEmployees_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Удалить запись?", "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if(textBox16.Text != "")
                {
                    if(int.TryParse(textBox16.Text, out int id))
                    {
                        connect.SqlOpen();
                        MySqlCommand mySqlCommand = new MySqlCommand("CALL Delete_sotrudniki (\"" + id + "\")", connect.connection);
                        mySqlCommand.ExecuteNonQuery();
                        connect.SqlClosing();
                        Update();
                        MessageBox.Show("Запись удалена!", "TsManager");
                    }
                    else
                    {
                        MessageBox.Show("Неправильные типы данных");
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

        private void dataGridViewEmployees_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = dataGridViewEmployees.CurrentCell.RowIndex;
            textBox1.Text = dataGridViewEmployees[1, index].Value.ToString();
            textBox2.Text = dataGridViewEmployees[2, index].Value.ToString();
            textBox3.Text = dataGridViewEmployees[3, index].Value.ToString();
            textBox4.Text = dataGridViewEmployees[4, index].Value.ToString();
            textBox5.Text = dataGridViewEmployees[5, index].Value.ToString();

            textBox8.Text = dataGridViewEmployees[0, index].Value.ToString();
            textBox9.Text = dataGridViewEmployees[1, index].Value.ToString();
            textBox10.Text = dataGridViewEmployees[2, index].Value.ToString();
            textBox11.Text = dataGridViewEmployees[3, index].Value.ToString();
            textBox12.Text = dataGridViewEmployees[4, index].Value.ToString();
            textBox13.Text = dataGridViewEmployees[5, index].Value.ToString();

            textBox16.Text = dataGridViewEmployees[0, index].Value.ToString();
        }

        private void Employees_FormClosing(object sender, FormClosingEventArgs e)
        {
            SystemAdministrator systemadministrator = Application.OpenForms.OfType<SystemAdministrator>().FirstOrDefault(); ;
            systemadministrator.parametersPanel();
        }
    }
}
