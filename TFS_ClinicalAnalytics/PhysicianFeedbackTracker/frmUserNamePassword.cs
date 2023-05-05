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
    public partial class frmUserNamePassword : Form
    {

        public string strUserName{ get; set; }
        public string strPassword { get; set; }

        public string strDBName { get; set; }

        public frmUserNamePassword()
        {
            InitializeComponent();
            txtUserName.Text = GlobalObjects.strCurrentUser;



            lblUserName.Text = strDBName + " Username:";
            lblPassword.Text = strDBName + " Password:";
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserName.Text) || string.IsNullOrEmpty(txtPassword.Text))
                return;

            strUserName = txtUserName.Text;
            strPassword = txtPassword.Text;

            this.Close();


        }
    }
}
