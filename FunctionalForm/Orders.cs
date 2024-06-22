using Inns.UserForm;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Inns.FunctionalForm
{
    public partial class Orders : Form
    {
        public Orders()
        {
            InitializeComponent();
        }

        SqlConnection connect;
        private void Orders_Load(object sender, EventArgs e)
        {
            connect = new SqlConnection();
            Update();
        }

        //метод загружающий данные заказов из БД в таблицу
        private void Update()
        {
            connect.SqlOpen();
            MySqlCommand mySqlCommand = new MySqlCommand("CALL Select_orders", connect.connection);
            MySqlDataAdapter my = new MySqlDataAdapter(mySqlCommand);
            DataTable dt = new DataTable();
            my.Fill(dt);
            dt.Columns[0].ColumnName = "Код";
            dt.Columns[1].ColumnName = "Код постояльца";
            dt.Columns[2].ColumnName = "Код номера";
            dt.Columns[3].ColumnName = "Дата и время заезда";
            dt.Columns[4].ColumnName = "Дата и время выезда";
            dt.Columns[5].ColumnName = "Код услуги";
            dt.Columns[6].ColumnName = "Цена";

            dataGridViewOrders.DataSource = dt;
            connect.SqlClosing();
        }

        //добавление записи заказа
        private void buttonAddOrders_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Добавить запись?", "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (textBox1.Text != "" & textBox2.Text != "" & maskedTextBox1.Text != "" & maskedTextBox2.Text != "")
                {
                    if (int.TryParse(textBox1.Text, out int idGuest) & int.TryParse(textBox2.Text, out int idNomer))
                    {
                        string arrivalDateTime = maskedTextBox1.Text;
                        string departureDateTime = maskedTextBox2.Text;
                        string idServices = textBox3.Text;
                        if (provercaDate(idNomer, arrivalDateTime, departureDateTime, "app") == true)
                        {
                            double price = calculatePrice(idNomer, idServices, arrivalDateTime, departureDateTime);
                            label7.Text = price.ToString();
                            if (idServices == "")
                            {
                                idServices = "null";
                                MessageBox.Show(idServices);
                            }
                            connect.SqlOpen();
                            MySqlCommand mySqlCommand = new MySqlCommand("CALL Insert_orders (\"" + idGuest + "\", \"" + idNomer + "\", \"" + arrivalDateTime + "\", \"" + departureDateTime + "\", " + idServices + ", \"" + price + "\")", connect.connection);
                            mySqlCommand.ExecuteNonQuery();
                            connect.SqlClosing();
                            Update();
                            MessageBox.Show("Запись добавлена!", "TsManager");
                        }
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

        //просчитывает цену для добавления
        private void buttonСalculateAmount_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "" & maskedTextBox1.Text != "" & maskedTextBox2.Text != "")
            {
                if (int.TryParse(textBox2.Text, out int idNomer))
                {
                    string arrivalDateTime = maskedTextBox1.Text;
                    string departureDateTime = maskedTextBox2.Text;
                    string idServices = textBox3.Text;
                    label7.Text = Convert.ToString(calculatePrice(idNomer, idServices, arrivalDateTime, departureDateTime));
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

        //редактирует запись заказа 
        private void buttonEditOrders_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Редактировать запись?", "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (textBox4.Text != "" & textBox5.Text != "" & textBox6.Text != "" & maskedTextBox3.Text != "" & maskedTextBox4.Text != "")
                {
                    if(int.TryParse(textBox4.Text, out int id) & int.TryParse(textBox5.Text, out int idGuest) & int.TryParse(textBox6.Text, out int idNomer))
                    {
                        string arrivalDateTime = maskedTextBox3.Text;
                        string departureDateTime = maskedTextBox4.Text;
                        string idServices = textBox7.Text;
                        if (provercaDate(idNomer, arrivalDateTime, departureDateTime, "edit") == true)
                        {
                            double price = calculatePrice(idNomer, idServices, arrivalDateTime, departureDateTime);
                            label7.Text = price.ToString();
                            if (idServices == "")
                            {
                                idServices = "null";
                                MessageBox.Show(idServices);
                            }
                            connect.SqlOpen();
                            MySqlCommand mySqlCommand = new MySqlCommand("CALL Update_orders (\"" + id + "\", \"" + idGuest + "\", \"" + idNomer + "\", \"" + arrivalDateTime + "\", \"" + departureDateTime + "\", " + idServices + ", \"" + price + "\")", connect.connection);
                            mySqlCommand.ExecuteNonQuery();
                            connect.SqlClosing();
                            Update();
                            MessageBox.Show("Запись отредактирована!", "TsManager");
                        }
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

        //просчитывает цену для редактирования
        private void buttonСalculateAmountEdit_Click(object sender, EventArgs e)
        {
            if (textBox6.Text != "" & maskedTextBox3.Text != "" & maskedTextBox4.Text != "")
            {
                if (int.TryParse(textBox6.Text, out int idNomer))
                {
                    string arrivalDateTime = maskedTextBox3.Text;
                    string departureDateTime = maskedTextBox4.Text;
                    string idServices = textBox7.Text;
                    label15.Text = Convert.ToString(calculatePrice(idNomer, idServices, arrivalDateTime, departureDateTime));
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

        //считается цена
        public double calculatePrice(int idNomer, string idServices, string arrivalDateTime, string departureDateTime)
        {
            //получили цену за один день в номере
            connect.SqlOpen();
            MySqlCommand CommandPriceNomer = new MySqlCommand("CALL Сalculate_price (\"" + idNomer + "\")", connect.connection);
            object NomerPrice = CommandPriceNomer.ExecuteScalar();
            double Nprice = Convert.ToDouble(NomerPrice);
            double Sprice = 0;
            if (idServices != "")
            {
                MySqlCommand CommandPriceServices = new MySqlCommand("CALL Service_price (\"" + idServices + "\")", connect.connection);
                object Servicesprice = CommandPriceServices.ExecuteScalar();
                Sprice = Convert.ToDouble(Servicesprice);
            }
           
            connect.SqlClosing();

            //узнали количество дней пробывания
            DateTime dateStart = DateTime.Parse(arrivalDateTime);
            DateTime dateOut = DateTime.Parse(departureDateTime);

            TimeSpan difference = dateOut.Subtract(dateStart);
            double colDay = difference.Days;
            MessageBox.Show("" + colDay);

            double price = 0;

            //расcчитали цену за проживание в номере 
            if (idServices != " ")
            {
                price = (colDay * Nprice) + (colDay * Sprice);
            }
            else
            {
                price = colDay * Nprice;
            }
            return price;
        }

        //проверка дат
        public bool provercaDate(int idNomer, string arrivalDateTime, string departureDateTime, string tip)
        {
            DateTime compareDate1 = DateTime.Parse(arrivalDateTime);
            DateTime compareDate2 = DateTime.Parse(departureDateTime);
            if (compareDate1 < compareDate2)
            {
                connect.SqlOpen();
                MySqlCommand mySqlCommand = new MySqlCommand("CALL Order_dates (\"" + idNomer + "\")", connect.connection);
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
                DataTable dt = new DataTable();
                mySqlDataAdapter.Fill(dt);
                connect.SqlClosing();

                //список дат уже имеющихся в системе
                DateTime startDate = new DateTime();
                DateTime endDate = new DateTime();

                HashSet<DateTime> dateList = new HashSet<DateTime>();
                //определяется для какого действия работает метод
                if (tip == "app")
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string sd = dt.Rows[i][0].ToString();
                        string ed = dt.Rows[i][1].ToString();
                        startDate = DateTime.Parse(sd);
                        endDate = DateTime.Parse(ed);
                        for (DateTime date = startDate; date <= endDate; date = date.AddMinutes(30))
                        {
                            dateList.Add(date);
                        }
                    }
                }
                else if (tip == "edit")
                {
                    string id = textBox4.Text;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string sd = dt.Rows[i][0].ToString();
                        string ed = dt.Rows[i][1].ToString();
                        startDate = DateTime.Parse(sd);
                        endDate = DateTime.Parse(ed);
                        if (id != dt.Rows[i][2].ToString())
                        {
                            for (DateTime date = startDate; date <= endDate; date = date.AddMinutes(30))
                            {
                                dateList.Add(date);
                            }
                        }
                    }
                }

                //список для введённых дат
                HashSet<DateTime> dateList2 = new HashSet<DateTime>();
                for (DateTime date2 = compareDate1; date2 <= compareDate2; date2 = date2.AddMinutes(30))
                {
                    dateList2.Add(date2);
                }

                //проверяем и сравниваем даты
                bool noCommonElements = dateList.Any(x => dateList2.Contains(x));
                if (noCommonElements)
                {
                    MessageBox.Show("Данный номер уже занят на этот периуд");
                    return false;
                }
                else
                {
                    MessageBox.Show("Дата не найдена в списке дат");
                    TimeSpan difference = compareDate2.Subtract(compareDate1);
                    double colDay = difference.Days;
                    if (colDay <= 0)
                    {
                        MessageBox.Show("Периуд должен быть минимум 24 часа");
                        return false;
                    }
                    else
                    {
                        MessageBox.Show("Всё хорошо");
                        return true;
                    }
                }
            }
            else
            {
                MessageBox.Show("Дата начала должна быть меньше даты окончания");
                return false;
            }
        }

        //удаление записи заказа
        private void buttonDeleteOrders_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Удалить запись?", "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if(textBox8.Text != "")
                {
                    if(int.TryParse(textBox8.Text, out int id))
                    {
                        connect.SqlOpen();
                        MySqlCommand mySqlCommand = new MySqlCommand("CALL Delete_orders (\"" + id + "\")", connect.connection);
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

        private void dataGridViewOrders_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = dataGridViewOrders.CurrentCell.RowIndex;

            textBox1.Text = dataGridViewOrders[1, index].Value.ToString();
            textBox2.Text = dataGridViewOrders[2, index].Value.ToString();
            try
            {
                DateTime date1 = DateTime.Parse(dataGridViewOrders[3, index].Value.ToString());
                maskedTextBox1.Text = date1.ToString("yyyy-MM-dd HH:mm:ss");
                DateTime date2 = DateTime.Parse(dataGridViewOrders[4, index].Value.ToString());
                maskedTextBox2.Text = date2.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch { }
            textBox3.Text = dataGridViewOrders[5, index].Value.ToString();

            textBox4.Text = dataGridViewOrders[0, index].Value.ToString();
            textBox5.Text = dataGridViewOrders[1, index].Value.ToString();
            textBox6.Text = dataGridViewOrders[2, index].Value.ToString();
            try
            {
                DateTime date3 = DateTime.Parse(dataGridViewOrders[3, index].Value.ToString());
                maskedTextBox3.Text = date3.ToString("yyyy-MM-dd HH:mm:ss");
                DateTime date4 = DateTime.Parse(dataGridViewOrders[4, index].Value.ToString());
                maskedTextBox4.Text = date4.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch { }
            textBox7.Text = dataGridViewOrders[5, index].Value.ToString();

            textBox8.Text = dataGridViewOrders[0, index].Value.ToString();
        }
    }
}
