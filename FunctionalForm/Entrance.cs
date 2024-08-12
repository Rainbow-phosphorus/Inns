using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Inns
{
    public partial class Entrance : Form
    {
        public Entrance()
        {
            InitializeComponent();
        }

        SqlConnection connect;

        DataProtection DataProtection = new DataProtection();

        public bool proverca = true;

        private void Entrance_Load(object sender, EventArgs e)
        {
            connect = new SqlConnection();
        }

        private void buttonPasswordVisible_Click(object sender, EventArgs e)
        {
            if(textBox2.PasswordChar == '\0')
            {
                textBox2.PasswordChar = '*';
                buttonPasswordVisible.BackgroundImage = Properties.Resources.not_visible;
            }
            else
            {
                textBox2.PasswordChar = '\0';
                buttonPasswordVisible.BackgroundImage = Properties.Resources.visible;
            }
        }

        //обрабатывает нажатие на кнопку "Войти"
        private void buttonEntrance_Click(object sender, EventArgs e)
        {
            receivingData();
        }
        //ищет id сотрудника
        public void receivingData()
        {
            connect.SqlOpen();
            MySqlCommand mySqlCommand = new MySqlCommand("CALL Authorization (\"" + Convert.ToBase64String(DataProtection.PBKDF2Hash(textBox1.Text)) + "\"," +
                " \"" + Convert.ToBase64String(DataProtection.PBKDF2Hash(textBox2.Text)) + "\")", connect.connection);
            object command = mySqlCommand.ExecuteScalar();
            string sravId = "";
            if(command != null)
            {
                sravId = command.ToString();
            }
            connect.SqlClosing();
            transitionForm(sravId);
        }
        
        public string kaphaText;

        //определяет статус авторизации
        public void transitionForm(string srav)
        {
            if((proverca == true || textBox3.Text == kaphaText) & srav != "")
            {
                connect.SqlOpen();
                MySqlCommand cmd = new MySqlCommand("CALL Employee_position (\"" + srav + "\")", connect.connection);
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter();
                DataTable dataTable = new DataTable();
                mySqlDataAdapter.SelectCommand = cmd;
                mySqlDataAdapter.Fill(dataTable);
                connect.SqlClosing();
                string[] mas = new string[dataTable.Columns.Count];
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    mas[i] = DataProtection.Decrypt(dataTable.Rows[0][i].ToString());
                }

                if (mas[0] == "Менеджер")
                {
                    Manager manager = new Manager();
                    manager.userInformation(srav);
                    manager.Show();
                }
                else if (mas[0] == "Системный администратор")
                {
                    SystemAdministrator systemAdministrator = new SystemAdministrator();
                    systemAdministrator.userInformation(srav);
                    systemAdministrator.Show();
                }
                else if (mas[0] == "Администратор")
                {
                    ReceptionAdministrator receptionAdministrator = new ReceptionAdministrator();
                    receptionAdministrator.userInformation(srav);
                    receptionAdministrator.Show();
                }
            }
            else
            {
                MessageBox.Show("Авторизация не пройдена");
                Kapha();
            }
        }

        public void Kapha()
        {
            proverca = false;
            label3.Visible = true;
            textBox3.Visible = true;
            pictureBox1.Visible = true;
            buttonEntrance.Location = new Point(38, 172);

            //формирование капчи
            Random r = new Random();
            string s = "";
            for (int i = 0; i < 1; i++)
            {
                s += (char)(r.Next(65, 90));
            }

            kaphaText = Convert.ToString(r.Next(0, 9)) + s + Convert.ToString(r.Next(0, 9));
            Bitmap bitmap = new Bitmap(300, 300);
            Graphics g = Graphics.FromImage(bitmap);
            Pen pen = new Pen(Color.Red, r.Next(2, 4));
            g.DrawString(kaphaText, new Font("Arial", r.Next(12, 14)), Brushes.Black, 2, 2);
            g.DrawLine(pen, 0, r.Next(8, 15), r.Next(80, 101), r.Next(10, 31));
            Pen fum = new Pen(Color.Black);
            for (int i = 0; i <= 300; i++)
            {
                for (int j = 0; j <= 300; j++)
                {
                    int f = r.Next(0, 100);
                    if (f < 14)
                    {
                        g.DrawLine(fum, i, j, i + 1, j + 1);
                    }
                }
            }
            pictureBox1.Image = bitmap;
        }
        
    }
}
