using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using UCS_Project_Manager_Models;
using UCS_Project_Manager_Services;

namespace UCS_Project_Manager
{
    public class ETG_Fact_Symmetry_ViewModel : ViewModelBase, INotifyPropertyChanged
    {


        //GLOBAL VARIABLES
        public RelayCommand SortCommand { get; set; }

        public RelayCommand SearchCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand NextCommand { get; set; }
        public RelayCommand PreviousCommand { get; set; }
        public RelayCommand ExportCommand { get; set; }

        public event Action Done = delegate { };

        private IETG_Fact_Symmetry_Repository _repo;

        //CONSTRUCTOR
        //CONSTRUCTOR
        //CONSTRUCTOR
        public ETG_Fact_Symmetry_ViewModel(IETG_Fact_Symmetry_Repository repo)
        {
            //REFGISTER MODEL FOR UPDATES
            AppMessages.ProjectChangeTracking.Register(this, new Action<ETG_Fact_Symmetry_Update_Tracker>((arg) => { this.RaisePropertyChanged(arg); }));
            //AppMessages.ProjectChangeTracking.Register(this, new Action<bool>((arg) => { this.RaisePropertyChanged(arg); }));



            _repo = repo;

            //LOAD SUPPORTING ARRAYS
            LoadSupportLists();

            //GET DATA FROM DB REPO
            this.ETG_Fact_SymmetryArr = new ObservableCollection<ETG_Fact_Symmetry_Interface_Model>(_repo.GetETGFactSymmetrySQL(_strCurrentSymmetry_Version, _strPreviousSymmetry_Version) as List<ETG_Fact_Symmetry_Interface_Model>);
            this.ETG_Fact_Symmetry_Update_TrackerArr = _repo.GetETGFactSymmetryUpdatesSQL(_strCurrentSymmetry_Version, _strPreviousSymmetry_Version);

            //ADD DATA TO MIDDLE LAYER
            _etg_Fact_SymmetryCollectionViewSource = new CollectionViewSource();
            _etg_Fact_SymmetryCollectionViewSource.Source = ETG_Fact_SymmetryArr;

            //CollectionChanged USED TO CAPTURE INSERTS OR DELETES
            //CollectionChanged USED TO CAPTURE INSERTS OR DELETES
            //CollectionChanged USED TO CAPTURE INSERTS OR DELETES
            //this.ETG_Fact_SymmetryArr.CollectionChanged += items_CollectionChanged;
            //_etg_Fact_SymmetryCollectionViewSource.View.CollectionChanged += items_CollectionChanged;


            //LINK COMMANDS TO FUNCTIONS
            SearchCommand = new RelayCommand(Search);
            NextCommand = new RelayCommand(ETG_Fact_SymmetryListCollectionView.MoveToNextPage);
            PreviousCommand = new RelayCommand(ETG_Fact_SymmetryListCollectionView.MoveToPreviousPage);
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            CancelCommand = new RelayCommand(OnCancel);
            SaveCommand = new RelayCommand(OnSave, CanSave);
            ExportCommand = new RelayCommand(ExportToExcel);
            SortCommand = new RelayCommand(Sort);

            this.NeedsUpdate = false;



            //EXPORT EXCEL COMMAND REGISTER
            //ExcelHelper.ExportDataSet(this.dgETGDataGrid.Columns.Select(cs => cs.Header).ToList(), "C:\\test");
        }


        #region DATA TRACKING
        //RaisePropertyChanged USED TO CAPTURE UPDATES
        //RaisePropertyChanged USED TO CAPTURE UPDATES
        //RaisePropertyChanged USED TO CAPTURE UPDATES
        private List<ETG_Fact_Symmetry_Update_Tracker> _lstUpdates;
        new public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(ETG_Fact_Symmetry_Update_Tracker caller = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("caller"));
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller.PropertyName.ToString()));

                if (_lstUpdates == null)
                    _lstUpdates = new List<ETG_Fact_Symmetry_Update_Tracker>();

                //CHECK PK FOR DUPES AKA CHANGED MIND
                //USER CHANGED THEIR MIND (BEFORE SUBMITTING) SO REMOVE OLD CHANGE 
                if (_lstUpdates.Any(up => up.ETG_Fact_Symmetry_id == caller.ETG_Fact_Symmetry_id))
                {
                    _lstUpdates.RemoveAll(up => up.ETG_Fact_Symmetry_id == caller.ETG_Fact_Symmetry_id);
                }
                _lstUpdates.Add(caller);
                this.NeedsUpdate = true;
            }

        }
        private bool _needsUpdate = false;
        public bool NeedsUpdate
        {
            get { return _needsUpdate; }
            set
            {
                this._needsUpdate = value;
                OnPropertyChanged("NeedsUpdate");//ENABLE/DISABLE SUBMIT BUTTON
            }
        }

        public void OnPropertyChanged([CallerMemberName]string property = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        //CollectionChanged USED TO CAPTURE INSERTS OR DELETES
        //CollectionChanged USED TO CAPTURE INSERTS OR DELETES
        //CollectionChanged USED TO CAPTURE INSERTS OR DELETES
        //static void items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.OldItems != null)
        //    {
        //        foreach (INotifyPropertyChanged item in e.OldItems)
        //            item.PropertyChanged -= item_PropertyChanged;
        //    }
        //    if (e.NewItems != null)
        //    {
        //        foreach (INotifyPropertyChanged item in e.NewItems)
        //            item.PropertyChanged += item_PropertyChanged;
        //    }
        //}

        //static void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion

        #region MAIN DATA COLLECTION
        private ObservableCollection<ETG_Fact_Symmetry_Interface_Model> _arrETG_Fact_Symmetry;
        public ObservableCollection<ETG_Fact_Symmetry_Interface_Model> ETG_Fact_SymmetryArr
        {
            get { return _arrETG_Fact_Symmetry; }
            set
            {
                _arrETG_Fact_Symmetry = value;

            }
        }



        private List<ETG_Fact_Symmetry_Update_Tracker> _arrETG_Fact_Symmetry_Update_Tracker;
        public List<ETG_Fact_Symmetry_Update_Tracker> ETG_Fact_Symmetry_Update_TrackerArr
        {
            get { return _arrETG_Fact_Symmetry_Update_Tracker; }
            set
            {
                _arrETG_Fact_Symmetry_Update_Tracker = value;

            }
        }




        private readonly CollectionViewSource _etg_Fact_SymmetryCollectionViewSource;

        public PagingListCollectionView _etg_Fact_SymmetryListCollectionView;
        private const int PageSize = 1000000;
        public PagingListCollectionView ETG_Fact_SymmetryListCollectionView
        {
            get
            {

                if (_etg_Fact_SymmetryListCollectionView == null)
                {
                    _etg_Fact_SymmetryListCollectionView = new PagingListCollectionView(_etg_Fact_SymmetryCollectionViewSource.View.Cast<ETG_Fact_Symmetry_Interface_Model>().ToList(), PageSize);
                }


                return _etg_Fact_SymmetryListCollectionView;

            }
        }


        #endregion

        #region  COMMAND SECTION
        private void Search(object item)
        {

            object[] parameters = item as object[];
            string strPrimarySpecialty = null;
            string strETGBase = null;
            string strSymmetryVersion = null;

            prepParametersForSQLSearch(parameters, strPrimarySpecialty, strETGBase, strSymmetryVersion);
            _etg_Fact_SymmetryListCollectionView.CurrentPage = 1; //RESET
            _etg_Fact_SymmetryListCollectionView.Filter = new Predicate<object>(o => FilterSearch(o as ETG_Fact_Symmetry_Interface_Model)); //FILTER
            _etg_Fact_SymmetryListCollectionView.PageCount = _etg_Fact_SymmetryListCollectionView.PageCount;//USED TO REFRESH WPF Binding :(

            //ETG_Fact_SymmetryListCollectionView.Refresh();
        }


        private void Version(object item)
        {

            object[] parameters = item as object[];

            //ETG_Fact_SymmetryListCollectionView.Refresh();
            //GET DATA FROM DB REPO
            this.ETG_Fact_SymmetryArr = new ObservableCollection<ETG_Fact_Symmetry_Interface_Model>(_repo.GetETGFactSymmetrySQL("12", "14") as List<ETG_Fact_Symmetry_Interface_Model>);
            this.ETG_Fact_Symmetry_Update_TrackerArr = _repo.GetETGFactSymmetryUpdatesSQL(_strCurrentSymmetry_Version, _strPreviousSymmetry_Version);

            //ADD DATA TO MIDDLE LAYER
            _etg_Fact_SymmetryCollectionViewSource.Source = ETG_Fact_SymmetryArr;

            ETG_Fact_SymmetryListCollectionView.Refresh();
        }



        //SAVE COMMAND
        private bool CanSave()
        {
            bool isValid = true;
            foreach (var e in this._arrETG_Fact_Symmetry)
            {
                if (!e.IsValid)
                {
                    isValid = false;
                    break;
                }
            }
            return isValid;
        }
        private async void OnSave()
        {

            //_lstUpdates loop UPDATES TO REPO
            //_repo.UpdateETGFactSymmetrySQLAsync(_lstUpdates, _strUserName);
            await _repo.UpdateETGFactSymmetrySQLAsync(_lstUpdates, _strUserName);
            this.NeedsUpdate = false;

            this.ETG_Fact_Symmetry_Update_TrackerArr = _repo.GetETGFactSymmetryUpdatesSQL(_strCurrentSymmetry_Version, _strPreviousSymmetry_Version);
            Done();
        }

        //DELETE COMMAND
        private void OnDelete()
        {
            ETG_Fact_SymmetryArr.Remove(ETG_Fact_Symmetry);
        }

        private bool CanDelete()
        {
            return ETG_Fact_Symmetry != null;
        }

        private void OnCancel()
        {
            Done();
        }

        private void ExportToExcel()
        {
            string strFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\tmp.xlsx";
            //strFilePath = "C:\\tmp.xlsx";
            //CLEANUP
            if (File.Exists(strFilePath))
                File.Delete(strFilePath);

            //GENERATE
            //if (_strUserName == "cgiorda" || _strUserName == "msing88" || _strUserName == "dmart58" || _strUserName == "rsequeir")
            //{
            List<ETG_Fact_Symmetry_Export_Model> lst1 = getCleanListForExport(ETG_Fact_SymmetryArr.ToList());
            List<ETG_Fact_Symmetry_PateintCentric> lst2 = _repo.GetETGFactSymmetryPATIENT_CENTRIC_CONFIGSQL();
            List<ETG_Fact_Symmetry_Config_Model> lst3 = _repo.GetETGFactSymmetryPOP_EPISODE_CONFIGSQL();

            List<ETG_Fact_Symmetry_Export_Model2> lst4 = getCleanListForExport2(ETG_Fact_SymmetryArr.Where(x => x.Current_Mapping == "Mapped").ToList());


            List<ETG_Fact_Symmetry_RxNrxConfig_Model> lst5 = _repo.GetETGFactSymmetryRX_NRX_CONFIGSQL();
            //List<ETG_Fact_Symmetry_Export_Model2> lst4 = getCleanListForExport2(ETG_Fact_SymmetryArr.ToList());

            ExcelHelper.ExportMultipleGenericListOpenXML(lst1, lst2, lst3, lst4, lst5, strFilePath);
            //}
            //else
            //{
            //    List<ETG_Fact_Symmetry_Export_Model> lst1 = getCleanListForExport(ETG_Fact_SymmetryArr.ToList());
            //    List<ETG_Fact_Symmetry_Export_Model2> lst2 = getCleanListForExport2(ETG_Fact_SymmetryArr.Where(x => x.Current_Mapping == "Mapped").ToList());


            //    ExcelHelper.ExportMultipleGenericListOpenXML(lst1, lst2, strFilePath);
            //}




            ////ExcelHelper.ExportGenericListClosedXML(ETG_Fact_SymmetryArr.ToList(), strFilePath);
            //var lst = getCleanListForExport(ETG_Fact_SymmetryArr.ToList());
            //ExcelHelper.ExportGenericListOpenXML(lst, strFilePath);

            //var lst2 = _repo.GetETGFactSymmetryPATIENT_CENTRIC_CONFIGSQL();

            //var lst3 = _repo.GetETGFactSymmetryPOP_EPISODE_CONFIGSQL();

            //DISPLAY
            System.Diagnostics.Process.Start(strFilePath);
        }


        private List<ETG_Fact_Symmetry_Export_Model> getCleanListForExport(List<ETG_Fact_Symmetry_Interface_Model> esmList)
        {
            var targetList = esmList.Select(x => new ETG_Fact_Symmetry_Export_Model()
            {
                ETG_Base_Class = x.ETG_Base_Class,
                ETG_Description = x.ETG_Description,
                Premium_Specialty = x.Premium_Specialty,
                Previous_Rx_NRx = x.has_nrx,
                Current_Rx_NRx = x.has_rx,
                Previous_LOB = x.LOBPreviousString,
                Current_LOB = x.LOBCurrentString,

                PC_Current_Treatment_Indicator = x.Pop_Cost_Current_Treatment_Indicator,
                PC_Previous_Treatment_Indicator = x.Pop_Cost_Previous_Treatment_Indicator,

                PC_Episode_Cnt = x.Pop_Cost_Episode_Count_Commercial_Only,
                PC_Tot_Cost = x.Total_Cost_Commercial_Only,

                PC_Avg_Cost = x.Average_Cost_Commercial_Only,
                PC_CV = x.Coefficients_of_Variation_Commercial_Only,

                PC_Spec_Episode_Cnt = x.Specialist_Episode_Count,


                PC_Spec_Episode_Distribution = x.Pop_Cost_Episode_Distribution,
                //UNCOMMENTED 10/3/2022
                PC_Spec_Perc_of_Episodes = x.Percent_of_Episodes,




                PC_Spec_Tot_Cost = x.Specialist_Total_Cost,
                PC_Spec_Avg_Cost = x.Specialist_Average_Cost,
                PC_Spec_CV = x.Specialist_CV,
                PC_Prev_Attribution = x.Previous_Attribution,
                PC_Current_Attribution = x.Current_Attribution,



                PC_Change_Comments = x.Pop_Cost_Change_Comments,

                EC_Current_Treatment_Indicator = x.Current_Episode_Cost_Treatment_Indicator,
                EC_Previous_Treatment_Indicator = x.Previous_Episode_Cost_Treatment_Indicator,
                //DAVE 82022
                //EC_Episode_Count = x.Episode_Count,
                EC_Episode_Count = x.Episode_Cost_Episode_Count_Commercial_Only,


                EC_Tot_Cost = x.Episode_Cost_Total_Cost_Commercial_Only,


                EC_Avg_Cost = x.Episode_Cost_Average_Cost_Commercial_Only,
                EC_CV = x.Episode_Cost_Coefficients_of_Variation_Commercial_Only,


                //DAVE 82022
                //EC_Spec_Episode_Cnt = x.Episode_Cost_Episode_Count_Commercial_Only,
                EC_Spec_Episode_Cnt = x.Episode_Count,


                EC_Spec_Episode_Distribution = x.Episode_Cost_Episode_Distribution,
                EC_Spec_Perc_of_Episodes = x.Episode_Cost_Percent_of_Episodes,
                EC_Spec_Tot_Cost = x.Episode_Cost_Total_Cost,
                EC_Spec_Avg_Cost = x.Episode_Cost_Average_Cost,
                EC_Spec_CV = x.Episode_Cost_Specialist_CV,
                EC_Previous_Mapping = x.Previous_Mapping,
                EC_Current_Mapping = x.Current_Mapping,



                EC_Change_Comments = x.Episode_Cost_Change_Comments,
                Previous_Pt_Centric_Mapping = x.Previous_Patient_Centric_Mapping,
                Current_Pt_Centric_Mapping = x.Current_Patient_Centric_Mapping,
                Pt_Centric_Change_Comments = x.Patient_Centric_Change_Comments,
                Measure_Status = x.Measure_Status


            }).ToList();




            //var targetList = esmList.Select(x => new ETG_Fact_Symmetry_Export_Model()
            //{
            //    ETGFactSymmetryId = x.ETG_Fact_Symmetry_id,
            //    ETGDescription = x.ETG_Description,
            //    ETGBaseClass = x.ETG_Base_Class,
            //    PremiumSpecialty = x.Premium_Specialty,
            //    PreviousLOB = x.LOBPreviousString,
            //    CurrentLOB = x.LOBCurrentString,
            //    PreviousPCTreatmentIndicator = x.Pop_Cost_Previous_Treatment_Indicator,
            //    CurrentPCTreatmentIndicator = x.Pop_Cost_Current_Treatment_Indicator,
            //    NRx = x.has_nrx,
            //    Rx = x.has_rx,
            //    PCEpisodeCnt = x.Pop_Cost_Episode_Count_Commercial_Only,
            //    TotalCost = x.Total_Cost_Commercial_Only,
            //    AvgCost = x.Average_Cost_Commercial_Only,
            //    CoefficientsOfVariation = x.Coefficients_of_Variation_Commercial_Only,
            //    //NormalizedPricingTotalCost = x.Normalized_Pricing_Episode_Count,
            //   // NormalizedPricingEpisodeCnt = x.Normalized_Pricing_Total_Cost,
            //    SpecialistEpisodeCnt = x.Specialist_Episode_Count,
            //    PopCostEpisodeDistribution = x.Pop_Cost_Episode_Distribution,
            //    PercentOfEpisodes = x.Percent_of_Episodes,
            //    SpecialistTotalCost = x.Specialist_Total_Cost,
            //    SpecialistAverageCost = x.Specialist_Average_Cost ,
            //    SpecialistCV = x.Specialist_CV,
            //    PopCostChangesMade = x.Pop_Cost_Changes_Made,
            //    ECTotalCostCommercialOnly = x.Episode_Cost_Total_Cost_Commercial_Only,
            //    ECAverageCostCommercialOnly = x.Episode_Cost_Average_Cost_Commercial_Only,
            //    ECCoefficientsOfVariationCommercialOnly = x.Episode_Cost_Coefficients_of_Variation_Commercial_Only,
            //    PreviousPCAttribution = x.Previous_Attribution,
            //    CurrentPCAttribution = x.Current_Attribution,
            //    PCChangeComments = x.Pop_Cost_Change_Comments,
            //    PreviousECTreatmentIndicator = x.Previous_Episode_Cost_Treatment_Indicator,
            //    CurrentECTreatmentIndicator = x.Current_Episode_Cost_Treatment_Indicator,
            //    ECPreviousMapping = x.Previous_Mapping,
            //    ECCurrentMapping = x.Current_Mapping,
            //    ECEpisodeCnt = x.Episode_Count,
            //    //ECNormalizedPricingEpisodeCount = x.Episode_Cost_Normalized_Pricing_Episode_Count ,
            //    //ECNormalizedPricingTotalCost = x.Episode_Cost_Normalized_Pricing_Total_Cost ,
            //    ECEpisodeCountCommercialOnly = x.Episode_Cost_Episode_Count_Commercial_Only,
            //    ECEpisodeDistribution = x.Episode_Cost_Episode_Distribution,
            //    ECPercentOfEpisodes = x.Episode_Cost_Percent_of_Episodes,
            //    ECTotalCost = x.Episode_Cost_Total_Cost ,
            //    ECAverageCost = x.Episode_Cost_Average_Cost,
            //    ECSpecialistCV = x.Episode_Cost_Specialist_CV,
            //    EC_ChangesMade = x.Episode_Cost_Changes_Made,
            //    ECChangeComments = x.Episode_Cost_Change_Comments,
            //    PreviousPatientCentricMapping = x.Previous_Patient_Centric_Mapping,
            //    CurrentPatientCentricMapping = x.Current_Patient_Centric_Mapping,
            //    PatientCentricChangeComments = x.Patient_Centric_Change_Comments


            //}).ToList();

            return targetList;

        }

        private List<ETG_Fact_Symmetry_Export_Model2> getCleanListForExport2(List<ETG_Fact_Symmetry_Interface_Model> esmList)
        {
            var targetList = esmList.Select(x => new ETG_Fact_Symmetry_Export_Model2()
            {
                ETG_Base_Class = x.ETG_Base_Class,
                ETG_Description = x.ETG_Description,
                Premium_Specialty = x.Premium_Specialty,
                //Previous_Rx_NRx = x.has_nrx,
                Current_Rx_NR = x.has_rx,
                //Previous_LOB = x.LOBPreviousString,
                Current_LOB = x.LOBCurrentString,
                //PC_Episode_Cnt = x.Pop_Cost_Episode_Count_Commercial_Only,
                //PC_Tot_Cost = x.Total_Cost_Commercial_Only,
                //PC_Avg_Cost = x.Average_Cost_Commercial_Only,
                //PC_CV = x.Coefficients_of_Variation_Commercial_Only,
                //PC_Spec_Episode_Cnt = x.Specialist_Episode_Count,
                //PC_Spec_Episode_Distribution = x.Pop_Cost_Episode_Distribution,
                //PC_Spec_Perc_of_Episodes = x.Percent_of_Episodes,
                //PC_Spec_Tot_Cost = x.Specialist_Total_Cost,
                //PC_Spec_Avg_Cost = x.Specialist_Average_Cost,
                //PC_Spec_CV = x.Specialist_CV,
                //PC_Prev_Attribution = x.Previous_Attribution,
                //PC_Current_Attribution = x.Current_Attribution,
                //PC_Change_Comments = x.Pop_Cost_Change_Comments,


                EC_Episode_Count = x.Episode_Cost_Episode_Count_Commercial_Only,



                EC_Tot_Cost = x.Episode_Cost_Total_Cost_Commercial_Only,
                EC_Avg_Cost = x.Episode_Cost_Average_Cost_Commercial_Only,
                EC_CV = x.Episode_Cost_Coefficients_of_Variation_Commercial_Only,



                EC_Spec_Episode_Cnt = x.Episode_Count,




                EC_Spec_Episode_Distribution = x.Episode_Cost_Episode_Distribution,
                EC_Spec_Perc_of_Episodes = x.Episode_Cost_Percent_of_Episodes,
                EC_Spec_Tot_Cost = x.Episode_Cost_Total_Cost,
                EC_Spec_Avg_Cost = x.Episode_Cost_Average_Cost,
                EC_Spec_CV = x.Episode_Cost_Specialist_CV,
                //EC_Previous_Mapping = x.Previous_Mapping,
                EC_Current_Mapping = x.Current_Mapping,
                EC_Change_Comments = x.Episode_Cost_Change_Comments,
                // Previous_Pt_Centric_Mapping = x.Previous_Patient_Centric_Mapping,
                //Current_Pt_Centric_Mapping = x.Current_Patient_Centric_Mapping,
                //Pt_Centric_Change_Comments = x.Patient_Centric_Change_Comments,
                EC_Current_Treatment_Indicator = x.Current_Episode_Cost_Treatment_Indicator
                //EC_Previous_Treatment_Indicator = x.Previous_Episode_Cost_Treatment_Indicator,
                //Measure_Status = x.Measure_Status


            }).ToList();

            List<ETG_Fact_Symmetry_Export_Model2> exDoubleUpList = new List<ETG_Fact_Symmetry_Export_Model2>();
            ETG_Fact_Symmetry_Export_Model2 exDoubleUp = new ETG_Fact_Symmetry_Export_Model2();
            foreach (ETG_Fact_Symmetry_Export_Model2 ex2 in targetList)
            {
                if (ex2.EC_Current_Treatment_Indicator == "0 & 1")
                {
                    ex2.EC_Current_Treatment_Indicator = "1";
                    //ex2.EC_Previous_Treatment_Indicator = "1";
                    //DOUBLE UP CLONE???
                    exDoubleUp = (ETG_Fact_Symmetry_Export_Model2)ex2.Clone();
                    //exDoubleUp = new ETG_Fact_Symmetry_Export_Model2() { = ex2 }
                    exDoubleUp.EC_Current_Treatment_Indicator = "0";
                    //exDoubleUp.EC_Previous_Treatment_Indicator = "0";
                    exDoubleUpList.Add(exDoubleUp);
                }
            }


            targetList = targetList.Concat(exDoubleUpList).ToList();
            //targetList.Sort((p, q) => p.ETG_Base_Class.CompareTo(q.ETG_Base_Class));
            targetList.Sort((x, y) =>
            {
                int ret = String.Compare(x.ETG_Base_Class, y.ETG_Base_Class);
                return ret != 0 ? ret : x.Premium_Specialty.CompareTo(y.Premium_Specialty);
            });




            return targetList;

        }


        //private List<ETG_Fact_Symmetry_Export_Model2> getCleanListForExport2(List<ETG_Fact_Symmetry_Interface_Model> esmList)
        //{
        //    var targetList = esmList.Select(x => new ETG_Fact_Symmetry_Export_Model2()
        //    {
        //        ETG_Base_Class = x.ETG_Base_Class,
        //        ETG_Description = x.ETG_Description,
        //        Premium_Specialty = x.Premium_Specialty,
        //        Previous_Rx_NRx = x.has_nrx,
        //        Current_Rx_NR = x.has_rx,
        //        PC_Episode_Cnt = x.Pop_Cost_Episode_Count_Commercial_Only,
        //        PC_Tot_Cost = x.Total_Cost_Commercial_Only,
        //        PC_Avg_Cost = x.Average_Cost_Commercial_Only,
        //        PC_CV = x.Coefficients_of_Variation_Commercial_Only,
        //        PC_Spec_Episode_Cnt = x.Specialist_Episode_Count,
        //        PC_Spec_Episode_Distribution = x.Pop_Cost_Episode_Distribution,
        //        PC_Spec_Perc_of_Episodes = x.Percent_of_Episodes,
        //        PC_Spec_Tot_Cost = x.Specialist_Total_Cost,
        //        PC_Spec_Avg_Cost = x.Specialist_Average_Cost,
        //        PC_Spec_CV = x.Specialist_CV,
        //        PC_Prev_Attribution = x.Previous_Attribution,
        //        PC_Current_Attribution = x.Current_Attribution,
        //        PC_Change_Comments = x.Pop_Cost_Change_Comments,
        //        EC_Episode_Count = x.Episode_Count,
        //        EC_Tot_Cost = x.Episode_Cost_Total_Cost_Commercial_Only,
        //        EC_Avg_Cost = x.Episode_Cost_Average_Cost_Commercial_Only,
        //        EC_CV = x.Episode_Cost_Coefficients_of_Variation_Commercial_Only,
        //        EC_Spec_Episode_Cnt = x.Episode_Cost_Episode_Count_Commercial_Only,
        //        EC_Spec_Episode_Distribution = x.Episode_Cost_Episode_Distribution,
        //        EC_Spec_Perc_of_Episodes = x.Episode_Cost_Percent_of_Episodes,
        //        EC_Spec_Tot_Cost = x.Episode_Cost_Total_Cost,
        //        EC_Spec_Avg_Cost = x.Episode_Cost_Average_Cost,
        //        EC_Spec_CV = x.Episode_Cost_Specialist_CV,
        //        EC_Previous_Mapping = x.Previous_Mapping,
        //        EC_Current_Mapping = x.Current_Mapping,
        //        EC_Change_Comments = x.Episode_Cost_Change_Comments,
        //        Previous_Pt_Centric_Mapping = x.Previous_Patient_Centric_Mapping,
        //        Current_Pt_Centric_Mapping = x.Current_Patient_Centric_Mapping,
        //        Pt_Centric_Change_Comments = x.Patient_Centric_Change_Comments,
        //        Current_Episode_Cost_Treatment_Indicator = x.Current_Episode_Cost_Treatment_Indicator,
        //        Previous_Episode_Cost_Treatment_Indicator = x.Previous_Episode_Cost_Treatment_Indicator,
        //        Measure_Status = x.Measure_Status


        //    }).ToList();



        //    return targetList;

        //}




        private static readonly SortDescription DefaultSortOrder = new SortDescription("Premium_Specialty", ListSortDirection.Ascending);
        private bool sortAscending = true;
        void Sort(object parameter)
        {
            string sortColumn = ((System.Windows.Controls.DataGridSortingEventArgs)parameter).Column.Header.ToString().Trim();
            this._etg_Fact_SymmetryListCollectionView.SortDescriptions.Clear();

            if (this.sortAscending)
            {
                this._etg_Fact_SymmetryListCollectionView.SortDescriptions.Add(new SortDescription(sortColumn, ListSortDirection.Ascending));
                this.sortAscending = false;
            }
            else
            {
                this._etg_Fact_SymmetryListCollectionView.SortDescriptions.Add(new SortDescription(sortColumn, ListSortDirection.Descending));
                this.sortAscending = true;
            }
        }
        #endregion

        #region FILTER ARRAYS AND CHECK
        private Int16[] _intPrimarySpecialtyFiltersArr;
        public Int16[] PrimarySpecialtyFiltersArr
        {
            get { return _intPrimarySpecialtyFiltersArr; }
            set { Set(ref _intPrimarySpecialtyFiltersArr, value); }
        }

        private string[] _strETGBaseFilters;
        public string[] ETGBaseFiltersArr
        {
            get { return _strETGBaseFilters; }
            set { Set(ref _strETGBaseFilters, value); }
        }

        private string[] _strCurrentAttributionFilters;
        public string[] CurrentAttributionFiltersArr
        {
            get { return _strCurrentAttributionFilters; }
            set { Set(ref _strCurrentAttributionFilters, value); }
        }


        private string[] _strCurrentMeasureStatusFilters;
        public string[] CurrentMeasureStatusFiltersArr
        {
            get { return _strCurrentMeasureStatusFilters; }
            set { Set(ref _strCurrentMeasureStatusFilters, value); }
        }



        private string[] _strSymmetryVersionFilterArr;
        public string[] SymmetryVersionFilterArr
        {
            get { return _strSymmetryVersionFilterArr; }
            set { Set(ref _strSymmetryVersionFilterArr, value); }
        }

        private bool _isUpdatesOnly;
        public bool IsUpdatesOnly
        {
            get { return _isUpdatesOnly; }
            set
            {
                if (_isUpdatesOnly == value) return;

                _isUpdatesOnly = value;
                //OnPropertyChanged("IsUpdatesOnly");//CHECKED

            }
        }

        private bool _isReadOnly;
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {

                _isReadOnly = value;
                //OnPropertyChanged("IsUpdatesOnly");//CHECKED

            }
        }


        private string _strCurrentSymmetry_Version;
        public string strCurrentSymmetry_Version
        {
            get { return _strCurrentSymmetry_Version; }
            set
            {
                _strCurrentSymmetry_Version = value;

            }
        }
        private string _strPreviousSymmetry_Version;


        private string _strCurrentSymmetry_VersionDisplay;
        public string strCurrentSymmetry_VersionDisplay
        {
            get { return _strCurrentSymmetry_VersionDisplay; }
            set
            {
                _strCurrentSymmetry_VersionDisplay = value;

            }
        }

        private string _strPreviousSymmetry_VersionDisplay;
        public string strPreviousSymmetry_VersionDisplay
        {
            get { return _strPreviousSymmetry_VersionDisplay; }
            set
            {
                _strPreviousSymmetry_VersionDisplay = value;

            }
        }


        private bool FilterSearch(ETG_Fact_Symmetry_Interface_Model f)
        {
            bool blExists = true;

            if (PrimarySpecialtyFiltersArr != null && ETGBaseFiltersArr != null && CurrentAttributionFiltersArr != null && CurrentMeasureStatusFiltersArr != null)
            {
                if (PrimarySpecialtyFiltersArr.Contains(f.Premium_Specialty_id) && ETGBaseFiltersArr.Contains(f.ETG_Base_Class) && CurrentAttributionFiltersArr.Contains(f.Current_Attribution) && CurrentMeasureStatusFiltersArr.Contains(f.Measure_Status))
                    blExists = true;
                else
                    blExists = false;
            }
            else if (PrimarySpecialtyFiltersArr != null && ETGBaseFiltersArr != null && CurrentAttributionFiltersArr != null)
            {
                if (PrimarySpecialtyFiltersArr.Contains(f.Premium_Specialty_id) && ETGBaseFiltersArr.Contains(f.ETG_Base_Class) && CurrentAttributionFiltersArr.Contains(f.Current_Attribution))
                    blExists = true;
                else
                    blExists = false;
            }
            else if (PrimarySpecialtyFiltersArr != null && ETGBaseFiltersArr != null && CurrentMeasureStatusFiltersArr != null)
            {
                if (PrimarySpecialtyFiltersArr.Contains(f.Premium_Specialty_id) && ETGBaseFiltersArr.Contains(f.ETG_Base_Class) && CurrentMeasureStatusFiltersArr.Contains(f.Measure_Status))
                    blExists = true;
                else
                    blExists = false;
            }
            else if (ETGBaseFiltersArr != null && CurrentMeasureStatusFiltersArr != null && CurrentMeasureStatusFiltersArr != null)
            {
                if (ETGBaseFiltersArr.Contains(f.ETG_Base_Class) && CurrentMeasureStatusFiltersArr.Contains(f.Measure_Status) && CurrentMeasureStatusFiltersArr.Contains(f.Measure_Status))
                    blExists = true;
                else
                    blExists = false;
            }
            else if (PrimarySpecialtyFiltersArr != null && CurrentMeasureStatusFiltersArr != null && CurrentMeasureStatusFiltersArr != null)
            {
                if (PrimarySpecialtyFiltersArr.Contains(f.Premium_Specialty_id) && CurrentAttributionFiltersArr.Contains(f.Current_Attribution) && CurrentMeasureStatusFiltersArr.Contains(f.Measure_Status))
                    blExists = true;
                else
                    blExists = false;
            }
            else if (ETGBaseFiltersArr != null && PrimarySpecialtyFiltersArr != null)
            {
                if (ETGBaseFiltersArr.Contains(f.ETG_Base_Class) && PrimarySpecialtyFiltersArr.Contains(f.Premium_Specialty_id))
                    blExists = true;
                else
                    blExists = false;
            }
            else if (PrimarySpecialtyFiltersArr != null && CurrentMeasureStatusFiltersArr != null)
            {
                if (PrimarySpecialtyFiltersArr.Contains(f.Premium_Specialty_id) && CurrentMeasureStatusFiltersArr.Contains(f.Measure_Status))
                    blExists = true;
                else
                    blExists = false;
            }
            else if (ETGBaseFiltersArr != null && CurrentMeasureStatusFiltersArr != null)
            {
                if (ETGBaseFiltersArr.Contains(f.ETG_Base_Class) && CurrentMeasureStatusFiltersArr.Contains(f.Measure_Status))
                    blExists = true;
                else
                    blExists = false;
            }
            else if (ETGBaseFiltersArr != null && CurrentAttributionFiltersArr != null)
            {
                if (ETGBaseFiltersArr.Contains(f.ETG_Base_Class) && CurrentAttributionFiltersArr.Contains(f.Current_Attribution))
                    blExists = true;
                else
                    blExists = false;
            }
            else if (PrimarySpecialtyFiltersArr != null && CurrentAttributionFiltersArr != null)
            {
                if (PrimarySpecialtyFiltersArr.Contains(f.Premium_Specialty_id) && CurrentAttributionFiltersArr.Contains(f.Current_Attribution))
                    blExists = true;
                else
                    blExists = false;
            }
            else if (CurrentAttributionFiltersArr != null && CurrentMeasureStatusFiltersArr != null)
            {
                if (CurrentAttributionFiltersArr.Contains(f.Current_Attribution) && CurrentMeasureStatusFiltersArr.Contains(f.Measure_Status))
                    blExists = true;
                else
                    blExists = false;
            }
            else if (PrimarySpecialtyFiltersArr != null)
            {
                if (PrimarySpecialtyFiltersArr.Contains(f.Premium_Specialty_id))
                    blExists = true;
                else
                    blExists = false;
            }
            else if (ETGBaseFiltersArr != null)
            {
                if (ETGBaseFiltersArr.Contains(f.ETG_Base_Class))
                    blExists = true;
                else
                    blExists = false;
            }
            else if (CurrentAttributionFiltersArr != null)
            {
                if (CurrentAttributionFiltersArr.Contains(f.Current_Attribution))
                    blExists = true;
                else
                    blExists = false;
            }
            else if (CurrentMeasureStatusFiltersArr != null)
            {
                if (CurrentMeasureStatusFiltersArr.Contains(f.Measure_Status))
                    blExists = true;
                else
                    blExists = false;
            }

            if (_isUpdatesOnly && blExists == true)
            {
                if (!_arrETG_Fact_Symmetry_Update_Tracker.Exists(x => x.ETG_Fact_Symmetry_id.Equals(f.ETG_Fact_Symmetry_id)))
                    blExists = false;
            }

            return blExists;
        }


        private void prepParametersForSQLSearch(object[] parameters, string strPrimarySpecialty, string strETGBase, string strSymmetryVersion)
        {


            //ELSE USER SELECTED PARAMETERS
            if (!parameters[0].ToString().Trim().Equals("-9999") && !parameters[0].ToString().Trim().Equals(""))
            {
                PrimarySpecialtyFiltersArr = Array.ConvertAll(parameters[0].ToString().Replace("-9999", "").TrimStart(',').Split(','), Int16.Parse);

                //IF ALL FILTERS NOT FILTER
                if (PrimarySpecialtyFiltersArr.Count() == (ETG_Dim_PremiumSpecDic.Count - 1))
                    PrimarySpecialtyFiltersArr = null;
            }
            else
                PrimarySpecialtyFiltersArr = null;


            if (!parameters[1].ToString().Trim().Equals("-9999") && !parameters[1].ToString().Trim().Equals(""))
            {
                ETGBaseFiltersArr = parameters[1].ToString().Replace("-9999", "").TrimStart(',').Split(',');

                //IF ALL FILTERS NOT FILTER
                if (ETGBaseFiltersArr.Count() == (ETG_Dim_MasterDic.Count - 1))
                    ETGBaseFiltersArr = null;
            }
            else
                ETGBaseFiltersArr = null;



            if (!parameters[2].ToString().Trim().Equals("--All--") && !parameters[2].ToString().Trim().Equals(""))
            {
                CurrentAttributionFiltersArr = parameters[2].ToString().Replace("--All--", "").TrimStart(',').Split(',');

                //IF ALL FILTERS NOT FILTER
                if (CurrentAttributionFiltersArr.Count() == (AttributionFilterLst.Count - 1))
                    CurrentAttributionFiltersArr = null;
            }
            else
                CurrentAttributionFiltersArr = null;



            if (!parameters[3].ToString().Trim().Equals("--All--") && !parameters[3].ToString().Trim().Equals(""))
            {
                CurrentMeasureStatusFiltersArr = parameters[3].ToString().Replace("--All--", "").TrimStart(',').Split(',');

                //IF ALL FILTERS NOT FILTER
                if (CurrentMeasureStatusFiltersArr.Count() == (MeasureStatusFilterLst.Count - 1))
                    CurrentMeasureStatusFiltersArr = null;
            }
            else
                CurrentMeasureStatusFiltersArr = null;



            //if (!parameters[3].ToString().Trim().Equals(""))
            //{
            //    SymmetryVersionFilter = parameters[3].ToString();

            //    ////IF ALL FILTERS NOT FILTER
            //    //if (SymmetryVersionFiltersArr.Count() == (ETG_Dim_MasterDic.Count - 1))
            //    //    SymmetryVersionFiltersArr = null;
            //}
            //else
            //    SymmetryVersionFilter = null;





        }

        #endregion

        #region DATAGRID EDIT PROPERTIES

        private string _strUserName;
        public string UserName
        {
            get { return _strUserName; }
            set { _strUserName = value; }
        }

        private List<KeyValuePair<string, string>> _dicETG_Dim_PremiumSpec;
        public List<KeyValuePair<string, string>> ETG_Dim_PremiumSpecDic
        {
            get;
            set;
        }

        //MAIN MODEL ARRAY
        private List<KeyValuePair<string, string>> _dicETG_Dim_MasterArr;
        public List<KeyValuePair<string, string>> ETG_Dim_MasterDic
        {
            get;
            set;
        }


        //MAIN MODEL ARRAY
        private List<string> _dicETG_AttributionFilterArr;
        public List<string> AttributionFilterLst
        {
            get;
            set;
        }

        //MAIN MODEL ARRAY
        private List<string> _dicETG_MeasureStatusFilterArr;
        public List<string> MeasureStatusFilterLst
        {
            get;
            set;
        }

        //MAIN MODEL ARRAY
        public ObservableCollection<string> LOBOptionsArr
        {
            get;
            set;
        }

        public ObservableCollection<string> TreatmentIndicatorOptionsArr
        {
            get;
            set;
        }

        public ObservableCollection<string> AttributionOptionsArr
        {
            get;
            set;
        }


        public ObservableCollection<string> TreatmentIndicatorECOptionsArr
        {
            get;
            set;
        }


        public ObservableCollection<string> MappingOptionsArr
        {
            get;
            set;
        }


        public ObservableCollection<string> PatientCentricMappingOptionsArr
        {
            get;
            set;
        }

        //MAIN MODEL ARRAY
        private ObservableCollection<ETG_Symmetry_Verion> _arrETG_Symmetry_Verion;
        public ObservableCollection<ETG_Symmetry_Verion> ETG_Symmetry_VerionArr
        {
            get;
            set;
        }
        #endregion

        #region LOAD SUPPORT LISTS
        public async void LoadSupportLists()
        {
            //DATAGRID INTERFACE OPTIONS COME FROM MODELS START
            ETG_Interface_Models eim = new ETG_Interface_Models();
            LOBOptionsArr = new ObservableCollection<string>(eim.lobOptionsArr as List<string>); ;
            TreatmentIndicatorOptionsArr = new ObservableCollection<string>(eim.treatmentIndicatorOptionsArr as List<string>);
            AttributionOptionsArr = new ObservableCollection<string>(eim.attributionOptionsArr as List<string>);
            TreatmentIndicatorECOptionsArr = new ObservableCollection<string>(eim.treatmentIndicatorECOptionsArr as List<string>);
            MappingOptionsArr = new ObservableCollection<string>(eim.mappingOptionsArr as List<string>);
            PatientCentricMappingOptionsArr = new ObservableCollection<string>(eim.patientCentricMappingOptionsArr as List<string>);


            //DATAGRID FILTERS
            _arrETG_Symmetry_Verion = new ObservableCollection<ETG_Symmetry_Verion>(_repo.GetSymmetryVersion() as List<ETG_Symmetry_Verion>);
            this.ETG_Symmetry_VerionArr = _arrETG_Symmetry_Verion;

            this.SymmetryVersionFilterArr = this.ETG_Symmetry_VerionArr.Select(p => p.Symmetry_Version.ToString()).ToArray();


            _dicETG_Dim_MasterArr = _repo.GetETGDimMaster().AsEnumerable().Select(item => new KeyValuePair<string, string>(item.ETG_Base_Class, item.ETG_Description)).ToList();
            _dicETG_Dim_MasterArr.Insert(0, new KeyValuePair<string, string>("-9999", "--All--"));
            this.ETG_Dim_MasterDic = _dicETG_Dim_MasterArr;

            _dicETG_Dim_PremiumSpec = _repo.GetETGDimPremiumSpec().AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Premium_Specialty_id.ToString(), item.Premium_Specialty)).ToList();
            _dicETG_Dim_PremiumSpec.Insert(0, new KeyValuePair<string, string>("-9999", "--All--"));
            this.ETG_Dim_PremiumSpecDic = _dicETG_Dim_PremiumSpec;



            _dicETG_AttributionFilterArr = new List<string>();
            _dicETG_AttributionFilterArr.Insert(0, "--All--");
            //_dicETG_AttributionFilterArr.Insert(1, "Not Selected");
            _dicETG_AttributionFilterArr.Insert(1, "Not Mapped");
            _dicETG_AttributionFilterArr.Insert(2, "Always Attributed");
            _dicETG_AttributionFilterArr.Insert(3, "If Involved");
            this.AttributionFilterLst = _dicETG_AttributionFilterArr;



            _dicETG_MeasureStatusFilterArr = new List<string>();
            _dicETG_MeasureStatusFilterArr.Insert(0, "--All--");
            //_dicETG_AttributionFilterArr.Insert(1, "Not Selected");
            _dicETG_MeasureStatusFilterArr.Insert(1, "Added");
            _dicETG_MeasureStatusFilterArr.Insert(2, "Inconsistent Mapping");
            _dicETG_MeasureStatusFilterArr.Insert(3, "No Change");
            _dicETG_MeasureStatusFilterArr.Insert(3, "Removed");
            this.MeasureStatusFilterLst = _dicETG_MeasureStatusFilterArr;




            //GET LATEST AND PREVIOUS DATES
            populateDateSpan();
        }

        private string _strFilteredSymmetry_Version;
        public string strFilteredSymmetry_Version
        {
            get { return _strFilteredSymmetry_Version; }
            set
            {
                _strFilteredSymmetry_Version = value;

            }
        }
        private void populateDateSpan()
        {

            this.IsReadOnly = true;

            string strVerion = null;
            if (File.Exists(GlobalState.strVersionPath))
                strVerion = File.ReadAllText(GlobalState.strVersionPath);

            if (strVerion != null)
            {
                _strCurrentSymmetry_Version = null;
                _strPreviousSymmetry_Version = null;
                foreach (var d in this.ETG_Symmetry_VerionArr)
                {
                    if (strVerion != d.Symmetry_Version.ToString() && _strCurrentSymmetry_Version == null)
                    {
                        this.IsReadOnly = false;
                        continue;
                    }

                    if (_strCurrentSymmetry_Version == null)
                        _strCurrentSymmetry_Version = "'" + d.Symmetry_Version + "'";
                    else
                    {
                        _strPreviousSymmetry_Version = "'" + d.Symmetry_Version + "'";
                        break;
                    }
                }
                if (_strPreviousSymmetry_Version == null)
                    _strPreviousSymmetry_Version = _strCurrentSymmetry_Version;
            }
            else
            {
                Int16 i = 0;
                foreach (var d in this.ETG_Symmetry_VerionArr)
                {
                    if (i == 0)
                    {
                        _strCurrentSymmetry_Version = "'" + d.Symmetry_Version + "'";
                    }
                    else if (i == 1)
                    {
                        _strPreviousSymmetry_Version = "'" + d.Symmetry_Version + "'";
                        break;
                    }

                    i++;
                }
            }


            strCurrentSymmetry_VersionDisplay = _strCurrentSymmetry_Version.Replace("'", "");
            strPreviousSymmetry_VersionDisplay = _strPreviousSymmetry_Version.Replace("'", "");

        }
        #endregion


        private bool _EditMode;
        public bool EditMode //UPDATE OR INSERT!!!!!
        {
            get { return _EditMode; }
            set { _EditMode = value; }
        }

        //VALIDATION PLACEHOLDER
        public bool IsValid
        {
            get
            {

                //REFRESH EXECUTION VALIDATION
                DeleteCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
                CancelCommand.RaiseCanExecuteChanged();
                SearchCommand.RaiseCanExecuteChanged();
                //SortCommand.RaiseCanExecuteChanged();
                bool isValid = this._arrETG_Fact_Symmetry.All(c => IsValid == false);

                return isValid;


            }
        }



        private void UpdateETG_Fact_Symmetry(ETG_Fact_Symmetry_Interface_Model source, ETG_Fact_Symmetry_Interface_Model target)
        {
            //target.FirstName = source.FirstName;
            //target.LastName = source.LastName;
            //target.Email = source.Email;
            //target.Username = source.Username;
            //target.DescriptionOfRequest = source.DescriptionOfRequest;
            //target.SpecialtyArea = source.SpecialtyArea;
            //target.BusinessArea = source.BusinessArea;
            //target.BusinessPurpose = source.BusinessPurpose;
            //target.BusinessValue = source.BusinessValue;
            //target.BusinessValueNon = source.BusinessValueNon;
            //target.LOB = source.LOB;
            //target.Market = source.Market;
            //target.Timeframe = source.Timeframe;
            //target.ExpectedKickoffDate = source.ExpectedKickoffDate;
            //target.BusinessSponsor = source.BusinessSponsor;
            //target.ClinicalSponsor = source.ClinicalSponsor;


        }












        //MAIN MODEL OBJECT ??????
        //MAIN MODEL OBJECT ??????
        //MAIN MODEL OBJECT ??????
        private ETG_Fact_Symmetry_Interface_Model _objETG_Fact_Symmetry;
        private ETG_Fact_Symmetry_Interface_Model _editingETG_Fact_Symmetry = null;
        public ETG_Fact_Symmetry_Interface_Model ETG_Fact_Symmetry
        {
            get { return _objETG_Fact_Symmetry; }
            set
            {
                _editingETG_Fact_Symmetry = value;
                _objETG_Fact_Symmetry = value;

                //REFRESH EXECUTION VALIDATION
                DeleteCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
                CancelCommand.RaiseCanExecuteChanged();
                //SearchCommand.RaiseCanExecuteChanged();
            }
        }





        //?????????????????????????????????????????????????


        //public bool IsValid
        //{
        //    get { return this._objETG_Fact_Symmetry.IsValid; }
        //}











        private void CopyETG_Fact_Symmetry(MHP_Yearly_Universes_Reporting_Model source, MHP_Yearly_Universes_Reporting_Model target)
        {
            //target.IntakeId = source.IntakeId;

            if (EditMode)
            {
                //target.FirstName = source.FirstName;
                //target.LastName = source.LastName;
                //target.Email = source.Email;
                //target.Username = source.Username;
                //target.DescriptionOfRequest = source.DescriptionOfRequest;
                //target.SpecialtyArea = source.SpecialtyArea;
                //target.BusinessArea = source.BusinessArea;
                //target.BusinessPurpose = source.BusinessPurpose;
                //target.BusinessValue = source.BusinessValue;
                //target.BusinessValueNon = source.BusinessValueNon;
                //target.LOB = source.LOB;
                //target.Market = source.Market;
                //target.Timeframe = source.Timeframe;
                //target.ExpectedKickoffDate = source.ExpectedKickoffDate;
                //target.BusinessSponsor = source.BusinessSponsor;
                //target.ClinicalSponsor = source.ClinicalSponsor;
            }
        }






        //    public virtual event PropertyChangedEventHandler PropertyChanged;
        //    protected virtual void NotifyPropertyChanged(params string[] propertyNames)
        //    {
        //        if (PropertyChanged != null)
        //        {
        //            foreach (string propertyName in propertyNames) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //            PropertyChanged(this, new PropertyChangedEventArgs("HasError"));
        //        }
        //    }

        //private string _Filter;

        //public string Filter
        //{
        //    get { return _Filter; }
        //    set
        //    {
        //        if (SetProperty(ref _Filter, value))
        //            PagingCollectionView.Refresh();
        //    }
        //}

        //protected bool SetProperty<T>(ref T prop, T value, [CallerMemberName] string propertyName = null)
        //{
        //    if (object.Equals(prop, value)) return false;
        //    prop = value;
        //    RaisePropertyChanged(propertyName);
        //    return true;
        //}

    }



}
