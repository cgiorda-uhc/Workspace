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
    public partial class frmHome : _BaseClass
    {
        public frmHome()
        {
            InitializeComponent();
        }

        frmAddProvders _frmAddProvders;
        private void btnAddProviders_Click(object sender, EventArgs e)
        {
            _frmAddProvders = new frmAddProvders();
            _frmAddProvders.ShowDialog(this);
        }

        frmQAWorkFlow _frmQAWorkFlow;
        private void btnQAWorkFlow_Click(object sender, EventArgs e)
        {
            _frmQAWorkFlow = new frmQAWorkFlow();
            _frmQAWorkFlow.ShowDialog(this);
        }
    }
}
