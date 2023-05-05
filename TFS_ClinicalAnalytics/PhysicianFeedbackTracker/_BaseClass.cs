using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.DirectoryServices.AccountManagement;
using System.Drawing;

namespace PhysicianFeedbackTracker
{
    public class _BaseClass : Form
    {

        public _BaseClass()
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }


    }
}
