using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhysicianFeedbackTracker
{
    public partial class frmSlpash : Form
    {
        //Delegate for cross thread call to close
        private delegate void CloseDelegate();

        //The type of form to be displayed as the splash screen.
        private static frmSlpash splashForm;

        static public void ShowSplashScreen()
        {


            // Make sure it is only launched once.    
            if (splashForm != null) return;
            splashForm = new frmSlpash();


            splashForm.Cursor = Cursors.WaitCursor;
            splashForm.Width = 400;
            splashForm.Height = 275;
            //splashForm.ControlBox = false;
            //splashForm.Text = String.Empty;
            splashForm.StartPosition = FormStartPosition.CenterScreen;
            splashForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Label lblSplash = new Label();
            lblSplash.Text = "Caching required data. Please wait...";
            lblSplash.AutoSize = false;
            lblSplash.TextAlign = ContentAlignment.MiddleCenter;
            lblSplash.Dock = DockStyle.Fill;
            lblSplash.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblSplash.Name = "lblSplash";
            splashForm.Controls.Add(lblSplash);




            Thread thread = new Thread(new ThreadStart(frmSlpash.ShowForm));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        static private void ShowForm()
        {
            if (splashForm != null) Application.Run(splashForm);
        }

        static public void CloseForm()
        {
            splashForm?.Invoke(new CloseDelegate(frmSlpash.CloseFormInternal));
        }

        static private void CloseFormInternal()
        {
            if (splashForm != null)
            {
                splashForm.Close();
                splashForm = null;
            };
        }

       
    }
}
