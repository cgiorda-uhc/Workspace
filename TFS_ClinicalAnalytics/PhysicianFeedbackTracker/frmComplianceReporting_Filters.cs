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
    public partial class frmComplianceReporting_Filters : Form
    {

        public Dictionary<string, string> dicProviderFiltersGlobal = new Dictionary<string, string>();
        public Dictionary<string, string> dicMeasureFiltersGlobal = new Dictionary<string, string>();
        public Dictionary<string, string> dicSpecialtyFiltersGlobal = new Dictionary<string, string>();
        public Dictionary<string, string> dicSurgicalFiltersGlobal = new Dictionary<string, string>();
        public Dictionary<string, string> dicQuarterFiltersGlobal = new Dictionary<string, string>();
        public Dictionary<string, string> dicFacilityTypeFiltersGlobal = new Dictionary<string, string>();
        public Dictionary<string, string> dicMarketFiltersGlobal = new Dictionary<string, string>();

        public frmComplianceReporting_Filters()
        {
            InitializeComponent();
        }

        //private const int CP_NOCLOSE_BUTTON = 0x200;
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams myCp = base.CreateParams;
        //        myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
        //        return myCp;
        //    }
        //}

        public bool blResetGLOBAL = false;
        public string strCurrentFilterGLOBAL = null;
        public void populateFilters(string strFilterName)
        {
            Dictionary<string, string> dicCurrentFilters = null;
            strCurrentFilterGLOBAL = strFilterName;
            switch (strFilterName)
            {
                case "Quarter":
                    dicCurrentFilters = dicQuarterFiltersGlobal;
                    break;
                case "Measure":
                    dicCurrentFilters = dicMeasureFiltersGlobal;
                    break;
                case "Market":
                    dicCurrentFilters = dicMarketFiltersGlobal;
                    break;
                case "Surgical":
                    dicCurrentFilters = dicSurgicalFiltersGlobal;
                    break;
                case "Specialty":
                    dicCurrentFilters = dicSpecialtyFiltersGlobal;
                    break;
                case "Provider":
                    dicCurrentFilters = dicProviderFiltersGlobal.ToDictionary(x => x.Key, x => x.Value + " - " + x.Key);
                    break;
                case "FacilityType":
                    dicCurrentFilters = dicFacilityTypeFiltersGlobal;
                    break;
            }

            lblCurrentFilter.Text =  strCurrentFilterGLOBAL + " Filters";

            //var dicTmp = dicCurrentFilters.ToDictionary(x => x.Key, x => x.Value + " - " + x.Key);



            clbCurrentFilters.DataSource = new BindingSource(dicCurrentFilters, null);
            clbCurrentFilters.DisplayMember = "Value";
            clbCurrentFilters.ValueMember = "Key";

            for (int i = 0; i < clbCurrentFilters.Items.Count; i++)
            {
                clbCurrentFilters.SetItemChecked(i, true);
            }

        }

        private void lvProviderResults_MouseClick(object sender, MouseEventArgs e)
        {
            //var where = clbCurrentFilter.HitTest(e.Location);
            //if (where.Location == ListViewHitTestLocations.Label)
            //{
            //    where.Item.Checked = !where.Item.Checked;
            //}
        }

        private void frmComplianceReporting_Filters_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }


        private void btnClearAllFilters_Click(object sender, EventArgs e)
        {


            DialogResult dr = MessageBox.Show("Are you sure you want to clear all "+ strCurrentFilterGLOBAL + " filters?", "Clear Filters", MessageBoxButtons.YesNo,
            MessageBoxIcon.Information);

            if (dr != DialogResult.Yes)
            {
                return;
            }

            Dictionary<string, string> dicCurrentFilters = null;
            switch (strCurrentFilterGLOBAL)
            {
                case "Quarter":
                    dicCurrentFilters = dicQuarterFiltersGlobal;
                    break;
                case "Measure":
                    dicCurrentFilters = dicMeasureFiltersGlobal;
                    break;
                case "Market":
                    dicCurrentFilters = dicMarketFiltersGlobal;
                    break;
                case "Surgical":
                    dicCurrentFilters = dicSurgicalFiltersGlobal;
                    break;
                case "Specialty":
                    dicCurrentFilters = dicSpecialtyFiltersGlobal;
                    break;
                case "Provider":
                    dicCurrentFilters = dicProviderFiltersGlobal;
                    break;
                case "FacilityType":
                    dicCurrentFilters = dicFacilityTypeFiltersGlobal;
                    break;
            }

            dicCurrentFilters.Clear();
            clbCurrentFilters.DataSource = null;
            clbCurrentFilters.Items.Clear();
            blResetGLOBAL = true;
            this.Hide();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //CHECK FOR UNCHECKED ITEMS
            Dictionary<string, string> dicCurrentFilters = null;
            switch (strCurrentFilterGLOBAL)
            {
                case "Quarter":
                    dicCurrentFilters = dicQuarterFiltersGlobal;
                    break;
                case "Measure":
                    dicCurrentFilters = dicMeasureFiltersGlobal;
                    break;
                case "Market":
                    dicCurrentFilters = dicMarketFiltersGlobal;
                    break;
                case "Surgical":
                    dicCurrentFilters = dicSurgicalFiltersGlobal;
                    break;
                case "Specialty":
                    dicCurrentFilters = dicSpecialtyFiltersGlobal;
                    break;
                case "Provider":
                    dicCurrentFilters = dicProviderFiltersGlobal;
                    break;
                case "FacilityType":
                    dicCurrentFilters = dicFacilityTypeFiltersGlobal;
                    break;
            }

            for (int i = 0; i < clbCurrentFilters.Items.Count; i++)
            {
                if(!clbCurrentFilters.GetItemChecked(i))
                {
                    dicCurrentFilters.Remove(((System.Collections.Generic.KeyValuePair<string, string>)clbCurrentFilters.Items[i]).Key);
                    blResetGLOBAL = true;
                }
            }
            clbCurrentFilters.DataSource = null;
            clbCurrentFilters.Items.Clear();
            this.Hide();
        }

        private void clbCurrentFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            clbCurrentFilters.ClearSelected();
        }
    }
}
