//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;


//namespace UCS_Project_Manager
//{
//    [Table("ProjectIntakeSample1")]
//    public  class ProjectIntakeSample1_Model : ModelBase
//    {

//        public ProjectIntakeSample1_Model()
//        {
            
//        }

//        private Guid _guidIntakeId;
//        [Key]
//        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        [Column("guidIntakeId", TypeName = "UNIQUEIDENTIFIER")]
//        // Guid guidIntakeId { get; set; }
//        public Guid guidIntakeId
//        {
//            get {
//                //???COMMENTED TO FIX ViewModel: this.ProjectIntakeSample1Arr = new ObservableCollection<ProjectIntakeSample1_Model>(await _repo.GetProjectIntakeSample1Async() as List<ProjectIntakeSample1_Model>);
//                //this._guidIntakeId = Guid.NewGuid();
//                return this._guidIntakeId;
//            }
//            set { this._guidIntakeId = value; }
//        }


//        private string _strFirstName;
//        [Column("FirstName", TypeName = "VARCHAR")]
//        [StringLength(30)]
//        public string FirstName
//        {
//            get { return this._strFirstName; }
//            set
//            {
//                if (this._strFirstName != value || this._strFirstName == null)//ADDED '|| this._strGender == null' 5252021
//                {
//                    if (value == "abc")
//                        base.AddError("FirstName", "abc not allowed");
//                    else if(value == null || value == "")
//                        base.AddError("FirstName", "You must enter a firstname");
//                    else
//                        base.RemoveError("FirstName");

//                    this._strFirstName = value;
//                    base.NotifyPropertyChanged("FirstName", new Action<bool>((valid) => { AppMessages.ProjectChangeTracking.Send(valid); }));
//                    base.NotifyPropertyChanged("FullName", new Action<bool>((valid) => { AppMessages.ProjectChangeTracking.Send(valid); }));
//                }
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
//                if (this._strLastName != value || this._strLastName == null)//ADDED '|| this._strGender == null' 5252021
//                {
//                    if (value == "abc")
//                        base.AddError("LastName", "abc not allowed");
//                    else if (value == null || value == "")
//                        base.AddError("LastName", "You must enter a lastname");
//                    else
//                        base.RemoveError("LastName");

//                    this._strLastName = value;
//                    base.NotifyPropertyChanged("LastName", new Action<bool>((valid) => { AppMessages.ProjectChangeTracking.Send(valid); }));
//                    base.NotifyPropertyChanged("FullName", new Action<bool>((valid) => { AppMessages.ProjectChangeTracking.Send(valid); }));
//                }
//            }
//        }


//        public string FullName
//        {
//            get { return this._strFirstName + " " + this._strLastName; }

//        }


//        private int? _intAge;
//        //[NotMapped]
//        //[Column("Age", TypeName = "INT")]
//        public int? Age
//        {
//            get {

//                //return IntExtensions.TryParseNullable(this._strAge);
//                return this._intAge;

//            }
//            set
//            {
//                if (this._intAge != value  || this._intAge == null) //ADDED '|| this._strGender == null' 5252021
//                {
//                    if (value == null)
//                        base.AddError("Age", "You must enter an Age");
//                    else if (!value.ToString().IsNumeric())
//                        base.AddError("Age", "Only numerics are allowed");
//                    else
//                        base.RemoveError("Age");

//                    this._intAge = value ;
//                    base.NotifyPropertyChanged("Age", new Action<bool>((valid) => { AppMessages.ProjectChangeTracking.Send(valid); }));
//                }
//            }
//        }

//        //[Column("Age", TypeName = "INT")]
//        //private int? AgeDb
//        //{
//        //    get
//        //    {
//        //        return IntExtensions.TryParseNullable(this._strAge);
//        //        //return this._strAge;
//        //    }
//        //    set
//        //    {
//        //        this._strAge = value + "";
               
//        //    }
//        //}



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

//        private string _strGender;
//        [Column("Gender", TypeName = "VARCHAR")]
//        [StringLength(30)]
//        public string Gender
//        {
//            get { return this._strGender; }
//            set
//            {
//                if (this._strGender != value || this._strGender == null) //ADDED '|| this._strGender == null' 5252021
//                {
//                    if (value == "abc")
//                        base.AddError("Gender", "abc not allowed");
//                    if (value == null || value == "")
//                        base.AddError("Gender", "You must select a Gender");
//                    else
//                        base.RemoveError("Gender");

//                    this._strGender = value;
//                    base.NotifyPropertyChanged("Gender", new Action<bool>((valid) => { AppMessages.ProjectChangeTracking.Send(valid); }));
//                }

//            }
//        }

//        private bool? _blIsMember;
//        [Column("IsMember", TypeName = "BIT")]
//        public bool? IsMember
//        {
//            get { return this._blIsMember; }
//            set
//            {
//                if (this._blIsMember != value || this._blIsMember == null)
//                {
//                    if (value == null)
//                    {
//                        value = false;
//                        base.RemoveError("IsMember");
//                    }  
//                    //else  if (value == false)
//                    //    base.AddError("IsMember", "abc not allowed");
//                    else
//                        base.RemoveError("IsMember");

//                    this._blIsMember = value;
//                    base.NotifyPropertyChanged("IsMember", new Action<bool>((valid) => { AppMessages.ProjectChangeTracking.Send(valid); }));
//                }
//            }


//        }
//    }
//}
