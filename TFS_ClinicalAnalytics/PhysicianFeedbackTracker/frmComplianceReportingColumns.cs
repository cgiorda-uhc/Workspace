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
    public partial class frmComplianceReportingColumns : Form
    {
        public frmComplianceReportingColumns()
        {
            InitializeComponent();

            populateColumns(true);

        }


        string[] strColAllArr = { "Practice (TIN and Name)" , "# of Surg", "Avg Total Allowed", "Measures", "Facility (TIN, Name and Type)", "Surgery", "Specialty",  "PD Status (# of Surg > 0 AND Provider = 'Practice')", "Line of Business", "Market Name", "Average Allowed Per Episode (Additional Sheet)" };
        string[] strColDefaultArr;
        public void populateColumns(bool isPractice, bool blResetAll = true)
        {
            List<string> items = null;
            if (!blResetAll)
            {
                //items = clbColumns.CheckedItems.OfType<object>().ToArray();
                items = clbColumns.CheckedItems.OfType<string>().ToList();
            }


            clbColumns.Items.Clear();
            strColDefaultArr = null; 
            foreach (string s in strColAllArr)
            {
                if(isPractice)
                {
                    if(s != "Facility (TIN, Name and Type)")
                    {
                        clbColumns.Items.Add(s);
                    }
                    else if (items != null)
                    {
                        var i = items.IndexOf(s);
                        if (i > -1)
                            items.RemoveAt(i);
                    }
                    if (strColDefaultArr == null)
                    {
                        strColDefaultArr = new string[] { "Practice (TIN and Name)", "# of Surg", "Avg Total Allowed", "Measures" };
                        if(items != null)
                            strColDefaultArr = strColDefaultArr.Union(items.ToArray()).ToArray();
                    }
                        
                }
                else
                {
                    if (s != "PD Status (# of Surg > 0 AND Provider = 'Practice')" && s != "Practice (TIN and Name)" && s != "Market Name")
                    {
                        clbColumns.Items.Add(s);
                    }
                    else if (items != null)
                    {
                        var i = items.IndexOf(s);
                        if (i > -1)
                            items.RemoveAt(i);
                    }
                    if (strColDefaultArr == null)
                    {
                        strColDefaultArr = new string[] { "Facility (TIN, Name and Type)", "# of Surg", "Avg Total Allowed", "Measures" };
                        if (items != null)
                            strColDefaultArr = strColDefaultArr.Union(items.ToArray()).ToArray();
                    }
                        
                }

            }

            //if(items != null)
            //    setDefaults(items.ToArray());
            //else
                setDefaults();
        }


        public void setDefaults()
        {

            for (int i = 0; i < clbColumns.Items.Count; i++)
            {


       
                    if (strColDefaultArr.Any(clbColumns.Items[i].ToString().Equals))
                    {
                        clbColumns.SetItemCheckState(i, CheckState.Checked);
                    }
                    else
                    {
                        clbColumns.SetItemCheckState(i, 0);
                    }
     
            }

        }

        private void tsmClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void tsmDefault_Click(object sender, EventArgs e)
        {
            setDefaults();
        }

        private void clbColumns_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            ////int number = clbColumns.CheckedItems.Count;
            //if (e.NewValue == CheckState.Checked)
            //{
            //    CheckedListBox senderCheckBox = (CheckedListBox)sender;
            //    if(senderCheckBox.Items[e.Index].ToString().StartsWith("PD Status"))
            //    {
            //        int index = senderCheckBox.Items.IndexOf("Facility (TIN, Name and Type)");
            //        if(clbColumns.GetItemCheckState(index) == CheckState.Checked)
            //        {

            //            DialogResult dr = MessageBox.Show("PD Status is not valid with Facilites. Change Facility to Practice?", "Change Facility to Practice", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            //            if (dr == DialogResult.Yes)
            //            {
            //                clbColumns.SetItemChecked(index, false);
            //                index = senderCheckBox.Items.IndexOf("Practice (TIN and Name)");
            //                clbColumns.SetItemChecked(index, true);
            //            }
            //            else
            //            {
            //                e.NewValue = e.CurrentValue;
            //                return;
            //            }
            //        }
            //    }
            //    else if (senderCheckBox.Items[e.Index].ToString().StartsWith("Facility (TIN, Name and Type)"))
            //    {
            //        int index = senderCheckBox.Items.IndexOf("PD Status (# of Surg > 0 AND Provider = 'Practice')");
            //        if (clbColumns.GetItemCheckState(index) == CheckState.Checked)
            //        {

            //            DialogResult dr = MessageBox.Show("PD Status is not valid with Facilites. Change Facility to Practice?", "Change Facility to Practice", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            //            if (dr == DialogResult.Yes)
            //            {
            //                index = senderCheckBox.Items.IndexOf("Practice (TIN and Name)");
            //                clbColumns.SetItemChecked(index, true);
            //                index = senderCheckBox.Items.IndexOf("Facility (TIN, Name and Type)");
            //                clbColumns.SetItemChecked(index, false);
            //                e.NewValue = e.CurrentValue;
            //            }
            //            else
            //            {
            //                clbColumns.SetItemChecked(index, false);
            //                return;
            //            }
            //        }
            //    }
            //}

        }
    }
}
