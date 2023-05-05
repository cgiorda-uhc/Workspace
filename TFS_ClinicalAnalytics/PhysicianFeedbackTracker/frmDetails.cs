using System;
using System.Collections;
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
    public partial class frmDetails : _BaseClass
    {

        private string _strIdentifier = null;
        private string _strValueToHighlight = null;

        public frmDetails(string strIdentifier, string strValueToHighlight = null)
        {
            InitializeComponent();

            _strIdentifier = strIdentifier;
            _strValueToHighlight = strValueToHighlight;


            populateDetails();

        }

        private void populateDetails()
        {

            Hashtable htTmp = GlobalObjects.htProviderDetailsSearchSQL(_strIdentifier.ToString());
            DataTable dt = DBConnection.getMSSQLDataTableSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strGetProviderDetailsSearchSQL, htTmp);

            dgvData.DataSource = dt;

            dgvData.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing; //or even better .DisableResizing. Most time consumption enum is DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders
            dgvData.RowHeadersVisible = false; // set it to false if not needed

            dgvData.AutoResizeColumns();
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvData.CurrentCell = null;
            dgvData.ClearSelection();

        }

        private void dgvData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;


            var senderGrid = (DataGridView)sender;

            if (senderGrid.Rows[e.RowIndex].Cells["phase_description"].Value == null)
                return;

            
            string phaseText = senderGrid.Rows[e.RowIndex].Cells["phase_description"].Value.ToString();

            Form frmAddProvders = Application.OpenForms["frmAddProvders"];
            if (frmAddProvders != null)
                ((frmAddProvders)frmAddProvders).cmbPhase.Text = phaseText;

            this.Close();


        }

        private void dgvData_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex < 1)
                return;



            if(this.dgvData.Rows[e.RowIndex].Cells["mpin"].Value != null)
            {
                if (this.dgvData.Rows[e.RowIndex].Cells["mpin"].Value.ToString().Equals(_strValueToHighlight))
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                    using (Pen p = new Pen(Color.Green, 2))
                    {
                        Rectangle rect = e.CellBounds;
                        rect.Width -= 2;
                        rect.Height -= 2;
                        e.Graphics.DrawRectangle(p, rect);
                    }
                    e.Handled = true;

                }
            }

            dgvData.CurrentCell = null;
            dgvData.ClearSelection();

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
