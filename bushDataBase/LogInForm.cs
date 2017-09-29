using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using MySql.Data.MySqlClient;
using System.Configuration;


namespace bushDataBase
{
    public partial class Activate : Form
    {
        static string firtSignInConnection = ConfigurationManager.ConnectionStrings["mocha_connection"].ConnectionString;
       // static string firtSignInConnection = "server=localhost;user id=root;  password=root;persistsecurityinfo=True;database=mocha_db";
        int numRowsUpdated;
        public Activate(Form1 tempForm1)
        {
            InitializeComponent();
            _parentForm = tempForm1;
            this.Focus();
            userNameTextBox.Select();
            userNameTextBox.Focus();
            
        }

        private Form1 _parentForm;
        
       
        private void Activate_User(object sender, EventArgs e)
        {
            ipDisplayTextBox.Text = GetComputer_LanIP();
        }
        private string GetComputer_LanIP()
        {
            string strHostName = System.Net.Dns.GetHostName();

            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

            foreach (IPAddress ipAddress in ipEntry.AddressList)
            {
                if (ipAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    return ipAddress.ToString();
                    
                }
            }

            return "-";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            connectToMain();
            
        }

        private void connectToMain()
        {
            if (string.IsNullOrEmpty(userNameTextBox.Text))
            {
                MessageBox.Show("이름을 입력해주세요", "Name Needed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
            else
            {
                saveNewUser();
                _parentForm.currentUserName = userNameTextBox.Text;
                _parentForm.activate_main_form();
                _parentForm.WindowState = FormWindowState.Normal;
            }
            
        }

        private void sign_in_by_enter(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)13)
            {
                connectToMain();

            }
        }


        private void saveNewUser()
        {
            
            string userName = userNameTextBox.Text.Trim();
            string userIP = GetComputer_LanIP();
            
            string userLastAccess = DateTime.Now.ToString();
           
            MySqlConnection conDatabaseBlock = new MySqlConnection(firtSignInConnection);
            MySqlCommand cmdDataBaseBlock = new MySqlCommand();
            MySqlDataAdapter adapterBlock = new MySqlDataAdapter();
            
            string Query = "INSERT INTO mocha_db.bush_sign_in (userName, userIP, userLastAccess, userExtraInfo1, userExtraInfo2, userExtraInfo3) Values ('" + userName +"', '" +userIP + "', '" + userLastAccess + "', '', '','')";

            Console.WriteLine(Query);
            cmdDataBaseBlock.Connection = conDatabaseBlock;
            cmdDataBaseBlock.CommandText = Query;
            

            conDatabaseBlock.Open();
            numRowsUpdated = cmdDataBaseBlock.ExecuteNonQuery();

            conDatabaseBlock.Dispose();
          
        }

        private void lgFormClosed(object sender, FormClosedEventArgs e)
        {
            if (numRowsUpdated>0)
            {

            }else
            {
                _parentForm.Close();
            }
            
        }
    }
}
