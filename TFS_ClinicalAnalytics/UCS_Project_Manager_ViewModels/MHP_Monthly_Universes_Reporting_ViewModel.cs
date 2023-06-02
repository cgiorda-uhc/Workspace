using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using ClosedXML.Excel;
using GalaSoft.MvvmLight;
using UCS_Project_Manager_Models;
using UCS_Project_Manager_Services;
using System.Data.Entity;
using System.Text;
using System.Windows;
using System.Threading;
using System.Diagnostics;

namespace UCS_Project_Manager
{
    public class MHP_Yearly_Universes_Reporting_ViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand ReportTypeCommand { get; set; }

        public RelayCommand StateChangedCommand { get; set; }

        public event Action Done = delegate { };

        private IMHP_Yearly_Universes_Reporting_Repository _repo;


        //CONSTRUCTOR
        //CONSTRUCTOR
        //CONSTRUCTOR
        public MHP_Yearly_Universes_Reporting_ViewModel(IMHP_Yearly_Universes_Reporting_Repository repo)
        {
            CancelReportVisibility = Visibility.Hidden;
            GenerateReportVisibility = Visibility.Visible;
            EIFormVisibility = Visibility.Visible;
            CSFormVisibility = Visibility.Hidden;
            IFPFormVisibility = Visibility.Hidden;


            _repo = repo;

            //LOAD SUPPORTING ARRAYS
            LoadSupportLists();


            //LINK COMMANDS TO FUNCTIONS
            SearchCommand = new RelayCommand(Search);

            //LINK COMMANDS TO FUNCTIONS
            CancelCommand = new RelayCommand(Cancel);


            ReportTypeCommand = new RelayCommand(ReportType);

            StateChangedCommand = new RelayCommand(StateChanged);

        }

        string _reportType;
        private void ReportType(object item)
        {
            _reportType = item.ToString();

            _selected_states = null;
           _selected_states = new List<string>();

            cleanGroups();

            if (_reportType == "CS")
            {
                EIFormVisibility = Visibility.Hidden;
                IFPFormVisibility = Visibility.Hidden;
                CSFormVisibility = Visibility.Visible;


            }
            else if(_reportType == "IFP")
            {
                EIFormVisibility = Visibility.Hidden;
                CSFormVisibility = Visibility.Hidden;
                IFPFormVisibility = Visibility.Visible;


            }
            else
            {
                EIFormVisibility = Visibility.Visible;
                CSFormVisibility = Visibility.Hidden;
                IFPFormVisibility = Visibility.Hidden;
            }


        }



        private List<string> _selected_states;
        private void StateChanged(object item)
        {
            string strItem = item.ToString();


            if(_selected_states == null)
                _selected_states = new List<string>();


            //if (_reportType == "CS")
            // {
            if (strItem == "--All--")
                {

                    _selected_states.Clear();

                    //if (_selected_states.Contains(strItem))
                    //{
                    //    _selected_states.Clear();
                    //}
                    //else
                    //{
                    //    _selected_states  = _lstGroupStateAll.GroupBy(s => s.State_of_Issue).Select(g => g.First()).OrderBy(s => s.State_of_Issue).Select(g => g.State_of_Issue).ToList();
                    //}
                }
                else if (_selected_states.Contains(strItem))
                {
                    _selected_states.Remove(strItem);
                }
                else
                {
                    _selected_states.Add(strItem);
                }

                cleanGroups();
            //}

        }



        private void cleanGroups()
        {
            //if (_reportType == "CS")
            //{
                List<string> tmp;
                this.GroupNumbers.Clear();

                if (_selected_states.Count() > 0)
                    tmp = _lstGroupStateAll.Where(x => _selected_states.Contains(x.State_of_Issue)).GroupBy(s => s.Group_Number).Select(g => g.First()).OrderBy(s => s.Group_Number).Select(g => g.Group_Number).ToList();
                else
                    tmp = _lstGroupStateAll.GroupBy(s => s.Group_Number).Select(g => g.First()).OrderBy(s => s.Group_Number).Select(g => g.Group_Number).ToList();

                foreach (string s in tmp)
                {
                    this.GroupNumbers.Add(s);
                }
                this.GroupNumbers.Insert(0, "--All--");
            //}
        }

        private CancellationTokenSource cancellationToken;
        private void Search(object item)
        {

             try
             {
                cancellationToken = new CancellationTokenSource();
                CancelReportVisibility = Visibility.Visible;
                GenerateReportVisibility = Visibility.Hidden;
                object[] parameters = item as object[];


                if(_reportType == "CS")
                {
                    var arr = parameters[3].ToString().Replace("--All--", "").Split(',');
                    Task.Run(() => WaitForCSToWork(arr.Count()));
                }
                else if (_reportType == "IFP")
                {
                    var arr = parameters[3].ToString().Replace("--All--", "").Split(',').ToList<string>();
                    Task.Run(() => WaitForIFPToWork(arr.Count()));
                }
                else
                {
                    var legalEnitiy = parameters[3].ToString().Replace("--All--~", "").Split('~').ToList<string>();
                    Task.Run(() => WaitForItToWork(legalEnitiy.Count()));
                }
   


         
                Task.Run(async () => await generateReports(parameters, cancellationToken.Token));

           

             }
            catch (OperationCanceledException)
            {

                Status = "~~~Report Generation Cancelled~~~";

                CancelReportVisibility = Visibility.Hidden;
                GenerateReportVisibility = Visibility.Visible;
            }

    }



        async Task<bool> WaitForItToWork(int intLegalEntitCnt)
        {
            var stopWatch = Stopwatch.StartNew();

            bool succeeded = false;
            while (!succeeded)
            {
                // do work
                succeeded = (GenerateReportVisibility == Visibility.Visible); // if it worked, make as succeeded, else retry
                StatusValue = "Legal Entity(s) selected: " + intLegalEntitCnt;
                TimerValue = "Time Elapsed: " + stopWatch.Elapsed.ToString(@"m\:ss\.fff");
                await Task.Delay(200); // arbitrary delay
            }
            stopWatch.Stop();
            return succeeded;
        }

        async Task<bool> WaitForCSToWork(int intMapCnt)
        {
            var stopWatch = Stopwatch.StartNew();

            bool succeeded = false;
            while (!succeeded)
            {
                // do work
                succeeded = (GenerateReportVisibility == Visibility.Visible); // if it worked, make as succeeded, else retry
                StatusValue = "CS_TADM_PRDCT_MAP(s) selected: " + intMapCnt;
                TimerValue = "Time Elapsed: " + stopWatch.Elapsed.ToString(@"m\:ss\.fff");
                await Task.Delay(200); // arbitrary delay
            }
            stopWatch.Stop();
            return succeeded;
        }


        async Task<bool> WaitForIFPToWork(int intMapCnt)
        {
            var stopWatch = Stopwatch.StartNew();

            bool succeeded = false;
            while (!succeeded)
            {
                // do work
                succeeded = (GenerateReportVisibility == Visibility.Visible); // if it worked, make as succeeded, else retry
                StatusValue = "Products(s) selected: " + intMapCnt;
                TimerValue = "Time Elapsed: " + stopWatch.Elapsed.ToString(@"m\:ss\.fff");
                await Task.Delay(200); // arbitrary delay
            }
            stopWatch.Stop();
            return succeeded;
        }


        private void Cancel()
        {

           Status = "Cancel requested. Please wait...";


            if (cancellationToken != null)
                cancellationToken.Cancel();

            // try
            // {
            CancelReportVisibility = Visibility.Hidden;
            GenerateReportVisibility = Visibility.Visible;




            // }
            //catch (Exception e)
            // {
            // throw e;
            //}

        }

        private Visibility _generateReportVisibility;
        public Visibility GenerateReportVisibility
        {
            get
            {
                return _generateReportVisibility;
            }
            set
            {
                _generateReportVisibility = value;

                NotifyPropertyChanged("GenerateReportVisibility");
            }
        }
        private Visibility _cancelReportVisibility;
        public Visibility CancelReportVisibility
        {
            get
            {
                return _cancelReportVisibility;
            }
            set
            {
                _cancelReportVisibility = value;

                NotifyPropertyChanged("CancelReportVisibility");
            }
        }


        private Visibility _eiFormVisibility;
        public Visibility EIFormVisibility
        {
            get
            {
                return _eiFormVisibility;
            }
            set
            {
                _eiFormVisibility = value;

                NotifyPropertyChanged("EIFormVisibility");
            }
        }


        private Visibility _csFormVisibility;
        public Visibility CSFormVisibility
        {
            get
            {
                return _csFormVisibility;
            }
            set
            {
                _csFormVisibility = value;

                NotifyPropertyChanged("CSFormVisibility");
            }
        }



        private Visibility _ifpFormVisibility;
        public Visibility IFPFormVisibility
        {
            get
            {
                return _ifpFormVisibility;
            }
            set
            {
                _ifpFormVisibility = value;

                NotifyPropertyChanged("IFPFormVisibility");
            }
        }



        private string _timerValue;
        public string TimerValue
        {
            get
            {
                return _timerValue; ;
            }
            set
            {
                _timerValue = value;

                NotifyPropertyChanged("TimerValue");
            }
        }


        private string _statusValue;
        public string StatusValue
        {
            get
            {
                return _statusValue; ;
            }
            set
            {
                _statusValue = value;

                NotifyPropertyChanged("StatusValue");
            }
        }

        private StringBuilder sbStatus = new StringBuilder();


        //LAZY VALUE TO EXCEL CS A1
        string _strGlobalFilterList;
       private async Task<bool> generateReports(object[] parameters, CancellationToken token)
        {


            try
            {
                if (token.IsCancellationRequested)
                {
                    Status = "~~~Report Generation Cancelled~~~";
                    return true;
                }

                // Execution of the async method will continue one second later, but without
                // blocking.
                //await Task.Delay(1000, token);
                //await Application.Current.Dispatcher.InvokeAsync((Action)(() => { Mouse.OverrideCursor = Cursors.Wait; })).Task.Unwrap();
                Application.Current.Dispatcher.Invoke((Action)(() => { Mouse.OverrideCursor = Cursors.Wait; })); 
                Disable = true;



                sbStatus.Append("-Processing selected filters" + Environment.NewLine);
                Status = sbStatus.ToString(); // use thi
                                              //await Task.Run(() => System.Threading.Thread.Sleep(1000));
                                              //string strState = parameters[0].ToString();





                string strState = "'" + parameters[0].ToString().Replace("--All--,", "").Replace(",", "','") + "'";
                string strStartDate = parameters[1].ToString();
                string strEndDate = parameters[2].ToString();
                List<string> lstLegalEntities = null;
                string strFINC_ARNG_CD = null;
                string strMKT_SEG_RLLP_DESC = null;
                string strMKT_TYP_DESC = null;
                string strCS_TADM_PRDCT_MAP = null;
                string strCUST_SEG = null;
                string strGroupNumbers = null;
                List<string> lstProducts = null;

                if (_reportType == "CS")
                {
                    strCS_TADM_PRDCT_MAP = "'" + parameters[3].ToString().Replace("--All--,", "").Replace(",", "','") + "'";
                    _strGlobalFilterList = strCS_TADM_PRDCT_MAP.Replace("'", "");

                    if (!string.IsNullOrEmpty(parameters[4] + ""))
                          strGroupNumbers = "'" + parameters[4].ToString().Replace("--All--,", "").Replace(",", "','") + "'";
                }
                else if (_reportType == "IFP")
                {
                    lstProducts = parameters[3].ToString().Replace("--All--,", "").Split(',').ToList<string>();
                }
                else
                {
                    lstLegalEntities = parameters[3].ToString().Replace("--All--~", "").Split('~').ToList<string>();
                   strFINC_ARNG_CD = "'" + parameters[4].ToString().Replace("--All--,", "").Replace(",", "','") + "'";
                    strMKT_SEG_RLLP_DESC = "'" + parameters[5].ToString().Replace("--All--,", "").Replace(",", "','") + "'";
                    if (!string.IsNullOrEmpty(parameters[6] +""))
                        strMKT_TYP_DESC = "'" + parameters[6].ToString().Replace("--All--,", "").Replace(",", "','") + "'";
                    if (!string.IsNullOrEmpty(parameters[7]+""))
                    {
                        var cA = parameters[7].ToString().Split(',');
                        StringBuilder sb = new StringBuilder();
                        foreach(string s in cA)
                        {
                            sb.Append("'" + s.Trim() + "',");
                        }

                        strCUST_SEG = sb.ToString().TrimEnd(',');

                        //strCUST_SEG = "'" + parameters[7].ToString().Trim().Replace(",", "','") + "'";
                    }
                        
                }




                sbStatus.Append("-Retreiving summary data from Database" + Environment.NewLine);
                Status = sbStatus.ToString(); // use thi//await Task.Run(() => System.Threading.Thread.Sleep(1000));

                if (token.IsCancellationRequested)
                {
                    Status = "~~~Report Generation Cancelled~~~";
                    return true;
                }

                List<MHP_Yearly_Universes_Reporting_Model> mhp_results = null;
                List<MHPCS_Yearly_Universes_Reporting_Model> mhpcs_results = null;
                List<MHPIFP_Yearly_Universes_Reporting_Model> mhpifp_results = null;
                try
                {

                    if (_reportType == "CS")
                    {
                        mhpcs_results = await Task.Run(async () => await _repo.GetMHPCSDataAsync(strState, strStartDate, strEndDate, strCS_TADM_PRDCT_MAP, strGroupNumbers, token));
                    }
                    else if (_reportType == "IFP")
                    {
                        mhpifp_results = await Task.Run(async () => await _repo.GetMHPIFPDataAsync(strState, strStartDate, strEndDate, lstProducts, token));
                    }
                    else
                         mhp_results = await Task.Run(async () => await _repo.GetMHPDataAsync(strState, strStartDate, strEndDate, strFINC_ARNG_CD, strMKT_SEG_RLLP_DESC, lstLegalEntities, strMKT_TYP_DESC, strCUST_SEG, _reportType == "IFP", token));
                }
                catch (OperationCanceledException)
                {

                    token.ThrowIfCancellationRequested();
                }
                catch (Exception e)
                {
                    sbStatus.Append("Error thrown in Summary:" + Environment.NewLine + e.ToString());
                    Status = sbStatus.ToString(); // use thi
                    return true;
                }

                //if (mhp_results == null)
                    //return;



                sbStatus.Append("-Retreiving details data from Database" + Environment.NewLine);
                Status = sbStatus.ToString(); // use thi


                if (token.IsCancellationRequested)
                {
                    Status = "~~~Report Generation Cancelled~~~";
                    return true;
                }
                List<MHP_Yearly_Universes_Details_Model> mhp_details = null;
                List<MHPCS_Yearly_Universes_Details_Model> mhpcs_details = null;
                List<MHPIFP_Yearly_Universes_Details_Model> mhpifp_details = null;
                try
                {
                    if (_reportType == "CS")
                    {
                        mhpcs_details = await Task.Run(async () => await _repo.GetMHPCSDetailsAsync(strState, strStartDate, strEndDate, strCS_TADM_PRDCT_MAP, strGroupNumbers, token));
                    }
                    else if (_reportType == "IFP")
                    {
                        mhpifp_details = await Task.Run(async () => await _repo.GetMHPIFPDetailsAsync(strState, strStartDate, strEndDate,  lstProducts, token));
                    }
                    else
                    {
                        mhp_details = await Task.Run(async () => await _repo.GetMHPDetailsAsync(strState, strStartDate, strEndDate, strFINC_ARNG_CD, strMKT_SEG_RLLP_DESC, lstLegalEntities, strMKT_TYP_DESC, strCUST_SEG, _reportType == "IFP", token));
                    }

                        
                }
                catch (OperationCanceledException)
                {

                    token.ThrowIfCancellationRequested();
                }
                catch (Exception e)
                {
                    sbStatus.Append("Error thrown in Details:" + Environment.NewLine + e.ToString());
                    Status = sbStatus.ToString(); // use thi
                    return true;
                }

               // if (mhp_details == null)
                    //return;

                sbStatus.Append("-Exporting results to Excel" + Environment.NewLine);
                Status = sbStatus.ToString(); // use thi

                if (token.IsCancellationRequested)
                {
                    Status = "~~~Report Generation Cancelled~~~";
                    return true;
                }
                //await Task.Run(() => System.Threading.Thread.Sleep(1000));
                //bool blExcelError = false;
                try
                {

                    if (_reportType == "CS")
                    {
                        await Task.Run(async () => await ExportCSToExcel(mhpcs_results, mhpcs_details, token));
                    }
                    else if (_reportType == "IFP")
                    {
                        await Task.Run(async () => await ExportIFPToExcel(mhpifp_results, mhpifp_details, token));
                    }
                    else
                    {
                        await Task.Run(async () => await ExportToExcel(mhp_results, mhp_details, token));
                    }

                }
                catch (OperationCanceledException)
                {

                    token.ThrowIfCancellationRequested();
                }
                catch (Exception e)
                {
                    sbStatus.Append("Error thrown in Excel generation:" + Environment.NewLine + e.ToString());
                    Status = sbStatus.ToString(); // use thi
                    return true;
                    //blExcelError = true ;
                }

                //if (blExcelError)
                   // return;



                sbStatus.Append("Process completed!" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
                sbStatus.Append("Ready" + Environment.NewLine);
                Status = sbStatus.ToString(); // use thi
                                              //await Task.Run(() => System.Threading.Thread.Sleep(1000));
                return true;
            }
            finally
            {
                Application.Current.Dispatcher.Invoke((Action)(() => { Mouse.OverrideCursor = null; }));
                Disable = false;
                sbStatus.Remove(0, sbStatus.Length);
                // Application.Current.Dispatcher.Invoke((Action)(() => { Disable = false; }));

                CancelReportVisibility = Visibility.Hidden;
                GenerateReportVisibility = Visibility.Visible;
            }
            return true;

        }

        private async Task ExportIFPToExcel(List<MHPIFP_Yearly_Universes_Reporting_Model> mhp_results, List<MHPIFP_Yearly_Universes_Details_Model> mhp_details, CancellationToken token)
        {

            //throw new Exception("Oh nooooooo!!!");

            mhp_results.OrderBy(o => o.Product).OrderBy(o => o.ExcelRow).ToList();
            //mhp_details.OrderBy(o => o.LegalEntity).OrderBy(o => o.Request_Date).OrderBy(o => o.Authorization).ToList();
            mhp_details.OrderBy(o => o.PRDCT_CD).OrderBy(o => o.Request_Date).OrderBy(o => o.Authorization).ToList();

            string strFilePath = @"\\WP000003507\Home Directory - UCS Team Portal\Files\MHP_Reporting_Template.xlsx";

            int intNameCntTmp = 0;
            XLWorkbook wb = new XLWorkbook(strFilePath);
            IXLWorksheet wsSource = null;
            IXLRange range;
            int rowCnt = 0;
            foreach (MHPIFP_Yearly_Universes_Reporting_Model mhp in mhp_results)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }


                if (mhp.ExcelRow == 4)
                {
                    sbStatus.Append("-Creating sheet for " + mhp.Product + Environment.NewLine);
                    Status = sbStatus.ToString();

                    wsSource = wb.Worksheet("template");
                    // Copy the worksheet to a new sheet in this workbook
                    //wsSource.CopyTo("template COPY1").SetTabColor(XLColor.Orange);
                    var newSheetName = mhp.Product.Split('-')[0].Trim();
                    wsSource.CopyTo(newSheetName);
                    wsSource = wb.Worksheet(newSheetName);
                    wsSource.Cell("A1").Value = mhp.State + " " + mhp.Product + " : " + mhp.StartDate + "-" + mhp.EndDate;
                    wsSource.Cell("A1").Style.Font.Bold = true;
                    wsSource.Cell("A1").Style.Fill.BackgroundColor = XLColor.Yellow;
                    wsSource.Cell("A1").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                    intNameCntTmp++;
                }

                wsSource.Cell("B" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_in_ip + "") ? null : mhp.cnt_in_ip + "");
                wsSource.Cell("D" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_on_ip + "") ? null : mhp.cnt_on_ip + "");
                wsSource.Cell("F" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_in_op + "") ? null : mhp.cnt_in_op + "");
                wsSource.Cell("H" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_on_op + "") ? null : mhp.cnt_on_op + "");


            }



            wb.Worksheet("template").Delete();

            rowCnt = 2;
            string lastProd = null;

            foreach (MHPIFP_Yearly_Universes_Details_Model mhp in mhp_details)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                if (lastProd != mhp.PRDCT_CD)
                {
                    sbStatus.Append("-Creating details sheet for " + mhp.PRDCT_CD + " - " + mhp.PRDCT_CD_DESC + Environment.NewLine);
                    Status = sbStatus.ToString();


                    //NOT FIRST PASS SO RESIZE LAST NEW SHEET
                    //if (lastEntity != null)
                    //{

                    //range = wsSource.Range(wsSource.Cell(1, 1).Address, wsSource.Cell(1, typeof(MHP_Yearly_Universes_Details_Model).GetProperties().Length).Address);
                    //range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    //range.Style.Font.Bold = true;
                    //range.Style.Fill.BackgroundColor = XLColor.Yellow;
                    ////range.Style

                    //wsSource.Columns().AdjustToContents(1, typeof(MHP_Yearly_Universes_Details_Model).GetProperties().Length);   // Adjust column width
                    //wsSource.Rows().AdjustToContents(1, mhp_details.Count(n => n.LegalEntity == lastEntity));
                    //}


                    //var newSheetName = mhp.LegalEntity.Split('-')[0].Trim();
                    var newSheetName = mhp.PRDCT_CD;
                    wsSource = wb.Worksheets.Add(newSheetName + "_Details");
                    // Copy the worksheet to a new sheet in this workbook
                    //wsSource.CopyTo("template COPY1").SetTabColor(XLColor.Orange);

                    wsSource.Cell("A1").Value = nameof(mhp.Authorization);
                    wsSource.Cell("B1").Value = nameof(mhp.Request_Decision);
                    wsSource.Cell("C1").Value = nameof(mhp.Authorization_Type);
                    wsSource.Cell("D1").Value = nameof(mhp.Par_NonPar_Site);
                    wsSource.Cell("E1").Value = nameof(mhp.Inpatient_Outpatient);
                    wsSource.Cell("F1").Value = nameof(mhp.Request_Date);
                    wsSource.Cell("G1").Value = nameof(mhp.State_of_Issue);
                    wsSource.Cell("H1").Value = nameof(mhp.Decision_Reason);
                    wsSource.Cell("I1").Value = "Products";
                    wsSource.Cell("J1").Value = nameof(mhp.Enrollee_First_Name);
                    wsSource.Cell("K1").Value = nameof(mhp.Enrollee_Last_Name);
                    wsSource.Cell("L1").Value = nameof(mhp.Cardholder_ID);
                    wsSource.Cell("M1").Value = nameof(mhp.Member_Date_of_Birth);
                    wsSource.Cell("N1").Value = nameof(mhp.Procedure_Code_Description);
                    wsSource.Cell("O1").Value = nameof(mhp.Primary_Procedure_Code_Req);
                    wsSource.Cell("P1").Value = nameof(mhp.Primary_Diagnosis_Code);

                    //wsSource.Cell("S1").Value = nameof(mhp.Diagnosis_Code_Description);



                    range = wsSource.Range(wsSource.Cell(1, 1).Address, wsSource.Cell(1, typeof(MHPIFP_Yearly_Universes_Details_Model).GetProperties().Length).Address);
                    range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    range.Style.Font.Bold = true;
                    range.Style.Fill.BackgroundColor = XLColor.Yellow;
                    //range.Style
                    if (mhp.PRDCT_CD != lastProd)
                        wsSource.Columns().AdjustToContents(1, typeof(MHPIFP_Yearly_Universes_Details_Model).GetProperties().Length);   // Adjust column width


                    lastProd = mhp.PRDCT_CD;
                    rowCnt = 2;
                    intNameCntTmp++;
                }

                wsSource.Cell("A" + rowCnt).Value = mhp.Authorization;
                wsSource.Cell("B" + rowCnt).Value = mhp.Request_Decision;
                wsSource.Cell("C" + rowCnt).Value = mhp.Authorization_Type;
                wsSource.Cell("D" + rowCnt).Value = mhp.Par_NonPar_Site;
                wsSource.Cell("E" + rowCnt).Value = mhp.Inpatient_Outpatient;
                wsSource.Cell("F" + rowCnt).Value = mhp.Request_Date;
                wsSource.Cell("G" + rowCnt).Value = mhp.State_of_Issue;
                wsSource.Cell("H" + rowCnt).Value = mhp.Decision_Reason;
                wsSource.Cell("I" + rowCnt).Value = mhp.PRDCT_CD + " - " + mhp.PRDCT_CD_DESC;
                wsSource.Cell("J" + rowCnt).Value = mhp.Enrollee_First_Name;
                wsSource.Cell("K" + rowCnt).Value = mhp.Enrollee_Last_Name;
                wsSource.Cell("L" + rowCnt).Value = mhp.Cardholder_ID;
                wsSource.Cell("M" + rowCnt).Value = mhp.Member_Date_of_Birth;
                wsSource.Cell("N" + rowCnt).Value = mhp.Procedure_Code_Description;
                wsSource.Cell("O" + rowCnt).Value = mhp.Primary_Procedure_Code_Req;
                wsSource.Cell("P" + rowCnt).Value = mhp.Primary_Diagnosis_Code;
                //wsSource.Cell("S" + rowCnt).Value = mhp.Diagnosis_Code_Description;

                rowCnt++;
            }
            //LAST SHEET RESIZE
            //wsSource.Columns().AdjustToContents(1, typeof(MHP_Yearly_Universes_Details_Model).GetProperties().Length);
            //wsSource.Columns().AdjustToContents();
            //range = wsSource.Range(wsSource.Cell(1, 1).Address, wsSource.Cell(1, typeof(MHP_Yearly_Universes_Details_Model).GetProperties().Length).Address);
            //range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            //range.Style.Font.Bold = true;
            //range.Style.Fill.BackgroundColor = XLColor.Yellow;


            //wsSource.Columns().AdjustToContents(1, typeof(MHP_Yearly_Universes_Details_Model).GetProperties().Length);   // Adjust column width
            //wsSource.Rows().AdjustToContents(1, mhp_details.Count(n => n.LegalEntity == lastEntity));
            //wsSource.Column(13).CellsUsed().SetDataType(XLDataType.Text);
            //wsSource.Column(14).CellsUsed().SetDataType(XLDataType.Text);
            wsSource.Column(17).CellsUsed().SetDataType(XLDataType.Text);

            if (token.IsCancellationRequested)
            {
                Status = "~~~Report Generation Cancelled~~~";
                token.ThrowIfCancellationRequested();
            }


            strFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MHP_Report_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx";
            sbStatus.Append("-Saving Excel here: " + strFilePath + Environment.NewLine);
            Status = sbStatus.ToString();

            //CLEANUP
            if (File.Exists(strFilePath))
                File.Delete(strFilePath);

            wb.SaveAs(strFilePath);


            sbStatus.Append("-Opening Excel" + Environment.NewLine);
            Status = sbStatus.ToString();
            //DISPLAY
            System.Diagnostics.Process.Start(strFilePath);

            await Task.CompletedTask;
        }



        private async Task ExportCSToExcel(List<MHPCS_Yearly_Universes_Reporting_Model> mhp_results, List<MHPCS_Yearly_Universes_Details_Model> mhp_details, CancellationToken token)
        {

            //throw new Exception("Oh nooooooo!!!");

            mhp_results.OrderBy(o => o.ExcelRow).ToList();
            //mhp_details.OrderBy(o => o.LegalEntity).OrderBy(o => o.Request_Date).OrderBy(o => o.Authorization).ToList();
            mhp_details.OrderBy(o => o.Request_Date).OrderBy(o => o.Authorization).ToList();

            string strFilePath  = @"\\WP000003507\Home Directory - UCS Team Portal\Files\MHPCS_Reporting_Template.xlsx";
            

            int intNameCntTmp = 0;
            XLWorkbook wb = new XLWorkbook(strFilePath);
            IXLWorksheet wsSource = null;

            foreach (MHPCS_Yearly_Universes_Reporting_Model mhp in mhp_results)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }


                if (mhp.ExcelRow == 4)
                {
                    sbStatus.Append("-Creating summary sheet for CS" + Environment.NewLine);
                    Status = sbStatus.ToString();

                    wsSource = wb.Worksheet("template");
                    // Copy the worksheet to a new sheet in this workbook
                    //wsSource.CopyTo("template COPY1").SetTabColor(XLColor.Orange);
                    var newSheetName = "CS";
                    wsSource.CopyTo(newSheetName);
                    wsSource = wb.Worksheet(newSheetName);
                    wsSource.Cell("A1").Value = mhp.State + " (" + _strGlobalFilterList + ") : " + mhp.StartDate + "-" + mhp.EndDate;
                    intNameCntTmp++;
                }

                wsSource.Cell("B" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_ip + "") ? null : mhp.cnt_ip + "");
                wsSource.Cell("E" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_op + "") ? null : mhp.cnt_op + "");

            }


            wb.Worksheet("template").Delete();

            bool blHead = true;
            int rowCnt = 2;
            IXLRange range;
            foreach (MHPCS_Yearly_Universes_Details_Model mhp in mhp_details)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                if (blHead)
                {
                    sbStatus.Append("-Creating detail sheet for CS " + Environment.NewLine);
                    Status = sbStatus.ToString();

                    range = wsSource.Range(wsSource.Cell(1, 1).Address, wsSource.Cell(1, typeof(MHP_Yearly_Universes_Details_Model).GetProperties().Length).Address);
                    range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    range.Style.Font.Bold = true;
                    range.Style.Fill.BackgroundColor = XLColor.Yellow;
                    //range.Style

                    wsSource.Columns().AdjustToContents(1, typeof(MHP_Yearly_Universes_Details_Model).GetProperties().Length);   // Adjust column width
                    //wsSource.Rows().AdjustToContents(1, mhp_details.Count(n => n.LegalEntity == lastEntity));
            


                    //var newSheetName = mhp.LegalEntity.Split('-')[0].Trim();
                    var newSheetName = "CS";
                    wsSource = wb.Worksheets.Add(newSheetName + "_Details");
                    // Copy the worksheet to a new sheet in this workbook
                    //wsSource.CopyTo("template COPY1").SetTabColor(XLColor.Orange);

                    wsSource.Cell("A1").Value = nameof(mhp.Authorization);
                    wsSource.Cell("B1").Value = nameof(mhp.Request_Decision);
                    wsSource.Cell("C1").Value = nameof(mhp.Authorization_Type);
                    wsSource.Cell("D1").Value = nameof(mhp.Par_NonPar_Site);
                    wsSource.Cell("E1").Value = nameof(mhp.Inpatient_Outpatient);
                    wsSource.Cell("F1").Value = nameof(mhp.Request_Date);
                    wsSource.Cell("G1").Value = nameof(mhp.State_of_Issue);
                    wsSource.Cell("H1").Value = nameof(mhp.Decision_Reason);
                    wsSource.Cell("I1").Value = nameof(mhp.CS_TADM_PRDCT_MAP);
                    wsSource.Cell("J1").Value = nameof(mhp.Enrollee_First_Name);
                    wsSource.Cell("K1").Value = nameof(mhp.Enrollee_Last_Name);
                    wsSource.Cell("L1").Value = nameof(mhp.Cardholder_ID);
                    wsSource.Cell("M1").Value = nameof(mhp.Member_Date_of_Birth);
                    wsSource.Cell("N1").Value = nameof(mhp.Procedure_Code_Description);
                    wsSource.Cell("O1").Value = nameof(mhp.Primary_Procedure_Code_Req);
                    wsSource.Cell("P1").Value = nameof(mhp.Primary_Diagnosis_Code);
                    wsSource.Cell("Q1").Value = nameof(mhp.Group_Number);
                    wsSource.Cell("R1").Value = nameof(mhp.PRDCT_CD_DESC);
                    //wsSource.Cell("S1").Value = nameof(mhp.Diagnosis_Code_Description);

                    rowCnt = 2;
                    intNameCntTmp++;
                    blHead = false;
                }

                wsSource.Cell("A" + rowCnt).Value = mhp.Authorization;
                wsSource.Cell("B" + rowCnt).Value = mhp.Request_Decision;
                wsSource.Cell("C" + rowCnt).Value = mhp.Authorization_Type;
                wsSource.Cell("D" + rowCnt).Value = mhp.Par_NonPar_Site;
                wsSource.Cell("E" + rowCnt).Value = mhp.Inpatient_Outpatient;
                wsSource.Cell("F" + rowCnt).Value = mhp.Request_Date;
                wsSource.Cell("G" + rowCnt).Value = mhp.State_of_Issue;
                wsSource.Cell("H" + rowCnt).Value = mhp.Decision_Reason;
                wsSource.Cell("I" + rowCnt).Value = mhp.CS_TADM_PRDCT_MAP;
                wsSource.Cell("J" + rowCnt).Value = mhp.Enrollee_First_Name;
                wsSource.Cell("K" + rowCnt).Value = mhp.Enrollee_Last_Name;
                wsSource.Cell("L" + rowCnt).Value = mhp.Cardholder_ID;
                wsSource.Cell("M" + rowCnt).Value = mhp.Member_Date_of_Birth;
                wsSource.Cell("N" + rowCnt).Value = mhp.Procedure_Code_Description;
                wsSource.Cell("O" + rowCnt).Value = mhp.Primary_Procedure_Code_Req;
                wsSource.Cell("P" + rowCnt).Value = mhp.Primary_Diagnosis_Code;
                wsSource.Cell("Q" + rowCnt).Value = mhp.Group_Number;
                wsSource.Cell("R" + rowCnt).Value = mhp.PRDCT_CD_DESC;
                //wsSource.Cell("S" + rowCnt).Value = mhp.Diagnosis_Code_Description;



                rowCnt++;
            }
            //LAST SHEET RESIZE
            //wsSource.Columns().AdjustToContents();
            range = wsSource.Range(wsSource.Cell(1, 1).Address, wsSource.Cell(1, typeof(MHPCS_Yearly_Universes_Details_Model).GetProperties().Length).Address);
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            range.Style.Font.Bold = true;
            range.Style.Fill.BackgroundColor = XLColor.Yellow;

            //wsSource.Column(13).CellsUsed().SetDataType(XLDataType.Text);
            //wsSource.Column(14).CellsUsed().SetDataType(XLDataType.Text);
            //PRIMARY DAIG COL P = 15?
            wsSource.Column(17).CellsUsed().SetDataType(XLDataType.Text);




            wsSource.Columns().AdjustToContents(1, typeof(MHPCS_Yearly_Universes_Details_Model).GetProperties().Length);   // Adjust column width
                                                                                                                         //wsSource.Rows().AdjustToContents(1, mhp_details.Count(n => n.LegalEntity == lastEntity));

            if (token.IsCancellationRequested)
            {
                Status = "~~~Report Generation Cancelled~~~";
                token.ThrowIfCancellationRequested();
            }


            strFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MHP_CS_Report_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx";
            sbStatus.Append("-Saving Excel here: " + strFilePath + Environment.NewLine);
            Status = sbStatus.ToString();

            //CLEANUP
            if (File.Exists(strFilePath))
                File.Delete(strFilePath);

            wb.SaveAs(strFilePath);


            sbStatus.Append("-Opening Excel" + Environment.NewLine);
            Status = sbStatus.ToString();
            //DISPLAY
            System.Diagnostics.Process.Start(strFilePath);

            await Task.CompletedTask;
        }

        private async Task ExportToExcel(List<MHP_Yearly_Universes_Reporting_Model> mhp_results, List<MHP_Yearly_Universes_Details_Model> mhp_details, CancellationToken token)
        {

            //throw new Exception("Oh nooooooo!!!");

            mhp_results.OrderBy(o => o.LegalEntity).OrderBy(o => o.ExcelRow).ToList();
            //mhp_details.OrderBy(o => o.LegalEntity).OrderBy(o => o.Request_Date).OrderBy(o => o.Authorization).ToList();
            mhp_details.OrderBy(o => o.LEG_ENTY_NBR).OrderBy(o => o.Request_Date).OrderBy(o => o.Authorization).ToList();

            string strFilePath = @"\\WP000003507\Home Directory - UCS Team Portal\Files\MHP_Reporting_Template.xlsx";

            int intNameCntTmp = 0;
            XLWorkbook wb = new XLWorkbook(strFilePath);
            IXLWorksheet wsSource = null;
            IXLRange range;
            int rowCnt = 0;
            foreach (MHP_Yearly_Universes_Reporting_Model mhp in mhp_results)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }


                if (mhp.ExcelRow == 4)
                {
                    sbStatus.Append("-Creating sheet for " + mhp.LegalEntity + Environment.NewLine);
                    Status = sbStatus.ToString();

                    wsSource = wb.Worksheet("template");
                    // Copy the worksheet to a new sheet in this workbook
                    //wsSource.CopyTo("template COPY1").SetTabColor(XLColor.Orange);
                    var newSheetName = mhp.LegalEntity.Split('-')[0].Trim();
                    wsSource.CopyTo(newSheetName);
                    wsSource = wb.Worksheet(newSheetName);
                    wsSource.Cell("A1").Value = mhp.State + " " + mhp.LegalEntity + " : " + mhp.StartDate + "-" + mhp.EndDate;
                    wsSource.Cell("A1").Style.Font.Bold = true;
                    wsSource.Cell("A1").Style.Fill.BackgroundColor = XLColor.Yellow;
                    wsSource.Cell("A1").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                    intNameCntTmp++;
                }

                wsSource.Cell("B" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_in_ip + "") ? null : mhp.cnt_in_ip + "");
                wsSource.Cell("D" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_on_ip + "") ? null : mhp.cnt_on_ip + "");
                wsSource.Cell("F" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_in_op + "") ? null : mhp.cnt_in_op + "");
                wsSource.Cell("H" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_on_op + "") ? null : mhp.cnt_on_op + "");


            }


      
            wb.Worksheet("template").Delete();

            rowCnt = 2;
            string lastEntity = null;

            foreach (MHP_Yearly_Universes_Details_Model mhp in mhp_details)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                if (lastEntity != mhp.LEG_ENTY_NBR)
                {
                    sbStatus.Append("-Creating details sheet for " + mhp.LEG_ENTY_NBR + " - " + mhp.LEG_ENTY_FULL_NM + Environment.NewLine);
                    Status = sbStatus.ToString();


                    //NOT FIRST PASS SO RESIZE LAST NEW SHEET
                    //if (lastEntity != null)
                    //{

                        //range = wsSource.Range(wsSource.Cell(1, 1).Address, wsSource.Cell(1, typeof(MHP_Yearly_Universes_Details_Model).GetProperties().Length).Address);
                        //range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                        //range.Style.Font.Bold = true;
                        //range.Style.Fill.BackgroundColor = XLColor.Yellow;
                        ////range.Style

                        //wsSource.Columns().AdjustToContents(1, typeof(MHP_Yearly_Universes_Details_Model).GetProperties().Length);   // Adjust column width
                        //wsSource.Rows().AdjustToContents(1, mhp_details.Count(n => n.LegalEntity == lastEntity));
                    //}


                    //var newSheetName = mhp.LegalEntity.Split('-')[0].Trim();
                    var newSheetName = mhp.LEG_ENTY_NBR;
                    wsSource = wb.Worksheets.Add(newSheetName + "_Details");
                    // Copy the worksheet to a new sheet in this workbook
                    //wsSource.CopyTo("template COPY1").SetTabColor(XLColor.Orange);

                    wsSource.Cell("A1").Value = nameof(mhp.Authorization);
                    wsSource.Cell("B1").Value = nameof(mhp.Request_Decision);
                    wsSource.Cell("C1").Value = nameof(mhp.Authorization_Type);
                    wsSource.Cell("D1").Value =  nameof(mhp.Par_NonPar_Site);
                    wsSource.Cell("E1").Value = nameof(mhp.Inpatient_Outpatient);
                    wsSource.Cell("F1").Value = nameof(mhp.Request_Date);
                    wsSource.Cell("G1").Value = nameof(mhp.State_of_Issue);
                    wsSource.Cell("H1").Value = nameof(mhp.FINC_ARNG_DESC);
                    wsSource.Cell("I1").Value = nameof(mhp.Decision_Reason);
                    wsSource.Cell("J1").Value = nameof(mhp.MKT_SEG_RLLP_DESC);
                    wsSource.Cell("K1").Value = nameof(mhp.MKT_TYP_DESC);
                    //wsSource.Cell("L1").Value = nameof(mhp.LEG_ENTY_FULL_NM);
                    wsSource.Cell("L1").Value = "LegalEntity";
                    wsSource.Cell("M1").Value = nameof(mhp.Enrollee_First_Name);
                    wsSource.Cell("N1").Value = nameof(mhp.Enrollee_Last_Name);
                    wsSource.Cell("O1").Value = nameof(mhp.Cardholder_ID);
                    wsSource.Cell("P1").Value = nameof(mhp.Member_Date_of_Birth);
                    wsSource.Cell("Q1").Value = nameof(mhp.Procedure_Code_Description);
                    wsSource.Cell("R1").Value = nameof(mhp.Primary_Procedure_Code_Req);
                    wsSource.Cell("S1").Value = nameof(mhp.Primary_Diagnosis_Code);
                    wsSource.Cell("T1").Value = nameof(mhp.CUST_SEG_NBR);
                    wsSource.Cell("U1").Value = nameof(mhp.CUST_SEG_NM);
                    //wsSource.Cell("S1").Value = nameof(mhp.Diagnosis_Code_Description);



                    range = wsSource.Range(wsSource.Cell(1, 1).Address, wsSource.Cell(1, typeof(MHP_Yearly_Universes_Details_Model).GetProperties().Length).Address);
                    range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    range.Style.Font.Bold = true;
                    range.Style.Fill.BackgroundColor = XLColor.Yellow;
                    //range.Style
                    if(mhp.LEG_ENTY_NBR != lastEntity )
                        wsSource.Columns().AdjustToContents(1, typeof(MHP_Yearly_Universes_Details_Model).GetProperties().Length);   // Adjust column width


                    lastEntity = mhp.LEG_ENTY_NBR;
                    rowCnt = 2;
                    intNameCntTmp++;
                }

                wsSource.Cell("A" + rowCnt).Value = mhp.Authorization;
                wsSource.Cell("B" + rowCnt).Value = mhp.Request_Decision;
                wsSource.Cell("C" + rowCnt).Value = mhp.Authorization_Type;
                wsSource.Cell("D" + rowCnt).Value =mhp.Par_NonPar_Site;
                wsSource.Cell("E" + rowCnt).Value = mhp.Inpatient_Outpatient;
                wsSource.Cell("F" + rowCnt).Value = mhp.Request_Date;
                wsSource.Cell("G" + rowCnt).Value = mhp.State_of_Issue;
                wsSource.Cell("H" + rowCnt).Value = mhp.FINC_ARNG_DESC;
                wsSource.Cell("I" + rowCnt).Value =mhp.Decision_Reason;
                wsSource.Cell("J" + rowCnt).Value = mhp.MKT_SEG_RLLP_DESC;
                wsSource.Cell("K" + rowCnt).Value = mhp.MKT_TYP_DESC;
                wsSource.Cell("L" + rowCnt).Value = mhp.LEG_ENTY_NBR + " - " + mhp.LEG_ENTY_FULL_NM;
                wsSource.Cell("M" + rowCnt).Value = mhp.Enrollee_First_Name;
                wsSource.Cell("N" + rowCnt).Value = mhp.Enrollee_Last_Name;
                wsSource.Cell("O" + rowCnt).Value = mhp.Cardholder_ID;
                wsSource.Cell("P" + rowCnt).Value = mhp.Member_Date_of_Birth;
                wsSource.Cell("Q" + rowCnt).Value = mhp.Procedure_Code_Description;
                wsSource.Cell("R" + rowCnt).Value = mhp.Primary_Procedure_Code_Req;
                wsSource.Cell("S" + rowCnt).Value = mhp.Primary_Diagnosis_Code;
                wsSource.Cell("T" + rowCnt).Value = mhp.CUST_SEG_NBR;
                wsSource.Cell("U" + rowCnt).Value = mhp.CUST_SEG_NM;
                //wsSource.Cell("S" + rowCnt).Value = mhp.Diagnosis_Code_Description;

                rowCnt++;
            }
            //LAST SHEET RESIZE
            //wsSource.Columns().AdjustToContents(1, typeof(MHP_Yearly_Universes_Details_Model).GetProperties().Length);
            //wsSource.Columns().AdjustToContents();
            //range = wsSource.Range(wsSource.Cell(1, 1).Address, wsSource.Cell(1, typeof(MHP_Yearly_Universes_Details_Model).GetProperties().Length).Address);
            //range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            //range.Style.Font.Bold = true;
            //range.Style.Fill.BackgroundColor = XLColor.Yellow;


            //wsSource.Columns().AdjustToContents(1, typeof(MHP_Yearly_Universes_Details_Model).GetProperties().Length);   // Adjust column width
            //wsSource.Rows().AdjustToContents(1, mhp_details.Count(n => n.LegalEntity == lastEntity));
            //wsSource.Column(17).CellsUsed().SetDataType(XLDataType.Text);
            //wsSource.Column(18).CellsUsed().SetDataType(XLDataType.Text);
            wsSource.Column(18).CellsUsed().SetDataType(XLDataType.Text);
            wsSource.Column(19).CellsUsed().SetDataType(XLDataType.Text);
            wsSource.Column(20).CellsUsed().SetDataType(XLDataType.Text);
            wsSource.Column(21).CellsUsed().SetDataType(XLDataType.Text);
            wsSource.Column(22).CellsUsed().SetDataType(XLDataType.Text);

            if (token.IsCancellationRequested)
            {
                Status = "~~~Report Generation Cancelled~~~";
                token.ThrowIfCancellationRequested();
            }


            strFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MHP_Report_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx";
            sbStatus.Append("-Saving Excel here: " + strFilePath + Environment.NewLine);
            Status = sbStatus.ToString();

            //CLEANUP
            if (File.Exists(strFilePath))
                File.Delete(strFilePath);

            wb.SaveAs(strFilePath);


            sbStatus.Append("-Opening Excel" + Environment.NewLine);
            Status = sbStatus.ToString(); 
            //DISPLAY
            System.Diagnostics.Process.Start(strFilePath);

            await Task.CompletedTask;
        }


        private List<string> _lstStates;
        public List<string> States
        {
            get { return _lstStates; }
            set { }
        }

        private List<string> _lstMKT_SEG_RLLP_DESC;
        public List<string> MKT_SEG_RLLP_DESC
        {
            get { return _lstMKT_SEG_RLLP_DESC; }
            set { }
        }


        private List<string> _lstFINC_ARNG_DESC;
        public List<string> FINC_ARNG_DESC
        {
            get { return _lstFINC_ARNG_DESC; }
            set { }
        }


        private List<string> _lstLEG_ENTY;
        public List<string> LEG_ENTY
        {
            get { return _lstLEG_ENTY; }
            set { }
        }


        private List<string> _lstCS_TADM_PRDCT_MAP;
        public List<string> CS_TADM_PRDCT_MAP
        {
            get { return _lstCS_TADM_PRDCT_MAP; }
            set { }
        }



        private List<string> _lstMKT_TYP_DESC;
        public List<string> MKT_TYP_DESC
        {
            get { return _lstMKT_TYP_DESC; }
            set { }
        }


        private List<string> _lstCUST_SEG;
        public List<string> CUST_SEG
        {
            get { return _lstCUST_SEG; }
            set { }
        }


        private List<Group_State_Model> _lstGroupStateAll;

        private ObservableCollection<string> _lstGroupNumbers;
        public ObservableCollection<string> GroupNumbers
        {
            get { return _lstGroupNumbers; }
            set { }
        }

        private List<string> _productCode;
        public List<string> ProductCode
        {
            get { return _productCode; }
            set { }
        }

        public void LoadSupportLists()
        {


            //_lstStates = _repo.GetStates(isCS: false);
            //_lstStates.Insert(0, "--Select a State--");
            _lstStates = _repo.GetStates(isCS: false);
            _lstStates.Insert(0, "--All--");
            _lstMKT_SEG_RLLP_DESC = _repo.GetMKT_SEG_RLLP_DESC(isCS: false) ;
            _lstMKT_SEG_RLLP_DESC.Insert(0, "--All--");
            _lstFINC_ARNG_DESC = _repo.GetFINC_ARNG_DESC(isCS: false);
            _lstFINC_ARNG_DESC.Insert(0, "--All--");
            _lstLEG_ENTY = _repo.GetLEG_ENTY(isCS: false);
            _lstLEG_ENTY.Insert(0, "--All--");


            _lstMKT_TYP_DESC = _repo.GetMKT_TYP_DESC(isCS: false);
            _lstMKT_TYP_DESC.Insert(0, "--All--");


            _lstCS_TADM_PRDCT_MAP = _repo.GetCS_TADM_PRDCT_MAP(isCS: true);
            _lstCS_TADM_PRDCT_MAP.Insert(0, "--All--");


            _lstGroupStateAll = _repo.GetGroupState();




            _productCode = _repo.GetProductCode();
            _productCode.Insert(0, "--All--");
            //TreatmentIndicatorECOptionsArr = new ObservableCollection<string>(eim.treatmentIndicatorECOptionsArr as List<string>);
            //MappingOptionsArr = new ObservableCollection<string>(eim.mappingOptionsArr as List<string>);
            //PatientCentricMappingOptionsArr = new ObservableCollection<string>(eim.patientCentricMappingOptionsArr as List<string>);


            ////DATAGRID FILTERS
            //_arrETG_Symmetry_Verion = new ObservableCollection<ETG_Symmetry_Verion>(_repo.GetSymmetryVersion() as List<ETG_Symmetry_Verion>);
            //this.ETG_Symmetry_VerionArr = _arrETG_Symmetry_Verion;

            _lstGroupNumbers = new ObservableCollection<string>(_lstGroupStateAll.GroupBy(s => s.Group_Number).Select(g => g.First()).OrderBy(s => s.Group_Number).Select(g => g.Group_Number).ToList() as List<string>);
            _lstGroupNumbers.Insert(0, "--All--");
            //_lstCUST_SEG = _repo.GetCUST_SEG(isCS: true);
            //_lstCUST_SEG.Insert(0, "--All--");

        }





        //private property that stores value
        private bool _blDisable = false;

        //public property the gets & sets value
        public bool Disable
        {
            get { return _blDisable; }
            set
            {
                if (_blDisable != value)
                {
                    _blDisable = value;
                    NotifyPropertyChanged("Disable");
                }
            }
        }


        //private property that stores value
        private string _status = "Ready";

        //public property the gets & sets value
        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    NotifyPropertyChanged("Status");
                }
            }
        }

        //Logic to notify that property values have changed.
        new public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }


    }
}
