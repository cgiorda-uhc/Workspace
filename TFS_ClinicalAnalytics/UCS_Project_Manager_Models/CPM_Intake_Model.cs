//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;


//namespace UCS_Project_Manager
//{
//    [Table("CPM_Intake")]
//    public  class CPM_Intake_Model : ModelBase
//    {

//        public CPM_Intake_Model()
//        {
            
//        }

//        private Int64 _intIntakeId;
//        [Key]
//        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        [Column("IntakeId", TypeName = "BIGINT")]
//        // Guid guidIntakeId { get; set; }
//        public Int64 IntakeId
//        {
//            get {
//                //???COMMENTED TO FIX ViewModel: this.CPM_IntakeArr = new ObservableCollection<CPM_Intake_Model>(await _repo.GetCPM_IntakeAsync() as List<CPM_Intake_Model>);
//                //this._guidIntakeId = Guid.NewGuid();
//                return this._intIntakeId;
//            }
//            set { this._intIntakeId = value; }
//        }


//        private string _strFirstName;
//        [Column("FirstName", TypeName = "VARCHAR")]
//        [StringLength(30)]
//        public string FirstName
//        {
//            get { return this._strFirstName; }
//            set
//            {
//                this._strFirstName = value;

//                //if (this._strFirstName != value || this._strFirstName == null)//ADDED '|| this._strGender == null' 5252021
//                //{
//                //    if (value == "abc")
//                //        base.AddError("FirstName", "abc not allowed");
//                //    else if(value == null || value == "")
//                //        base.AddError("FirstName", "You must enter a firstname");
//                //    else
//                //        base.RemoveError("FirstName");

//                //    this._strFirstName = value;
//                //    base.NotifyPropertyChanged("FirstName", new Action<bool>((valid) => { AppMessages.ProjectIsValid.Send(valid); }));
//                //    base.NotifyPropertyChanged("FullName", new Action<bool>((valid) => { AppMessages.ProjectIsValid.Send(valid); }));
//                //}
//            }
//        }

//        private string _strLastName;
//        [Column("LastName", TypeName = "VARCHAR")]
//        [StringLength(30)]
//        public string LastName
//        {
//            get { return this._strLastName; }
//            set
//            {
//                this._strLastName = value;

//                //if (this._strLastName != value || this._strLastName == null)//ADDED '|| this._strGender == null' 5252021
//                //{
//                //    if (value == "abc")
//                //        base.AddError("LastName", "abc not allowed");
//                //    else if (value == null || value == "")
//                //        base.AddError("LastName", "You must enter a lastname");
//                //    else
//                //        base.RemoveError("LastName");

//                //    this._strLastName = value;
//                //    base.NotifyPropertyChanged("LastName", new Action<bool>((valid) => { AppMessages.ProjectIsValid.Send(valid); }));
//                //    base.NotifyPropertyChanged("FullName", new Action<bool>((valid) => { AppMessages.ProjectIsValid.Send(valid); }));
//                //}
//            }
//        }



//        private string _strEmail;
//        [Column("Email", TypeName = "VARCHAR")]
//        [StringLength(50)]
//        public string Email
//        {
//            get { return this._strEmail; }
//            set
//            {
//                this._strEmail = value;
//                //if (this._strEmail != value || this._strEmail == null)//ADDED '|| this._strGender == null' 5252021
//                //{
//                //    if (value == "abc")
//                //        base.AddError("Email", "abc not allowed");
//                //    else if (value == null || value == "")
//                //        base.AddError("Email", "You must enter a lastname");
//                //    else
//                //        base.RemoveError("Email");

//                //    this._strEmail = value;
//                //    base.NotifyPropertyChanged("Email", new Action<bool>((valid) => { AppMessages.ProjectIsValid.Send(valid); }));
//                //}
//            }
//        }

//        private string _strUsername;
//        [Column("Username", TypeName = "VARCHAR")]
//        [StringLength(10)]
//        public string Username
//        {
//            get { return this._strUsername; }
//            set
//            {
//                this._strUsername = value;


//                //if (this._strUsername != value || this._strUsername == null)//ADDED '|| this._strGender == null' 5252021
//                //{
//                //    if (value == "abc")
//                //        base.AddError("Username", "abc not allowed");
//                //    else if (value == null || value == "")
//                //        base.AddError("Username", "You must enter a lastname");
//                //    else
//                //        base.RemoveError("Username");

//                //    this._strUsername = value;
//                //    base.NotifyPropertyChanged("Username", new Action<bool>((valid) => { AppMessages.ProjectIsValid.Send(valid); }));
//                //}
//            }
//        }


//        public string FullName
//        {
//            get { return this._strFirstName + " " + this._strLastName; }

//        }


//        private string _strDescriptionOfRequest;
//        [Column("DescriptionOfRequest", TypeName = "VARCHAR")]
//        [StringLength(30)]
//        public string DescriptionOfRequest
//        {
//            get { return this._strDescriptionOfRequest; }
//            set
//            {
//                if (this._strDescriptionOfRequest != value || this._strDescriptionOfRequest == null)//ADDED '|| this._strGender == null' 5252021
//                {
//                    if (value == "abc")
//                        base.AddError("DescriptionOfRequest", "abc not allowed");
//                    else if (value == null || value == "")
//                        base.AddError("DescriptionOfRequest", "You must enter a DescriptionOfRequest");
//                    else
//                        base.RemoveError("DescriptionOfRequest");

//                    this._strDescriptionOfRequest = value;
//                    base.NotifyPropertyChanged("DescriptionOfRequest", new Action<bool>((valid) => { AppMessages.ProjectChangeTracking.Send(valid); }));
//                }
//            }
//        }

//        private string _strSpecialtyArea;
//        [Column("SpecialtyArea", TypeName = "VARCHAR")]
//        [StringLength(30)]
//        public string SpecialtyArea
//        {
//            get { return this._strSpecialtyArea; }
//            set
//            {
//                if (this._strSpecialtyArea != value || this._strSpecialtyArea == null)//ADDED '|| this._strGender == null' 5252021
//                {
//                    if (value == "abc")
//                        base.AddError("SpecialtyArea", "abc not allowed");
//                    else if (value == null || value == "")
//                        base.AddError("SpecialtyArea", "You must enter a SpecialtyArea");
//                    else
//                        base.RemoveError("SpecialtyArea");

//                    this._strSpecialtyArea = value;
//                    base.NotifyPropertyChanged("SpecialtyArea", new Action<bool>((valid) => { AppMessages.ProjectChangeTracking.Send(valid); }));
//                }
//            }
//        }



//        private string _strBusinessArea;
//        [Column("BusinessArea", TypeName = "VARCHAR")]
//        [StringLength(30)]
//        public string BusinessArea
//        {
//            get { return this._strBusinessArea; }
//            set
//            {
//                if (this._strBusinessArea != value || this._strBusinessArea == null)//ADDED '|| this._strGender == null' 5252021
//                {
//                    if (value == "abc")
//                        base.AddError("BusinessArea", "abc not allowed");
//                    else if (value == null || value == "")
//                        base.AddError("BusinessArea", "You must enter a BusinessArea");
//                    else
//                        base.RemoveError("BusinessArea");

//                    this._strBusinessArea = value;
//                    base.NotifyPropertyChanged("BusinessArea", new Action<bool>((valid) => { AppMessages.ProjectChangeTracking.Send(valid); }));
//                }
//            }
//        }

//        private string _strBusinessPurpose;
//        [Column("BusinessPurpose", TypeName = "VARCHAR")]
//        [StringLength(30)]
//        public string BusinessPurpose
//        {
//            get { return this._strBusinessPurpose; }
//            set
//            {
//                if (this._strBusinessPurpose != value || this._strBusinessPurpose == null)//ADDED '|| this._strGender == null' 5252021
//                {
//                    if (value == "abc")
//                        base.AddError("BusinessPurpose", "abc not allowed");
//                    else if (value == null || value == "")
//                        base.AddError("BusinessPurpose", "You must enter a BusinessPurpose");
//                    else
//                        base.RemoveError("BusinessPurpose");

//                    this._strBusinessPurpose = value;
//                    base.NotifyPropertyChanged("BusinessPurpose", new Action<bool>((valid) => { AppMessages.ProjectChangeTracking.Send(valid); }));
//                }
//            }
//        }


//        private string _strBusinessValue;
//        [Column("BusinessValue", TypeName = "VARCHAR")]
//        [StringLength(30)]
//        public string BusinessValue
//        {
//            get { return this._strBusinessValue; }
//            set
//            {
//                if (this._strBusinessValue != value || this._strBusinessValue == null)//ADDED '|| this._strGender == null' 5252021
//                {
//                    if (value == "abc")
//                        base.AddError("BusinessValue", "abc not allowed");
//                    else if (value == null || value == "")
//                        base.AddError("BusinessValue", "You must enter a BusinessValue");
//                    else
//                        base.RemoveError("BusinessValue");

//                    this._strBusinessValue = value;
//                    base.NotifyPropertyChanged("BusinessValue", new Action<bool>((valid) => { AppMessages.ProjectChangeTracking.Send(valid); }));
//                }
//            }
//        }

//        private string _strBusinessValueNon;
//        [Column("BusinessValueNon", TypeName = "VARCHAR")]
//        [StringLength(30)]
//        public string BusinessValueNon
//        {
//            get { return this._strBusinessValueNon; }
//            set
//            {
//                if (this._strBusinessValueNon != value || this._strBusinessValueNon == null)//ADDED '|| this._strGender == null' 5252021
//                {
//                    if (value == "abc")
//                        base.AddError("BusinessValueNon", "abc not allowed");
//                    else if (value == null || value == "")
//                        base.AddError("BusinessValueNon", "You must enter a BusinessValueNon");
//                    else
//                        base.RemoveError("BusinessValueNon");

//                    this._strBusinessValueNon = value;
//                    base.NotifyPropertyChanged("BusinessValueNon", new Action<bool>((valid) => { AppMessages.ProjectChangeTracking.Send(valid); }));
//                }
//            }
//        }


//        private string _strLOB;
//        [Column("LOB", TypeName = "VARCHAR")]
//        [StringLength(30)]
//        public string LOB
//        {
//            get { return this._strLOB; }
//            set
//            {
//                if (this._strLOB != value || this._strLOB == null) //ADDED '|| this._strGender == null' 5252021
//                {
//                     if (value == null || value == "")
//                        base.AddError("LOB", "You choose an LOB");
//                    else
//                        base.RemoveError("LOB");

//                    this._strLOB = value;
//                    base.NotifyPropertyChanged("LOB", new Action<bool>((valid) => { AppMessages.ProjectChangeTracking.Send(valid); }));
//                }
//            }
//        }


//        private string _strMarket;
//        [Column("Market", TypeName = "VARCHAR")]
//        [StringLength(30)]
//        public string Market
//        {
//            get { return this._strMarket; }
//            set
//            {
//                if (this._strMarket != value || this._strMarket == null) //ADDED '|| this._strGender == null' 5252021
//                {
//                    if (value == null || value == "")
//                        base.AddError("Market", "You choose an Market");
//                    else
//                        base.RemoveError("Market");

//                    this._strMarket = value;
//                    base.NotifyPropertyChanged("Market", new Action<bool>((valid) => { AppMessages.ProjectChangeTracking.Send(valid); }));
//                }
//            }
//        }

//        private string _strTimeframe;
//        [Column("Timeframe", TypeName = "VARCHAR")]
//        [StringLength(30)]
//        public string Timeframe
//        {
//            get { return this._strTimeframe; }
//            set
//            {
//                if (this._strTimeframe != value || this._strTimeframe == null) //ADDED '|| this._strGender == null' 5252021
//                {
//                    if (value == null || value == "")
//                        base.AddError("Timeframe", "You choose an Timeframe");
//                    else
//                        base.RemoveError("Timeframe");

//                    this._strTimeframe = value;
//                    base.NotifyPropertyChanged("Timeframe", new Action<bool>((valid) => { AppMessages.ProjectChangeTracking.Send(valid); }));
//                }
//            }
//        }



//        private string _strExpectedKickoffDate;
//        [Column("ExpectedKickoffDate", TypeName = "VARCHAR")]
//        [StringLength(30)]
//        public string ExpectedKickoffDate
//        {
//            get { return this._strExpectedKickoffDate; }
//            set
//            {
//                if (this._strExpectedKickoffDate != value || this._strExpectedKickoffDate == null) //ADDED '|| this._strGender == null' 5252021
//                {
//                    if (value == null || value == "")
//                        base.AddError("ExpectedKickoffDate", "You choose an ExpectedKickoffDate");
//                    else
//                        base.RemoveError("ExpectedKickoffDate");

//                    this._strExpectedKickoffDate = value;
//                    base.NotifyPropertyChanged("ExpectedKickoffDate", new Action<bool>((valid) => { AppMessages.ProjectChangeTracking.Send(valid); }));
//                }
//            }
//        }





//        private string _strBusinessSponsor;
//        [Column("BusinessSponsor", TypeName = "VARCHAR")]
//        [StringLength(30)]
//        public string BusinessSponsor
//        {
//            get { return this._strBusinessSponsor; }
//            set
//            {
//                if (this._strBusinessSponsor != value || this._strBusinessSponsor == null) //ADDED '|| this._strBusinessSponsor == null' 5252021
//                {
//                    if (value == "abc")
//                        base.AddError("BusinessSponsor", "abc not allowed");
//                    if (value == null || value == "")
//                        base.AddError("BusinessSponsor", "You must select a BusinessSponsor");
//                    else
//                        base.RemoveError("BusinessSponsor");

//                    this._strBusinessSponsor = value;
//                    base.NotifyPropertyChanged("BusinessSponsor", new Action<bool>((valid) => { AppMessages.ProjectChangeTracking.Send(valid); }));
//                }

//            }
//        }

//        private string _strClinicalSponsor;
//        [Column("ClinicalSponsor", TypeName = "VARCHAR")]
//        [StringLength(30)]
//        public string ClinicalSponsor
//        {
//            get { return this._strClinicalSponsor; }
//            set
//            {
//                if (this._strClinicalSponsor != value || this._strClinicalSponsor == null) //ADDED '|| this._strClinicalSponsor == null' 5252021
//                {
//                    if (value == "abc")
//                        base.AddError("ClinicalSponsor", "abc not allowed");
//                    if (value == null || value == "")
//                        base.AddError("ClinicalSponsor", "You must select a ClinicalSponsor");
//                    else
//                        base.RemoveError("ClinicalSponsor");

//                    this._strClinicalSponsor = value;
//                    base.NotifyPropertyChanged("ClinicalSponsor", new Action<bool>((valid) => { AppMessages.ProjectChangeTracking.Send(valid); }));
//                }

//            }
//        }
//    }

//}
