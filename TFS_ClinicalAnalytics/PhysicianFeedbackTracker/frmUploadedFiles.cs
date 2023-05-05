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
    public partial class frmUploadedFiles : _BaseClass
    {
        private string _strPath = null;


        public frmUploadedFiles(string strPath)
        {
            InitializeComponent();

            _strPath = strPath;

            LoadFiles();

            
        }



        private void LoadFiles()
        {
            var files = System.IO.Directory.GetFiles(_strPath);


            dgvUploadedFiles.Rows.Clear();


            if(dgvUploadedFiles.Columns.Count <= 0)
            {
                DataGridViewLinkColumn col = new DataGridViewLinkColumn();
                col.DataPropertyName = "FileName";
                col.Name = "FileName";

                dgvUploadedFiles.Columns.Add(col);
                dgvUploadedFiles.Columns.Add("FileLink", "FileLink");
                SharedWinFormFunctions.addButtonColumnToDataGridView(ref dgvUploadedFiles, "Delete", Color.Red, Color.White, 0, false);
                SharedWinFormFunctions.hideColumnsInDataGridView(ref dgvUploadedFiles, new string[] { "FileLink" });
            }



            foreach (var f in files)
            {
                dgvUploadedFiles.Rows.Add("Delete", Path.GetFileName(f), f);
                //dgvUploadedFiles.Rows.Add();
            }

            dgvUploadedFiles.AutoResizeColumns();
            dgvUploadedFiles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            
        }

        private void dgvUploadedFiles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                // Open the link in the default browser
                System.Diagnostics.Process.Start(dgvUploadedFiles.Rows[e.RowIndex].Cells["FileLink"].Value.ToString());
            }
        }

        private void dgvUploadedFiles_SelectionChanged(object sender, EventArgs e)
        {
            dgvUploadedFiles.ClearSelection();
        }

        private void dgvUploadedFiles_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1)
            {
                var senderGrid = (DataGridView)sender;
                if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)//IGNORE BUTTON ROWS
                {
                    var confirmResult = MessageBox.Show("Delete selected file from uploads?", "Confirm Delete!", MessageBoxButtons.YesNo); //ALWAYS CONFIRM FIRST
                    if (confirmResult == DialogResult.Yes)
                    {
                        string strDeletePath = dgvUploadedFiles.Rows[e.RowIndex].Cells["FileLink"].Value.ToString();

                        if(File.Exists(strDeletePath))
                            File.Delete(strDeletePath);

                        LoadFiles();
                    }

                    return;
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
