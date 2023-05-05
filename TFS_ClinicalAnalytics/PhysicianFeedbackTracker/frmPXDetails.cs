using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhysicianFeedbackTracker
{
    public partial class frmPXDetails : Form
    {
        DataTable dtProcCategories;
        TableLayoutPanel panel;
        EventHandler eh_radioButtons_CheckedChanged;
        string strCatCode_GLOBAL;
        string strProcCode_GLOBAL;
        string strCatDesc_GLOBAL;
        string strProcDesc_GLOBAL;
        string strCurrent_GLOBAL;
        public frmPXDetails(string strCatCode, string procCode, string catDesc, string procDesc)
        {
            InitializeComponent();

            //IF DEFAULT DOESNT CHANGE THEN ITS BEEN CANCELLED!
            this.DialogResult = DialogResult.Cancel;


            // [AHRQ_PROC_DTL_CATGY_CD] ,[AHRQ_PROC_DTL_CATGY_DESC], [PROC_CD],[PROC_DESC]
            strCatCode_GLOBAL = strCatCode;
            strProcCode_GLOBAL = procCode;
            strCatDesc_GLOBAL = catDesc;
            strProcDesc_GLOBAL = procDesc;
            strCurrent_GLOBAL = "AHRQ_PROC_DTL_CATGY_CD = {" + strCatCode_GLOBAL  + "} , AHRQ_PROC_DTL_CATGY_DESC] = {" + strCatDesc_GLOBAL + "} , PROC_CD = {" + strProcCode_GLOBAL  + "} , PROC_DESC  = {" + strProcDesc_GLOBAL  + "} ";
            lblCurrent.Text = strCurrent_GLOBAL;

            dtProcCategories = DBConnection.getMSSQLDataTable(GlobalObjects.strILUCAConnectionString, GlobalObjects.getILUCAProcCategorySQL());
            DataRow newRow = dtProcCategories.NewRow();
            newRow[0] = "--select--";
            dtProcCategories.Rows.InsertAt(newRow, 0);

            string strProcCategory = null;
            string strSens = null;
            string strSensOB = null;
            int iControlCnt = 1;


            eh_radioButtons_CheckedChanged = new EventHandler(radioButtons_CheckedChanged);


            DataTable dtOptions = DBConnection.getMSSQLDataTable(GlobalObjects.strILUCAConnectionString, GlobalObjects.getILUCAPXGroupingsSQL().Replace("{$AHRQ_PROC_DTL_CATGY_CD}", strCatCode));

            panel = new TableLayoutPanel();



            panel.ColumnCount = 4;
            panel.RowCount = 1;

    
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));



            Padding padding = new Padding(0);
            Padding padding2 = new Padding();

            //pnl = new FlowLayoutPanel();
            panel.Width = 1000;
            panel.BorderStyle = BorderStyle.Fixed3D;
            panel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            //pnl.

            //pnl.FlowDirection = FlowDirection.TopDown;
            panel.Dock = DockStyle.Fill;
            RadioButton rb = new RadioButton()
            {
                Name = "radSelection0",
                Text = "Create New Proc Category:",
                ImageIndex = 0,
                AutoSize = true,
                Dock = DockStyle.Fill,
               Margin = padding2
            };
            rb.CheckedChanged += eh_radioButtons_CheckedChanged;


            panel.Controls.Add(rb,0, 0);
            panel.Controls.Add(new ComboBox { Name = "cmbProcCategory0", Text = "", Visible = true, Dock = DockStyle.Fill, Margin = padding2, DataSource = dtProcCategories, DisplayMember = "Proc_Categ" }, 1, 0);
            panel.Controls.Add(new CheckBox { Name = "chkSens0", Text = "Sens=Y", Visible = true, Dock = DockStyle.Fill, Margin = padding, Anchor = AnchorStyles.None }, 2, 0);
            panel.Controls.Add(new CheckBox { Name = "chkSensOB0", Text = "SensOB=Y", Visible = true, Dock = DockStyle.Fill, Margin = padding, Anchor = AnchorStyles.None }, 3, 0);


            foreach (DataRow dr in dtOptions.Rows)
            {
                strProcCategory = (dr["Proc_Categ"] == DBNull.Value ? "NULL" : dr["Proc_Categ"].ToString());
                strSens = (dr["Sens"] == DBNull.Value ? "NULL" : dr["Sens"].ToString());
                strSensOB = (dr["Sens_OB"] == DBNull.Value ? "NULL" : dr["Sens_OB"].ToString());

                rb = new RadioButton()
                {
                    Name = "radSelection" + iControlCnt,
                    Text = "Proc_Categ={" + strProcCategory + "}  AND  Sens={" + strSens + "}  AND  Sens_OB={" + strSensOB + "}",
                    ImageIndex = iControlCnt,
                    AutoSize = true,
                    Dock = DockStyle.Fill,
                    Margin = padding2
                };
                rb.CheckedChanged += eh_radioButtons_CheckedChanged;

                panel.RowCount = panel.RowCount + 1;
                panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                panel.Controls.Add(rb, 0, panel.RowCount - 1);
                panel.Controls.Add(new ComboBox { Name= "cmbProcCategory" + iControlCnt, Text = strProcCategory, Visible=false, Margin = padding2 }, 1, panel.RowCount - 1);
                panel.Controls.Add(new CheckBox  { Name = "chkSens" + iControlCnt, Text = strSens, Visible = false, Margin = padding, Anchor = AnchorStyles.None }, 2, panel.RowCount - 1);
                panel.Controls.Add(new CheckBox { Name = "chkSensOB" + iControlCnt, Text = strSensOB, Visible = false, Margin = padding, Anchor = AnchorStyles.None }, 3, panel.RowCount - 1);

                iControlCnt++;
            }

            grpSelectGroup.Controls.Add(panel);


            dgvPXDetails.DataSource = DBConnection.getMSSQLDataTable(GlobalObjects.strILUCAConnectionString, GlobalObjects.getILUCAPXDetailsSQL().Replace("{$AHRQ_PROC_DTL_CATGY_CD}", strCatCode));
            dgvPXDetails.AutoResizeColumns();
            dgvPXDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

        }

        public string errorMessage { get; set; }
        private void button1_Click(object sender, EventArgs e)
        {

            RadioButton rbSelected = panel.Controls
                         .OfType<RadioButton>()
                         .FirstOrDefault(r => r.Checked);

            if (rbSelected == null)
            {
                MessageBox.Show("No item selected for update");
                return;
            }


            int? intResultCnt = 0;
            errorMessage = null;
            try
            {


                string strTick = null;

                string strProcCat = null;
                string strSens = null;
                string strSensOB = null;

                ComboBox cmb = panel.Controls
                 .OfType<ComboBox>()
                 .FirstOrDefault(r => r.Name == "cmbProcCategory" + rbSelected.ImageIndex);


                strProcCat = cmb.Text;

                if (strProcCat == "--select--")
                {
                    MessageBox.Show(strProcCat + " is an invalid category name");
                    return;
                }
                else
                    strProcCat = "'" + strProcCat + "'";
                // MessageBox.Show("ProcCategory=" + txt.Text);


                CheckBox cbx = panel.Controls
                 .OfType<CheckBox>()
                 .FirstOrDefault(r => r.Name == "chkSens" + rbSelected.ImageIndex);


                if (!cbx.Checked && cbx.Visible)
                    strSens = "NULL";
                else if (cbx.Checked && cbx.Visible)
                    strSens = "'Y'";
                else if (!cbx.Visible)
                {
                    if (cbx.Text.Trim().ToLower().Equals("null"))
                        strTick = "";
                    else
                        strTick = "'";

                    strSens = strTick + cbx.Text + strTick;
                }
                

                cbx = panel.Controls
                 .OfType<CheckBox>()
                 .FirstOrDefault(r => r.Name == "chkSensOB" + rbSelected.ImageIndex);


                if (!cbx.Checked && cbx.Visible)
                    strSensOB = "NULL";
                else if (cbx.Checked && cbx.Visible)
                    strSensOB = "'Y'";
                else if(!cbx.Visible)
                {
                    if (cbx.Text.Trim().ToLower().Equals("null"))
                        strTick = "";
                    else
                        strTick = "'";

                    strSensOB = strTick + cbx.Text + strTick;
                }

                //MessageBox.Show(GlobalObjects.getILUCAPXUpdateSQL().Replace("{$Proc_Categ}", strProcCat).Replace("{$Sens}", strSens).Replace("{$Sens_OB}", strSensOB).Replace("{$PROC_CD}", strProcCode_GLOBAL));

           
                intResultCnt = (int?)DBConnection.getMSSQLExecuteScalar(GlobalObjects.strILUCAConnectionString, GlobalObjects.getILUCAPXUpdateSQL().Replace("{$Proc_Categ}", strProcCat).Replace("{$Sens}", strSens).Replace("{$Sens_OB}", strSensOB).Replace("{$PROC_CD}", strProcCode_GLOBAL));

                if (intResultCnt > 0)
                {
                    this.DialogResult = DialogResult.OK;

                    DBConnection.ExecuteMSSQL(GlobalObjects.strILUCAConnectionString, GlobalObjects.getILUCAPXUpdateProperCaseSQL().Replace("{$PROC_CD}", strProcCode_GLOBAL));

                }
                else
                {
                    errorMessage = "No Rows Updated!!!";
                    this.DialogResult = DialogResult.Cancel;
                }

                this.Close();

            }
            catch(Exception ex)
            {
                errorMessage = ex.ToString();
                this.DialogResult = DialogResult.None;
            }
  
        }


        private void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control c in panel.Controls)
            {
                c.ForeColor = Color.Black;
            }


            RadioButton rb = sender as RadioButton;
            if (rb.Checked)
            {
                rb.ForeColor = Color.Red;

                ComboBox cmb = panel.Controls
                 .OfType<ComboBox>()
                 .FirstOrDefault(r => r.Name == "cmbProcCategory" + rb.ImageIndex);

                cmb.ForeColor = Color.Red;

                // MessageBox.Show("ProcCategory=" + txt.Text);


                CheckBox cbx = panel.Controls
                 .OfType<CheckBox>()
                 .FirstOrDefault(r => r.Name == "chkSens" + rb.ImageIndex);


                // MessageBox.Show("Sens=" + cbx.Text);
                cbx.ForeColor = Color.Red;

                cbx = panel.Controls
                 .OfType<CheckBox>()
                 .FirstOrDefault(r => r.Name == "chkSensOB" + rb.ImageIndex);


                //MessageBox.Show("SensOB=" + cbx.Text);

                cbx.ForeColor = Color.Red;




            }
        }


    }
}
