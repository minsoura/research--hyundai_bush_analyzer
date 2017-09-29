using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.DataVisualization;
using System.Web;
using System.Configuration;


namespace bushDataBase
{
    public partial class Form3 : Form
    {
        static string connectionString2 = ConfigurationManager.ConnectionStrings["mocha_connection"].ConnectionString;
       // static string connectionString2 = "server=localhost;user id=root;  password=root;persistsecurityinfo=True;database=mocha_db";
        MySqlConnection conDatabase = new MySqlConnection(connectionString2);
        MySqlCommand cmdDataBase = new MySqlCommand();
        MySqlDataAdapter adapter = new MySqlDataAdapter();

        Button[] documentOpenButton;
     
        TextBox[] documentFileNameText;
        FlowLayoutPanel[] documentListPanel;

        String selectedX;
        String selectedY;
        String xValue;
        String yValue;
        String indexString;
        static string jsonDocString = "";
      
        public Form3(String selectedX, String xValue, String selectedY, String yValue)
        {
            this.selectedX = selectedX;
            this.selectedY = selectedY;
            this.xValue = xValue;
            this.yValue = yValue;
            Console.WriteLine("Accepted Values=>" + selectedX + ": " + xValue + " / " + selectedY + ": " + yValue);
            InitializeComponent();
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            // TODO: 이 코드는 데이터를 'mocha_dbDataSet.bush_table_real' 테이블에 로드합니다. 필요 시 이 코드를 이동하거나 제거할 수 있습니다.
            this.bush_table_realTableAdapter.Fill(this.mocha_dbDataSet.bush_table_real);
            bush_table_realBindingSource.DataSource = this.mocha_dbDataSet.bush_table_real;
            tableInitialization();
        
        }
        private void tableInitialization()
        {
            
            string Query = @"SELECT * FROM mocha_db.bush_table_real WHERE " + selectedX + " = " + xValue + " AND "  + selectedY + " = " + yValue ;
            Console.WriteLine(Query);
            try
            {
         
                cmdDataBase.Connection = conDatabase;
                cmdDataBase.CommandText = Query;
                adapter = new MySqlDataAdapter(cmdDataBase);
                DataTable tempTable = new DataTable();
                DataSet tempSet = new DataSet();
                adapter.Fill(tempTable);
                adapter.Fill(tempSet);
                conDatabase.Open();
                // int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                bush_table_realBindingSource.DataSource = tempTable;

               
                int numRowsUpdated = cmdDataBase.ExecuteNonQuery();

                Console.WriteLine("test in form 3 : String " + BushIDTextBox.Text.ToString());
                cmdDataBase.Parameters.Clear();
                conDatabase.Dispose();
                indexString =  BushIDTextBox.Text.ToString();

                conDatabase.Close();
                pictureInitialization();
                relatedFileInitialization();
                propertyComboBoxInitialization();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message in Form3", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void relatedFileInitialization()
        {
            documentPanel2.Controls.Clear();
            jsonDocString = "";
            string Query = "SELECT Documents FROM mocha_db.bush_table_real WHERE BushID= " + indexString.ToString();
            String jsonString = "";
            Console.WriteLine("insdie relatedFileInitialziation in form 3");
            try
            {

                MySqlConnection conDatabase2 = new MySqlConnection(connectionString2);
                MySqlCommand cmdDataBase2 = new MySqlCommand();
             
                cmdDataBase2.Connection = conDatabase2;
                cmdDataBase2.CommandText = Query;
                Console.WriteLine("indexString inside rF: " + indexString);
               
                conDatabase2.Open();
                int numRowsUpdated = cmdDataBase2.ExecuteNonQuery();
                MySqlDataReader reader = cmdDataBase2.ExecuteReader();


                if (reader.Read())
                {
                    jsonString = reader["Documents"].ToString();
                    Console.WriteLine("test in form 3 : documents string = " + jsonString);
                }


                cmdDataBase2.Parameters.Clear();
                conDatabase2.Dispose();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            if (!jsonString.Equals(""))
            {

                jsonDocString = jsonString;
                Console.WriteLine(jsonString);
                try
                {
                    var items = JsonConvert.DeserializeObject<List<Item>>(jsonString);

                    documentListPanel = new FlowLayoutPanel[items.Count];
                    documentFileNameText = new TextBox[items.Count];
                    documentOpenButton = new Button[items.Count];
                   

                    for (var i = 0; i < items.Count; i++)
                    {
                        documentFileNameText[i] = new TextBox();
                        string filePathString = items[i].filePath;
                        String trimmedFilePath = "";
                        if (filePathString.Contains("\\\\Users"))
                        {
                            trimmedFilePath = filePathString.Replace("\\\\Users", "\\Users");
                        }
                        else
                        {
                            trimmedFilePath = filePathString;
                        }
                        documentFileNameText[i].Text = items[i].fileName;
                        documentFileNameText[i].Name = trimmedFilePath;
                        documentFileNameText[i].Size = new System.Drawing.Size(150, 20);
                        documentFileNameText[i].BorderStyle = BorderStyle.None;

                        documentOpenButton[i] = new Button();
                        documentOpenButton[i].Size = new Size(50, 20);
                        documentOpenButton[i].Name = "" + i + "";
                        documentOpenButton[i].Text = "Open";
                        documentOpenButton[i].Font = new Font(Font.FontFamily, 7);
                        documentOpenButton[i].Click += documentOpens_Click;                                    

                        documentListPanel[i] = new FlowLayoutPanel();
                        documentListPanel[i].FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
                        documentListPanel[i].Size = new Size(300, 25);
                        documentListPanel[i].Controls.Add(documentFileNameText[i]);
                        documentListPanel[i].Controls.Add(documentOpenButton[i]);
                  
                        Console.WriteLine(items[i].fileName);
                        Console.WriteLine(i.ToString());
                        documentPanel2.Controls.Add(documentListPanel[i]);
                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }


            }
        }
        private void documentOpens_Click(object sender, EventArgs e)
        {
            documentButtonsFunction((Button)sender);

        }

        private void documentButtonsFunction(Button button)
        {
            try
            {
                int documentButtonIndex = Convert.ToInt32(button.Name);
                if (button.Text.Equals("Open"))
                {
                    String documentFilePath = documentFileNameText[documentButtonIndex].Name;
                    System.Diagnostics.Process.Start(@documentFilePath);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message");
            }
          
        }
        private void pictureInitialization()
        {
          

            try
            {
            

                string Query = @"SELECT Figures FROM mocha_db.bush_table_real WHERE BushID= @bushID ";
            
                Console.WriteLine("index String in pictureInitialization in form 3: " + indexString);

                // String indexString = this.bush_table_realBindingSource.Position.ToString();
                cmdDataBase.Connection = conDatabase;
                cmdDataBase.CommandText = Query;
                cmdDataBase.Parameters.AddWithValue("@bushID", indexString);
                conDatabase.Open();
                int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                MySqlDataReader reader = cmdDataBase.ExecuteReader();
                String filePath = "";
                if (reader.Read())
                {
                    filePath = reader["Figures"].ToString();
                    Console.WriteLine("Inside figrues:" + filePath);
                }

                cmdDataBase.Parameters.Clear();
                conDatabase.Dispose();


                if (!filePath.Equals(""))
                {
                    figurePicture2.Image = Image.FromFile(filePath);
                    figurePicture2.SizeMode = PictureBoxSizeMode.Zoom;
                
                }
             


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);

            }
        }

        private void bush_table_realBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bush_table_realBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.mocha_dbDataSet);

        }
        private void propertyComboBoxInitialization()
        {
            massComboBox.SelectedItem = "g";
            innerPipeRadiusComboBox.SelectedItem = "mm";
            innerPipeThicknessComboBox.SelectedItem = "mm";
            innerPipeLengthComboBox.SelectedItem = "mm";
            outerPipeRadiusComboBox.SelectedItem = "mm";
            outerPipeThicknessComboBox.SelectedItem = "mm";
            outerPipeLengthComboBox.SelectedItem = "mm";
            rubberRadialThicknessComboBox.SelectedItem = "mm";
            rubberAxialThicknessComboBox.SelectedItem = "mm"; ;
            angleToArmComboBox.SelectedItem = "degree";


        }
    }
}

public class Item
{
    public string filePath { get; set; }
    public string fileName { get; set; }
}

public class ListItem
{
    public string listID { get; set; }
    public string listValue { get; set; }
}



public class RootObject
{
    public List<Item> items { get; set; }
}
public class RootObjectList
{
    public List<ListItem> items { get; set; }
}