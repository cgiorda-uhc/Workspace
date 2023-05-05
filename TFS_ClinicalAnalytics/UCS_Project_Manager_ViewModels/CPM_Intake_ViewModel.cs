//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using GalaSoft.MvvmLight;
//using UCS_Project_Manager_Models;
//using UCS_Project_Manager_Services;

//namespace UCS_Project_Manager
//{
//    class CPM_Intake_ViewModel : ViewModelBase
//    {
//        //GLOBAL VARIABLES
//        public RelayCommand DeleteCommand { get; set; }
//        public RelayCommand CancelCommand { get; private set; }
//        public RelayCommand SaveCommand { get; private set; }
//        public event Action Done = delegate { };

//        private ICPM_Intake_Repository _repo;

//        //CONSTRUCTOR
//        public CPM_Intake_ViewModel(ICPM_Intake_Repository repo)
//        {
//            //REFGISTER MODEL FOR VALIDATION
//            AppMessages.ProjectChangeTracking.Register(this,
//                    new Action<bool>((arg) => { this.RaisePropertyChanged("IsValid"); }));

//            //INSTANTIATE OBJECTS
//            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
//            CancelCommand = new RelayCommand(OnCancel);
//            SaveCommand = new RelayCommand(OnSave, CanSave);
//            _repo = repo;

//            //this.CPM_Intake = new CPM_Intake_Model();
//            //_objCPM_Intake = new CPM_Intake_Model();
//            //LOAD PROPERTIES
//            LoadCPM_Intake();

//        }

//        //MAIN MODEL OBJECT
//        private CPM_Intake_Model _objCPM_Intake;
//        private CPM_Intake_Model _editingCPM_Intake = null;
//        public CPM_Intake_Model CPM_Intake
//        {
//            get { return _objCPM_Intake; }
//            set
//            {
//                _editingCPM_Intake = value;
//                //_objCPM_Intake = new CPM_Intake_Model();
//                //_editingCPM_Intake = new CPM_Intake_Model();
//                _objCPM_Intake = value;
//                //this.CPM_Intake = value;
//                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Collection)));



//                //REFRESH EXECUTION VALIDATION
//                DeleteCommand.RaiseCanExecuteChanged();
//                SaveCommand.RaiseCanExecuteChanged();
//                CancelCommand.RaiseCanExecuteChanged();
                 
//            }
//        }


//        //MAIN MODEL ARRAY
//        private ObservableCollection<CPM_Intake_Model> _arrCPM_Intake;
//        public ObservableCollection<CPM_Intake_Model> CPM_IntakeArr
//        {
//            get;
//            set;
//        }

//        //LOAD MODEL(S)
//        ActiveDirectoryHelper _ad;
//        public async void LoadCPM_Intake()
//        {
//            this.CPM_Intake = new CPM_Intake_Model();

//            if (GlobalState.IsDesignMode)//TESTING
//            {

//                //POPULATE OBJECT FOR TESTING
//                this.CPM_Intake.FirstName = "Chris";
//                this.CPM_Intake.LastName = "Giordano";
//                this.CPM_Intake.Email = "chris_giordano@uhc.com";
//                this.CPM_Intake.Username = "cgiorda";
//                this.CPM_Intake.DescriptionOfRequest = "Text";
//                //this.CPM_Intake.FullName
//                this.CPM_Intake.SpecialtyArea = "Text, DropDown, AD ??";
//                this.CPM_Intake.BusinessArea = "Text, DropDown, AD ??";
//                this.CPM_Intake.BusinessPurpose = "???";
//                this.CPM_Intake.BusinessValue = "???";
//                this.CPM_Intake.BusinessValueNon = "???";
//                this.CPM_Intake.LOB = "???";
//                this.CPM_Intake.Market = "???";
//                this.CPM_Intake.Timeframe = "???";
//                this.CPM_Intake.ExpectedKickoffDate = "???";
//                this.CPM_Intake.BusinessSponsor = "???";
//                this.CPM_Intake.ClinicalSponsor = "???";



//                ////POPULATE ARRAY MANUALLY FOR TESTING
//                _arrCPM_Intake = new ObservableCollection<CPM_Intake_Model>();

//                _arrCPM_Intake.Add(this.CPM_Intake);



//                //_arrCPM_Intake.Add(new CPM_Intake_Model {  FirstName = "abc", LastName = null, Email="", Username="", DescriptionOfRequest = "", SpecialtyArea = "", BusinessArea = "", BusinessPurpose = "", BusinessValue = "", BusinessValueNon = "", LOB = "", Market = "", Timeframe = "", ExpectedKickoffDate = "", BusinessSponsor = "", ClinicalSponsor = ""});
//                //_arrCPM_Intake.Add(new CPM_Intake_Model { FirstName = "Mark", LastName = "Allain", Email = "", Username = "", DescriptionOfRequest = "", SpecialtyArea = "", BusinessArea = "", BusinessPurpose = "", BusinessValue = "", BusinessValueNon = "", LOB = "", Market = "", Timeframe = "", ExpectedKickoffDate = "", BusinessSponsor = "", ClinicalSponsor = "" });
//                //_arrCPM_Intake.Add(new CPM_Intake_Model {  FirstName = "Allen", LastName = "Brown", Email = "", Username = "", DescriptionOfRequest = "", SpecialtyArea = "", BusinessArea = "", BusinessPurpose = "", BusinessValue = "", BusinessValueNon = "", LOB = "", Market = "", Timeframe = "", ExpectedKickoffDate = "", BusinessSponsor = "", ClinicalSponsor = "" });
//                //_arrCPM_Intake.Add(new CPM_Intake_Model {  FirstName = "Linda", LastName = "Hamerski", Email = "", Username = "", DescriptionOfRequest = "", SpecialtyArea = "", BusinessArea = "", BusinessPurpose = "", BusinessValue = "", BusinessValueNon = "", LOB = "", Market = "", Timeframe = "", ExpectedKickoffDate = "", BusinessSponsor = "", ClinicalSponsor = "" });
//                this.CPM_IntakeArr = _arrCPM_Intake;

//            }
//            else  //PRODUCTION
//            {
//                //POPULATE EMPTY OBJECT FOR PRODUCTION
//                this.CPM_Intake.FirstName = null ;
//                this.CPM_Intake.LastName = null;
//                this.CPM_Intake.Email = null;
//                this.CPM_Intake.Username = null;
//                this.CPM_Intake.DescriptionOfRequest = null;
//                this.CPM_Intake.SpecialtyArea = null;
//                this.CPM_Intake.BusinessArea = null;
//                this.CPM_Intake.BusinessPurpose = null;
//                this.CPM_Intake.BusinessValue = null;
//                this.CPM_Intake.BusinessValueNon = null;
//                this.CPM_Intake.LOB = null;
//                this.CPM_Intake.Market = null;
//                this.CPM_Intake.Timeframe = null;
//                this.CPM_Intake.ExpectedKickoffDate = null;
//                this.CPM_Intake.BusinessSponsor = null;
//                this.CPM_Intake.ClinicalSponsor = null;

//                //POPULATE ARRAY FROM TABLE FOR PRODUCTION
//                //MUST CONVERT repo.List<CPM_Intake_Model> to this.ObservableCollection<CPM_Intake_Model> ;
//                // _arrCPM_Intake = new ObservableCollection<CPM_Intake_Model>(await _repo.GetCPM_IntakeAsync() as List<CPM_Intake_Model>);
//                // this.CPM_IntakeArr = _arrCPM_Intake;
//                _arrCPM_Intake = new ObservableCollection<CPM_Intake_Model>();
//                _arrCPM_Intake.Add(this.CPM_Intake);
//                this.CPM_IntakeArr = _arrCPM_Intake;
//            }

//        }

//        //?????????????????????????????????????????????????
//        private bool _EditMode;
//        public bool EditMode //UPDATE OR INSERT!!!!!
//        {
//            get { return _EditMode; }
//            set { _EditMode = value; }
//        }

//        //VALIDATION PLACEHOLDER
//        public bool IsValid
//        {
//            get {

//                //REFRESH EXECUTION VALIDATION
//                DeleteCommand.RaiseCanExecuteChanged();
//                SaveCommand.RaiseCanExecuteChanged();
//                CancelCommand.RaiseCanExecuteChanged();

//                return this._objCPM_Intake.IsValid;
//            }
//        }

//        //public bool IsValid
//        //{
//        //    get { return this._objCPM_Intake.IsValid; }
//        //}

//        //DELETE COMMAND
//        private void OnDelete()
//        {
//            CPM_IntakeArr.Remove(CPM_Intake);
//        }

//        private bool CanDelete()
//        {
//            return CPM_Intake != null;
//        }

//        //SAVE COMMAND
//        private void OnCancel()
//        {
//            Done();
//        }

//        private async void OnSave()
//        {
//            UpdateCPM_Intake(_objCPM_Intake, _editingCPM_Intake);

//            if (EditMode) //UPDATE OR INSERT!!!!!
//                await _repo.UpdateCPM_IntakeAsync(_editingCPM_Intake);
//            else
//                await _repo.AddCPM_IntakeAsync(_editingCPM_Intake);

//            Done();
//        }

//        private void UpdateCPM_Intake(CPM_Intake_Model source, CPM_Intake_Model target)
//        {
//            target.FirstName = source.FirstName;
//            target.LastName = source.LastName;
//            target.Email= source.Email;
//            target.Username = source.Username;
//            target.DescriptionOfRequest = source.DescriptionOfRequest;
//            target.SpecialtyArea = source.SpecialtyArea;
//            target.BusinessArea = source.BusinessArea;
//            target.BusinessPurpose = source.BusinessPurpose;
//            target.BusinessValue = source.BusinessValue;
//            target.BusinessValueNon = source.BusinessValueNon;
//            target.LOB = source.LOB;
//            target.Market = source.Market;
//            target.Timeframe = source.Timeframe;
//            target.ExpectedKickoffDate = source.ExpectedKickoffDate;
//            target.BusinessSponsor = source.BusinessSponsor;
//            target.ClinicalSponsor = source.ClinicalSponsor;


//        }



//        private bool CanSave()
//        {
//            //return this._objCPM_Intake.IsValid;
//            return CPM_Intake.IsValid;
//        }


//        private void CopyCPM_Intake(CPM_Intake_Model source, CPM_Intake_Model target)
//        {
//            target.IntakeId = source.IntakeId;

//            if (EditMode)
//            {
//                target.FirstName = source.FirstName;
//                target.LastName = source.LastName;
//                target.Email = source.Email;
//                target.Username = source.Username;
//                target.DescriptionOfRequest = source.DescriptionOfRequest;
//                target.SpecialtyArea = source.SpecialtyArea;
//                target.BusinessArea = source.BusinessArea;
//                target.BusinessPurpose = source.BusinessPurpose;
//                target.BusinessValue = source.BusinessValue;
//                target.BusinessValueNon = source.BusinessValueNon;
//                target.LOB = source.LOB;
//                target.Market = source.Market;
//                target.Timeframe = source.Timeframe;
//                target.ExpectedKickoffDate = source.ExpectedKickoffDate;
//                target.BusinessSponsor = source.BusinessSponsor;
//                target.ClinicalSponsor = source.ClinicalSponsor;
//            }
//        }


//    }
//}
