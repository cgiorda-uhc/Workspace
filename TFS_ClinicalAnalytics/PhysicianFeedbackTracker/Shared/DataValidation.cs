using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhysicianFeedbackTracker
{
    static class DataValidation
    {

        private static DataSet _dsFinalResult;
        private static DataTable _dtFinalResultTmp;
        private static DataTable _dtOverviewFinalTmp;
        //private static DataTable dtMainFinal = null;
        //private static DataTable dtOverviewFinal = null;



        private static string[] arrValidateDataRulesColumns = new string[] { "Column Name", "Column Data Type", "Validate Data Type", "Data Required", "Columns To Rows", "CSV Cell to Row", "Action Name" };
        private static string[] arrQueryColumns = new string[] { "Action Name", "Action", "Action Type", "Connection String" };

        private static DataTable _dtMain = null;
        private static DataTable _dtRules = null;
        private static DataTable _dtActions = null;

        public static DataSet getValidatedData(DataTable dtMain, DataTable dtRules, DataTable dtActions, ref TextBox txtStatus)
        {

            //SET PRIVATE VARIABLE WITH EXCEL VALUES
            _dtMain = dtMain;
            _dtRules = dtRules;
            _dtActions = dtActions;


            _dtFinalResultTmp = new DataTable();
            _dtFinalResultTmp.Columns.Add("Clone Id", typeof(int));
            _dtFinalResultTmp.Columns.Add("Clone Cnt", typeof(int));
            _dtFinalResultTmp.dataRowsToColumnsInTable(dtRules, "Column Name", "Column Data Type");

            _dtOverviewFinalTmp = new DataTable();
            DataColumn newColumn = new DataColumn("Message", System.Type.GetType("System.String"));
            _dtOverviewFinalTmp.Columns.Add(newColumn);

            //SETUP RESULTS CONTAINER END


            //START CHECKS
            checkTemplateIntegrity(ref txtStatus);


            _dsFinalResult = new DataSet();
            _dsFinalResult.Tables.Add(_dtOverviewFinalTmp);
            (_dsFinalResult.Tables[_dsFinalResult.Tables.Count - 1]).TableName = "dtOverviewFinal";


            if (_dtOverviewFinalTmp.Rows.Count > 0)
                return _dsFinalResult;


            //OTHER CHECKS......
            validateMainDataSet(ref txtStatus);
            
           DataTable tmpFinal = cleanUpDataTable(_dtFinalResultTmp,ref txtStatus);
           tmpFinal = csvCellToRow(tmpFinal, ref txtStatus);
           tmpFinal = runActions(tmpFinal, ref txtStatus);


            _dsFinalResult.Tables.Add(tmpFinal); //clone source table
            (_dsFinalResult.Tables[_dsFinalResult.Tables.Count - 1]).TableName = "dtMainFinal";

            return _dsFinalResult;

        }


        private static DataTable runActions(DataTable dt, ref TextBox txtStatus)
        {
            txtStatus.AppendText("Validating Data via Action Scripts..." + Environment.NewLine);

            string strCurrentColumn;
            string strNewColumnName;
            int iRowCnt = 0;

            DataRow drTmp;
            DataRow drActionResultCache;
            DataTable dtActionResultCache = new DataTable();
            dtActionResultCache.Columns.Add("SQL", typeof(String));
            dtActionResultCache.Columns.Add("ErrorMessage", typeof(String));

      
            //MAIN VALIDATION LOOP
            foreach (DataRow rowRule in _dtRules.Rows)
            {
                strCurrentColumn = rowRule["Column Name"].ToString();
                iRowCnt = 0;

                if (rowRule["Action Name"] != DBNull.Value)
                {
                    dtActionResultCache.Rows.Clear();

                    foreach (DataRow row in dt.Rows)
                    {

                        string strErrorMessage = null;
                        string strSQL = null;
                        string strConnectionString = null;

                        string[] strActionArr = rowRule["Action Name"].ToString().Split(',');

                        foreach (string s in strActionArr)
                        {
                            string srtAction = s.Trim();

                            string strLocation = "The Action Named [" + srtAction + "]";


                            DataRow[] drAction = _dtActions.Select("[Action Name] = '" + srtAction + "'");
                            if (drAction.Count() > 1)
                            {
                                strErrorMessage = strLocation + " was listed multiple times within the Actions Sheet";
                            }
                            else
                            {
                                //NO ACTION IF NULL AND NOT REQUIRED
                                if (row[strCurrentColumn]== DBNull.Value && !rowRule["Data Required"].ToString().Equals("Yes"))
                                {
                                    continue;
                                }
                                


                                strSQL = prepSQL(drAction[0]["Action"].ToString(), row, strLocation, strCurrentColumn, out strErrorMessage);
                                strConnectionString = drAction[0]["Connection String"].ToString();

                                drTmp = dtActionResultCache.Select("[SQL] = '" + strSQL.Replace("'","''") + "'").FirstOrDefault();

                                if(drTmp != null)
                                {
                                    txtStatus.AppendText("Cached Action Script '" + srtAction + "': {" + strSQL + "}..." + Environment.NewLine);
                                    if(drTmp["ErrorMessage"]  != DBNull.Value)
                                    {
                                        strErrorMessage = drTmp["ErrorMessage"].ToString();
                                    }
                                }
                                else
                                {
                                    if (drAction[0]["Action Type"].ToString() == "Validate")
                                    {
                                        if (strErrorMessage == null)
                                        {

                                            try
                                            {
                                                txtStatus.AppendText("Running Action Script '" + srtAction + "': {" + strSQL + "}..." + Environment.NewLine);

                                                DataTable t = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                                                if (t.Rows.Count <= 0)
                                                {
                                                    strErrorMessage = strLocation + " - Failed Validation! The following query return no results {" + strSQL + "}.";
                                                }
                                             

                                            }
                                            catch (Exception ex)
                                            {
                                                strErrorMessage = strLocation + " - Unxpected Error! {" + ex.Message + "} for the following query {" + strSQL + "}.";
                                            }
                                            finally
                                            {
                                                drActionResultCache = dtActionResultCache.NewRow();
                                                drActionResultCache["SQL"] = strSQL;
                                                drActionResultCache["ErrorMessage"] = strErrorMessage;
                                                dtActionResultCache.Rows.Add(drActionResultCache);
                                            }

                                        }
                                    }
                                }

                            }

                            if (strErrorMessage != null)
                                break;

                        }
                        if (strErrorMessage != null)
                        {
                            strNewColumnName = strCurrentColumn + "_Feedback!!!";
                            if (!dt.Columns.Contains(strNewColumnName))
                            {
                                DataColumn Col = dt.Columns.Add(strNewColumnName);
                                Col.SetOrdinal(dt.Columns[strCurrentColumn].Ordinal + 1);
                            }

                            if (dt.Rows[iRowCnt][strNewColumnName] == DBNull.Value)
                                dt.Rows[iRowCnt][strNewColumnName] = strErrorMessage;
                        }

                        iRowCnt++;


                        Application.DoEvents();

                    }

                }
            }
            return dt;
        }


        private static DataTable csvCellToRow(DataTable dt, ref TextBox txtStatus)
        {

            string strCurrentColumn;
            string strNewColumnName;
            string[] strToRowArr = null;
            string strToRow = null;
            DataView view = null;
            DataTable distinctValues = null;
            DataRow[] drResults = null;
            int intIteration = 0;
            int iRowCnt = 0;
            foreach (DataRow rowRule in _dtRules.Rows)
            {
                if(rowRule["CSV Cell to Row"].ToString().Equals("Yes"))
                {

                    strCurrentColumn = rowRule["Column Name"].ToString();



                    txtStatus.AppendText("Transposing Columns To Rows for "+ strCurrentColumn + "..." + Environment.NewLine);

                    view = new DataView(dt);
                    distinctValues = view.ToTable(true, strCurrentColumn);

                    foreach (DataRow r in distinctValues.Rows)
                    {

                        drResults = dt.Select(strCurrentColumn + " = '" + r[strCurrentColumn] + "'");
                        if (drResults.Count() < 1)
                            continue;


                        strToRow = drResults[0][strCurrentColumn].ToString().removeNoise();
                        strToRowArr = strToRow.Split(',');
                        if (strToRowArr.Count() <= 1)
                                continue;

                        intIteration = 0;
                        foreach (string s in strToRowArr)
                        {
                            foreach (DataRow row in drResults)
                            {
                                if (intIteration == 0)
                                {
                                    row[strCurrentColumn] = s;
                                }
                                else
                                {
                                    var desRow = dt.NewRow();
                                    desRow.ItemArray = row.ItemArray;
                                    desRow[strCurrentColumn] = s;
                                    dt.Rows.Add(desRow);
                                }
                                    
                            }
                            intIteration++;
                        }
                        Application.DoEvents();
                    }



                }

            }

            //view = dt.DefaultView;
            //view.Sort = "[Clone Id] asc";
            //dt = view.ToTable();



            foreach (DataRow rowRule in _dtRules.Rows)
            {
                if (rowRule["CSV Cell to Row"].ToString().Equals("Yes") && rowRule["Validate Data Type"] + "" == "Yes" )
                {
                    strCurrentColumn = rowRule["Column Name"].ToString();
                    string strValue = null;
                    string strType = null;
                    bool blValid = false;
                    
                    foreach (DataRow r in dt.Rows)
                    {
                        strValue = r[strCurrentColumn].ToString();
                        strType = rowRule["Column Data Type"].ToString();
                        switch (strType)
                        {
                            case "Int":
                                blValid = strValue.IsNumeric();
                                break;
                            case "Date":
                                blValid = strValue.IsDate();
                                break;
                            default:
                                blValid = true;
                                break;
                        }

                        if(!blValid)
                        {

                            r[strCurrentColumn] = null;

                            strNewColumnName = strCurrentColumn + "_Feedback!!!";
                            if (!dt.Columns.Contains(strNewColumnName))
                            {
                                DataColumn Col = dt.Columns.Add(strNewColumnName);
                                Col.SetOrdinal(dt.Columns[strCurrentColumn].Ordinal + 1);
                            }
                            if (r[strNewColumnName] == DBNull.Value)
                                r[strNewColumnName] = "For the Column '" + strCurrentColumn + "' the Value '" + strValue + "' is not a valid " + strType;


                            //if (dt.Rows[iRowCnt][strNewColumnName] == DBNull.Value)
                            //    dt.Rows[iRowCnt][strNewColumnName] = "For the Column '" + strCurrentColumn + "' the Value '" + strValue + "' is not a valid " + strType;



                        }
                        blValid = true;
                        iRowCnt++;
                    }
               }
            }



            view = dt.DefaultView;
            view.Sort = "[Clone Id] asc";

            DataTable t = view.ToTable();
            t.Columns.Remove("Clone Id");
            t.Columns.Remove("Clone Cnt");

            return t;

        }

        private static DataTable cleanUpDataTable(DataTable dt, ref TextBox txtStatus)
        {

            txtStatus.AppendText("Running Initial Data Cleanup..." + Environment.NewLine);

            DataView dv = dt.DefaultView;

            //MERGE CLONED ROWS
            DataTable dtClones = dv.ToTable(true, "Clone Id");
            DataRow[] drArr = null;
            DataRow dr = null;
            foreach (DataRow dataRow in  dtClones.Rows)
            {

                if (dataRow["Clone Id"] == DBNull.Value)
                    continue;

                drArr = dt.Select("[Clone Id] = " + dataRow["Clone Id"], "[Clone Cnt] ASC");

                if (drArr.Count() <= 1)
                    continue;

                //COPY HERE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                dr = drArr[0];


                for (int i = 1; i < drArr.Count(); i++)
                {
                    DataRow currRow = drArr[i];
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if(currRow[dt.Columns[j].ColumnName] == DBNull.Value)
                        {
                            currRow[dt.Columns[j].ColumnName] = dr[dt.Columns[j].ColumnName];
                        }
                    }

                }

                Application.DoEvents();
            }


            //SORT
            dv = dt.DefaultView;
            dv.Sort = "[Clone Id] asc";
            dt = dv.ToTable();
            //dt.DeleteEmptyRows();
            return dt;
        }

        private static string prepSQL( string strSQL, DataRow dr, string strLocation, string strCurrentColumn, out string strError)
        {
            StringBuilder sbError = new StringBuilder();


            foreach (DataRow rowRule in _dtRules.Rows)
            {

                //!!!!!!!!!!!!!!!!if (rowRule["Columns To Rows"].ToString().Equals("Yes"))
                //!!!!!!!!!!!!!!!if (rowRule["Columns To Rows"].ToString().Equals("Yes"))
                //!!!!!!!!!!!!!!!if (rowRule["Columns To Rows"].ToString().Equals("Yes"))

                if(strCurrentColumn.ToLower() == "mpin")
                {
                    string s = "";
                }


                if (strSQL.Contains("{$this}") && strCurrentColumn == rowRule["Column Name"].ToString())
                {
                    string strTick = (rowRule["Column Data Type"].ToString().Equals("Int") || rowRule["Column Data Type"].ToString().Equals("Bool") ? "" : "'");
                    strSQL = strSQL.Replace("{$this}", strTick + dr[rowRule["Column Name"].ToString()].ToString().Replace("'","''") + strTick);
                }
                else
                {
                    if (!dr.Table.Columns.Contains(rowRule["Column Name"].ToString()))
                        continue;

                    if (strSQL.Contains("{$" + rowRule["Column Name"] + "}"))
                    {
                        if (dr[rowRule["Column Name"].ToString()] == DBNull.Value)
                        {
                            sbError.Append(rowRule["Column Name"].ToString() + ",");
                            //break;
                        }
                        else
                        {
                            string strTick = (rowRule["Column Data Type"].ToString().Equals("Int") || rowRule["Column Data Type"].ToString().Equals("Bool") ? "" : "'");
                            strSQL = strSQL.Replace("{$" + rowRule["Column Name"] + "}", strTick + dr[rowRule["Column Name"].ToString()].ToString().Replace("'", "''") + strTick);
                        }
                    }
                }

            }

            strError = null;
            if (sbError.Length > 0)
            {
                strError = strLocation + " could not be executed because the following filters are null: " + sbError.ToString().TrimEnd(',');
            }



            return strSQL;

        } 


        private static void validateMainDataSet(ref TextBox txtStatus)
        {
            string  strCurrentColumn;
            string strNewColumnName;
            int iRowCnt = 0;
            int iColCnt = 2;


            int intCurrentClone = 0;


            txtStatus.AppendText("Verifying Data Integrity..." + Environment.NewLine);

            //PREP TABLE FOR DATA TYPE VALIDATION IF NEED BE
            foreach (DataRow rowRule in _dtRules.Rows)
            {
                strCurrentColumn = rowRule["Column Name"].ToString();
                if (rowRule["Validate Data Type"] + "" != "Yes" || rowRule["CSV Cell to Row"].ToString().Equals("Yes"))
                {
                    _dtFinalResultTmp.Columns[strCurrentColumn].DataType = typeof(String);
                }
            }

            //MAIN VALIDATION LOOP
            foreach (DataRow rowRule in _dtRules.Rows)
            {
                strCurrentColumn = rowRule["Column Name"].ToString();

                foreach (DataRow row in _dtMain.Rows)
                {
                    //if (row.AreAllColumnsEmpty())
                    //    continue;

                    if (iColCnt == 2)
                        _dtFinalResultTmp.Rows.Add();

                    //UNCOMMWENT ME!!!! WELL DEAL WITH THESE INSTANCES AFTER THE INITIAL CLEANUP
                    //if (rowRule["CSV Cell to Row"].ToString().Equals("Yes"))
                    //    continue;


                    if (rowRule["Data Required"].ToString().Equals("Yes"))
                    {
                        if(row[strCurrentColumn] == DBNull.Value)
                        {
                            strNewColumnName = strCurrentColumn + "_Feedback!!!";
                            if (!_dtFinalResultTmp.Columns.Contains(strNewColumnName))
                            {
                                DataColumn Col = _dtFinalResultTmp.Columns.Add(strNewColumnName);
                                Col.SetOrdinal(_dtFinalResultTmp.Columns[strCurrentColumn].Ordinal + 1);
                            }

                            if (_dtFinalResultTmp.Rows[iRowCnt][strNewColumnName] == DBNull.Value)
                                _dtFinalResultTmp.Rows[iRowCnt][strNewColumnName] = "Data is Required for {" + strCurrentColumn + "}";
                            //continue;

                        }
                    }



                    if (rowRule["Columns To Rows"].ToString().Equals("Yes"))
                    {

                        if (iRowCnt == 0)
                            intCurrentClone = 1;

                        for (int i = 1; i <= 2; i++)
                        {

                            if (row.Table.Columns.Contains(strCurrentColumn + " " + i))
                            {
                                if (row[strCurrentColumn + " " + i] == DBNull.Value)
                                    continue;


                                //if(intCurrentClone == 59)
                                //{
                                //    string s = "";
                                //    s = "";
                                //}

                                DataRow dr = _dtFinalResultTmp.Select("[Clone Id] = " + intCurrentClone + " AND [Clone Cnt] = " + i).FirstOrDefault();
                                if (dr == null)
                                {
                                    if(i > 1)
                                    {
                                        dr = _dtFinalResultTmp.Rows.Add();
                                        dr["Clone Id"] = intCurrentClone;
                                        dr["Clone Cnt"] = i;
                                        dr[strCurrentColumn] = row[strCurrentColumn + " " + i];

                                    }
                                    else
                                    {
                                        _dtFinalResultTmp.Rows[iRowCnt]["Clone Id"] = intCurrentClone;
                                        _dtFinalResultTmp.Rows[iRowCnt]["Clone Cnt"] = i;
                                        _dtFinalResultTmp.Rows[iRowCnt][strCurrentColumn] = row[strCurrentColumn + " " + i];
                                    }

                                }
                                else
                                {
                                    dr[strCurrentColumn] = row[strCurrentColumn + " " + i];
                                }
                                

                            }
       
                            
                        }
                        intCurrentClone++;
                    }
                    else
                    {
                        try
                        {

                            if (rowRule["Validate Data Type"].ToString().Equals("Yes") && rowRule["Column Data Type"].ToString().Equals("Bool") && row[strCurrentColumn] != DBNull.Value)
                            {
                                string strValue = row[strCurrentColumn].ToString();
                                _dtFinalResultTmp.Rows[iRowCnt][strCurrentColumn] = (strValue.ToLower() == "yes" ? "true" : (strValue.ToLower() == "no" ? "false" : strValue));
                            }
                            else
                                _dtFinalResultTmp.Rows[iRowCnt][strCurrentColumn] = row[strCurrentColumn];
                        }
                        catch(Exception ex)
                        {
                            strNewColumnName = strCurrentColumn + "_Feedback!!!";
                            if (!_dtFinalResultTmp.Columns.Contains(strNewColumnName))
                            {
                                DataColumn Col = _dtFinalResultTmp.Columns.Add(strNewColumnName);
                                Col.SetOrdinal(_dtFinalResultTmp.Columns[strCurrentColumn].Ordinal + 1);
                            }
                            if (_dtFinalResultTmp.Rows[iRowCnt][strNewColumnName] == DBNull.Value)
                                _dtFinalResultTmp.Rows[iRowCnt][strNewColumnName] = ex.Message;

                        }     
                    }

                    iRowCnt++;
                    Application.DoEvents();
                }
                iColCnt++;
                iRowCnt = 0;
            }
        }

        private static void checkTemplateIntegrity(ref TextBox txtStatus)
        {

            txtStatus.AppendText("Verifying Template Integrity..." + Environment.NewLine);


            DataRow feedbackRow;

            string strColumnValue;
            string strColumnValue2;
            string strColumnToRows;
            bool blPassed = false;

            string[] strValuesArr = null;

            //Validate Sheets' Existence
            //Validate Sheets' Existence
            //Validate Sheets' Existence
            if(_dtMain==null || _dtRules == null || _dtActions == null)
            {
                if(_dtMain == null)
                {
                    feedbackRow = _dtOverviewFinalTmp.NewRow();
                    feedbackRow[0] = "{Validate_Data} Sheet is Missing";
                    _dtOverviewFinalTmp.Rows.Add(feedbackRow);
                }

                if (_dtRules == null)
                {
                    feedbackRow = _dtOverviewFinalTmp.NewRow();
                    feedbackRow[0] = "{Validate_Data_Rules} Sheet is Missing";
                    _dtOverviewFinalTmp.Rows.Add(feedbackRow);
                }

                if (_dtActions == null)
                {
                    feedbackRow = _dtOverviewFinalTmp.NewRow();
                    feedbackRow[0] = "{Actions} Sheet is Missing";
                    _dtOverviewFinalTmp.Rows.Add(feedbackRow);
                }

                return;
            }


            //Validate_Data_Rules Column Check
            //Validate_Data_Rules Column Check
            //Validate_Data_Rules Column Check
            blPassed = true;
            foreach (string s in arrValidateDataRulesColumns)
            {
                if (!_dtRules.Columns.Contains(s))
                {
                    feedbackRow = _dtOverviewFinalTmp.NewRow();
                    feedbackRow[0] = "{Validate_Data_Rules} Sheet is Missing Column {" + s + "}";
                    //feebackRow[1] = s;
                    _dtOverviewFinalTmp.Rows.Add(feedbackRow);
                    blPassed = false;
                }
                Application.DoEvents();
            }
            if (!blPassed)
                return;


            //Queries Column Check
            //Queries Column Check
            //Queries Column Check
            blPassed = true;
            foreach (string s in arrQueryColumns)
            {
                if (!_dtActions.Columns.Contains(s))
                {
                    feedbackRow = _dtOverviewFinalTmp.NewRow();
                    feedbackRow[0] = "{Actions} Sheet is Missing Column {" + s + "}";
                    //feebackRow[1] = s;
                    _dtOverviewFinalTmp.Rows.Add(feedbackRow);
                    blPassed = false;
                }
                Application.DoEvents();
            }
            if (!blPassed)
                return;


            //Compare Validate_Data Colums with Validate_Data_Rules Columns 
            //Compare Validate_Data Colums with Validate_Data_Rules Columns 
            //Compare Validate_Data Colums with Validate_Data_Rules Columns 
            blPassed = false;
            foreach (DataColumn col in _dtMain.Columns)
            {

                strColumnValue = col.ColumnName.Trim().Replace("\n", "");

                foreach (DataRow row in _dtRules.Rows)
                {
                    strColumnValue2 = row["Column Name"].ToString().Trim().Replace("\n", "");
                    strColumnToRows = row["Columns To Rows"].ToString();

                    if ((strColumnToRows == "Yes" && strColumnValue.Contains(strColumnValue2)) || (strColumnValue == strColumnValue2))
                    {
                        blPassed = true;
                        break;
                    }
                }

                if(!blPassed)
                {
                    feedbackRow = _dtOverviewFinalTmp.NewRow();
                    feedbackRow[0] = "{Validate_Data_Rules} Sheet is Missing {" + strColumnValue + "} Under {Column Name} ";
                    //feebackRow[1] = s;
                    _dtOverviewFinalTmp.Rows.Add(feedbackRow);
                }
                blPassed = false;
            }

             if (!blPassed)
                return;


            //Compare Validate_Data_Rules Queries with Query Sheet
            //Compare Validate_Data_Rules Queries with Query Sheet
            //Compare Validate_Data_Rules Queries with Query Sheet 
            foreach (DataRow row in _dtRules.Rows)
            {
                strColumnValue = row["Action Name"].ToString().Trim();
                strColumnValue2 = row["Column Name"].ToString().Trim();

                strValuesArr = strColumnValue.Trim().Split(',');
                foreach(string s in strValuesArr)
                {
                    if (String.IsNullOrEmpty(s.Trim()))
                        continue;


                    if (_dtActions.Select("[Action Name] = '" + s.Trim() + "'").Count() <= 0)
                    {
                        feedbackRow = _dtOverviewFinalTmp.NewRow();
                        feedbackRow[0] = "The Action {" + s.Trim() + "} listed in {Validate_Data_Rules} under {" + strColumnValue2 + "} cannot be found in {Actions} sheet ";
                        //feebackRow[1] = s;
                        _dtOverviewFinalTmp.Rows.Add(feedbackRow);
                    }
                }

            }



            //CHECK FOR NULL VALUES WITHIN ACTIONS
            //CHECK FOR NULL VALUES WITHIN ACTIONS
            //CHECK FOR NULL VALUES WITHIN ACTIONS
            StringBuilder sbErrors = new StringBuilder();
            int iCnt = 2;
            foreach (DataRow row in _dtActions.Rows)
            {
                if(row["Action Name"]== DBNull.Value)
                {
                    sbErrors.Append("[Action Name] value is missing on sheet [Actions] row # " + iCnt + "\n");
                }

                if (row["Action"] == DBNull.Value)
                {
                    sbErrors.Append("[Action] is missing on sheet [Actions]  row # " + iCnt + "\n");
                }

                if (row["Action Type"] == DBNull.Value)
                {
                    sbErrors.Append("[Action Type] is missing on sheet [Actions] row # " + iCnt + "\n");
                }

                if (row["Connection String"] == DBNull.Value)
                {
                    sbErrors.Append("[Connection String] is missing on sheet [Actions] row # " + iCnt + "\n");
                }

                iCnt++;
            }

            if (sbErrors.Length > 0)
            {
                feedbackRow = _dtOverviewFinalTmp.NewRow();
                feedbackRow[0] = sbErrors.ToString();
                //feebackRow[1] = s;
                _dtOverviewFinalTmp.Rows.Add(feedbackRow);
            }

            return;


        }
    }

}
