
using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PhysicianFeedbackTracker
{
    static class SharedWinFormFunctions
    {

        public static void checkUncheckCheckBoxList(ref CheckedListBox clb, bool blChecked, string strValueToCheck = null)
        {
            clb.ClearSelected();

            if(strValueToCheck == null)
            {
                foreach (int i in clb.CheckedIndices)
                {
                    if (blChecked)
                        clb.SetItemCheckState(i, CheckState.Checked);
                    else
                        clb.SetItemCheckState(i, CheckState.Unchecked);
                }
            }
            else
            {
                DataRowView drv;
                int i = 0;
                foreach (object value in clb.Items)
                {

                    drv = ((DataRowView)value);

                    if (drv[0].ToString().Equals(strValueToCheck))
                    {
                        clb.SetItemCheckState(i, CheckState.Checked);
                        break;
                    }
                    else
                        clb.SetItemCheckState(i, CheckState.Unchecked);

                    i++;
                }
            }

            //AUTOSCROLL TO FIRST CHECK ITEM
            if(clb.CheckedIndices.Count > 0)
                clb.SelectedIndex = clb.CheckedIndices[0];

        }



        public static void checkUncheckCheckBoxList(ref CheckedListBox clb, bool blChecked)
        {
            for (int i = 0; i < clb.Items.Count; i++)
            {
                clb.SetItemChecked(i, blChecked);
            }


        }


        public static string checkBoxListCheckedToCSV(CheckedListBox clb)
        {

            StringBuilder sb = new StringBuilder();
            DataRow row;

            foreach (object itemChecked in clb.CheckedItems)
            {
                row = (itemChecked as DataRowView).Row;
                sb.Append(row[0] + ",");
            }

            return sb.ToString().TrimEnd(','); 

        }




        public static void addCheckBoxColumnToDataGridView(ref DataGridView dgv, string strColumnName, Int16? intColumnIndex = null)
        {
            int intColumnIndexTmp = (intColumnIndex == null ? dgv.ColumnCount : (int)intColumnIndex);

            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
            checkColumn.Name = strColumnName;
            checkColumn.HeaderText = strColumnName;
            checkColumn.Width = (strColumnName.Length * 7);
            checkColumn.ReadOnly = false;
            checkColumn.FillWeight = 10; //if the datagridview is resized (on form resize) the checkbox won't take up too much; value is relative to the other columns' fill values
            //dgv.Columns.Add(checkColumn);
            dgv.Columns.Insert(intColumnIndexTmp, checkColumn);
        }

        


        public static void addButtonColumnToDataGridView(ref DataGridView dgv, string strColumnName,Color BackColor, Color ForeColor, int? intColumnIndex = null, bool blHasHeader = true )
        {
            int intColumnIndexTmp = (intColumnIndex == null ? dgv.ColumnCount  : (int)intColumnIndex);

            if (dgv.Columns.Contains(strColumnName) != true)
            {


                DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
                buttonColumn.Name = strColumnName;

                buttonColumn.HeaderText = (blHasHeader ? strColumnName : "");

                buttonColumn.Text = strColumnName;
                buttonColumn.Width = (strColumnName.Length * 10);
                buttonColumn.ReadOnly = false;
                buttonColumn.FillWeight = 100; //if the datagridview is resized (on form resize) the checkbox won't take up too much; value is relative to the other columns' fill values
                buttonColumn.UseColumnTextForButtonValue = true;
                buttonColumn.FlatStyle = FlatStyle.Flat;
                buttonColumn.DefaultCellStyle.BackColor = BackColor;
                buttonColumn.DefaultCellStyle.ForeColor = ForeColor;
                buttonColumn.DefaultCellStyle.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
                dgv.Columns.Insert(intColumnIndexTmp, buttonColumn);
            }
        }


        public static void removeSortGlyphsFromDataGridView(ref DataGridView dgv, string strColumnNameExclude = null)
        {
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                if (column.Name != strColumnNameExclude)
                {
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
            }
        }



        public static DataTable handeDataGridViewSorting(ref DataGridView dgv, DataTable dt, int intColumnIndex)
        {
            string strSortOrder = "";
            string strColumnName = "";

            strColumnName = dgv.Columns[intColumnIndex].Name;
            if (dgv.Columns[intColumnIndex].HeaderCell.SortGlyphDirection == System.Windows.Forms.SortOrder.Ascending)
            {
                dgv.Columns[intColumnIndex].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Descending;
                strSortOrder = " ASC";
            }
            else if (dgv.Columns[intColumnIndex].HeaderCell.SortGlyphDirection == System.Windows.Forms.SortOrder.Descending)
            {
                dgv.Columns[intColumnIndex].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
                strSortOrder = " DESC";
            }
            else
            {
                dgv.Columns[intColumnIndex].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Descending;
                strSortOrder = " ASC";
            }

            removeSortGlyphsFromDataGridView(ref dgv, strColumnName);


            dt.DefaultView.Sort = strColumnName + " " + strSortOrder;

            dgv.Rows.Clear();
            dgv.RowCount = GlobalObjects.dtTrackingParentCache.Rows.Count;

            return dt.DefaultView.ToTable();

        }




        public static void checkUncheckListView(ref ListView lb, bool blChecked)
        {
            for (int i = 0; i < lb.Items.Count; i++)
            {
                lb.Items[i].Checked = blChecked;
            }
        }


        public static void addDataTableToListView(ref ListView lv, DataTable dt,string[] strArrColumnsToExclude, bool blFullRowSelect = true, bool blMultiSelect = false, bool blCheckbox = false)
        {
            lv.Clear();
            lv.View = View.Details;
            lv.FullRowSelect = blFullRowSelect;
            lv.MultiSelect = blMultiSelect;
            lv.CheckBoxes = blCheckbox;

            foreach (DataColumn col in dt.Columns)
            {
                if (Array.IndexOf(strArrColumnsToExclude, col.ColumnName) > -1)
                    continue;

                lv.Columns.Add(col.ColumnName);
            }

            foreach (DataRow row in dt.Rows)
            {
                ListViewItem item = null;

                foreach (DataColumn col in dt.Columns)
                {
                    if (Array.IndexOf(strArrColumnsToExclude, col.ColumnName) > -1)
                        continue;


                    if (item == null)
                        item = new ListViewItem(row[col.ColumnName].ToString());
                    else
                        item.SubItems.Add(row[col.ColumnName].ToString());
                }

                lv.Items.Add(item);
            }


            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }



        public static void hideColumnsInDataGridView(ref DataGridView dgv, string[] strArrColumnsToHide)
        {

            foreach(string s in strArrColumnsToHide)
            {
                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    if(column.Name == s)
                    {
                        column.Visible = false;
                    }
                }
            }

 
        }




        public static  void DrawGroupBox(GroupBox box, Graphics g, Color textColor, Color borderColor)
        {
            if (box != null)
            {
                Brush textBrush = new SolidBrush(textColor);
                Brush borderBrush = new SolidBrush(borderColor);
                Pen borderPen = new Pen(borderBrush);
                SizeF strSize = g.MeasureString(box.Text, box.Font);
                Rectangle rect = new Rectangle(box.ClientRectangle.X,
                                               box.ClientRectangle.Y + (int)(strSize.Height / 2),
                                               box.ClientRectangle.Width - 1,
                                               box.ClientRectangle.Height - (int)(strSize.Height / 2) - 1);

                // Clear text and border
                //g.Clear(this.BackColor);

                // Draw text
                //g.DrawString(box.Text, box.Font, textBrush, box.Padding.Left, 0);

                // Drawing Border
                //Left
                g.DrawLine(borderPen, rect.Location, new Point(rect.X, rect.Y + rect.Height));
                //Right
                g.DrawLine(borderPen, new Point(rect.X + rect.Width, rect.Y), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                //Bottom
                g.DrawLine(borderPen, new Point(rect.X, rect.Y + rect.Height), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                //Top1
                g.DrawLine(borderPen, new Point(rect.X, rect.Y), new Point(rect.X + box.Padding.Left, rect.Y));
                //Top2
                g.DrawLine(borderPen, new Point(rect.X + box.Padding.Left + (int)(strSize.Width), rect.Y), new Point(rect.X + rect.Width, rect.Y));
            }
        }




    }

}
