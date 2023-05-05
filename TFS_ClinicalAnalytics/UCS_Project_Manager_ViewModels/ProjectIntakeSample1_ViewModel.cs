//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using GalaSoft.MvvmLight;
//using UCS_Project_Manager_Models;
//using UCS_Project_Manager_Services;

//namespace UCS_Project_Manager
//{
//    class ProjectIntakeSample1_ViewModel : ViewModelBase
//    {
//        //GLOBAL VARIABLES
//        public RelayCommand DeleteCommand { get; set; }
//        public RelayCommand CancelCommand { get; private set; }
//        public RelayCommand SaveCommand { get; private set; }
//        public event Action Done = delegate { };

//        private IProjectIntakeSample1_Repository _repo;

//        //CONSTRUCTOR
//        public ProjectIntakeSample1_ViewModel(IProjectIntakeSample1_Repository repo)
//        {
//            //REFGISTER MODEL FOR VALIDATION
//            AppMessages.ProjectChangeTracking.Register(this,
//                    new Action<bool>((arg) => { this.RaisePropertyChanged("IsValid"); }));

//            //INSTANTIATE OBJECTS
//            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
//            CancelCommand = new RelayCommand(OnCancel);
//            SaveCommand = new RelayCommand(OnSave, CanSave);
//            _repo = repo;

//            //this.ProjectIntakeSample1 = new ProjectIntakeSample1_Model();
//            //_objProjectIntakeSample1 = new ProjectIntakeSample1_Model();
//            //LOAD PROPERTIES
//            LoadProjectIntakeSample1();

//        }

//        //MAIN MODEL OBJECT
//        private ProjectIntakeSample1_Model _objProjectIntakeSample1;
//        private ProjectIntakeSample1_Model _editingProjectIntakeSample1 = null;
//        public ProjectIntakeSample1_Model ProjectIntakeSample1
//        {
//            get { return _objProjectIntakeSample1; }
//            set
//            {
//                _editingProjectIntakeSample1 = value;
//                //_objProjectIntakeSample1 = new ProjectIntakeSample1_Model();
//                //_editingProjectIntakeSample1 = new ProjectIntakeSample1_Model();
//                _objProjectIntakeSample1 = value;
//                //this.ProjectIntakeSample1 = value;
//                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Collection)));



//                //REFRESH EXECUTION VALIDATION
//                DeleteCommand.RaiseCanExecuteChanged();
//                SaveCommand.RaiseCanExecuteChanged();
//                CancelCommand.RaiseCanExecuteChanged();
                 
//            }
//        }


//        //MAIN MODEL ARRAY
//        private ObservableCollection<ProjectIntakeSample1_Model> _arrProjectIntakeSample1;
//        public ObservableCollection<ProjectIntakeSample1_Model> ProjectIntakeSample1Arr
//        {
//            get;
//            set;
//        }

//        //LOAD MODEL(S)
//        public async void LoadProjectIntakeSample1()
//        {
//            this.ProjectIntakeSample1 = new ProjectIntakeSample1_Model();

//            if (GlobalState.IsDesignMode)//TESTING
//            {
//                //POPULATE OBJECT FOR TESTING
//                this.ProjectIntakeSample1.FirstName = "Chris";
//                this.ProjectIntakeSample1.LastName = "Giordano";
//                this.ProjectIntakeSample1.Age = 49;
//                this.ProjectIntakeSample1.LOB = "Commercial";
//                this.ProjectIntakeSample1.Gender = "Male";
//                this.ProjectIntakeSample1.IsMember = true;

//                ////POPULATE ARRAY MANUALLY FOR TESTING
//                _arrProjectIntakeSample1 = new ObservableCollection<ProjectIntakeSample1_Model>();
//                _arrProjectIntakeSample1.Add(new ProjectIntakeSample1_Model { guidIntakeId = Guid.NewGuid(), FirstName = "abc", LastName = null, Age = 23, LOB = "Medicaid", Gender = "Male", IsMember = true });
//                _arrProjectIntakeSample1.Add(new ProjectIntakeSample1_Model { guidIntakeId = Guid.NewGuid(), FirstName = "Mark", LastName = "Allain", Age = 23, LOB = "Medicaid", Gender = "Male", IsMember = true });
//                _arrProjectIntakeSample1.Add(new ProjectIntakeSample1_Model { guidIntakeId = Guid.NewGuid(), FirstName = "Allen", LastName = "Brown", Age = 45, LOB = "Medicare", Gender = "Male", IsMember = false });
//                _arrProjectIntakeSample1.Add(new ProjectIntakeSample1_Model { guidIntakeId = Guid.NewGuid(), FirstName = "Linda", LastName = "Hamerski", Age = 32, LOB = "Commercial", Gender = "Female", IsMember = true });
//                this.ProjectIntakeSample1Arr = _arrProjectIntakeSample1;

//            }
//            else  //PRODUCTION
//            {
//                //POPULATE EMPTY OBJECT FOR PRODUCTION
//                this.ProjectIntakeSample1.FirstName = null;
//                this.ProjectIntakeSample1.LastName = null;
//                this.ProjectIntakeSample1.Age = null;
//                this.ProjectIntakeSample1.LOB = null;
//                this.ProjectIntakeSample1.Gender = null;
//                this.ProjectIntakeSample1.IsMember = false;

//                //POPULATE ARRAY FROM TABLE FOR PRODUCTION
//                //MUST CONVERT repo.List<ProjectIntakeSample1_Model> to this.ObservableCollection<ProjectIntakeSample1_Model> ;
//                _arrProjectIntakeSample1 = new ObservableCollection<ProjectIntakeSample1_Model>(await _repo.GetProjectIntakeSample1Async() as List<ProjectIntakeSample1_Model>);
//                this.ProjectIntakeSample1Arr = _arrProjectIntakeSample1;
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

//                return this._objProjectIntakeSample1.IsValid;
//            }
//        }

//        //public bool IsValid
//        //{
//        //    get { return this._objProjectIntakeSample1.IsValid; }
//        //}

//        //DELETE COMMAND
//        private void OnDelete()
//        {
//            ProjectIntakeSample1Arr.Remove(ProjectIntakeSample1);
//        }

//        private bool CanDelete()
//        {
//            return ProjectIntakeSample1 != null;
//        }

//        //SAVE COMMAND
//        private void OnCancel()
//        {
//            Done();
//        }

//        private async void OnSave()
//        {
//            UpdateProjectIntakeSample1(_objProjectIntakeSample1, _editingProjectIntakeSample1);

//            if (EditMode) //UPDATE OR INSERT!!!!!
//                await _repo.UpdateProjectIntakeSample1Async(_editingProjectIntakeSample1);
//            else
//                await _repo.AddProjectIntakeSample1Async(_editingProjectIntakeSample1);

//            Done();
//        }

//        private void UpdateProjectIntakeSample1(ProjectIntakeSample1_Model source, ProjectIntakeSample1_Model target)
//        {
//            target.FirstName = source.FirstName;
//            target.LastName = source.LastName;
//            target.LOB = source.LOB;
//            target.Age = source.Age;
//            target.IsMember = source.IsMember;
//        }



//        private bool CanSave()
//        {
//            //return this._objProjectIntakeSample1.IsValid;
//            return ProjectIntakeSample1.IsValid;
//        }


//        private void CopyProjectIntakeSample1(ProjectIntakeSample1_Model source, ProjectIntakeSample1_Model target)
//        {
//            target.guidIntakeId = source.guidIntakeId;

//            if (EditMode)
//            {
//                target.FirstName = source.FirstName;
//                target.LastName = source.LastName;
//                target.LOB = source.LOB;
//                target.Age = source.Age;
//                target.IsMember = source.IsMember;
//            }
//        }


//    }
//}
