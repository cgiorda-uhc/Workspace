using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhysicianFeedbackTracker
{
    public partial class frmLinks : _BaseClass
    {
        public frmLinks()
        {
            InitializeComponent();
        }

        public void populateList(List<string> strFiles)
        {
            //var stringBuilder = new StringBuilder();
            //var links = new List<LinkLabel.Link>();

            //foreach (var address in strFiles)
            //{
            //    if (stringBuilder.Length > 0) stringBuilder.AppendLine();

            //    // We cannot add the new LinkLabel.Link to the LinkLabel yet because
            //    // there is no text in the label yet, so the label will complain about
            //    // the link location being out of range. So we'll temporarily store
            //    // the links in a collection and add them later.
            //    //links.Add(new LinkLabel.Link(stringBuilder.Length, address.Length, address));



            //    string s = Path.GetFileName(address);


            //    //links.Add(new LinkLabel.Link((stringBuilder.ToString() + address).IndexOf(s), s.Length, address));
            //    links.Add(new LinkLabel.Link(stringBuilder.Length, address.Length, address));
            //    stringBuilder.Append(address);
            //}

            //var linkLabel = new LinkLabel();
            //// We must set the text before we add the links.
            //linkLabel.Text = stringBuilder.ToString();
            //foreach (var link in links)
            //{
            //    linkLabel.Links.Add(link);
            //}
            //linkLabel.AutoSize = true;
            //linkLabel.ForeColor = this.BackColor;
            //linkLabel.LinkClicked += (s, e) => {
            //    System.Diagnostics.Process.Start((string)e.Link.LinkData);
            //};

            //this.Controls.Add(linkLabel);




            //LinkLabel linkLabel;
            Int16 labelCounter = 1;
            string fileName;
            foreach (var address in strFiles)
            {
                LinkLabel linkLabel = new LinkLabel();
                fileName = Path.GetFileName(address);

                linkLabel.Text = fileName;
                linkLabel.Links.Add(0, address.Length, address);
                linkLabel.LinkClicked += (s, e) =>
                {
                    System.Diagnostics.Process.Start((string)e.Link.LinkData);
                };
                linkLabel.AutoSize = true;
                linkLabel.Top = 25 * labelCounter;
                linkLabel.Left = 10;
                this.Controls.Add(linkLabel);
                labelCounter += 1;

            }


        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
