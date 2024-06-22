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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Inns.FunctionalForm
{
    public partial class Reservation : Form
    {
        public Reservation()
        {
            InitializeComponent();
        }

        SqlConnection connect;
        private void Reservation_Load(object sender, EventArgs e)
        {
            connect = new SqlConnection();
            Update();
        }

        private void Update()
        {
            connect.SqlOpen();
            MySqlCommand mySqlCommand = new MySqlCommand("CALL Select_reservation", connect.connection);
            MySqlDataAdapter my = new MySqlDataAdapter(mySqlCommand);
            DataTable dt = new DataTable();
            my.Fill(dt);
            dt.Columns[0].ColumnName = "Код";
            dt.Columns[1].ColumnName = "Код постояльца";
            dt.Columns[2].ColumnName = "Код номера";
            dt.Columns[3].ColumnName = "Дата и время заезда";
            dt.Columns[4].ColumnName = "Дата и время выезда";
            dt.Columns[5].ColumnName = "Предварительная цена";
            dataGridViewReservation.DataSource = dt;
            connect.SqlClosing();
        }

        //добавляет запись бронирования
        private void buttonAddReservation_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Добавить запись?", "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if(textBox1.Text != "" & textBox2.Text != "" & maskedTextBox1.Text != "" & maskedTextBox2.Text != "")
                {
                    if(int.TryParse(textBox1.Text, out int idGuest) & int.TryParse(textBox2.Text, out int idNomer))
                    {
                        string arrivalDateTime = maskedTextBox1.Text;
                        string departureDateTime = maskedTextBox2.Text;
                        if (provercaDate(idNomer, arrivalDateTime, departureDateTime, "app") == true)
                        {
                            double price = calculatePrice(idNomer, arrivalDateTime, departureDateTime);
                            label6.Text = price.ToString();
                            connect.SqlOpen();
                            MySqlCommand mySqlCommand = new MySqlCommand("CALL Insert_reservation (\"" + idGuest + "\", \"" + idNomer + "\", \"" + arrivalDateTime + "\", \"" + departureDateTime + "\", \"" + price + "\")", connect.connection);
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

        //редактирует запись бронирования
        private void buttonEditReservation_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Редактировать запись?", "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (textBox3.Text != "" & textBox4.Text != "" & textBox5.Text != "" & maskedTextBox3.Text != "" & maskedTextBox4.Text != "")
                {
                    if (int.TryParse(textBox3.Text, out int id) & int.TryParse(textBox4.Text, out int idGuest) & int.TryParse(textBox5.Text, out int idNomer))
                    {
                        string arrivalDateTime = maskedTextBox3.Text;
                        string departureDateTime = maskedTextBox4.Text;
                        if (provercaDate(idNomer, arrivalDateTime, departureDateTime, "edit") == true)
                        {
                            double price = calculatePrice(idNomer, arrivalDateTime, departureDateTime);
                            label13.Text = price.ToString();
                            connect.SqlOpen();
                            MySqlCommand mySqlCommand = new MySqlCommand("CALL Update_reservation (\"" + id + "\", \"" + idGuest + "\", \"" + idNomer + "\", \"" + arrivalDateTime + "\", \"" + departureDateTime + "\", \"" + price + "\")", connect.connection);
                            mySqlCommand.ExecuteNonQuery();
                            connect.SqlClosing();
                            Update();
                            MessageBox.Show("Запись отредактированна!", "TsManager");
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

        //считается цена
        public double calculatePrice(int idNomer, string arrivalDateTime, string departureDateTime)
        {
            //получили цену за один день в номере
            connect.SqlOpen();
            MySqlCommand mySqlCommand = new MySqlCommand("CALL Сalculate_price (\"" + idNomer + "\")", connect.connection);
            object Pprice = mySqlCommand.ExecuteScalar();
            double price = Convert.ToDouble(Pprice);
            connect.SqlClosing();

            //узнали количество дней пребывания
            DateTime dateStart = DateTime.Parse(arrivalDateTime);
            DateTime dateOut = DateTime.Parse(departureDateTime);

            TimeSpan difference = dateOut.Subtract(dateStart);
            double colDay = difference.Days;

            //расcчитали цену за проживание в номере 
            price = colDay * price;
            return price;
        }

        //проверка дат 
        public bool provercaDate(int idNomer, string arrivalDateTime, string departureDateTime, string tip)
        {
            DateTime compareDate1 = DateTime.Parse(arrivalDateTime);
            DateTime compareDate2 = DateTime.Parse(departureDateTime);
            if(compareDate1<compareDate2)
            {
                connect.SqlOpen();
                MySqlCommand mySqlCommand = new MySqlCommand("CALL Booking_dates (\"" + idNomer + "\")", connect.connection);
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
                DataTable dt = new DataTable();
                mySqlDataAdapter.Fill(dt);
                connect.SqlClosing();

                //список дат уже имеющихся в системе
                DateTime startDate = new DateTime();
                DateTime endDate = new DateTime();

                HashSet<DateTime> dateList = new HashSet<DateTime>();
                //определяется для какого действия работает метод
                if(tip == "app")
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
                else if(tip == "edit")
                {
                    string id = textBox3.Text;                                                                                                          
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
                    MessageBox.Show("Данный номер уже забронирован на этот периуд");
                    return false;
                }
                else
                {
                    MessageBox.Show("Дата не найдена в списке дат.");
                    TimeSpan difference = compareDate2.Subtract(compareDate1);
                    double colDay = difference.Days;
                    if (colDay <= 0)
                    {
                        MessageBox.Show("Периуд бронирвоания должен быть минимум 24 часа");
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

        //рассчитывает цену для добавления записи
        private void buttonСalculateAmount_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "" & maskedTextBox1.Text != "" & maskedTextBox2.Text != "")
            {
                if (int.TryParse(textBox2.Text, out int idNomer))
                {
                    string arrivalDateTime = maskedTextBox1.Text;
                    string departureDateTime = maskedTextBox2.Text;
                    label6.Text = Convert.ToString(calculatePrice(idNomer, arrivalDateTime, departureDateTime));
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
        //рассчитывает цену для редактирования записи 
        private void buttonСalculateAmountEdit_Click(object sender, EventArgs e)
        {
            if (textBox5.Text != "" & maskedTextBox3.Text != "" & maskedTextBox4.Text != "")
            {
                if(int.TryParse(textBox5.Text, out int idNomer))
                {
                    string arrivalDateTime = maskedTextBox3.Text;
                    string departureDateTime = maskedTextBox4.Text;
                    label13.Text = Convert.ToString(calculatePrice(idNomer, arrivalDateTime, departureDateTime));
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

        //удаление записи о бронировании
        private void buttonDeleteReservation_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Удалить запись?", "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if(textBox6.Text != "")
                {
                    if(int.TryParse(textBox6.Text, out int id))
                    {
                        connect.SqlOpen();
                        MySqlCommand mySqlCommand = new MySqlCommand("CALL Delete_reservation (\"" + id + "\")", connect.connection);
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

        private void dataGridViewReservation_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = dataGridViewReservation.CurrentCell.RowIndex;

            textBox1.Text = dataGridViewReservation[1, index].Value.ToString();
            textBox2.Text = dataGridViewReservation[2, index].Value.ToString();
            try
            {
                DateTime date1 = DateTime.Parse(dataGridViewReservation[3, index].Value.ToString());
                maskedTextBox1.Text = date1.ToString("yyyy-MM-dd HH:mm:ss");
                DateTime date2 = DateTime.Parse(dataGridViewReservation[4, index].Value.ToString());
                maskedTextBox2.Text = date2.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch { }

            textBox3.Text = dataGridViewReservation[0, index].Value.ToString();
            textBox4.Text = dataGridViewReservation[1, index].Value.ToString();
            textBox5.Text = dataGridViewReservation[2, index].Value.ToString();
            try
            {
                DateTime date3 = DateTime.Parse(dataGridViewReservation[3, index].Value.ToString());
                maskedTextBox3.Text = date3.ToString("yyyy-MM-dd HH:mm:ss");
                DateTime date4 = DateTime.Parse(dataGridViewReservation[4, index].Value.ToString());
                maskedTextBox4.Text = date4.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch { }

            textBox6.Text = dataGridViewReservation[0, index].Value.ToString();
        }

        private void Reservation_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void Reservation_FormClosing(object sender, FormClosingEventArgs e)
        {
            ReceptionAdministrator receptionAdministrator = Application.OpenForms.OfType<ReceptionAdministrator>().FirstOrDefault(); ;
            receptionAdministrator.parametersPanel();
        }
    }
}
