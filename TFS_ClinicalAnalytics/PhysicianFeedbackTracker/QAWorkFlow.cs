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
    public partial class frmQAWorkFlow : _BaseClass
    {
        public frmQAWorkFlow()
        {
            InitializeComponent();
        }

        private void QAWorkFlow_Load(object sender, EventArgs e)
        {
            DataTable dtTmp = GlobalObjects.getNameValueDataTable("qa_type");

            dtTmp = SharedDataTableFunctions.addToNameValueDatatable("--Select One--", "-9999", dtTmp);
            cmbQAType.DataSource = dtTmp;
            cmbQAType.DisplayMember = "name";
            cmbQAType.ValueMember = "value";

            dtTmp = GlobalObjects.getNameValueDataTable("qa_measure");
            dtTmp = SharedDataTableFunctions.addToNameValueDatatable("--Select One--", "-9999", dtTmp);
            cmbQAMeasures.DataSource = dtTmp;
            cmbQAMeasures.DisplayMember = "name";
            cmbQAMeasures.ValueMember = "value";


        }

        private void cmbQAMeasures_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            if (!cmb.Focused) //ONLY WHEN CLICKED
                return;


            // int selectedIndex = cmb.SelectedIndex;
            string strSelectedValue = cmb.SelectedValue.ToString();


            DataTable dtTmp = GlobalObjects.getNameValueDataTable("qa_specialty");
            dtTmp = SharedDataTableFunctions.filterNameValueDatatable(strSelectedValue, dtTmp);


            clbSpecialties.DataSource = dtTmp;
            clbSpecialties.DisplayMember = "name";
            clbSpecialties.ValueMember = "value";

            SharedWinFormFunctions.checkUncheckCheckBoxList(ref clbSpecialties, false);


        }

        private void btnGetSamplingFile_Click(object sender, EventArgs e)
        {

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //System.IO.StreamReader sr = new
                //   System.IO.StreamReader(openFileDialog1.FileName);
                //MessageBox.Show(sr.ReadToEnd());
                //sr.Close();

                dvSampling.DataSource = SharedDataTableFunctions.getDataTableFromExcel(openFileDialog.FileName, "QA", "ProviderID,Volume,Rate");
                SharedWinFormFunctions.addCheckBoxColumnToDataGridView(ref dvSampling, "Selected");






            }


            //string name = "Items";
            //string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
            //                "C:\\Sample.xls" +
            //                ";Extended Properties='Excel 8.0;HDR=YES;';";

            //OleDbConnection con = new OleDbConnection(constr);
            //OleDbCommand oconn = new OleDbCommand("Select * From [" + name + "$]", con);
            //con.Open();

            //OleDbDataAdapter sda = new OleDbDataAdapter(oconn);
            //DataTable data = new DataTable();
            //sda.Fill(data);
            //grid_items.DataSource = data;
        }

      
    }
}
