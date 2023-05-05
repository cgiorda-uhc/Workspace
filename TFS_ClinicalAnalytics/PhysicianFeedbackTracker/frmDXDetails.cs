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
    public partial class frmDXDetails : Form
    {
        public frmDXDetails(string strValue)
        {
            InitializeComponent();


            string strSQL = GlobalObjects.getILUCADXDetailsSQL().Replace("{$AHRQ_DIAG_DTL_CATGY_CD}", strValue);
            string strConnectionString = GlobalObjects.strILUCAConnectionString;
            dgvDetails.DataSource = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
            dgvDetails.AutoResizeColumns();
            dgvDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }
    }
}
