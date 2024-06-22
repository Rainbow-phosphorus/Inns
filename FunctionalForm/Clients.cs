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
    public partial class Clients : Form
    {
        public Clients()
        {
            InitializeComponent();
        }

        SqlConnection connect;

        DataProtection DataProtection = new DataProtection();

        private void Clients_Load(object sender, EventArgs e)
        {
            connect = new SqlConnection();
            Update();
        }

        //загрузка данных постояльцев из БД в таблицу
        private void Update()
        {
            connect.SqlOpen();
            MySqlCommand cmdUpdate = new MySqlCommand("CALL Select_guests", connect.connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmdUpdate);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            dataTable.Columns[0].ColumnName = "Код";
            dataTable.Columns[1].ColumnName = "Фамилия";
            dataTable.Columns[2].ColumnName = "Имя";
            dataTable.Columns[3].ColumnName = "Отчество";
            dataTable.Columns[4].ColumnName = "Номер телефона";
            dataTable.Columns[5].ColumnName = "Почта";
            for (int j = 0; j < dataTable.Rows.Count; j++)
            {
                for (int i = 1; i < dataTable.Columns.Count; i++)
                {
                    string decryptedString = DataProtection.Decrypt(dataTable.Rows[j][i].ToString());
                    dataTable.Rows[j][i] = decryptedString;
                }
            }
            dataGridViewClients.DataSource = dataTable;
            connect.SqlClosing();
        }

        //кнопка добавления записи
        private void buttonAddClients_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Добавить запись?", "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (textBox1.Text != "" & textBox2.Text != "" & textBox3.Text != "" & textBox4.Text != "" & textBox5.Text != "")
                {
                    if (double.TryParse(textBox4.Text, out double nomer))
                    {
                        connect.SqlOpen();
                        string surname = DataProtection.Encrypt(textBox1.Text);
                        string name = DataProtection.Encrypt(textBox2.Text);
                        string patronymic = DataProtection.Encrypt(textBox3.Text);
                        string email = DataProtection.Encrypt(textBox5.Text);
                        MySqlCommand addClient = new MySqlCommand("CALL Insert_guests (\"" + surname + "\", \"" + name + "\"," +
                            " \"" + patronymic + "\", \"" + DataProtection.Encrypt(Convert.ToString(nomer)) + "\", \"" + email + "\")", connect.connection);
                        addClient.ExecuteNonQuery();
                        connect.SqlClosing();
                        Update();
                        MessageBox.Show("Запись добавлена!", "TsManager");
                    }
                    else
                    {
                        MessageBox.Show("Неправильный тип данных");
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

        //кнопка редактирования записи
        private void buttonEditClients_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Редактировать запись?", "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (textBox6.Text != "" & textBox7.Text != "" & textBox8.Text != "" & textBox9.Text != "" & textBox10.Text != "" & textBox11.Text != "")
                {
                    if(int.TryParse(textBox6.Text, out int id) & double.TryParse(textBox10.Text, out double nomer))
                    {
                        connect.SqlOpen();
                        string surname = DataProtection.Encrypt(textBox7.Text);
                        string name = DataProtection.Encrypt(textBox8.Text);
                        string patronymic = DataProtection.Encrypt(textBox9.Text);
                        string email = DataProtection.Encrypt(textBox11.Text);
                        MySqlCommand editClient = new MySqlCommand("CALL Update_guests (\"" + id + "\", \"" + surname + "\", \"" + name + "\", \"" + patronymic + "\", \"" + DataProtection.Encrypt(Convert.ToString(nomer)) + "\", \"" + email + "\")", connect.connection);
                        editClient.ExecuteNonQuery();
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

        //кнопка удаления записи 
        private void buttonDeleteClients_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Удалить запись?", "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (textBox12.Text != "")
                {
                    if(int.TryParse(textBox12.Text, out int id))
                    {
                        connect.SqlOpen();
                        MySqlCommand delClient = new MySqlCommand("CALL Delete_guests (\"" + id + "\")", connect.connection);
                        delClient.ExecuteNonQuery();
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
                    MessageBox.Show("Вы не ввели значения");
                }
            }
            else if (result == DialogResult.No)
            {
                MessageBox.Show("Действие отмененно!", "TsManager");
            }
        }

        //метод добавления в поля данных из таблицы
        private void dataGridViewClients_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = dataGridViewClients.CurrentCell.RowIndex;

            textBox1.Text = dataGridViewClients[1, index].Value.ToString();
            textBox2.Text = dataGridViewClients[2, index].Value.ToString();
            textBox3.Text = dataGridViewClients[3, index].Value.ToString();
            textBox4.Text = dataGridViewClients[4, index].Value.ToString();
            textBox5.Text = dataGridViewClients[5, index].Value.ToString();

            textBox6.Text = dataGridViewClients[0, index].Value.ToString();
            textBox7.Text = dataGridViewClients[1, index].Value.ToString();
            textBox8.Text = dataGridViewClients[2, index].Value.ToString();
            textBox9.Text = dataGridViewClients[3, index].Value.ToString();
            textBox10.Text = dataGridViewClients[4, index].Value.ToString();
            textBox11.Text = dataGridViewClients[5, index].Value.ToString();

            textBox12.Text = dataGridViewClients[0, index].Value.ToString();

        }

    }
}
