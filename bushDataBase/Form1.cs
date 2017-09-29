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
using System.Net;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;









namespace bushDataBase
{
    public partial class Form1 : Form
    {
        static string bushPictureLocation = ConfigurationManager.AppSettings["bushPictureLocation"];
        static string documentsPictureLocation = ConfigurationManager.AppSettings["documentsPictureLocation"];
        static string bushFileLocation = ConfigurationManager.AppSettings["bushFileLocation"];
        static string documentsFileLocation = ConfigurationManager.AppSettings["documentsFileLocation"];
        static string hikiPictureLocation = ConfigurationManager.AppSettings["hikiPictureLocation"];

        static string connectionString2 = ConfigurationManager.ConnectionStrings["mocha_connection"].ConnectionString;
        //static string connectionString2 = "server=localhost;user id=root;  password=root;persistsecurityinfo=True;database=mocha_db";
        MySqlConnection conDatabase = new MySqlConnection(connectionString2);
        MySqlCommand cmdDataBase = new MySqlCommand();
        MySqlDataAdapter adapter = new MySqlDataAdapter();

        MySqlConnection conDatabase_RP = new MySqlConnection(connectionString2);
        MySqlCommand cmdDataBase_RP = new MySqlCommand();
        MySqlDataAdapter adapter_RP = new MySqlDataAdapter();

        MySqlConnection conDatabase_RPSET = new MySqlConnection(connectionString2);
        MySqlCommand cmdDataBase_RPSET = new MySqlCommand();
        MySqlDataAdapter adapter_RPSET = new MySqlDataAdapter();

        Button[] documentOpenButton;
        Button[] documentDeleteButton;
        TextBox[] documentFileNameText;
        FlowLayoutPanel[] documentListPanel;

        Button[] documentOpenButton_RP; // RP stands for related projects. using the previous coding only with replacing the names
        Button[] documentDeleteButton_RP;
        TextBox[] documentFileNameText_RP;
        FlowLayoutPanel[] documentListPanel_RP;
        FlowLayoutPanel[] documentListButtonPanel_RP;

        static string jsonDocString = "";
        static string jsonDocString_RP = "";
        static Boolean isDocumentBeingRecorded = false;
        static Boolean isDocumentBeingRecorded_RP = false;
        static Boolean isDocumentBeingRecorded_HIKI = false;
        String parenthesisLeft = "{";
        String parenthesisRight = "}";
        String commaMark = ",";
        String doubleQuote = "\"";
        bool isForm2Activated = false;
        bool isForm3Activated = false;
        bool isSearchable = false;
        String xClickedValue = "";
        String yClickedValue = "";

        String mass = "";
        String innerPipeRadius = "";
        String innerPipeLength = "";
        String innerPipeThickness = "";

        String outerPipeRadius = "";
        String outerPipeLength = "";
        String outerPipeThickness = "";
        String rubberRadialThickness = "";
        String rubberAxialThickness = "";

        String angleToArm = "";

        ArrayList searchSetForCompanyList = new ArrayList();
        ArrayList searchSetForBushTypeList = new ArrayList();
        ArrayList searchSetForInnerPipeShapeList = new ArrayList();
        ArrayList searchSetForInnerPipeMaterialList = new ArrayList();
        ArrayList searchSetForOuterPipeShapeList = new ArrayList();
        ArrayList searchSetForOuterPipeMaterialList = new ArrayList();

     
        RootObjectChart chartValuesContainer = new RootObjectChart();
        String selectedX = "";
        String selectedY = "";
       
       
        BackgroundWorker taskWorker = new BackgroundWorker();

        BackgroundWorker taskWorker_RP = new BackgroundWorker();
        BackgroundWorker taskWorker_HIKI = new BackgroundWorker();

        String[] filterPropertiesArray = { "CompanyList", "BushTypeList", "InnerPipeShapeList", "InnerPipeMaterialList", "OuterPipeShapeList", "OuterPipeMaterialList" };
        ArrayList filterPropertiesArrayList = new ArrayList();
        Activate lgForm;
        public  String currentUserName = ".";
        String bushPicturePath = "";
        String documentsPicturePath = "";
        String hikiPicturePath = "";
        int enoughPointsX = 0;
        int enoughPointsY = 0;

        public Form1()
        {
            InitializeComponent();
            taskWorker.WorkerSupportsCancellation = true;
            taskWorker.WorkerReportsProgress = true;
            taskWorker.DoWork += new DoWorkEventHandler(taskWorker_DoWork);

            taskWorker_RP.WorkerSupportsCancellation = true;
            taskWorker_RP.WorkerReportsProgress = true;
            taskWorker_RP.DoWork += new DoWorkEventHandler(taskWorker_RP_DoWork);

            taskWorker_HIKI.WorkerSupportsCancellation = true;
            taskWorker_HIKI.WorkerReportsProgress = true;
            taskWorker_HIKI.DoWork += new DoWorkEventHandler(taskWorker_HIKI_DoWork);

        }
   
        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: 이 코드는 데이터를 'mocha_dbDataSet_RPSET.bush_table_doc_set' 테이블에 로드합니다. 필요 시 이 코드를 이동하거나 제거할 수 있습니다.
            this.bush_table_doc_setTableAdapter.Fill(this.mocha_dbDataSet_RPSET.bush_table_doc_set);
            // TODO: 이 코드는 데이터를 'mocha_dbDataSet_HIKI.bush_table_hiki' 테이블에 로드합니다. 필요 시 이 코드를 이동하거나 제거할 수 있습니다.
            this.bush_table_hikiTableAdapter.Fill(this.mocha_dbDataSet_HIKI.bush_table_hiki);
            // TODO: 이 코드는 데이터를 'mocha_dbDataSet_RP.bush_table_doc' 테이블에 로드합니다. 필요 시 이 코드를 이동하거나 제거할 수 있습니다.
            this.bush_table_docTableAdapter.Fill(this.mocha_dbDataSet_RP.bush_table_doc);
            this.bush_table_realTableAdapter.Fill(this.mocha_dbDataSet.bush_table_real);

            bush_table_realBindingSource.DataSource = this.mocha_dbDataSet.bush_table_real;
            bush_table_docBindingSource.DataSource = this.mocha_dbDataSet_RP.bush_table_doc;
            bush_table_hikiBindingSource.DataSource = this.mocha_dbDataSet_HIKI.bush_table_hiki;

            currentRowIndex.Text = this.bush_table_realBindingSource.Position.ToString();
          
            if (!this.bush_table_docBindingSource.Position.ToString().Equals("-1"))
            {
                currentRowIndex_RP.Text = this.bush_table_docBindingSource.Position.ToString();
            }

            if (!this.bush_table_realBindingSource.Position.ToString().Equals("-1"))
            {
                currentRowIndex.Text = this.bush_table_realBindingSource.Position.ToString();
            }

            initializePropertyValue();

            
            pictureInitialization();
            relatedFileInitialization();
            propertyComboBoxInitialization();
            textBoxReadOnlyControl(true);


            pictureDocInitialization();
            relatedDocFileInitialization();


            comboBoxDefaultInitialization();
            comboBoxValueInitialization();
            filterAllCheck();

            filterPanelInitialization();

            pictureHikiInitialization();
            chartValuesContainer.chartitem = new List<ChartItem>();
            xValuesComboBox.SelectedItem = "Mass";
            yValuesComboBox.SelectedItem = "BushID";
            this.analysisChart.GetToolTipText += this.analysisChart_GetToolTipText;            
            Shown += Form1_Shown;
            nextButton_RP.Enabled = true;
            previousButton_RP.Enabled = true;
            //TODO: delete this line after production
            //figurePicture.Image = Image.FromFile(bushPictureLocation + "t11.png");
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (checkRegistrationWithIP())
            {
                this.Enabled = true;
            }else
            {
                lgForm = new Activate(this);
                activate_user();
            }
            
        }
       
        private Boolean checkRegistrationWithIP()
        {
            Boolean isThisRegistered = true;
            MySqlConnection conDatabaseBlock = new MySqlConnection(connectionString2);
            MySqlCommand cmdDataBaseBlock = new MySqlCommand();
            MySqlDataAdapter adapterBlock = new MySqlDataAdapter();
            string Query = @"SELECT COUNT(*) FROM mocha_db.bush_sign_in WHERE userIP= @userIP ";

            MySqlConnection conDatabaseBlock2 = new MySqlConnection(connectionString2);
            MySqlCommand cmdDataBaseBlock2 = new MySqlCommand();
            MySqlDataAdapter adapterBlock2 = new MySqlDataAdapter();
            string Query2 = @"SELECT userName FROM mocha_db.bush_sign_in WHERE userIP= @userIP ";

            try
            {
                String indexString = GetComputer_LanIP();
                
                cmdDataBaseBlock.Connection = conDatabaseBlock;
                cmdDataBaseBlock.CommandText = Query;
                cmdDataBaseBlock.Parameters.AddWithValue("@userIP", indexString);
                conDatabaseBlock.Open();
                int numRowsUpdated = cmdDataBaseBlock.ExecuteNonQuery();
                String ipCountString = Convert.ToString(cmdDataBaseBlock.ExecuteScalar());
                int ipCount = Convert.ToInt32(ipCountString);
                Console.WriteLine("ipCountString" + ipCountString + " Your IP " + indexString);
                if (ipCount > 0 )
                {
                    cmdDataBaseBlock2.Connection = conDatabaseBlock2;
                    cmdDataBaseBlock2.CommandText = Query2;
                    cmdDataBaseBlock2.Parameters.AddWithValue("@userIP", indexString);
                    conDatabaseBlock2.Open();
                    int numRowsUpdated2 = cmdDataBaseBlock2.ExecuteNonQuery();

                    MySqlDataReader reader = cmdDataBaseBlock2.ExecuteReader();
                    String userName = "not";
                    while (reader.Read())
                    {
                        userName = reader["userName"].ToString();
                    }
                   
                    cmdDataBaseBlock2.Parameters.Clear();
                    conDatabaseBlock2.Dispose();

                    userName_BushPage.Text = userName;
                    userName_AnalysisPage.Text = userName;
                    userName_RP_Page.Text = userName;
                    userName_RP_II_Page.Text = userName;
                    userName_HIKI_Page.Text = userName;

                }
                else
                {
                    isThisRegistered = false;
                   // MessageBox.Show("not registered", "Message", MessageBoxButtons.OK);
                }
                
                cmdDataBaseBlock.Parameters.Clear();
                conDatabaseBlock.Dispose();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
            }
            return isThisRegistered;
        }
        public void activate_user()
        {
            lgForm.Show();
            lgForm.Activate();
            
            this.WindowState = FormWindowState.Minimized;
            lgForm.Focus();
            lgForm.userNameTextBox.Select();
            lgForm.userNameTextBox.Focus();
            
        }

        public void activate_main_form()
        {
            userName_BushPage.Text = currentUserName;
            userName_AnalysisPage.Text = currentUserName;
            userName_RP_Page.Text = currentUserName;
            userName_RP_II_Page.Text = currentUserName; 
            userName_HIKI_Page.Text = currentUserName;
            this.Enabled = true;
            lgForm.Close();
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
        /*
        private void comboBox_SelectedValuechanged(object sender, EventArgs e)
        {
            String sizeClass = sizeComboBox.SelectedItem.ToString();
            String typeClass = typeComboBox.SelectedItem.ToString();
            String carSuspensionFront = frontComboBox.SelectedItem.ToString();
            String carSuspensionRear = rearComboBox.SelectedItem.ToString();

            String carComparedSusFront = frontCompCombo.SelectedItem.ToString();
            String carComparedSusRear = rearCompCombo.SelectedItem.ToString();


            ComboBox tempCombo = (ComboBox)sender;
            if (tempCombo.Name.Equals("sizeComboBox"))
            {
                sizeComboBox.SelectedItem
            }
        }*/
        private void analysisChart_GetToolTipText(object sender, ToolTipEventArgs e)
        {
            switch (e.HitTestResult.ChartElementType)
            {
                case ChartElementType.DataPoint:
                    var dataPoint = e.HitTestResult.Series.Points[e.HitTestResult.PointIndex];
                   // e.Text = string.Format("X:\t{0}\nY:\t{1}", dataPoint.XValue, dataPoint.YValues[0]);
                    xClickedValue = dataPoint.XValue.ToString();
                    yClickedValue = dataPoint.YValues[0].ToString();
                   
                    break;
            }

        }
        private void xCombobox_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox tempCombo = (ComboBox)sender;
            selectedX = tempCombo.SelectedItem.ToString();
            Console.WriteLine("For X: " + selectedX);
            loadCharts();
        }

        private void yCombobox_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox tempCombo = (ComboBox)sender;
            selectedY = tempCombo.SelectedItem.ToString();
            Console.WriteLine(" / For Y : " + selectedY);
            loadCharts();
        }
        private void initializePropertyValue()
        {
            //this.bush_table_realTableAdapter.Fill(this.mocha_dbDataSet.bush_table_real);
           // bush_table_realBindingSource.DataSource = this.mocha_dbDataSet.bush_table_real;

            mass = massTextBox.Text.ToString();
            innerPipeRadius = innerPipeRadiusTextBox.Text.ToString();
            innerPipeLength = innerPipeLengthTextBox.Text.ToString();
            innerPipeThickness = innerPipeThicknessTextBox.Text.ToString();

            outerPipeRadius = outerPipeRadiusTextBox.Text.ToString();
            outerPipeLength = outerPipeLengthTextBox.Text.ToString();
            outerPipeThickness = outerPipeThicknessTextBox.Text.ToString();
            rubberRadialThickness = rubberRadialThicknessTextBox.Text.ToString();
            rubberAxialThickness = rubberAxialThicknessTextBox.Text.ToString();
            angleToArm = angleToArmTextBox.Text.ToString();
        }

        private void bringInitialPropertyValue()
        {
           // initializePropertyValue();
             massTextBox.Text = mass;
            innerPipeRadiusTextBox.Text = innerPipeRadius;
            innerPipeLengthTextBox.Text = innerPipeLength;
            innerPipeThicknessTextBox.Text = innerPipeThickness;

            outerPipeRadiusTextBox.Text = outerPipeRadius;
            outerPipeLengthTextBox.Text = outerPipeLength;
            outerPipeThicknessTextBox.Text = outerPipeThickness;
            rubberRadialThicknessTextBox.Text = rubberRadialThickness;
            rubberAxialThicknessTextBox.Text = rubberAxialThickness;
            angleToArmTextBox.Text = angleToArm;

            massComboBox.SelectedItem = "g";
            innerPipeRadiusComboBox.SelectedItem = "mm";
            innerPipeLengthComboBox.SelectedItem = "mm";
            innerPipeThicknessComboBox.SelectedItem = "mm";

            outerPipeRadiusComboBox.SelectedItem = "mm";
            outerPipeLengthComboBox.SelectedItem = "mm";
            outerPipeThicknessComboBox.SelectedItem = "mm";
            rubberRadialThicknessComboBox.SelectedItem = "mm";
            rubberAxialThicknessComboBox.SelectedItem = "mm";

            angleToArmComboBox.SelectedItem = "degree";
        }


        private void bushPropValue_SelectedUnitChanged(object sender, EventArgs e)
        {
            ComboBox tempCombo = (ComboBox)sender;
            try
            {
                switch (tempCombo.Name.ToString())
                {
                    case "massComboBox":
                        if (tempCombo.SelectedItem.ToString().Equals("g"))
                        {
                            massTextBox.Text = mass;
                        }
                        else
                        {
                            massTextBox.Text = Convert.ToString(Convert.ToInt32(mass) * 0.00220462);
                        }
                        break;
                    case "innerPipeRadiusComboBox":
                        if (tempCombo.SelectedItem.ToString().Equals("mm"))
                        {
                            innerPipeRadiusTextBox.Text = innerPipeRadius;
                        }
                        else
                        {
                            innerPipeRadiusTextBox.Text = Convert.ToString(Convert.ToInt32(innerPipeRadius) * 0.0393701);
                        }
                        break;
                    case "innerPipeLengthComboBox":
                        if (tempCombo.SelectedItem.ToString().Equals("mm"))
                        {
                            innerPipeLengthTextBox.Text = innerPipeLength;
                        }
                        else
                        {
                            innerPipeLengthTextBox.Text = Convert.ToString(Convert.ToInt32(innerPipeLength) * 0.0393701);
                        }
                        break;
                    case "innerPipeThicknessComboBox":
                        if (tempCombo.SelectedItem.ToString().Equals("mm"))
                        {
                            innerPipeThicknessTextBox.Text = innerPipeThickness;
                        }
                        else
                        {
                            innerPipeThicknessTextBox.Text = Convert.ToString(Convert.ToInt32(innerPipeThickness) * 0.0393701);
                        }
                        break;
                    case "outerPipeRadiusComboBox":
                        if (tempCombo.SelectedItem.ToString().Equals("mm"))
                        {
                            outerPipeRadiusTextBox.Text = outerPipeRadius;
                        }
                        else
                        {
                            outerPipeRadiusTextBox.Text = Convert.ToString(Convert.ToInt32(outerPipeRadius) * 0.0393701);
                        }
                        break;
                    case "outerPipeLengthComboBox":
                        if (tempCombo.SelectedItem.ToString().Equals("mm"))
                        {
                            outerPipeLengthTextBox.Text = outerPipeLength;
                        }
                        else
                        {
                            outerPipeLengthTextBox.Text = Convert.ToString(Convert.ToInt32(outerPipeLength) * 0.0393701);
                        }
                        break;
                    case "outerPipeThicknessComboBox":
                        if (tempCombo.SelectedItem.ToString().Equals("mm"))
                        {
                            outerPipeThicknessTextBox.Text = outerPipeThickness;
                        }
                        else
                        {
                            outerPipeThicknessTextBox.Text = Convert.ToString(Convert.ToInt32(outerPipeThickness) * 0.0393701);
                        }
                        break;
                    case "rubberRadialThicknessComboBox":
                        if (tempCombo.SelectedItem.ToString().Equals("mm"))
                        {
                            rubberRadialThicknessTextBox.Text = rubberRadialThickness;
                        }
                        else
                        {
                            rubberRadialThicknessTextBox.Text = Convert.ToString(Convert.ToInt32(rubberRadialThickness) * 0.0393701);
                        }
                        break;
                    case "rubberAxialThicknessComboBox":
                        if (tempCombo.SelectedItem.ToString().Equals("mm"))
                        {
                            rubberAxialThicknessTextBox.Text = rubberAxialThickness;
                        }
                        else
                        {
                            rubberAxialThicknessTextBox.Text = Convert.ToString(Convert.ToInt32(rubberAxialThickness) * 0.0393701);
                        }
                        break;
                    case "AngleToArmComboBox":
                        if (tempCombo.SelectedItem.ToString().Equals("degree"))
                        {
                            angleToArmTextBox.Text = angleToArm;
                        }
                        else
                        {
                            angleToArmTextBox.Text = Convert.ToString(Convert.ToInt32(angleToArm) * 0.0174533);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            
               
        }
        private void filterAllCheck()
        {
            companyCheck.Checked = true;
            bushTypeCheck.Checked = true;
            innerShapeCheck.Checked = true;
            innerMaterialCheck.Checked = true;
            outerMaterialCheck.Checked = true;
            outerShapeCheck.Checked = true;
        }

        private void filterUncheckAll()
        {
            companyCheck.Checked = false;
            bushTypeCheck.Checked = false;
            innerShapeCheck.Checked = false;
            innerMaterialCheck.Checked = false;
            outerMaterialCheck.Checked = false;
            outerShapeCheck.Checked = false;
        }
        private void checkNumbersOfFilters()
        {
            filterPropertiesArrayList.Clear();            
            if (companyCheck.Checked)
            {
                filterPropertiesArrayList.Add("CompanyList");
               
            }
            if (bushTypeCheck.Checked)
            {
                filterPropertiesArrayList.Add("BushTypeList");
            }
            if (innerShapeCheck.Checked)
            {
                filterPropertiesArrayList.Add("InnerPipeShapeList");
            }
            if (innerMaterialCheck.Checked)
            {
                filterPropertiesArrayList.Add("InnerPipeMaterialList");
            }
            if (outerShapeCheck.Checked)
            {
                filterPropertiesArrayList.Add("OuterPipeShapeList");
            }
            if (outerMaterialCheck.Checked)
            {
                filterPropertiesArrayList.Add("OuterPipeMaterialList");
            }
        }
        private void filterPanelInitialization()
        {
            checkNumbersOfFilters();
           // filterPanel.Controls.Clear();
            String jsonValueListString = "";
            string Query = @"SELECT * FROM mocha_db.bush_table_valuelist ";
            ArrayList listValue = new ArrayList();
            FlowLayoutPanel[] filterListPanel;
            Label[] filterNameLabel;
            FlowLayoutPanel[] filterContainerPanel;// it contains one set of the filterNameLabel + filterListPanel

            try
            {
                cmdDataBase.Connection = conDatabase;
                cmdDataBase.CommandText = Query;
                conDatabase.Open();
                int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                MySqlDataReader reader = cmdDataBase.ExecuteReader();
                
                if (reader.Read())
                {
                    for (int i=0; i<filterPropertiesArrayList.Count; i++)
                    {   
                        listValue.Add(reader[filterPropertiesArrayList[i].ToString()].ToString());
                        Console.WriteLine("the list value of index " + i + " is " + listValue[i]);
                    }
                    
                }
                cmdDataBase.Parameters.Clear();
                conDatabase.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
            }
            filterContainerPanel = new FlowLayoutPanel[listValue.Count];
            filterListPanel = new FlowLayoutPanel[listValue.Count];
            filterNameLabel = new Label[listValue.Count];
           
            for (int j = 0; j < listValue.Count; j++)
            {
                if (!listValue[j].Equals(""))
                {
                    jsonValueListString = listValue[j].ToString();
                    try
                    {
                   
                        var items = JsonConvert.DeserializeObject<List<ListItem>>(jsonValueListString);
                        filterContainerPanel[j] = new FlowLayoutPanel();
                        filterContainerPanel[j].FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
                        //filterContainerPanel[j].Size = new Size(560, 100);
                        filterContainerPanel[j].AutoSize = true;
                        filterContainerPanel[j].MaximumSize = new Size(550, 0 );
                        filterContainerPanel[j].Margin = new Padding(5,5,5,5);

                        filterNameLabel[j] = new Label();
                        filterNameLabel[j].Text = filterPropertiesArrayList[j].ToString();
                        filterNameLabel[j].Name = listValue[j].ToString();
                        filterNameLabel[j].Size = new System.Drawing.Size(500, 25);
                        filterNameLabel[j].BorderStyle = BorderStyle.None;

                        CheckBox[] filterContentsCheckBox = new CheckBox[items.Count];
                        filterListPanel[j] = new FlowLayoutPanel();
                        filterListPanel[j].FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
                        //filterListPanel[j].Size = new Size(400, 60);
                        filterListPanel[j].AutoSize = true;
                        filterListPanel[j].MaximumSize = new Size(540, 0);              
                                          
                        for (var i = 0; i < items.Count; i++)
                        {                                                      
                            filterContentsCheckBox[i] = new CheckBox();                            
                            filterContentsCheckBox[i].Size = new System.Drawing.Size(100, 22);
                            filterContentsCheckBox[i].Text = items[i].listValue;
                            filterContentsCheckBox[i].Name = filterPropertiesArrayList[j].ToString();
                            filterContentsCheckBox[i].CheckedChanged += new EventHandler(filterContentsCheckBox_CheckChanged);
                            filterListPanel[j].Controls.Add(filterContentsCheckBox[i]);                                                        
                        }           
                        filterContainerPanel[j].Controls.Add(filterNameLabel[j]);
                        filterContainerPanel[j].Controls.Add(filterListPanel[j]);
                        filterPanel.Controls.Add(filterContainerPanel[j]);                        
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
                     }
                }
            }
        }

        private void filterContentsCheckBox_CheckChanged(object sender, EventArgs e)
        {
            int filterReady = 0;
            filterReady += isFilterContentsReady(searchSetForCompanyList);
            filterReady += isFilterContentsReady(searchSetForBushTypeList);
            filterReady += isFilterContentsReady(searchSetForInnerPipeShapeList);
            filterReady += isFilterContentsReady(searchSetForInnerPipeMaterialList);
            filterReady += isFilterContentsReady(searchSetForOuterPipeShapeList);
            filterReady += isFilterContentsReady(searchSetForOuterPipeMaterialList);

            if (filterReady !=0)
            {
                isSearchable = true;
            }
            filterContentsFunction((CheckBox)sender);
          
        }
        private int isFilterContentsReady(ArrayList filterSet)
        {
            return filterSet.Count;
        }
        private void filterContentsFunction(CheckBox checkbox)
        {
            if (checkbox.Checked)
            {
                foreach(string propertiesName in filterPropertiesArrayList)
                {
                    if (propertiesName == checkbox.Name)
                    {
                        if (propertiesName.Equals("CompanyList"))
                        {
                            searchSetForCompanyList.Add(checkbox.Text);
                           
                        }
                       else if (propertiesName.Equals("BushTypeList"))
                        {
                            searchSetForBushTypeList.Add(checkbox.Text);
                            
                        }
                       else if (propertiesName.Equals("InnerPipeShapeList"))
                        {
                            searchSetForInnerPipeShapeList.Add(checkbox.Text);
                          
                        }
                       else if (propertiesName.Equals("InnerPipeMaterialList"))
                        {
                            searchSetForInnerPipeMaterialList.Add(checkbox.Text);
                            
                        }
                       else if (propertiesName.Equals("OuterPipeShapeList"))
                        {
                            searchSetForOuterPipeShapeList.Add(checkbox.Text);
                           
                        }
                       else if (propertiesName.Equals("OuterPipeMaterialList"))
                        {
                            searchSetForOuterPipeMaterialList.Add(checkbox.Text);
                          
                        }
                    }
                }
                
            }
            else
            {
                foreach (string propertiesName in filterPropertiesArrayList)
                {
                    if (propertiesName == checkbox.Name)
                    {
                        if (propertiesName.Equals("CompanyList"))
                        {
                            searchSetForCompanyList.Remove(checkbox.Text);
                            
                        }
                        else if (propertiesName.Equals("BushTypeList"))
                        {
                            searchSetForBushTypeList.Remove(checkbox.Text);
                            
                        }
                        else if (propertiesName.Equals("InnerPipeShapeList"))
                        {
                            searchSetForInnerPipeShapeList.Remove(checkbox.Text);
                            
                        }
                        else if (propertiesName.Equals("InnerPipeMaterialList"))
                        {
                            searchSetForInnerPipeMaterialList.Remove(checkbox.Text);
                            
                        }
                        else if (propertiesName.Equals("OuterPipeShapeList"))
                        {
                            searchSetForOuterPipeShapeList.Remove(checkbox.Text);
                            
                        }
                        else if (propertiesName.Equals("OuterPipeMaterialList"))
                        {
                            searchSetForOuterPipeMaterialList.Remove(checkbox.Text);
                           
                        }
                    }

                  
                }
            }
            /*
            Console.WriteLine("===============");
            foreach (string items in searchSetForCompanyList)
            {
                Console.WriteLine(items);
            }
            Console.WriteLine("===============");
            foreach (string items in searchSetForBushTypeList)
            {
                Console.WriteLine(items);
            }
            Console.WriteLine("===============");
            foreach (string items in searchSetForInnerPipeShapeList)
            {
                Console.WriteLine(items);
            }
            Console.WriteLine("===============");
            foreach (string items in searchSetForInnerPipeMaterialList)
            {
                Console.WriteLine(items);
            }
            Console.WriteLine("===============");
            foreach (string items in searchSetForOuterPipeShapeList)
            {
                Console.WriteLine(items);
            }
            Console.WriteLine("===============");
            foreach (string items in searchSetForOuterPipeMaterialList)
            {
                Console.WriteLine(items);
            }
            Console.WriteLine("===============");
            */
         

            Console.WriteLine("===============");
            foreach (string items in searchSetForCompanyList)
            {
                Console.WriteLine(items);
            }
            Console.WriteLine("===============");
            foreach (string items in searchSetForBushTypeList)
            {
                Console.WriteLine(items);
            }
            Console.WriteLine("===============");
            foreach (string items in searchSetForInnerPipeShapeList)
            {
                Console.WriteLine(items);
            }
            Console.WriteLine("===============");
            foreach (string items in searchSetForInnerPipeMaterialList)
            {
                Console.WriteLine(items);
            }
            Console.WriteLine("===============");
            foreach (string items in searchSetForOuterPipeShapeList)
            {
                Console.WriteLine(items);
            }
            Console.WriteLine("===============");
            foreach (string items in searchSetForOuterPipeMaterialList)
            {
                Console.WriteLine(items);
            }
            Console.WriteLine("===============");


        }
      

        private void relatedFileInitialization()
        {
            documentPanel.Controls.Clear();
            jsonDocString = "";
            string Query = @"SELECT Documents FROM mocha_db.bush_table_real WHERE BushID= @bushID ";
            String jsonString = "";
            
            try
            {

                 String indexString = ((DataRowView)this.bush_table_realBindingSource.Current).Row["BushID"].ToString();
                //String indexString = this.bush_table_realBindingSource.Position.ToString();
                cmdDataBase.Connection = conDatabase;
                cmdDataBase.CommandText = Query;
                cmdDataBase.Parameters.AddWithValue("@bushID", indexString);
                conDatabase.Open();
                int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                MySqlDataReader reader = cmdDataBase.ExecuteReader();
 
                
                if (reader.Read())
                {
                    jsonString = reader["Documents"].ToString();
                }
    
         
                cmdDataBase.Parameters.Clear();
                conDatabase.Dispose();

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
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
                    documentDeleteButton = new Button[items.Count];

                    for (var i=0; i<items.Count; i++)
                    {
                        documentFileNameText[i] = new TextBox();
                       
                        string filePathString = items[i].filePath;
                        String trimmedFilePath = "";
                        if (filePathString.Contains("\\\\Users")){
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

                        documentDeleteButton[i] = new Button();
                        documentDeleteButton[i].Size = new Size(50, 20);
                        documentDeleteButton[i].Name = "" + i + "";
                        documentDeleteButton[i].Text = "Delete";
                        documentDeleteButton[i].Font = new Font(Font.FontFamily, 7);
                        documentDeleteButton[i].Click += documentOpens_Click;

                        documentListPanel[i] = new FlowLayoutPanel();
                        documentListPanel[i].FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
                        documentListPanel[i].Size = new Size(300, 25);
                        documentListPanel[i].Controls.Add(documentFileNameText[i]);
                        documentListPanel[i].Controls.Add(documentOpenButton[i]);
                        documentListPanel[i].Controls.Add(documentDeleteButton[i]);
                        Console.WriteLine(items[i].fileName);
                        Console.WriteLine(i.ToString());
                        documentPanel.Controls.Add(documentListPanel[i]);
                    }

                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);

                }

              
            }
        }

        private void relatedDocFileInitialization()
        {
            panelRelatedDoc.Controls.Clear();
            jsonDocString_RP = "";
            string Query = @"SELECT RelatedDoc FROM mocha_db.bush_table_doc WHERE DocID= @docID ";
            String jsonString_RP = "";

            try
            {
                String indexString = ((DataRowView)this.bush_table_docBindingSource.Current).Row["DocID"].ToString();
                cmdDataBase.Connection = conDatabase;
                cmdDataBase.CommandText = Query;

                Console.WriteLine("debugging:indexString: " + indexString);
                Console.WriteLine("before:" + jsonString_RP);
                cmdDataBase.Parameters.AddWithValue("@docID", indexString);
                conDatabase.Open();
                int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                MySqlDataReader reader = cmdDataBase.ExecuteReader();


                if (reader.Read())
                {
                    jsonString_RP = reader["RelatedDoc"].ToString();
                }

                Console.WriteLine("after:"+jsonString_RP);
                cmdDataBase.Parameters.Clear();
                conDatabase.Dispose();

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
            }


            if (!jsonString_RP.Equals(""))
            {

                jsonDocString_RP = jsonString_RP;
                Console.WriteLine("RP" +jsonString_RP);
                try
                {
                    var items = JsonConvert.DeserializeObject<List<Item>>(jsonString_RP);

                    documentListPanel_RP = new FlowLayoutPanel[items.Count];
                    documentFileNameText_RP = new TextBox[items.Count];
                    documentOpenButton_RP = new Button[items.Count];
                    documentDeleteButton_RP = new Button[items.Count];
                    documentListButtonPanel_RP = new FlowLayoutPanel[items.Count];
                    

                    for (var i = 0; i < items.Count; i++)
                    {
                        documentFileNameText_RP[i] = new TextBox();
                        String filePathString = items[i].filePath;
                        String trimmedFilePath = "";
                        if (filePathString.Contains("\\\\Users"))
                        {
                            trimmedFilePath = filePathString.Replace("\\\\Users", "\\Users");
                        }
                        else
                        {
                            trimmedFilePath = filePathString;
                        }
                                             
                        documentFileNameText_RP[i].Text = items[i].fileName;
                        documentFileNameText_RP[i].Name = trimmedFilePath;
                        documentFileNameText_RP[i].Font = new Font(Font.FontFamily, 7);
                        documentFileNameText_RP[i].Size = new System.Drawing.Size(150, 20);
                        documentFileNameText_RP[i].BorderStyle = BorderStyle.None;
                        documentFileNameText_RP[i].Margin = new Padding(0,0,0,0);

                        documentOpenButton_RP[i] = new Button();
                        documentOpenButton_RP[i].Size = new Size(50, 20);
                        documentOpenButton_RP[i].Name = "" + i + "";
                        documentOpenButton_RP[i].Text = "Open";
                        documentOpenButton_RP[i].Font = new Font(Font.FontFamily, 7);
                        documentOpenButton_RP[i].BackColor = Color.LightSteelBlue;
                        documentOpenButton_RP[i].Margin = new Padding(0, 0, 0, 0);
                        documentOpenButton_RP[i].Click += documentOpens_RP_Click;

                        documentDeleteButton_RP[i] = new Button();
                        documentDeleteButton_RP[i].Size = new Size(50, 20);
                        documentDeleteButton_RP[i].Name = "" + i + "";
                        documentDeleteButton_RP[i].Text = "Delete";
                        documentDeleteButton_RP[i].Font = new Font(Font.FontFamily, 7);
                        documentDeleteButton_RP[i].BackColor = Color.LightSteelBlue;
                        documentDeleteButton_RP[i].Margin = new Padding(0, 0, 0, 0);
                        documentDeleteButton_RP[i].Click += documentOpens_RP_Click;

                        documentListPanel_RP[i] = new FlowLayoutPanel();
                        documentListPanel_RP[i].FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
                        documentListPanel_RP[i].Size = new Size(180, 55);
                        documentListPanel_RP[i].Padding = new Padding(0, 0, 0, 0);

                        documentListButtonPanel_RP[i] = new FlowLayoutPanel();
                        documentListButtonPanel_RP[i].FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
                        documentListButtonPanel_RP[i].Size = new Size(150, 20);
                        documentListButtonPanel_RP[i].Padding = new Padding(0, 0, 0, 0);


                        documentListButtonPanel_RP[i].Controls.Add(documentOpenButton_RP[i]);
                        documentListButtonPanel_RP[i].Controls.Add(documentDeleteButton_RP[i]);

                        documentListPanel_RP[i].Controls.Add(documentFileNameText_RP[i]);
                        documentListPanel_RP[i].Controls.Add(documentListButtonPanel_RP[i]);
                        
                        Console.WriteLine(items[i].fileName);
                        Console.WriteLine(i.ToString());
                        panelRelatedDoc.Controls.Add(documentListPanel_RP[i]);
                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);

                }


            }
        }

        private void documentOpens_Click(object sender, EventArgs e)

        {
            documentButtonsFunction((Button)sender);
                        
        }

        private void documentButtonsFunction(Button button)
        {
            try{
                int documentButtonIndex = Convert.ToInt32(button.Name);
                if (button.Text.Equals("Open"))
                {
                    String documentFilePath = documentFileNameText[documentButtonIndex].Name;
                    System.Diagnostics.Process.Start(@documentFilePath);
                }
                else
                {
                    String indexString = ((DataRowView)this.bush_table_realBindingSource.Current).Row["BushID"].ToString();
                    removeDocumentAtIndexOf(indexString, documentButtonIndex);
                    // documentListPanel[documentButtonIndex].Controls.Remove(documentOpenButton[documentButtonIndex]);
                    //TODO: function that makes a D query from the database and deletes the value in the jsonString for documentRelatedFile
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Message");
            }
            
        }

        private void documentOpens_RP_Click(object sender, EventArgs e)
        {
            documentButtonsFunction_RP((Button)sender);

        }

        private void documentButtonsFunction_RP(Button button)
        {
            try
            {
                int documentButtonIndex = Convert.ToInt32(button.Name);
                if (button.Text.Equals("Open"))
                {
                    String documentFilePath = documentFileNameText_RP[documentButtonIndex].Name;
                    System.Diagnostics.Process.Start(@documentFilePath);
                }
                else
                {
                    String indexString = ((DataRowView)this.bush_table_docBindingSource.Current).Row["DocID"].ToString();
                    removeDocumentAtIndexOf_RP(indexString, documentButtonIndex);
                    // documentListPanel[documentButtonIndex].Controls.Remove(documentOpenButton[documentButtonIndex]);
                    //TODO: function that makes a D query from the database and deletes the value in the jsonString for documentRelatedFile
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Message");
            }
           
        }

        private void removeDocumentAtIndexOf(string rowIndex, int documentIndex)
        {
                string relatedDocString = editJsonStringDelete(documentIndex, jsonDocString);
                string Query = @"UPDATE mocha_db.bush_table_real SET Documents = @jsonFileString WHERE BushID= @bushID ";
                cmdDataBase.Connection = conDatabase;
                cmdDataBase.CommandText = Query;
                cmdDataBase.Parameters.AddWithValue("@jsonFileString", relatedDocString);
                cmdDataBase.Parameters.AddWithValue("@bushID", rowIndex);
                conDatabase.Open();
                int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                conDatabase.Dispose();
                cmdDataBase.Parameters.Clear();
                

            relatedFileInitialization();
        }
        private void removeDocumentAtIndexOf_RP(string rowIndex, int documentIndex)
        {
            string relatedDocString = editJsonStringDelete(documentIndex, jsonDocString_RP);
            string Query = @"UPDATE mocha_db.bush_table_doc SET RelatedDoc = @jsonFileString WHERE DocID= @docID ";
            cmdDataBase.Connection = conDatabase;
            cmdDataBase.CommandText = Query;
            cmdDataBase.Parameters.AddWithValue("@jsonFileString", relatedDocString);
            cmdDataBase.Parameters.AddWithValue("@docID", rowIndex);
            conDatabase.Open();
            int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
            conDatabase.Dispose();
            cmdDataBase.Parameters.Clear();
           

            relatedDocFileInitialization();
        }

        private string editJsonStringDelete(int documentIndex, String inputString)
        {
            var serializer = new JavaScriptSerializer();
            var finalJsonEditedString = "";
            
            try
            {
                var items = JsonConvert.DeserializeObject<List<Item>>(inputString);
                var jsonEditedString = new RootObject();
                 jsonEditedString.items = new List<Item>();
                

                for (var i=0; i<items.Count; i++)
                {
                    if (i != documentIndex)
                    {
                    Item fileCollection = new Item();
                    fileCollection.fileName = items[i].fileName;
                    fileCollection.filePath = items[i].filePath;
                    jsonEditedString.items.Add(fileCollection);
                    }
              
                }

                finalJsonEditedString= serializer.Serialize(jsonEditedString.items);     
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);

            }
            return finalJsonEditedString;
        }


        private void bush_tableBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        
        private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this.Validate();
            this.bush_table_realBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.mocha_dbDataSet);
        }

        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView tempDataGridView = (DataGridView)sender;

           
            switch (tempDataGridView.Name.ToString())
            {
                case "bush_table_realDataGridView":
                    if (figurePicture.Image != null)
                    {
                        figurePicture.Image.Dispose();
                        figurePicture.Image = null;
                    }
                    pictureInitialization();
                    relatedFileInitialization();
                    initializePropertyValue();

                    break;
                    
                case "bush_table_docDataGridView":
                    if (pictureBoxDoc.Image != null)
                    {
                        pictureBoxDoc.Image.Dispose();
                        pictureBoxDoc.Image = null;
                    }
                    pictureDocInitialization();
                    relatedDocFileInitialization();
                    break;

                case "bush_table_hikiDataGridView":
                   
                    if (pictureBoxHiki.Image != null)
                    {
                        pictureBoxHiki.Image.Dispose();
                        pictureBoxHiki.Image = null;
                    }
                    pictureHikiInitialization();
                    break;
                case "dataGridViewSet":
                    docEdit.SelectedIndex = 3;
                    if (pictureBoxHiki.Image != null)
                    {
                        pictureBoxHiki.Image.Dispose();
                        pictureBoxHiki.Image = null;
                    }
                    pictureHikiInitialization();
                    break;
            }
            
                  
        }

      

        private void One_Click(object sender, EventArgs e)
        {

        }

        private void bush_tableBindingNavigatorSaveItem_Click_1(object sender, EventArgs e)
        {
            this.Validate();
            this.bush_table_realBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.mocha_dbDataSet);

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void textBoxReadOnlyControl(Boolean onOroff)
        {
            if (onOroff)
            {
                companyTextBox.ReadOnly = true;
                bushNameTextBox.ReadOnly = true;
                bushTypeTextBox.ReadOnly = true;
                massTextBox.ReadOnly = true;
                carModelName.ReadOnly = true;
                innerPipeShapeTextBox.ReadOnly = true;
                innerPipeMaterialTextBox.ReadOnly = true;
                outerPipeShapeTextBox.ReadOnly = true;
                outerPipeMaterialTextBox.ReadOnly = true;
                innerPipeRadiusTextBox.ReadOnly = true; 
                innerPipeLengthTextBox.ReadOnly = true;
                innerPipeThicknessTextBox.ReadOnly = true;
                outerPipeRadiusTextBox.ReadOnly = true;
                outerPipeLengthTextBox.ReadOnly = true;
                outerPipeThicknessTextBox.ReadOnly = true;
                rubberRadialThicknessTextBox.ReadOnly = true;
                rubberAxialThicknessTextBox.ReadOnly = true;                           
                angleToArmTextBox.ReadOnly = true;
                remarkTextBox.ReadOnly = true;
                


            }
            else
            {
                companyTextBox.ReadOnly = false;
                bushNameTextBox.ReadOnly = false;
                bushTypeTextBox.ReadOnly = false;
                massTextBox.ReadOnly = false;
                carModelName.ReadOnly = false;
                innerPipeShapeTextBox.ReadOnly = false;
                innerPipeMaterialTextBox.ReadOnly = false;
                outerPipeShapeTextBox.ReadOnly = false;
                outerPipeMaterialTextBox.ReadOnly = false;
                innerPipeRadiusTextBox.ReadOnly = false;
                innerPipeLengthTextBox.ReadOnly = false;
                innerPipeThicknessTextBox.ReadOnly = false;
                outerPipeRadiusTextBox.ReadOnly = false;
                outerPipeLengthTextBox.ReadOnly = false;
                outerPipeThicknessTextBox.ReadOnly = false;
                rubberRadialThicknessTextBox.ReadOnly = false;
                rubberAxialThicknessTextBox.ReadOnly = false;               
                angleToArmTextBox.ReadOnly = false;
                remarkTextBox.ReadOnly = false;


            }
        }
        private void propertyNew_Click(object sender, EventArgs e)
        {
            textBoxReadOnlyControl(false);
              
            try
            {
                                 
                companyTextBox.Focus();
                this.mocha_dbDataSet.bush_table_real.Addbush_table_realRow(this.mocha_dbDataSet.bush_table_real.Newbush_table_realRow());
                bush_table_realBindingSource.MoveLast();
                currentRowIndex.Text = this.bush_table_realBindingSource.Position.ToString();
                isDocumentBeingRecorded = true;

                if (figurePicture.Image != null)
                {
                    figurePicture.Image.Dispose();
                    figurePicture.Image = null;
                }
                pictureInitialization();
                relatedFileInitialization();

               

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
            }
       
        }

        private void propertyEdit_Click(object sender, EventArgs e)
        {
            textBoxReadOnlyControl(false);
            companyTextBox.Focus();
            initializePropertyValue();
            bringInitialPropertyValue();
        }

        private void propertySave_Click(object sender, EventArgs e)
        {
            try
            {
                bush_table_realBindingSource.EndEdit();
                bush_table_realTableAdapter.Update(this.mocha_dbDataSet.bush_table_real);
                textBoxReadOnlyControl(true);
                isDocumentBeingRecorded = false;
                if (isTableListReady() && taskWorker.IsBusy != true)
                {
                    bringInitialPropertyValue();
                    taskWorker.RunWorkerAsync();
                }else
                {
                    Console.WriteLine("ListStorage is not Ready");
                    prepareTheListStorage();
                }

               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
            }

        }



        private void propertyDelete_Click(object sender, EventArgs e)
        {
            try
            {   if(MessageBox.Show("삭제하시겠습니까?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    bush_table_realBindingSource.RemoveCurrent();
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
            }


        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (!isDocumentBeingRecorded)
            {
                bush_table_realBindingSource.MoveNext();
                currentRowIndex.Text = this.bush_table_realBindingSource.Position.ToString();
                if (figurePicture.Image != null)
                {
                    figurePicture.Image.Dispose();
                    figurePicture.Image = null;
                }
                pictureInitialization();
               relatedFileInitialization();
                initializePropertyValue();
               
            }
            else
            {
                MessageBox.Show("저장을 먼저 해주세요.", "Message", MessageBoxButtons.OK);
            }
            textBoxReadOnlyControl(true);
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            if (!isDocumentBeingRecorded)
            {
                bush_table_realBindingSource.MovePrevious();
                currentRowIndex.Text = this.bush_table_realBindingSource.Position.ToString();

                if (figurePicture.Image != null)
                {
                    figurePicture.Image.Dispose();
                    figurePicture.Image = null;
                }
                pictureInitialization();
                relatedFileInitialization();
                initializePropertyValue();
            }else
            {
                MessageBox.Show("저장을 먼저 해주세요.", "Message", MessageBoxButtons.OK);
            }
            textBoxReadOnlyControl(true);
        }
        public static string TrimNewLines(string text)
        {
            while (text.EndsWith(Environment.NewLine))
            {
                text = text.Substring(0, text.Length - Environment.NewLine.Length);
            }
            return text;
        }
        
        private void searchTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            
           
            if (e.KeyChar == (char) 13)
            {
                
                TextBox tempTextBox = (TextBox)sender;
                String searchContents = tempTextBox.Text.Trim();


                searchContents = TrimNewLines(searchContents);
                Console.WriteLine("Search Text Contents:" + searchContents);
                searchContents_RP.Clear();
                searchTextBox.Clear();
                searchTextBox_RP.Clear();
                searchContents_RP.Text = String.Empty;

                if (string.IsNullOrEmpty(searchContents) || searchContents.Equals(""))
                {
                    
                }else
                {
                    try
                    {
                        
                        switch (tempTextBox.Name.ToString())
                        {
                            case "searchTextBox":
                                String[] searchItems = searchContents.Split('/');
                                Console.WriteLine("search Item Numbers " + searchItems.Count());
                                
                                if (searchItems.Length == 0)
                                {

                                } else if (searchItems.Count()== 1)
                                {
                                    Console.WriteLine("search Item[0]" + searchItems[0]);
                                  
                                    var query = from o in this.mocha_dbDataSet.bush_table_real
                                                where o.Company.Contains(searchItems[0].Trim()) || o.ModelName.Contains(searchItems[0].Trim()) || o.BushName.Contains(searchItems[0].Trim())
                                                select o;
                                    bush_table_realBindingSource.DataSource = query.CopyToDataTable();
                                    pictureInitialization();
                                    relatedFileInitialization();
                                }
                                else if (searchItems.Count()== 2)
                                {
                                    Console.WriteLine("search Item[0]" + searchItems[0]);
                                    Console.WriteLine("search Item[1]" + searchItems[1]);
                                   
                                    var query = from o in this.mocha_dbDataSet.bush_table_real
                                                where o.Company.Contains(searchItems[0].Trim()) && o.ModelName.Contains(searchItems[1].Trim())
                                                select o;
                                    bush_table_realBindingSource.DataSource = query.CopyToDataTable();
                                    pictureInitialization();
                                    relatedFileInitialization();
                                }
                                else if (searchItems.Count() == 3)
                                {
                                    Console.WriteLine("search Item[0]" + searchItems[0]);
                                    Console.WriteLine("search Item[1]" + searchItems[1]);
                                    Console.WriteLine("search Item[2]" + searchItems[2]);
                                    var query = from o in this.mocha_dbDataSet.bush_table_real
                                                where o.Company.Contains(searchItems[0].Trim()) && o.ModelName.Contains(searchItems[1].Trim()) && o.BushName.Contains(searchItems[2].Trim())
                                                select o;
                                    bush_table_realBindingSource.DataSource = query.CopyToDataTable();
                                    pictureInitialization();
                                    relatedFileInitialization();
                                }
                                else{

                                }
                              
                                break;

                            case "searchContents_RP":
                                var query4 = from o in this.mocha_dbDataSet_RP.bush_table_doc
                                             where o.DocName.Contains(searchContents) || o.DocType.Contains(searchContents) || o.Organization.Contains(searchContents) || o.SubjectCar.Contains(searchContents)
                                             select o;
                                bush_table_docBindingSource.DataSource = query4.CopyToDataTable();
                                searchContents_RP.Clear();
                                searchTextBox.Clear();
                                searchTextBox_RP.Clear();
                                searchContents_RP.Text = String.Empty;
                                break;

                            case "searchTextBox_RP":
                                var query2 = from o in this.mocha_dbDataSet_RP.bush_table_doc
                                            where o.DocName.Contains(searchContents) || o.DocType.Contains(searchContents) || o.Organization.Contains(searchContents) || o.SubjectCar.Contains(searchContents)
                                            select o;
                                bush_table_docBindingSource.DataSource = query2.CopyToDataTable();
                                pictureDocInitialization();
                                relatedDocFileInitialization();
                                break;

                            case "searchTextBox_Hiki":
                                var query3 = from o in this.mocha_dbDataSet_HIKI.bush_table_hiki
                                            where o.Title.Contains(searchContents) || o.Contents.Contains(searchContents) 
                                            select o;
                                bush_table_hikiBindingSource.DataSource = query3.CopyToDataTable();
                                pictureHikiInitialization();
                                break;
                        
                        }    
                                         
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
                        searchContents_RP.Clear();
                        searchTextBox.Clear();
                        searchTextBox_RP.Clear();
                    }

                  
                }
              

            }
        }

        private string dataBaseDirectory(String fullFilePath, String inputType)
        {   
            string fileName = System.IO.Path.GetFileName(fullFilePath);
            string targetDir = "";
            switch (inputType)
            {
                case "bushPicture":
                    targetDir = bushPictureLocation;
                    break;
                case "bushFile":
                    targetDir = bushFileLocation;
                    break;
                case "documentsPicture":
                    targetDir = documentsPictureLocation;
                    break;
                case "documentsFile":
                    targetDir = documentsFileLocation;
                    break;
                case "hikiPicture":
                    targetDir = hikiPictureLocation;
                    break;

            }
            string newFilePathForServerDir = targetDir + fileName;
            //TODO: copy to serverDir
            try
            {
                File.Copy(fullFilePath, newFilePathForServerDir);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
            }
           
            return newFilePathForServerDir;
        }
        




        private void figureAdd_Click(object sender, EventArgs e)
        {
            if (pictureFileDialog.ShowDialog() == DialogResult.OK)
            {
              
                String indexString = ((DataRowView)this.bush_table_realBindingSource.Current).Row["BushID"].ToString();
                try
                {
                    string sourceDir = dataBaseDirectory(pictureFileDialog.FileName, "bushPicture");
                    string Query = @"UPDATE mocha_db.bush_table_real SET Figures = @filePath WHERE BushID= @bushID ";
                    cmdDataBase.Connection = conDatabase;
                    cmdDataBase.CommandText = Query;
                    cmdDataBase.Parameters.AddWithValue("@filePath", sourceDir);
                    cmdDataBase.Parameters.AddWithValue("@bushID", indexString);
                    conDatabase.Open();
                    int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                    conDatabase.Dispose();
                    cmdDataBase.Parameters.Clear();
                    figurePicture.Image= Image.FromFile(sourceDir);
                    figurePicture.SizeMode = PictureBoxSizeMode.Zoom; //CenterImage/ Normal/AutoSize
                    figureEdit.Enabled = true;
                    figureAdd.Enabled = false;
                    figureDelete.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);

                }

            }
        }

        private void figureEdit_Click(object sender, EventArgs e)
        {
              
            if (pictureFileDialog.ShowDialog() == DialogResult.OK)
            {

                String indexString = ((DataRowView)this.bush_table_realBindingSource.Current).Row["bushID"].ToString();
                try
                {
                    string sourceDir = dataBaseDirectory(pictureFileDialog.FileName, "bushPicture");
                    string Query = @"UPDATE mocha_db.bush_table_real SET Figures = @filePath WHERE BushID= @bushID ";
                    cmdDataBase.Connection = conDatabase;
                    cmdDataBase.CommandText = Query;
                    cmdDataBase.Parameters.AddWithValue("@filePath", sourceDir);
                    cmdDataBase.Parameters.AddWithValue("@bushID", indexString);
                    conDatabase.Open();
                    int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                    conDatabase.Dispose();
                    cmdDataBase.Parameters.Clear();
                    figurePicture.Image = Image.FromFile(sourceDir);
                    figurePicture.SizeMode = PictureBoxSizeMode.Zoom; //CenterImage/ Normal/AutoSize
                    figureEdit.Enabled = true;
                    figureAdd.Enabled = false;
                    figureDelete.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);

                }

            }
        }

        private void pictureInitialization()
        {
            figureEdit.Enabled = true;
            figureAdd.Enabled = true;
            figureDelete.Enabled = true;

            try
            {
              

                string Query = @"SELECT Figures FROM mocha_db.bush_table_real WHERE BushID= @bushID ";
                DataRowView items = (DataRowView)this.bush_table_realBindingSource.List[bush_table_realBindingSource.Position];
                String indexString = items["BushID"].ToString();
                Console.WriteLine("index String for debugging: " + indexString);
               
               // String indexString = this.bush_table_realBindingSource.Position.ToString();
                cmdDataBase.Connection = conDatabase;
                cmdDataBase.CommandText = Query;
                cmdDataBase.Parameters.AddWithValue("@bushID", indexString);
                conDatabase.Open();
                int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                MySqlDataReader reader = cmdDataBase.ExecuteReader();
                
                if (reader.Read())
                {
                    bushPicturePath = reader["Figures"].ToString();
                }
              
                cmdDataBase.Parameters.Clear();
                conDatabase.Dispose();
             
                
                if (!bushPicturePath.Equals(""))
                {
                figurePicture.Image = Image.FromFile(bushPicturePath);
                figurePicture.SizeMode = PictureBoxSizeMode.Zoom;
                figureAdd.Enabled = false;
                }
                else
                {
                    figureEdit.Enabled = false;
                    figureDelete.Enabled = false;
                }
        
           
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
             
            }           
       }

        private void pictureDocInitialization()
        {
            docImageEdit.Enabled = true;
            docImageAdd.Enabled = true;
            docImageDelete.Enabled = true;

            try
            {
                string Query = @"SELECT RepresentiveImage FROM mocha_db.bush_table_doc WHERE DocID= @docID ";
                String indexString = ((DataRowView)this.bush_table_docBindingSource.Current).Row["DocID"].ToString();
                
                cmdDataBase.Connection = conDatabase;
                cmdDataBase.CommandText = Query;
                cmdDataBase.Parameters.AddWithValue("@docID", indexString);
                conDatabase.Open();
                int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
               MySqlDataReader reader = cmdDataBase.ExecuteReader();
                
                if (reader.Read())
                {
                    documentsPicturePath = reader["RepresentiveImage"].ToString();
                }

                cmdDataBase.Parameters.Clear();
                conDatabase.Dispose();
              


                if (!documentsPicturePath.Equals(""))
                {
                    pictureBoxDoc.Image = Image.FromFile(documentsPicturePath);
                    pictureBoxDoc.SizeMode = PictureBoxSizeMode.Zoom;
                    docImageAdd.Enabled = false;
                }
                else
                {
                    docImageEdit.Enabled = false;
                    docImageDelete.Enabled = false;
                }


            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
            }
        }

        private void figureDelete_Click(object sender, EventArgs e)
        {
            String indexString = ((DataRowView)this.bush_table_realBindingSource.Current).Row["bushID"].ToString();
            try
            {
                if (MessageBox.Show("그림을 삭제하시겠습니까?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string Query = @"UPDATE mocha_db.bush_table_real SET Figures = @filePath WHERE BushID= @bushID ";
                    cmdDataBase.Connection = conDatabase;
                    cmdDataBase.CommandText = Query;
                    cmdDataBase.Parameters.AddWithValue("@filePath", "");
                    cmdDataBase.Parameters.AddWithValue("@bushID", indexString);
                    conDatabase.Open();
                    int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                    conDatabase.Dispose();
                    cmdDataBase.Parameters.Clear();
                  
                    if (figurePicture.Image != null)
                    {
                        
                        figurePicture.Image.Dispose();
                        figurePicture.Image = null;

                        figureEdit.Enabled = false;
                        figureAdd.Enabled = true;
                        figureDelete.Enabled = false;
                    }

                }
                          
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);

            }
        }

        private void documentAdd_Click(object sender, EventArgs e)
        {
            if (documentFileDialog.ShowDialog() == DialogResult.OK)
            {
                String sourceDir = dataBaseDirectory(documentFileDialog.FileName, "documentsFile");
                String newFilePath = sourceDir;
                String newFileName = System.IO.Path.GetFileName(newFilePath);
                String relatedDocString = editJsonStringAdd(newFilePath, newFileName);
                
                String rowIndex = ((DataRowView)this.bush_table_realBindingSource.Current).Row["BushID"].ToString();

                string Query = @"UPDATE mocha_db.bush_table_real SET Documents = @jsonFileString WHERE BushID= @bushID ";
                cmdDataBase.Connection = conDatabase;
                cmdDataBase.CommandText = Query;
                cmdDataBase.Parameters.AddWithValue("@jsonFileString", relatedDocString);
                cmdDataBase.Parameters.AddWithValue("@bushID", rowIndex);
                conDatabase.Open();
                int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                conDatabase.Dispose();
                cmdDataBase.Parameters.Clear();
                relatedFileInitialization();
            }
            
        }

        private string editJsonStringAdd(String newFilePath, String newFileName)
        {
            var serializer = new JavaScriptSerializer();
            var finalJsonEditedString = "";

            try
            {   
                var jsonEditedString = new RootObject();
                jsonEditedString.items = new List<Item>();

                if (!jsonDocString.Equals(""))
                {
                    var items = JsonConvert.DeserializeObject<List<Item>>(jsonDocString);

                    for (var i = 0; i < items.Count; i++)
                    {                    
                            Item fileCollection = new Item();
                            fileCollection.fileName = items[i].fileName;
                            fileCollection.filePath = items[i].filePath;
                            jsonEditedString.items.Add(fileCollection);
                    
                    }

              
                }
                Item newFileCollection = new Item();
                newFileCollection.fileName = newFileName;
                newFileCollection.filePath = newFilePath;
                jsonEditedString.items.Add(newFileCollection);

                finalJsonEditedString = serializer.Serialize(jsonEditedString.items);
                Console.WriteLine(finalJsonEditedString);

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);

            }
            return finalJsonEditedString;
        }

        private string editJsonStringAdd_RP(String newFilePath, String newFileName)
        {
            var serializer = new JavaScriptSerializer();
            var finalJsonEditedString = "";

            try
            {
                var jsonEditedString = new RootObject();
                jsonEditedString.items = new List<Item>();

                if (!jsonDocString_RP.Equals(""))
                {
                    var items = JsonConvert.DeserializeObject<List<Item>>(jsonDocString_RP);

                    for (var i = 0; i < items.Count; i++)
                    {
                        Item fileCollection = new Item();
                        fileCollection.fileName = items[i].fileName;
                        fileCollection.filePath = items[i].filePath;
                        jsonEditedString.items.Add(fileCollection);

                    }


                }
                Item newFileCollection = new Item();
                newFileCollection.fileName = newFileName;
                newFileCollection.filePath = newFilePath;
                jsonEditedString.items.Add(newFileCollection);

                finalJsonEditedString = serializer.Serialize(jsonEditedString.items);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);

            }
            return finalJsonEditedString;
        }

        private string makeJsonAsysString_RP(String newAsysValue, String newAsysUnit)
        {
            var serializer = new JavaScriptSerializer();
            var finalJsonEditedString = "";

            try
            {
                var jsonEditedString = new AsysObject();
                jsonEditedString.items = new List<AsysItem>();                         
                                    
                AsysItem newCollection = new AsysItem();
                newCollection.asysValue = newAsysValue;
                newCollection.asysUnit = newAsysUnit;
                jsonEditedString.items.Add(newCollection);

                finalJsonEditedString = serializer.Serialize(jsonEditedString.items);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);

            }
            return finalJsonEditedString;
        }



        static void WriteArray(RootObject[] array)
        {
            foreach (object element in array)
            {
                if (element !=null)
                {
                    Console.WriteLine(element.ToString());
                    Console.WriteLine(element.GetType());
                    Console.WriteLine("---");
                }
            }
        }

        private void advancedLabel_Click(object sender, EventArgs e)
        {      
            if (!isForm2Activated)
            {
                Form2 filterForm = new Form2();
                isForm2Activated = true;
                filterForm.FormClosing += new FormClosingEventHandler(Form2_Closing);

                filterForm.Show();
                filterForm.Activate();
            }         
         }

         private void Form2_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isForm2Activated = false;

        }

        private void Form3_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isForm3Activated = false;

        }





        private void activateListCreator()
        {
           
            Console.WriteLine("listCreator");
            String makeCompanyList = "no";
            String makeBushTypeList = "no";
            String makeInnerPipeShapeList = "no";
            String makeInnerPipeMaterialList = "no";
            String makeOuterPipeShapeList = "no";
            String makeOuterPipeMaterialList = "no";
            
            if (!companyTextBox.Text.Equals(""))
            {
                makeCompanyList = "yes";
            }
            if (!bushTypeTextBox.Text.Equals(""))
            {
                makeBushTypeList = "yes";
            }
            if (!innerPipeShapeTextBox.Text.Equals(""))
            {
                makeInnerPipeShapeList = "yes";
            }
            if (!innerPipeMaterialTextBox.Text.Equals(""))
            {
                makeInnerPipeMaterialList = "yes";
            }
            if (!outerPipeShapeTextBox.Text.Equals(""))
            {
                makeOuterPipeShapeList = "yes";
            }
            if (!outerPipeMaterialTextBox.Text.Equals(""))
            {
                makeOuterPipeMaterialList = "yes";
            }

            listCreator(makeCompanyList, makeBushTypeList, makeInnerPipeShapeList, makeInnerPipeMaterialList, makeOuterPipeShapeList, makeOuterPipeMaterialList);
        }

    private void listCreator(String makeCompany, String makeBushType, String makeInnerPipeShape, String makeInnerPipeMaterial, String makeOuterPipeShape, String makeOuterPipeMaterial)
        {
            String rowIndex = ((DataRowView)this.bush_table_realBindingSource.Current).Row["BushID"].ToString();
            String bushID = rowIndex;
            String listValue = "empty";
            String parameterToUpdate = "empty";

            Console.WriteLine("makeCompany:" + makeCompany + " " + "  makeBushType:" + makeBushType + "  makeInnerPipeShape:" + makeInnerPipeShape + "  makeInnerPipeMaterial: " + makeInnerPipeMaterial + " makeOuterPipeShape: " + makeOuterPipeShape + "  makeOuterPipeMaterial: " + makeOuterPipeMaterial);

            if (makeCompany.Equals("yes"))
            {
                listValue = companyTextBox.Text;
                parameterToUpdate = "CompanyList";
                queryFunction(parameterToUpdate, bushID, listValue);
            }
            if (makeBushType.Equals("yes"))
            {
                listValue = bushTypeTextBox.Text;
                parameterToUpdate = "BushTypeList";
                queryFunction(parameterToUpdate, bushID, listValue);
            }
            if (makeInnerPipeShape.Equals("yes"))
            {
                listValue = innerPipeShapeTextBox.Text;
                parameterToUpdate = "InnerPipeShapeList";
                queryFunction(parameterToUpdate, bushID, listValue);
            }
            if (makeInnerPipeMaterial.Equals("yes"))
            {
                listValue = innerPipeMaterialTextBox.Text;
                parameterToUpdate = "InnerPipeMaterialList";
                queryFunction(parameterToUpdate, bushID, listValue);
            }
            if (makeOuterPipeShape.Equals("yes"))
            {
                listValue = outerPipeShapeTextBox.Text;
                parameterToUpdate = "OuterPipeShapeList";
                queryFunction(parameterToUpdate, bushID, listValue);
            }
            if (makeOuterPipeMaterial.Equals("yes"))
            {
                listValue = outerPipeMaterialTextBox.Text;
                parameterToUpdate = "outerPipeMaterialList";
                queryFunction(parameterToUpdate, bushID, listValue);
            }
            filterPanelInitialization();
                   
        }
          public void queryFunction(String queryParameter, String bushID_Func, String listValue_Func)
        {
            Console.WriteLine("Inside Query Function for: " + queryParameter);

            String jsonifiedString = editJsonStringListAdd(queryParameter,bushID_Func, listValue_Func);
            if (!jsonifiedString.Equals("duplicate"))
            {
                MySqlConnection conDatabaseBlock = new MySqlConnection(connectionString2);
                MySqlCommand cmdDataBaseBlock = new MySqlCommand();
                MySqlDataAdapter adapterBlock = new MySqlDataAdapter();
                string Query = @"UPDATE mocha_db.bush_table_valuelist SET " + queryParameter + " = @jsonListString ";
                cmdDataBaseBlock.Connection = conDatabaseBlock;
                cmdDataBaseBlock.CommandText = Query;
                cmdDataBaseBlock.Parameters.AddWithValue("@jsonListString", jsonifiedString);
            
                conDatabaseBlock.Open();
                int numRowsUpdated = cmdDataBaseBlock.ExecuteNonQuery();

                conDatabaseBlock.Dispose();
                cmdDataBaseBlock.Parameters.Clear();
            }
            
         }

        private string editJsonStringListAdd(String queryParmeter_Func, String listID_Func, String listValue_Func)
        {
            Console.WriteLine("inside editJsonSringListAdd: " + queryParmeter_Func);

        
            bool isThisNewRow = true;
            bool isTheIdNew = true;
            bool deleteListValue = false;
            bool commaTrimmer = true;
            

            var serializer = new JavaScriptSerializer();
            var finalJsonEditedString = "";
            
           
            string Query = @"SELECT " +queryParmeter_Func+ " FROM mocha_db.bush_table_valuelist ";
            String jsonListString = "";

            MySqlConnection conDatabaseBlock = new MySqlConnection(connectionString2);
            MySqlCommand cmdDataBaseBlock = new MySqlCommand();
            MySqlDataAdapter adapterBlock = new MySqlDataAdapter();

            try
            {
                cmdDataBaseBlock.Connection = conDatabaseBlock;
                cmdDataBaseBlock.CommandText = Query;
                conDatabaseBlock.Open();
               
                MySqlDataReader reader = cmdDataBaseBlock.ExecuteReader();


                if (reader.Read())
                {
                    jsonListString = reader[queryParmeter_Func].ToString();
                }
                Console.WriteLine(jsonListString);

                cmdDataBaseBlock.Parameters.Clear();
                conDatabaseBlock.Dispose();


                var jsonEditedString = new RootObjectList();
                jsonEditedString.items = new List<ListItem>();

                if (!jsonListString.Equals(""))
                {
                    var items = JsonConvert.DeserializeObject<List<ListItem>>(jsonListString);

                    for (int i = 0; i < items.Count; i++)
                    {
                        ListItem listCollection = new ListItem();
                        items[i].listID = items[i].listID.Replace("{", "");
                        items[i].listID = items[i].listID.Replace("}", "");

                        String[] listID_Array = items[i].listID.Split(',');
                        ArrayList listID_Processor = new ArrayList();
                        StringBuilder str = new StringBuilder();
                        String setOfListIDs = "";
                        Console.WriteLine("Number Of BushIDs in " +"item Number( "+ items[i].listID +") == " + listID_Array.Length);
                                            
                        for (int j = 0; j < listID_Array.Length; j++)
                        {
                            deleteListValue = false;
                            isTheIdNew = true;
                            Console.WriteLine("Trial Number for " + queryParmeter_Func + " is " + j);
                            Console.WriteLine("the value of string array split for " + queryParmeter_Func + ": " + listID_Array[j]);
                            if (items[i].listValue != listValue_Func)
                            {   isTheIdNew = false;
                                Console.WriteLine("input: "+listValue_Func + " Is Not Matched with " + "output:" + items[i].listValue);
                                if (listID_Array[j] == listID_Func)
                                {
                                    Console.WriteLine("List Value is Unique but the Bush ID exists ==> Editiding was conducted");
                                    if (listID_Array.Length == 1)
                                    {
                                        Console.WriteLine("List Value is Unique and the previous List Value will be deleted");
                                        deleteListValue = true;
                                        Console.WriteLine("DeleteListValue was activated");
                                    }else
                                    {
                                        Console.WriteLine("Input List Value is U");
                                    }
                                   
                                }else
                                {
                                    Console.WriteLine("List Value is Unique and the Bush ID is New as well ==> Addition was conducted");
                                    //str.Append(");

                                    str.Append(listID_Array[j]);
                                 
                                    //str.Append(doubleQuote);
                                }
                                 //id is found but the value isn't matched
                                //means it needs to be edited
                                //move to a different json string
                                //two cases : moving to the existing string or to the new string
                            }
                            else if (items[i].listValue == listValue_Func)
                            {
                                Console.WriteLine("input: " + listValue_Func + " Is Matched with " + "output:" + items[i].listValue);
                                isThisNewRow = false;
                                Console.WriteLine("Item won' be Added; just Rearragement will be done.");
                                //str.Append(doubleQuote);
                               
                               // str.Append(doubleQuote);
                               
                                if (listID_Array[j] == listID_Func)
                                {   //if the id is found within the string, it wont be concatenated to the json string
                                    Console.WriteLine("Total Duplicate Value was Saved Again: " + listID_Array[j] + " and " + listID_Func);
                                    Console.WriteLine("Leave the item as it is!");
                                    finalJsonEditedString = "duplicate";
                                    isTheIdNew = false;
                                }else
                                {
                                    Console.WriteLine("the input id: " +listID_Func + " is not found under the ListValue " +  listValue_Func+" which existed before;");
                                    Console.WriteLine(" A new id will be added under the same List Value!");
                                   
                                    str.Append(listID_Array[j]);
                                   // commaTrimmer = false;
                                   
                                    Console.WriteLine("the existing value(BUSH ID) is " +listID_Array[j]);
                                    Console.WriteLine("the appended Value is " + str);
                                }
                            }
                         
                            if (j != listID_Array.Length - 1 && commaTrimmer)
                            {
                                str.Append(commaMark);
                            }
                            Console.WriteLine(str);
                            Console.WriteLine("Status of Boolean Value: ");
                            Console.WriteLine("isThisNewRow:" + isThisNewRow + "  isTheIdNew: " + isTheIdNew + "   deleteListValue: " + deleteListValue);
                         
                        }   if (isTheIdNew)
                            {   Console.WriteLine("bushID should be added very soon in JsonString for Count: " + i);
                                str.Append(commaMark);
                                //str.Append(doubleQuote);
                                str.Append(listID_Func);
                                // str.Append(doubleQuote);
                                Console.WriteLine("the appended str value after the additing work: " + str);
                           
                            }

                        
                        String trimmedStr = str.ToString().TrimEnd(',');

                        setOfListIDs = parenthesisLeft + trimmedStr + parenthesisRight;
                        Console.WriteLine("set of List IDs to Be Added : " + setOfListIDs);
                        listCollection.listID = setOfListIDs;
                        listCollection.listValue = items[i].listValue;
                        if (!deleteListValue) 
                        {
                            Console.WriteLine("ItemString was rewritten for Count: " + i);
                            jsonEditedString.items.Add(listCollection);
                        }
                                               
                     }
                }
                if (isThisNewRow)
                {
                ListItem newListCollection = new ListItem();
                newListCollection.listID = parenthesisLeft + listID_Func +  parenthesisRight;
                newListCollection.listValue = listValue_Func;
                jsonEditedString.items.Add(newListCollection);
                finalJsonEditedString = serializer.Serialize(jsonEditedString.items);
                }else
                {
                    if (!finalJsonEditedString.Equals("duplicate"))
                    {
                        finalJsonEditedString = serializer.Serialize(jsonEditedString.items);
                    }
                   
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);

            }



            Console.WriteLine("isThisNewRow:" + isThisNewRow + "  isTheIdNew: " + isTheIdNew + "   deleteListValue: " + deleteListValue);
            Console.WriteLine("The return Value of finalJsonEditedString: " + finalJsonEditedString);
            Console.WriteLine("");
            return finalJsonEditedString;
        }
         
        public bool isTableListReady()
        {
            string Query = @"SELECT CompanyList FROM mocha_db.bush_table_valuelist ";
            Boolean isRowThere = false;

            MySqlConnection conDatabaseBlock = new MySqlConnection(connectionString2);
            MySqlCommand cmdDataBaseBlock = new MySqlCommand();
            MySqlDataAdapter adapterBlock = new MySqlDataAdapter();

            try
            {
                cmdDataBaseBlock.Connection = conDatabaseBlock;
                cmdDataBaseBlock.CommandText = Query;
                conDatabaseBlock.Open();
                int numRowsUpdated = cmdDataBaseBlock.ExecuteNonQuery();

                MySqlDataReader reader = cmdDataBaseBlock.ExecuteReader();

                
                while (reader.Read())
                {
                    String testString = reader["CompanyList"].ToString();
                    Console.WriteLine("testString for Compnay List" + testString);
                    isRowThere = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
            }
                Console.WriteLine("return value in isTableReady:" + isRowThere);
                return isRowThere;
        }
        public void prepareTheListStorage()
        {
            string Query = @"INSERT INTO mocha_db.bush_table_valuelist (CompanyList,BushTypeList,InnerPipeShapeList, InnerPipeMaterialList, OuterPipeShapeList,OuterPipeMaterialList) Values ('','','','','','')";
            
            MySqlConnection conDatabaseBlock = new MySqlConnection(connectionString2);
            MySqlCommand cmdDataBaseBlock = new MySqlCommand();
            MySqlDataAdapter adapterBlock = new MySqlDataAdapter();

            try
            {
                cmdDataBaseBlock.Connection = conDatabaseBlock;
                cmdDataBaseBlock.CommandText = Query;
                conDatabaseBlock.Open();
                int numRowsUpdated = cmdDataBaseBlock.ExecuteNonQuery();
                if (numRowsUpdated == -1)
                {
                    Console.WriteLine("making the row has failed.");
                   
                }
                else
                {
                    Console.WriteLine("row has been prepared ");
                    Console.WriteLine(numRowsUpdated);
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
            }
        }

        private void taskWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bWorker = sender as BackgroundWorker;
            //this.progressTest.Text = "querying...";
            activateListCreator();
            bWorker.ReportProgress((100));
                       
        }

        private void taskWorker_HIKI_DoWork(object sender, DoWorkEventArgs e)
        {
            bush_table_hikiBindingSource.EndEdit();
            bush_table_hikiTableAdapter.Update(this.mocha_dbDataSet_HIKI.bush_table_hiki);

            // textBoxReadOnlyControl(true);
            isDocumentBeingRecorded_HIKI = false;
        }

        private void taskWorker_RP_DoWork(object sender, DoWorkEventArgs e)
        {
            bush_table_docBindingSource.EndEdit();
            bush_table_docTableAdapter.Update(this.mocha_dbDataSet_RP.bush_table_doc);
          
            // textBoxReadOnlyControl(true);
            isDocumentBeingRecorded_RP = false;
        }

       

        private void storeValuesByCommand()
        {
            
            String indexString = ((DataRowView)this.bush_table_docBindingSource.Current).Row["DocID"].ToString();
            String sizeClass = "";
            String typeClass = "";

            String carSuspensionFront = "";
            String carSuspensionRear = "";
            String carComparedSusFront = "";
            String carComparedSusRear = "";

            String asys1 = "";
            String asys2 = "";
            String asys3 = "";
            String asys4 = "";
            String asys5 = "";
            String asys6 = "";

            String bushFront1 = "";
            String bushFront2 = "";
            String bushFront3 = "";
            String bushFront4 = "";

            String bushRear1 = "";
            String bushRear2 = "";
            String bushRear3 = "";
            String bushRear4 = "";
            String bushRear5 = "";
            String bushRear6 = "";
            String bushRear7 = "";
            String bushRear8 = "";
            //needs to be changed later on

            // code for convertime combobox.selected value to the data variables
            // as well for the displaying it should put the data variables back to the combobox with a matching value
            sizeClass = sizeComboBox.SelectedItem.ToString();
            typeClass = typeComboBox.SelectedItem.ToString();
            carSuspensionFront = frontComboBox.SelectedItem.ToString();
            carSuspensionRear = rearComboBox.SelectedItem.ToString();
            carComparedSusFront = frontCompCombo.SelectedItem.ToString();
            carComparedSusRear = rearCompCombo.SelectedItem.ToString();
            //code where a key and value pair is stored inside the db with JSON format and for storing it should save a value and unit value
            //{"value":value,"unit": unit}
            // a function is needed where it converts the JSON data to the client display
            // another function that converts a combobox value to a json data format upcon comboboxchangedEvent
            asys1 = makeJsonAsysString_RP(asysCombo1.SelectedItem.ToString(), asysUnitCombo1.SelectedItem.ToString());
            asys2 = makeJsonAsysString_RP(asysCombo2.SelectedItem.ToString(), asysUnitCombo2.SelectedItem.ToString());
            asys3 = makeJsonAsysString_RP(asysCombo3.SelectedItem.ToString(), asysUnitCombo3.SelectedItem.ToString());
            asys4 = makeJsonAsysString_RP(asysCombo4.SelectedItem.ToString(), asysUnitCombo4.SelectedItem.ToString());
            asys5 = makeJsonAsysString_RP(asysCombo5.SelectedItem.ToString(), asysUnitCombo5.SelectedItem.ToString());
            asys6 = makeJsonAsysString_RP(asysCombo6.SelectedItem.ToString(), asysUnitCombo6.SelectedItem.ToString());
            // code for convertime combobox.selected value to the data variables
            // as well for the displaying it should put the data variables back to the combobox with a matching value
            bushFront1 = bushFrontCombo1.SelectedItem.ToString();
            bushFront2 = bushFrontCombo2.SelectedItem.ToString();
            bushFront3 = bushFrontCombo3.SelectedItem.ToString();
            bushFront4 = bushFrontCombo4.SelectedItem.ToString();
            // code for convertime combobox.selected value to the data variables
            // as well for the displaying it should put the data variables back to the combobox with a matching value
            bushRear1 = bushRearCombo1.SelectedItem.ToString();
            bushRear2 = bushRearCombo2.SelectedItem.ToString();
            bushRear3 = bushRearCombo3.SelectedItem.ToString();
            bushRear4 = bushRearCombo4.SelectedItem.ToString();
            bushRear5 = bushRearCombo5.SelectedItem.ToString();
            bushRear6 = bushRearCombo6.SelectedItem.ToString();
            bushRear7 = bushRearCombo7.SelectedItem.ToString();
            bushRear8 = bushRearCombo8.SelectedItem.ToString();

            String docNumberString = this.bush_table_docBindingSource.Position.ToString();

            try
            {
                //string Query = @"UPDATE mocha_db.bush_table_doc SET DateStart = @dateStart, DateEnd = @dateEnd WHERE DocID= @docID ";

                string Query = @"UPDATE mocha_db.bush_table_doc SET DocNumber=@docNumber, DateStart = @dateStart, DateEnd = @dateEnd,SizeClass = @sizeClass, TypeClass = @typeClass, CarSuspensionFront = @carSuspensionFront,CarSuspensionRear = @carSuspensionRear, CarComparedSusFront = @carComparedSusFront,CarComparedSusRear = @carComparedSusRear, ASYS1 = @asys1, ASYS2 = @asys2, ASYS3 = @asys3, ASYS4 = @asys4, ASYS5 = @asys5, ASYS6 = @asys6, BushFront1 = @bushFront1,BushFront2 = @bushFront2, BushFront3 = @bushFront3, BushFront4 = @bushFront4, BushRear1 = @bushRear1,BushRear2 = @bushRear2, BushRear3 = @bushRear3,BushRear4 = @bushRear4,BushRear5 = @bushRear5, BushRear6 = @bushRear6, BushRear7 = @bushRear7,BushRear8 = @bushRear8 WHERE DocID= @docID ";

                cmdDataBase.Connection = conDatabase;
                cmdDataBase.CommandText = Query;
                cmdDataBase.Parameters.AddWithValue("@dateStart", dateStartPicker.Value.Date.ToString("yyyy-MM-dd"));
                cmdDataBase.Parameters.AddWithValue("@dateEnd", dateEndPicker.Value.Date.ToString("yyyy-MM-dd"));
                cmdDataBase.Parameters.AddWithValue("@sizeClass", sizeClass);
                cmdDataBase.Parameters.AddWithValue("@typeClass", typeClass);
                cmdDataBase.Parameters.AddWithValue("@carSuspensionFront", carSuspensionFront);
                cmdDataBase.Parameters.AddWithValue("@carSuspensionRear", carSuspensionRear);
                cmdDataBase.Parameters.AddWithValue("@carComparedSusFront", carComparedSusFront);
                cmdDataBase.Parameters.AddWithValue("@carComparedSusRear", carComparedSusRear);
                cmdDataBase.Parameters.AddWithValue("@docNumber", docNumberString);
                
                
                cmdDataBase.Parameters.AddWithValue("@asys1", asys1);
                cmdDataBase.Parameters.AddWithValue("@asys2", asys2);
                cmdDataBase.Parameters.AddWithValue("@asys3", asys3);
                cmdDataBase.Parameters.AddWithValue("@asys4", asys4);
                cmdDataBase.Parameters.AddWithValue("@asys5", asys5);
                cmdDataBase.Parameters.AddWithValue("@asys6", asys6);

                
                cmdDataBase.Parameters.AddWithValue("@bushFront1", bushFront1);
                cmdDataBase.Parameters.AddWithValue("@bushFront2", bushFront2);
                cmdDataBase.Parameters.AddWithValue("@bushFront3", bushFront3);
                cmdDataBase.Parameters.AddWithValue("@bushFront4", bushFront4);
                cmdDataBase.Parameters.AddWithValue("@bushRear1", bushRear1);
                cmdDataBase.Parameters.AddWithValue("@bushRear2", bushRear2);
                cmdDataBase.Parameters.AddWithValue("@bushRear3", bushRear3);
                cmdDataBase.Parameters.AddWithValue("@bushRear4", bushRear4);
                cmdDataBase.Parameters.AddWithValue("@bushRear5", bushRear5);
                cmdDataBase.Parameters.AddWithValue("@bushRear6", bushRear6);
                cmdDataBase.Parameters.AddWithValue("@bushRear7", bushRear7);
                cmdDataBase.Parameters.AddWithValue("@bushRear8", bushRear8);
                
                cmdDataBase.Parameters.AddWithValue("@docID", indexString);
                conDatabase.Open();

                int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                conDatabase.Dispose();
                cmdDataBase.Parameters.Clear();
        
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MessageLog", MessageBoxButtons.OK);
            }
        }

        private void storeValuesForSummaryTable()
        {

        
            this.mocha_dbDataSet_RPSET.bush_table_doc_set.Addbush_table_doc_setRow(this.mocha_dbDataSet_RPSET.bush_table_doc_set.Newbush_table_doc_setRow());
            bush_table_docBindingSource.MoveLast();
            String indexString = ((DataRowView)this.bush_table_doc_setBindingSource.Current).Row["DocSetID"].ToString();

            String docName = "";

            String docType = "";
            String dateStart = "";
            String organization = "";
            String team= "";

            String subjectCar= "";
            String randH = "";
            String NVH = "";
            String durability = "";
            String improvementMethod = "";
            String improvementResult = "";

            docName = docNameTextBox.Text;
            docType = docTypeTextBox.Text;
            dateStart = dateStartPicker.Value.Date.ToString("yyyy-MM-dd");
            organization = organizationTextBox.Text;
            team = teamTextBox.Text;
            subjectCar = subjectCarTextBox.Text;
            randH = randHTextBox.Text;
            NVH = nVHTextBox.Text;
            durability = durabilityTextBox.Text;
            improvementMethod = improvementMethodTextBox.Text;
            improvementResult = improvedResultTextBox.Text;


            String docNumberString = this.bush_table_docBindingSource.Position.ToString();

            try
            {
                //string Query = @"UPDATE mocha_db.bush_table_doc SET DateStart = @dateStart, DateEnd = @dateEnd WHERE DocID= @docID ";

                string Query = @"UPDATE mocha_db.bush_table_doc_set SET DocNumber=@docNumber, DateStart = @dateStart,DocName = @docName, DocType = @docType, Organization = @organization, Team = @team, SubjectCar = @subjectCar, RandH = @randH, NVH = @nvh, Durability =@durability,ImprovementMethod =@improvementMethod,ImprovementResult =@improvementResult WHERE DocSetID= @docID ";

                cmdDataBase_RPSET.Connection = conDatabase_RPSET;
                cmdDataBase_RPSET.CommandText = Query;
                cmdDataBase_RPSET.Parameters.AddWithValue("@dateStart", dateStartPicker.Value.Date.ToString("yyyy-MM-dd"));
                cmdDataBase_RPSET.Parameters.AddWithValue("@docName", docName);
                cmdDataBase_RPSET.Parameters.AddWithValue("@docType", docType);
                cmdDataBase_RPSET.Parameters.AddWithValue("@organization", organization);
                cmdDataBase_RPSET.Parameters.AddWithValue("@team", team);
                cmdDataBase_RPSET.Parameters.AddWithValue("@subjectCar",subjectCar );
                cmdDataBase_RPSET.Parameters.AddWithValue("@randH", randH);
                cmdDataBase_RPSET.Parameters.AddWithValue("@nvh", NVH);
                cmdDataBase_RPSET.Parameters.AddWithValue("@durability", durability);
                cmdDataBase_RPSET.Parameters.AddWithValue("@improvementMethod",improvementMethod );
                cmdDataBase_RPSET.Parameters.AddWithValue("@improvementResult", improvementResult);
                cmdDataBase_RPSET.Parameters.AddWithValue("@docNumber", docNumberString);
                cmdDataBase_RPSET.Parameters.AddWithValue("@docID", indexString);
                conDatabase_RPSET.Open();

                int numRowsUpdated = cmdDataBase_RPSET.ExecuteNonQuery();
                conDatabase_RPSET.Dispose();
                cmdDataBase_RPSET.Parameters.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MessageLog", MessageBoxButtons.OK);
            }
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
            rubberAxialThicknessComboBox.SelectedItem = "mm";             
            angleToArmComboBox.SelectedItem = "degree";


        }

        private void comboBoxDefaultInitialization()
        {
            sizeComboBox.SelectedItem = "-";
            typeComboBox.SelectedItem = "-";
            frontComboBox.SelectedItem = "-";
            rearComboBox.SelectedItem = "-";
            frontCompCombo.SelectedItem = "-";
            rearCompCombo.SelectedItem = "-";
            //code where a key and value pair is stored inside the db with JSON format and for storing it should save a value and unit value
            //{"value":value,"unit": unit}
            // a function is needed where it converts the JSON data to the client display
            // another function that converts a combobox value to a json data format upcon comboboxchangedEvent
            asysCombo1.SelectedItem = "-";
            asysUnitCombo1.SelectedItem = "-";
            asysCombo2.SelectedItem = "-";
            asysUnitCombo2.SelectedItem = "-";
            asysCombo3.SelectedItem = "-";
            asysUnitCombo3.SelectedItem = "-";
            asysCombo4.SelectedItem = "-";
            asysUnitCombo4.SelectedItem = "-";
            asysCombo5.SelectedItem = "-";
            asysUnitCombo5.SelectedItem = "-";
            asysCombo6.SelectedItem = "-";
            asysUnitCombo6.SelectedItem = "-";
            // code for convertime combobox.selected value to the data variables
            // as well for the displaying it should put the data variables back to the combobox with a matching value
            bushFrontCombo1.SelectedItem = "-";
            bushFrontCombo2.SelectedItem = "-";
            bushFrontCombo3.SelectedItem = "-";
            bushFrontCombo4.SelectedItem = "-";
            // code for convertime combobox.selected value to the data variables
            // as well for the displaying it should put the data variables back to the combobox with a matching value
            bushRearCombo1.SelectedItem = "-";
            bushRearCombo2.SelectedItem = "-";
            bushRearCombo3.SelectedItem = "-";
            bushRearCombo4.SelectedItem = "-";
            bushRearCombo5.SelectedItem = "-";
            bushRearCombo6.SelectedItem = "-";
            bushRearCombo7.SelectedItem = "-";
            bushRearCombo8.SelectedItem = "-";
        }

        private void comboBoxValueInitialization()
        {
            string Query = @"SELECT * FROM mocha_db.bush_table_doc WHERE DocID= @docID ";
            
            try
            {
                String indexString = ((DataRowView)this.bush_table_docBindingSource.Current).Row["DocID"].ToString();
                Console.WriteLine("comboBoxValueInitialization was activated with index value of " + indexString);
                cmdDataBase_RP.Connection = conDatabase_RP;
                cmdDataBase_RP.CommandText = Query;
                cmdDataBase_RP.Parameters.AddWithValue("@docID", indexString);
                conDatabase_RP.Open();
                int numRowsUpdated = cmdDataBase_RP.ExecuteNonQuery();
                MySqlDataReader reader = cmdDataBase_RP.ExecuteReader();
                
                while (reader.Read())
                {
                    sizeComboBox.SelectedItem = reader["SizeClass"].ToString();
                    typeComboBox.SelectedItem = reader["TypeClass"].ToString();
                    frontComboBox.SelectedItem = reader["CarSuspensionFront"].ToString();
                    rearComboBox.SelectedItem = reader["CarSuspensionRear"].ToString();
                    frontCompCombo.SelectedItem = reader["CarComparedSusFront"].ToString();
                    rearCompCombo.SelectedItem = reader["CarComparedSusRear"].ToString();
                    //code where a key and value pair is stored inside the db with JSON format and for storing it should save a value and unit value
                    //{"value":value,"unit": unit}
                    // a function is needed where it converts the JSON data to the client display
                    // another function that converts a combobox value to a json data format upcon comboboxchangedEvent
                    var items = JsonConvert.DeserializeObject<List<AsysItem>>(reader["ASYS1"].ToString());
                    asysCombo1.SelectedItem = items[0].asysValue;
                    asysUnitCombo1.SelectedItem = items[0].asysUnit;

                    var items2 = JsonConvert.DeserializeObject<List<AsysItem>>(reader["ASYS2"].ToString());
                    asysCombo2.SelectedItem = items2[0].asysValue;
                    asysUnitCombo2.SelectedItem = items2[0].asysUnit;

                    var items3 = JsonConvert.DeserializeObject<List<AsysItem>>(reader["ASYS3"].ToString());
                    asysCombo3.SelectedItem = items3[0].asysValue;
                    asysUnitCombo3.SelectedItem = items3[0].asysUnit;

                    var items4 = JsonConvert.DeserializeObject<List<AsysItem>>(reader["ASYS4"].ToString());
                    asysCombo4.SelectedItem = items4[0].asysValue;
                    asysUnitCombo4.SelectedItem = items4[0].asysUnit;

                    var items5 = JsonConvert.DeserializeObject<List<AsysItem>>(reader["ASYS5"].ToString());
                    asysCombo5.SelectedItem = items5[0].asysValue;
                    asysUnitCombo5.SelectedItem = items5[0].asysUnit;

                    var items6 = JsonConvert.DeserializeObject<List<AsysItem>>(reader["ASYS6"].ToString());
                    asysCombo6.SelectedItem = items6[0].asysValue;
                    asysUnitCombo6.SelectedItem = items6[0].asysUnit;
                    // code for convertime combobox.selected value to the data variables
                    // as well for the displaying it should put the data variables back to the combobox with a matching value
                    bushFrontCombo1.SelectedItem = reader["BushFront1"].ToString();
                    bushFrontCombo2.SelectedItem = reader["BushFront2"].ToString();
                    bushFrontCombo3.SelectedItem = reader["BushFront3"].ToString();
                    bushFrontCombo4.SelectedItem = reader["BushFront4"].ToString();
                    // code for convertime combobox.selected value to the data variables
                    // as well for the displaying it should put the data variables back to the combobox with a matching value
                    bushRearCombo1.SelectedItem = reader["BushRear1"].ToString();
                    bushRearCombo2.SelectedItem = reader["BushRear2"].ToString();
                    bushRearCombo3.SelectedItem = reader["BushRear3"].ToString();
                    bushRearCombo4.SelectedItem = reader["BushRear4"].ToString();
                    bushRearCombo5.SelectedItem = reader["BushRear5"].ToString();
                    bushRearCombo6.SelectedItem = reader["BushRear6"].ToString();
                    bushRearCombo7.SelectedItem = reader["BushRear7"].ToString();
                    bushRearCombo8.SelectedItem = reader["BushRear8"].ToString();

                    int dateYear = Convert.ToInt32(reader["DateStart"].ToString().Split('-')[0]);
                    int dateMonth = Convert.ToInt32(reader["DateStart"].ToString().Split('-')[1]);
                    int dateDays = Convert.ToInt32(reader["DateStart"].ToString().Split('-')[2]);
                    dateStartPicker.Value = new DateTime(dateYear, dateMonth, dateDays);

                    dateYear = Convert.ToInt32(reader["DateEnd"].ToString().Split('-')[0]);
                    dateMonth = Convert.ToInt32(reader["DateEnd"].ToString().Split('-')[1]);
                    dateDays = Convert.ToInt32(reader["DateEnd"].ToString().Split('-')[2]);
                    dateEndPicker.Value = new DateTime(dateYear, dateMonth, dateDays);
                }

                cmdDataBase_RP.Parameters.Clear();
                conDatabase_RP.Dispose();
                
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
            }
        }
        private void searchLabel_Click(object sender, EventArgs e)
        {
            isSearchable = false;
            int filterReady = 0;
            filterReady += isFilterContentsReady(searchSetForCompanyList);
            filterReady += isFilterContentsReady(searchSetForBushTypeList);
            filterReady += isFilterContentsReady(searchSetForInnerPipeShapeList);
            filterReady += isFilterContentsReady(searchSetForInnerPipeMaterialList);
            filterReady += isFilterContentsReady(searchSetForOuterPipeShapeList);
            filterReady += isFilterContentsReady(searchSetForOuterPipeMaterialList);

            if (filterReady != 0)
            {
                isSearchable = true;
            }

            if (isSearchable)
            {
            string Query = queryMakerTotal();                           
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

            MySqlDataReader reader = cmdDataBase.ExecuteReader();
           // List<double> massValues = new List<double>();

            chartValuesContainer = new RootObjectChart();
            chartValuesContainer.chartitem = new List<ChartItem>();
            
                while (reader.Read())
                {
                    ChartItem item = new ChartItem();
                    item.mass = reader["Mass"].ToString();
                   
                    item.bushID = reader["BushID"].ToString();
                    item.innerPipeRadiusValues= filterEmptyString(reader["InnerPipeRadius"].ToString());
                    item.innerPipeLengthValues = filterEmptyString(reader["InnerPipeLength"].ToString());
                    item.innerPipeThicknessValues = filterEmptyString(reader["InnerPipeThickness"].ToString());
                    item.outerPipeRadiusValues = filterEmptyString(reader["OuterPipeRadius"].ToString());
                    item.outerPipeLengthValues = filterEmptyString(reader["OuterPipeLength"].ToString());
                    item.outerPipeThicknessValues = filterEmptyString(reader["OuterPipeThickness"].ToString());
                    item.rubberRadialThicknessValues = filterEmptyString(reader["RubberRadialThickness"].ToString());
                    item.rubberAxialThicknessValues = filterEmptyString(reader["RubberAxialThickness"].ToString());
                    item.voidGapValues = filterEmptyString(reader["VoidGap"].ToString());
                    item.angleToArmValues = filterEmptyString(reader["AngleToArm"].ToString());
                    chartValuesContainer.chartitem.Add(item);
                }
                cmdDataBase.Parameters.Clear();
                conDatabase.Dispose();
                loadCharts();
              
                conDatabase.Close();
            }else
            {
                MessageBox.Show("항목을 선택해주세요", "Message", MessageBoxButtons.OK);
            }
            
        }
        private List<Point> loadPoints(String loadX, String loadY)
        {
            //String[] chartChoices = { "Mass", "InnerPipeRadius", "InnerPipeLength", "InnerPipeThickness", "OuterPipeRadius", "OuterPipeLength", "OuterPipeThickness", "RubberAxialThickness", "RubberRadialThickness", "VoidGap", "AngleToArm" };
            List<Point> pointsSet = new List<Point>();
            for (int i = 0; i < chartValuesContainer.chartitem.Count; i++)
            {
                if (!chartValuesContainer.chartitem[i].getItemByName(loadX).Equals("") && !chartValuesContainer.chartitem[i].getItemByName(loadX).Equals("empty") && !chartValuesContainer.chartitem[i].getItemByName(loadY).Equals("empty")) 
                {
                    Console.WriteLine(loadX + ": " + chartValuesContainer.chartitem[i].getItemByName(loadX) + "/ " + loadY + chartValuesContainer.chartitem[i].getItemByName(loadY));
                    pointsSet.Add(new Point(Convert.ToInt32(chartValuesContainer.chartitem[i].getItemByName(loadX).ToString()), Convert.ToInt32(chartValuesContainer.chartitem[i].getItemByName(loadY).ToString())));
                }
            }
            return pointsSet;
        }
      
        private List<Double> loadPointsDoubleX(String loadX, String loadY)
        {
            //String[] chartChoices = { "Mass", "InnerPipeRadius", "InnerPipeLength", "InnerPipeThickness", "OuterPipeRadius", "OuterPipeLength", "OuterPipeThickness", "RubberAxialThickness", "RubberRadialThickness", "VoidGap", "AngleToArm" };
            List<Double> pointsSetX = new List<Double>();
            enoughPointsX = 0;
            for (int i = 0; i < chartValuesContainer.chartitem.Count; i++)
            {
                if (!chartValuesContainer.chartitem[i].getItemByName(loadX).Equals("") && !chartValuesContainer.chartitem[i].getItemByName(loadX).Equals("empty") && !chartValuesContainer.chartitem[i].getItemByName(loadY).Equals("empty"))
                {
                    Console.WriteLine(loadX + ": " + chartValuesContainer.chartitem[i].getItemByName(loadX) + "/ " + loadY + chartValuesContainer.chartitem[i].getItemByName(loadY));
                    pointsSetX.Add(Convert.ToDouble(chartValuesContainer.chartitem[i].getItemByName(loadX).ToString()));
                    enoughPointsX++;
                    
                }
            }
            return pointsSetX;
        }
        private List<Double> loadPointsDoubleY(String loadX, String loadY)
        {
            //String[] chartChoices = { "Mass", "InnerPipeRadius", "InnerPipeLength", "InnerPipeThickness", "OuterPipeRadius", "OuterPipeLength", "OuterPipeThickness", "RubberAxialThickness", "RubberRadialThickness", "VoidGap", "AngleToArm" };
            List<Double> pointsSetY = new List<Double>();
            enoughPointsY = 0;
            for (int i = 0; i < chartValuesContainer.chartitem.Count; i++)
            {
                if (!chartValuesContainer.chartitem[i].getItemByName(loadX).Equals("") && !chartValuesContainer.chartitem[i].getItemByName(loadX).Equals("empty") && !chartValuesContainer.chartitem[i].getItemByName(loadY).Equals("empty"))
                {
                    Console.WriteLine(loadX + ": " + chartValuesContainer.chartitem[i].getItemByName(loadX) + "/ " + loadY + chartValuesContainer.chartitem[i].getItemByName(loadY));
                    pointsSetY.Add(Convert.ToDouble(chartValuesContainer.chartitem[i].getItemByName(loadY).ToString()));
                    enoughPointsY++;
                }
            }
            return pointsSetY;
        }
        protected void Chart_Click(object sender, MouseEventArgs e)
        {
            ToolTip tooltip = new ToolTip();
            Point? clickPosition = null;
            var pos = e.Location;
            clickPosition = pos;
            var results = analysisChart.HitTest(pos.X, pos.Y, false,
                                   ChartElementType.DataPoint);
            foreach (var result in results)
            {
                if (result.ChartElementType == ChartElementType.DataPoint)
                {
                    var xVal = result.ChartArea.AxisX.PixelPositionToValue(pos.X);
                    var yVal = result.ChartArea.AxisY.PixelPositionToValue(pos.Y);

                    Console.WriteLine("clickset " + selectedX + ": " + xClickedValue+ " / " + selectedY + ": " + yClickedValue);
                    if (chartValuesContainer.chartitem.Count >0)
                    {
                        traceBackItemsByXandYValues(xClickedValue, yClickedValue);
                    }
                        
                    
                    
                }
            }

            Console.WriteLine(results);

        }
        private void traceBackItemsByXandYValues(String xValue, String yValue)
        {
            /*if (!isForm3Activated)
            {
                Form3 propertyForm = new Form3(selectedX, xValue, selectedY, yValue);
                isForm3Activated = true;
                propertyForm.FormClosing += new FormClosingEventHandler(Form3_Closing);

                propertyForm.Show();
                propertyForm.Activate();

                
            }*/

            Form3 propertyForm = new Form3(selectedX, xValue, selectedY, yValue);
            propertyForm.FormClosing += new FormClosingEventHandler(Form3_Closing);
            propertyForm.Show();
            propertyForm.Activate();


        }
        private void chartTracking_MouseEnter(object sender, EventArgs e)
        {
            try
            {
               if (chartValuesContainer.chartitem.Count > 0)
                {
                   if (enoughPointsX >=2 && enoughPointsY >=2)
                    {
                        analysisChart.Series.Remove(analysisChart.Series["TrendLine"]);
                    }
                   
                }
              
               
                
               
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Message");
            }
          
        }

        private void chartTracking_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                if (enoughPointsX >= 2 && enoughPointsY >= 2)
                {
                    activateTrendLine();
                }

            
               
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Message");
            }
            
        }

        private void loadCharts()
        {   
            
            if (chartValuesContainer.chartitem.Count>0 && chartValuesContainer.chartitem != null)
            {
                try
                {                 
                 // var pointsData = loadPoints(selectedX, selectedY).Select(x => new { X = x. .X, Y = x.Y }).ToList();
                 var pointsYSet = loadPointsDoubleY(selectedX, selectedY).ToList();
                 var pointsXSet = loadPointsDoubleX(selectedX, selectedY).ToList();
                 analysisChart.Series[0].Points.DataBindXY(pointsXSet,"X", pointsYSet, "Y");                   
                 
                 analysisChart.ChartAreas[0].AxisX.Title = selectedX;
                 analysisChart.ChartAreas[0].AxisY.Title = selectedY;
                 
                 
                 analysisChart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;

                    analysisChart.Series[0].ToolTip = selectedX +" :" +"#VALX " + selectedY + " :" +"#VALY";
                if (analysisChart.Series.Count > 1)
                {
                    analysisChart.Series.Remove(analysisChart.Series["TrendLine"]);
                    
                }

                    activateTrendLine();
                }
                catch (Exception ex)
                {

                    // MessageBox.Show("좌표 값이 없습니다", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
                }
                
            }
           
        }
        private void activateTrendLine()
        {
            analysisChart.Series.Add("TrendLine");
            analysisChart.Series["TrendLine"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            analysisChart.Series["TrendLine"].BorderWidth = 1;
            analysisChart.Series["TrendLine"].Color = Color.Orange;
            
            String typeRegression = "Polynomial";
            String forecasting = "4";
            String error = "false";
            String forecastingError = "false";
            String parameters = typeRegression + ',' + forecasting + ',' + error + ',' + forecastingError;
            analysisChart.Series[0].Sort(System.Windows.Forms.DataVisualization.Charting.PointSortOrder.Ascending, "X");
            analysisChart.DataManipulator.FinancialFormula(System.Windows.Forms.DataVisualization.Charting.FinancialFormula.Forecasting, parameters, analysisChart.Series[0], analysisChart.Series["TrendLine"]);

        }

        private string filterEmptyString(string inputString)
        {   
            if (inputString.Equals("") || inputString == null)
            {
                return "empty";
            }else
            {
                return inputString;
            }
       
        }

        private List<Point> filterEmptyList(int[] xValues, int[] yValues)
        {    List<Point> dataPoints = new List<Point>();
             
            return dataPoints;
        }
        private string queryMakerPart(String queryName, ArrayList querySet)
        {
                     
            string queryStringField = "";
       
            for (int i= 0; i < querySet.Count; i++)
            {
                if (i != querySet.Count -1)
                {
                    queryStringField += " " + queryName + " = ";
                    queryStringField += doubleQuote;
                    queryStringField += querySet[i].ToString();
                    queryStringField += doubleQuote;
                    queryStringField += " OR";
                }
                else
                {
                    queryStringField += " " + queryName + " = ";
                    queryStringField += doubleQuote;
                    queryStringField += querySet[i].ToString();
                    queryStringField += doubleQuote;

                }
                                      
            }            
            Console.WriteLine(queryStringField);
            return queryStringField;
        }

        private string queryMakerTotal()
        {
            string queryMode = " OR ";
            string Query = "SELECT * FROM mocha_db.bush_table_real WHERE";
            string queryControllerTotal = "";
            ArrayList queryControllerPartFilter = new ArrayList();
            ArrayList queryControllerPart = new ArrayList();

            queryControllerPartFilter.Add(queryMakerPart("Company", searchSetForCompanyList));
            queryControllerPartFilter.Add(queryMakerPart("BushType", searchSetForBushTypeList));
            queryControllerPartFilter.Add(queryMakerPart("InnerPipeShape", searchSetForInnerPipeShapeList));
            queryControllerPartFilter.Add(queryMakerPart("InnerPipeMaterial", searchSetForInnerPipeMaterialList));
            queryControllerPartFilter.Add(queryMakerPart("OuterPipeShape", searchSetForOuterPipeShapeList));
            queryControllerPartFilter.Add(queryMakerPart("OuterPipeMaterial", searchSetForOuterPipeMaterialList));

            foreach(string setOfValues in queryControllerPartFilter)
            {
                if (!setOfValues.Equals(""))
                {
                    queryControllerPart.Add(setOfValues);
                }
            }

            for(int i=0; i< queryControllerPart.Count; i++)
            {
                if (!queryControllerPart[i].Equals(""))
                {
                    if (i != queryControllerPart.Count -1)
                    {
                         queryControllerTotal += queryControllerPart[i].ToString();
                         queryControllerTotal += queryMode;
                    }else
                    {
                        queryControllerTotal += queryControllerPart[i].ToString();
                    }
               
                }
            }

            Query += queryControllerTotal + ";";
            Console.WriteLine(Query);
            return Query;
        }

        private void docNew_Click(object sender, EventArgs e)
        {
            try
            {

                docNameTextBox.Focus();
                this.mocha_dbDataSet_RP.bush_table_doc.Addbush_table_docRow(this.mocha_dbDataSet_RP.bush_table_doc.Newbush_table_docRow());
                bush_table_docBindingSource.MoveLast();
                currentRowIndex_RP.Text = this.bush_table_docBindingSource.Position.ToString();
                isDocumentBeingRecorded_RP = true;

                if (pictureBoxDoc.Image != null)
                {
                    pictureBoxDoc.Image.Dispose();
                    pictureBoxDoc.Image = null;
                }
                pictureDocInitialization();
                relatedDocFileInitialization();
                comboBoxDefaultInitialization();



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
            }
        }

        private void docSave_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (taskWorker_RP.IsBusy != true)
                {
                    taskWorker_RP.RunWorkerAsync();
                    storeValuesByCommand();
                    docPanel.Enabled = false;
                    nextButton_RP.Enabled = true;
                    previousButton_RP.Enabled = true;

                    //  storeValuesForSummaryTable();
                }
             
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MessageLog2", MessageBoxButtons.OK);
            }
        }

        private void docEditt_Click(object sender, EventArgs e)
        {
            docPanel.Enabled = true;
            docNameTextBox.Focus();
        }

        private void docDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("정말로 삭제하시겠습니까?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    bush_table_docBindingSource.RemoveCurrent();
                    bush_table_doc_setBindingSource.RemoveCurrent();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
            }
        }

        private void docImageAdd_Click(object sender, EventArgs e)
        {
            try
            {

                if (taskWorker_RP.IsBusy != true)
                {
                    taskWorker_RP.RunWorkerAsync();
                    storeValuesByCommand();
                    //  storeValuesForSummaryTable();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MessageLog2", MessageBoxButtons.OK);
            }
            if (pictureFileDialog_RP.ShowDialog() == DialogResult.OK)
            {

                String indexString = ((DataRowView)this.bush_table_docBindingSource.Current).Row["DocID"].ToString();
                try
                {
                    string sourceDir = dataBaseDirectory(pictureFileDialog_RP.FileName, "documentsPicture");
                    string Query = @"UPDATE mocha_db.bush_table_doc SET RepresentiveImage = @filePath WHERE DocID= @docID ";
                    cmdDataBase.Connection = conDatabase;
                    cmdDataBase.CommandText = Query;
                    cmdDataBase.Parameters.AddWithValue("@filePath", sourceDir);
                    cmdDataBase.Parameters.AddWithValue("@docID", indexString);
                    conDatabase.Open();
                    int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                    conDatabase.Dispose();
                    cmdDataBase.Parameters.Clear();
                    pictureBoxDoc.Image = Image.FromFile(sourceDir);
                    pictureBoxDoc.SizeMode = PictureBoxSizeMode.Zoom; //CenterImage/ Normal/AutoSize
                    docImageEdit.Enabled = true;
                    docImageAdd.Enabled = false;
                    docImageDelete.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);

                }

            }
        }

        private void docImageDelete_Click(object sender, EventArgs e)
        {
            String indexString = ((DataRowView)this.bush_table_docBindingSource.Current).Row["DocID"].ToString();
            try
            {
                if (MessageBox.Show("그림을 삭제하시겠습니까?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string Query = @"UPDATE mocha_db.bush_table_doc SET RepresentiveImage = @filePath WHERE DocID= @docID ";
                    cmdDataBase.Connection = conDatabase;
                    cmdDataBase.CommandText = Query;
                    cmdDataBase.Parameters.AddWithValue("@filePath", "");
                    cmdDataBase.Parameters.AddWithValue("@docID", indexString);
                    conDatabase.Open();
                    int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                    conDatabase.Dispose();
                    cmdDataBase.Parameters.Clear();
                    
                    if (pictureBoxDoc.Image != null)
                    {
                        pictureBoxDoc.Image.Dispose();
                        pictureBoxDoc.Image = null;

                        docImageEdit.Enabled = false;
                        docImageAdd.Enabled = true;
                        docImageDelete.Enabled = false;
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);

            }
        }

        private void docImageEdit_Click(object sender, EventArgs e)
        {
            if (pictureFileDialog_RP.ShowDialog() == DialogResult.OK)
            {

                String indexString = ((DataRowView)this.bush_table_docBindingSource.Current).Row["DocID"].ToString();
                try
                {
                    string sourceDir = dataBaseDirectory(pictureFileDialog_RP.FileName, "documentsPicture");
                    string Query = @"UPDATE mocha_db.bush_table_doc SET RepresentiveImage = @filePath WHERE DocID= @docID ";
                    cmdDataBase.Connection = conDatabase;
                    cmdDataBase.CommandText = Query;
                    cmdDataBase.Parameters.AddWithValue("@filePath", sourceDir);
                    cmdDataBase.Parameters.AddWithValue("@docID", indexString);
                    conDatabase.Open();
                    int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                    conDatabase.Dispose();
                    cmdDataBase.Parameters.Clear();
                    pictureBoxDoc.Image = Image.FromFile(sourceDir);
                    pictureBoxDoc.SizeMode = PictureBoxSizeMode.Zoom; //CenterImage/ Normal/AutoSize
                    docImageEdit.Enabled = true;
                    docImageAdd.Enabled = false;
                    docImageDelete.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);

                }

            }
        }

        private void relatedDocAdd_Click(object sender, EventArgs e)
        {
            try
            {

                if (taskWorker_RP.IsBusy != true)
                {
                    taskWorker_RP.RunWorkerAsync();
                    storeValuesByCommand();
                    //  storeValuesForSummaryTable();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MessageLog2", MessageBoxButtons.OK);
            }
            if (documentFileDialog_RP.ShowDialog() == DialogResult.OK)
            {
                string sourceDir = dataBaseDirectory(documentFileDialog_RP.FileName, "documentsFile");
                String newFilePath = sourceDir;
                String newFileName = System.IO.Path.GetFileName(newFilePath);
                String relatedDocString = editJsonStringAdd_RP(newFilePath, newFileName);

                String rowIndex = ((DataRowView)this.bush_table_docBindingSource.Current).Row["DocID"].ToString();

                string Query = @"UPDATE mocha_db.bush_table_doc SET RelatedDoc = @jsonFileString WHERE DocID= @docID ";
                cmdDataBase.Connection = conDatabase;
                cmdDataBase.CommandText = Query;
                cmdDataBase.Parameters.AddWithValue("@jsonFileString", relatedDocString);
                cmdDataBase.Parameters.AddWithValue("@docID", rowIndex);
                conDatabase.Open();
                int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                conDatabase.Dispose();
                cmdDataBase.Parameters.Clear();
                relatedDocFileInitialization();
            }
        }

        private void previousButton_RP_Click(object sender, EventArgs e)
        {
            if (!isDocumentBeingRecorded_RP)
            {
                bush_table_docBindingSource.MovePrevious();
                currentRowIndex_RP.Text = this.bush_table_docBindingSource.Position.ToString();

                if (pictureBoxDoc.Image != null)
                {
                    pictureBoxDoc.Image.Dispose();
                    pictureBoxDoc.Image = null;
                }
                pictureDocInitialization();
                relatedDocFileInitialization();
               
                comboBoxValueInitialization();
            }
            else
            {
                MessageBox.Show("저장을 먼저 해주세요.", "Message", MessageBoxButtons.OK);
            }
        }

        private void nextButton_RP_Click(object sender, EventArgs e)
        {
            if (!isDocumentBeingRecorded_RP)
            {
                bush_table_docBindingSource.MoveNext();
                currentRowIndex_RP.Text = this.bush_table_docBindingSource.Position.ToString();
                if (pictureBoxDoc.Image != null)
                {
                    pictureBoxDoc.Image.Dispose();
                    pictureBoxDoc.Image = null;
                }
                pictureDocInitialization();
                relatedDocFileInitialization();
           
                comboBoxValueInitialization();
            }
            else
            {
                MessageBox.Show("저장을 먼저 해주세요", "Message", MessageBoxButtons.OK);
            }
        }

        private void hikiNew_Click(object sender, EventArgs e)
        {
            try
            {
                hikiPanel.Enabled = true;
                titleTextBox.Focus();
                this.mocha_dbDataSet_HIKI.bush_table_hiki.Addbush_table_hikiRow(this.mocha_dbDataSet_HIKI.bush_table_hiki.Newbush_table_hikiRow());
                bush_table_hikiBindingSource.MoveLast();
                currentRowIndex_hiki.Text = this.bush_table_hikiBindingSource.Position.ToString();
                isDocumentBeingRecorded_HIKI = true;

                if (pictureBoxHiki.Image != null)
                {
                    pictureBoxHiki.Image.Dispose();
                    pictureBoxHiki.Image = null;
                }
                pictureHikiInitialization();
        



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
            }
        }

        private void pictureHikiInitialization()
        {
            hikiImageEdit.Enabled = true;
            hikiImageAdd.Enabled = true;
            hikiImageDelete.Enabled = true;
            currentRowIndex_hiki.Text = this.bush_table_hikiBindingSource.Position.ToString();
            try
            {
                string Query = @"SELECT RelatedFigures FROM mocha_db.bush_table_hiki WHERE HikiID= @hikiID ";
                String indexString = ((DataRowView)this.bush_table_hikiBindingSource.Current).Row["HikiID"].ToString();

                cmdDataBase.Connection = conDatabase;
                cmdDataBase.CommandText = Query;
                cmdDataBase.Parameters.AddWithValue("@hikiID", indexString);
                conDatabase.Open();
                int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                MySqlDataReader reader = cmdDataBase.ExecuteReader();
                
                if (reader.Read())
                {
                    hikiPicturePath = reader["RelatedFigures"].ToString();
                }

                cmdDataBase.Parameters.Clear();
                conDatabase.Dispose();



                if (!hikiPicturePath.Equals(""))
                {
                    pictureBoxHiki.Image = Image.FromFile(hikiPicturePath);
                    pictureBoxHiki.SizeMode = PictureBoxSizeMode.Zoom;
                    hikiImageAdd.Enabled = false;
                }
                else
                {
                    hikiImageEdit.Enabled = false;
                    hikiImageDelete.Enabled = false;
                }


            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);

            }
        }

        private void hikiEdit_Click(object sender, EventArgs e)
        {
            hikiPanel.Enabled = true;
            titleTextBox.Focus();
        }

        private void hikiSave_Click(object sender, EventArgs e)
        {
            try
            {

                if (taskWorker_HIKI.IsBusy != true)
                {
                    taskWorker_HIKI.RunWorkerAsync();
                    
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MessageLog2", MessageBoxButtons.OK);
            }
            hikiPanel.Enabled = false;
        }

        private void hikiDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("정말로 삭제하시겠습니까?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    bush_table_hikiBindingSource.RemoveCurrent();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);
            }
        }

        private void hikiImageAdd_Click(object sender, EventArgs e)
        {
            if (pictureFileDialog_HIKI.ShowDialog() == DialogResult.OK)
            {

                String indexString = ((DataRowView)this.bush_table_hikiBindingSource.Current).Row["HikiID"].ToString();
                try
                {
                    string sourceDir = dataBaseDirectory(pictureFileDialog_HIKI.FileName, "hikiPicture");
                    string Query = @"UPDATE mocha_db.bush_table_hiki SET RelatedFigures = @filePath WHERE HikiID= @hikiID ";
                    cmdDataBase.Connection = conDatabase;
                    cmdDataBase.CommandText = Query;
                    cmdDataBase.Parameters.AddWithValue("@filePath", sourceDir);
                    cmdDataBase.Parameters.AddWithValue("@HikiID", indexString);
                    conDatabase.Open();
                    int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                    conDatabase.Dispose();
                    cmdDataBase.Parameters.Clear();
                    pictureBoxHiki.Image = Image.FromFile(sourceDir);
                    pictureBoxHiki.SizeMode = PictureBoxSizeMode.Zoom; //CenterImage/ Normal/AutoSize
                    hikiImageEdit.Enabled = true;
                    hikiImageAdd.Enabled = false;
                    hikiImageDelete.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);

                }

            }
        }

        private void hikiImageEdit_Click(object sender, EventArgs e)
        {
            if (pictureFileDialog_HIKI.ShowDialog() == DialogResult.OK)
            {

                String indexString = ((DataRowView)this.bush_table_hikiBindingSource.Current).Row["HikiID"].ToString();
                try
                {
                    string sourceDir = dataBaseDirectory(pictureFileDialog_HIKI.FileName, "hikiPicture");
                    string Query = @"UPDATE mocha_db.bush_table_Hiki SET RelatedFigures = @filePath WHERE HikiID= @hikiID ";
                    cmdDataBase.Connection = conDatabase;
                    cmdDataBase.CommandText = Query;
                    cmdDataBase.Parameters.AddWithValue("@filePath", sourceDir);
                    cmdDataBase.Parameters.AddWithValue("@hikiID", indexString);
                    conDatabase.Open();
                    int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                    conDatabase.Dispose();
                    cmdDataBase.Parameters.Clear();
                    pictureBoxHiki.Image = Image.FromFile(sourceDir);
                    pictureBoxHiki.SizeMode = PictureBoxSizeMode.Zoom; //CenterImage/ Normal/AutoSize
                    hikiImageEdit.Enabled = true;
                    hikiImageAdd.Enabled = false;
                    hikiImageDelete.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);

                }

            }
        }

        private void hikiImageDelete_Click(object sender, EventArgs e)
        {
            String indexString = ((DataRowView)this.bush_table_hikiBindingSource.Current).Row["HikiID"].ToString();
            try
            {
                if (MessageBox.Show("그림을 삭제하시겠습니까?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string Query = @"UPDATE mocha_db.bush_table_hiki SET RelatedFigures = @filePath WHERE HikiID= @hikiID ";
                    cmdDataBase.Connection = conDatabase;
                    cmdDataBase.CommandText = Query;
                    cmdDataBase.Parameters.AddWithValue("@filePath", "");
                    cmdDataBase.Parameters.AddWithValue("@hikiID", indexString);
                    conDatabase.Open();
                    int numRowsUpdated = cmdDataBase.ExecuteNonQuery();
                    conDatabase.Dispose();
                    cmdDataBase.Parameters.Clear();
                   
                    if (pictureBoxHiki.Image != null)
                    {
                        pictureBoxHiki.Image.Dispose();
                        pictureBoxHiki.Image = null;

                        hikiImageEdit.Enabled = false;
                        hikiImageAdd.Enabled = true;
                        hikiImageDelete.Enabled = false;
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK);

            }
        }

        private void nextButton_hiki_Click(object sender, EventArgs e)
        {
            if (!isDocumentBeingRecorded_HIKI)
            {
                bush_table_hikiBindingSource.MoveNext();
                currentRowIndex_hiki.Text = this.bush_table_hikiBindingSource.Position.ToString();
                if (pictureBoxHiki.Image != null)
                {
                    pictureBoxHiki.Image.Dispose();
                    pictureBoxHiki.Image = null;
                }
                pictureHikiInitialization();
               
            }
            else
            {
                MessageBox.Show("저장을 먼저해주세요. ", "Message", MessageBoxButtons.OK);
            }
        }

        private void previousButton_hiki_Click(object sender, EventArgs e)
        {
            if (!isDocumentBeingRecorded_HIKI)
            {
                bush_table_hikiBindingSource.MovePrevious();
                currentRowIndex_hiki.Text = this.bush_table_hikiBindingSource.Position.ToString();

                if (pictureBoxHiki.Image != null)
                {
                    pictureBoxHiki.Image.Dispose();
                    pictureBoxHiki.Image = null;
                }
                pictureHikiInitialization();
                
            }
            else
            {
                MessageBox.Show("Save the document first", "Message", MessageBoxButtons.OK);
            }
        }

      

        private void uncheckAllLabel_Click(object sender, EventArgs e)
        {
            filterUncheckAll();
        }

        private void setLabel_Click(object sender, EventArgs e)
        {
            filterPanel.Controls.Clear();
            filterPanelInitialization();
        }

        Double[] xValues = { 1, 2, 3, 4, 5 };
        Double[] yValues = { 6, 7, 8, 9, 10 };


        DoubleDataSet[] setOfdata;
        public void dataInputFunction()
        {
        
            for (int i = 0; i < xValues.Length; i++)
            {
                setOfdata[i] = new DoubleDataSet(xValues[i], yValues[i]);
                
            }

        }

       
    }



    public class ChartItem
    {
        public string innerPipeRadiusValues  { get; set; }
        public string innerPipeLengthValues { get; set; }
        public string innerPipeThicknessValues { get; set; }
        public string outerPipeRadiusValues { get; set; }
        public string outerPipeLengthValues { get; set; }
        public string outerPipeThicknessValues { get; set; }
        public string rubberRadialThicknessValues { get; set; }
        public string rubberAxialThicknessValues { get; set; }
        public string voidGapValues { get; set; }
        public string angleToArmValues { get; set; }
        public string bushID { get; set; }
        public string mass { get; set; }

     
        public string getItemByName(string itemName)
        {   String returnString = "";

            if (itemName.Equals("Mass"))
            {
                returnString = this.mass;
            }
            if (itemName.Equals("InnerPipeRadius"))
            {
                returnString = this.innerPipeRadiusValues;
            }
            if (itemName.Equals("InnerPipeLength"))
            {
                returnString = this.innerPipeLengthValues;
            }
            if (itemName.Equals("InnerPipeThickness"))
            {
                returnString = this.innerPipeThicknessValues;
            }
            if (itemName.Equals("OuterPipeRadius"))
            {
                returnString = this.outerPipeRadiusValues;
            }
            if (itemName.Equals("OuterPipeLength"))
            {
                returnString = this.outerPipeLengthValues;
            }
            if (itemName.Equals("OuterPipeThickness"))
            {
                returnString = this.outerPipeThicknessValues;
            }
            if (itemName.Equals("RubberRadialThickness"))
            {
                returnString = this.rubberRadialThicknessValues;
            }
            if (itemName.Equals("RubberAxialThickness"))
            {
                returnString = this.rubberAxialThicknessValues;
            }
            if (itemName.Equals("VoidGap"))
            {
                returnString = this.voidGapValues;
            }
            if (itemName.Equals("AngleToArm"))
            {
                returnString = this.angleToArmValues;
            }
            if (itemName.Equals("BushID"))
            {
                returnString = this.bushID;
            }
            return returnString;
        }

       
        
       
    }
         

  
    public class RootObjectChart
    {
       public List<ChartItem> chartitem { get; set; }
    }
    public class Item
    {
        public string filePath { get; set; }
        public string fileName { get; set; }
    }

    public class ListItem
    {
        public string listID { get; set; }
        public string listValue { get; set;}
    }

    public class AsysItem
    {
        public string asysValue { get; set; }
        public string asysUnit { get; set; }
    }
    
    public class AsysObject
    {
        public List<AsysItem> items { get; set; }
    }
    
    public class RootObject
    {
        public List<Item> items { get; set; }
    }

    public class StringPoint
    {
        public string xDoublePoint { get; set; }
        public string yDoublePoint { get; set; }
        public StringPoint(string xVal, string yVal)
        {
            this.xDoublePoint = xVal;
            this.yDoublePoint = yVal;
        }
        
        


    }
    public class RootObjectList
    {
        public List<ListItem> items { get; set; }
    }
    public class DoubleDataSet
    {
        public DoubleDataSet(double xInput, double yInput)
        {
            this.xValue = xInput;
            this.yValue = yInput;
        }
        double xValue { get; set; }
        double yValue { get; set; }
    }



}
