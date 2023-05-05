using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCS_Project_Manager
{
    [Table("ETG_Fact_Symmetry")]
    public class ETG_Fact_Symmetry_Model : ModelBase
    {
        public ETG_Fact_Symmetry_Model()
        {

        }

        private Int64 _intETGFactSymmetryid;
        [Key]
        [Column("ETG_Fact_Symmetry_id", TypeName = "BIGINT")]
        public Int64 ETG_Fact_Symmetry_id
        {
            get { return this._intETGFactSymmetryid; }
            set { this._intETGFactSymmetryid = value; }
        }

        private string _strETGBaseClass;
        [Column("ETG_Base_Class", TypeName = "VARCHAR")]
        [StringLength(15)]
        public string ETG_Base_Class
        {
            get { return this._strETGBaseClass; }
            set { this._strETGBaseClass = value; }
        }
        [ForeignKey(nameof(ETG_Base_Class))]
        public virtual ETG_Dim_Master_Model ETG_Dim_Master_Model { get; set; }






        private double _dblSymmetry_Version;
        [Column("Symmetry_Version", TypeName = "FLOAT")]
        public double Symmetry_Version
        {
            get { return this._dblSymmetry_Version; }
            set { this._dblSymmetry_Version = value; }
        }





        private Int16 _intPremiumSpecialtyId;
        [Column("Premium_Specialty_id", TypeName = "SMALLINT")]
        public Int16 Premium_Specialty_id
        {
            get { return this._intPremiumSpecialtyId; }
            set { this._intPremiumSpecialtyId = value; }
        }
        [ForeignKey(nameof(Premium_Specialty_id))]
        public virtual ETG_Dim_Premium_Spec_Master_Model ETG_Dim_Premium_Spec_Master_Model { get; set; }



        private string _strETGPopCostTreatmentIndicator;
        [Column("Pop_Cost_Treatment_Indicator", TypeName = "VARCHAR")]
        [StringLength(10)]
        public string Pop_Cost_Treatment_Indicator
        {
            get { return this._strETGPopCostTreatmentIndicator; }
            set { this._strETGPopCostTreatmentIndicator = value; }
        }


        private double? _dblPop_Cost_Episode_Count_Commercial_Only;
        [Column("Pop_Cost_Episode_Count_Commercial_Only", TypeName = "FLOAT")]
        public double? Pop_Cost_Episode_Count_Commercial_Only
        {
            get { return this._dblPop_Cost_Episode_Count_Commercial_Only; }
            set { this._dblPop_Cost_Episode_Count_Commercial_Only = value; }
        }

        private double? _dblTotal_Cost_Commercial_Only;
        [Column("Total_Cost_Commercial_Only", TypeName = "FLOAT")]
        public double? Total_Cost_Commercial_Only
        {
            get { return this._dblTotal_Cost_Commercial_Only; }
            set { this._dblTotal_Cost_Commercial_Only = value; }
        }

        private double? _dblAverage_Cost_Commercial_Only;
        [Column("Average_Cost_Commercial_Only", TypeName = "FLOAT")]
        public double? Average_Cost_Commercial_Only
        {
            get { return this._dblAverage_Cost_Commercial_Only; }
            set { this._dblAverage_Cost_Commercial_Only = value; }
        }

        private double? _dblCoefficients_of_Variation_Commercial_Only;
        [Column("Coefficients_of_Variation_Commercial_Only", TypeName = "FLOAT")]
        public double? Coefficients_of_Variation_Commercial_Only
        {
            get { return this._dblCoefficients_of_Variation_Commercial_Only; }
            set { this._dblCoefficients_of_Variation_Commercial_Only = value; }
        }

        private double? _dblNormalized_Pricing_Episode_Count;
        [Column("Normalized_Pricing_Episode_Count", TypeName = "FLOAT")]
        public double? Normalized_Pricing_Episode_Count
        {
            get { return this._dblNormalized_Pricing_Episode_Count; }
            set { this._dblNormalized_Pricing_Episode_Count = value; }
        }

        private double? _dblNormalized_Pricing_Total_Cost;
        [Column("Normalized_Pricing_Total_Cost", TypeName = "FLOAT")]
        public double? Normalized_Pricing_Total_Cost
        {
            get { return this._dblNormalized_Pricing_Total_Cost; }
            set { this._dblNormalized_Pricing_Total_Cost = value; }
        }

        private double? _dblSpecialist_Episode_Count;
        [Column("Specialist_Episode_Count", TypeName = "FLOAT")]
        public double? Specialist_Episode_Count
        {
            get { return this._dblSpecialist_Episode_Count; }
            set { this._dblSpecialist_Episode_Count = value; }
        }

        private double? _dblEpisode_Count;
        [Column("Episode_Count", TypeName = "FLOAT")]
        public double? Episode_Count
        {
            get { return this._dblEpisode_Count; }
            set { this._dblEpisode_Count = value; }
        }

        private string _strAttribution;
        [Column("Attribution", TypeName = "VARCHAR")]
        [StringLength(25)]
        public string Attribution
        {
            get { return this._strAttribution; }
            set { this._strAttribution = value; }
        }


        private string _strPopCostChangeComments;
        [Column("Pop_Cost_Change_Comments", TypeName = "NTEXT")]
        public string Pop_Cost_Change_Comments
        {
            get { return this._strPopCostChangeComments; }
            set { this._strPopCostChangeComments = value; }
        }

        private string _strEpisodeCostTreatmentIndicator;
        [Column("Episode_Cost_Treatment_Indicator", TypeName = "VARCHAR")]
        [StringLength(10)]
        public string Episode_Cost_Treatment_Indicator
        {
            get { return this._strEpisodeCostTreatmentIndicator; }
            set { this._strEpisodeCostTreatmentIndicator = value; }
        }



        private string _strCurrentMapping;
        [Column("Mapping", TypeName = "VARCHAR")]
        [StringLength(50)]
        public string Mapping
        {
            get { return this._strCurrentMapping; }
            set { this._strCurrentMapping = value; }
        }


        private string _strEpisodeCostChangeComments;
        [Column("Episode_Cost_Change_Comments", TypeName = "NTEXT")]
        public string Episode_Cost_Change_Comments
        {
            get { return this._strEpisodeCostChangeComments; }
            set { this._strEpisodeCostChangeComments = value; }
        }

        private string _strPatientCentricMapping;
        [Column("Patient_Centric_Mapping", TypeName = "VARCHAR")]
        [StringLength(25)]
        public string Patient_Centric_Mapping
        {
            get { return this._strPatientCentricMapping; }
            set { this._strPatientCentricMapping = value; }
        }



        private string _strPatientCentricChangeComments;
        [Column("Patient_Centric_Change_Comments", TypeName = "NTEXT")]
        public string PatientCentricChangeComments
        {
            get { return this._strPatientCentricChangeComments; }
            set { this._strPatientCentricChangeComments = value; }
        }


        private string _strData_Period;
        [Column("Data_Period", TypeName = "VARCHAR")]
        [StringLength(15)]
        public string Data_Period
        {
            get { return this._strData_Period; }
            set { this._strData_Period = value; }
        }

        private DateTime _dtData_Date;
        [Column("Data_Date", TypeName = "Date")]
        public DateTime Data_Date
        {
            get { return this._dtData_Date; }
            set { this._dtData_Date = value; }
        }

        private bool? _blHasCommercial;
        [Column("has_Commercial", TypeName = "BIT")]
        public bool? has_Commercial
        {
            get { return this._blHasCommercial; }
            set { this._blHasCommercial = value; }
        }

        private bool? _blHasMedicare;
        [Column("has_Medicare", TypeName = "BIT")]
        public bool? has_Medicare
        {
            get { return this._blHasMedicare; }
            set { this._blHasMedicare = value; }
        }

        private bool? _blHasMedicaid;
        [Column("has_Medicaid", TypeName = "BIT")]
        public bool? has_Medicaid
        {
            get { return this._blHasMedicaid; }
            set { this._blHasMedicaid = value; }
        }

        private string _strLOBString;
        [NotMapped]
        public string LOBString
        {
            get
            {

                if (has_Commercial == null && has_Medicare == null && has_Medicaid == null)
                    _strLOBString = "Not Selected";
                else if (has_Commercial == true && has_Medicare == true && has_Medicaid == true)
                    _strLOBString = "All";
                else if (has_Commercial == true && has_Medicare == true)
                    _strLOBString = "Commercial + Medicare";
                else if (has_Commercial == true && has_Medicaid == true)
                    _strLOBString = "Commercial + Medicaid";
                else if (has_Medicare == true && has_Medicaid == true)
                    _strLOBString = "Medicare + Medicaid";
                else if (has_Commercial == true)
                    _strLOBString = "Commercial Only";
                else if (has_Medicare == true)
                    _strLOBString = "Medicare Only";
                else if (has_Medicaid == true)
                    _strLOBString = "Medicaid Only";
                else
                    _strLOBString = "Not Selected";


                return _strLOBString;
            }
            set
            {
                this._strLOBString = value;

                if (_strLOBString == "Not Selected")
                {
                    this.has_Commercial = null; this.has_Medicare = null; this.has_Medicaid = null;
                }
                else if (_strLOBString == "All")
                {
                    this.has_Commercial = true; this.has_Medicare = true; this.has_Medicaid = true;
                }
                else if (_strLOBString == "Commercial + Medicare")
                {
                    this.has_Commercial = true; this.has_Medicare = true; this.has_Medicaid = false;
                }
                else if (_strLOBString == "Commercial + Medicaid")
                {
                    this.has_Commercial = true; this.has_Medicare = false; this.has_Medicaid = true;
                }
                else if (_strLOBString == "Medicare + Medicaid")
                {
                    this.has_Commercial = false; this.has_Medicare = true; this.has_Medicaid = true;
                }
                else if (_strLOBString == "Commercial Only")
                {
                    this.has_Commercial = true; this.has_Medicare = false; this.has_Medicaid = false;
                }
                else if (_strLOBString == "Medicare Only")
                {
                    this.has_Commercial = false; this.has_Medicare = true; this.has_Medicaid = false;
                }
                else if (_strLOBString == "Medicaid Only")
                {
                    this.has_Commercial = false; this.has_Medicare = false; this.has_Medicaid = true;
                }
                else
                {
                    this.has_Commercial = null; this.has_Medicare = null; this.has_Medicaid = null;
                }

            }
        }


        //[ForeignKey("ETG_Fact_Symmetry_id")]
        //public virtual ICollection<ETG_Fact_Symmetry_LOB_Model> ETG_Fact_Symmetry_LOBs { get; set; }

    }



    public class ETG_Fact_Symmetry_Interface_Model : ModelBase
    {
        public ETG_Fact_Symmetry_Interface_Model()
        {

        }

        private Int64 _intETGFactSymmetryid;
        public Int64 ETG_Fact_Symmetry_id
        {
            get { return this._intETGFactSymmetryid; }
            set { this._intETGFactSymmetryid = value; }
        }

        private Int64 _intETGFactSymmetryidPrevious;
        public Int64 ETG_Fact_Symmetry_id_Previous
        {
            get { return this._intETGFactSymmetryidPrevious; }
            set { this._intETGFactSymmetryidPrevious = value; }
        }




        private string _strETGBaseClass;
        public string ETG_Base_Class
        {
            get { return this._strETGBaseClass; }
            set { this._strETGBaseClass = value; }
        }

        private string _strETGDescription;
        public string ETG_Description
        {
            get { return this._strETGDescription; }
            set { this._strETGDescription = value; }
        }

        private Int16 _intPremiumSpecialtyId;
        public Int16 Premium_Specialty_id
        {
            get { return this._intPremiumSpecialtyId; }
            set { this._intPremiumSpecialtyId = value; }
        }

        private string _strPremiumSpecialty;
        public string Premium_Specialty
        {
            get { return this._strPremiumSpecialty; }
            set { this._strPremiumSpecialty = value; }
        }



        private string _strETGCurrentPopCostTreatmentIndicator;
        public string Pop_Cost_Current_Treatment_Indicator
        {
            get { return this._strETGCurrentPopCostTreatmentIndicator; }
            set { this._strETGPreviousPopCostTreatmentIndicator = this._strETGCurrentPopCostTreatmentIndicator; this._strETGCurrentPopCostTreatmentIndicator = value; notifyUpdates(); }
        }


        private double? _dblPop_Cost_Episode_Count_Commercial_Only;
        public double? Pop_Cost_Episode_Count_Commercial_Only
        {
            get { return this._dblPop_Cost_Episode_Count_Commercial_Only; }
            set { this._dblPop_Cost_Episode_Count_Commercial_Only = value; }
        }

        private double? _dblTotal_Cost_Commercial_Only;
        public double? Total_Cost_Commercial_Only
        {
            get { return this._dblTotal_Cost_Commercial_Only; }
            set { this._dblTotal_Cost_Commercial_Only = value; }
        }

        private double? _dblAverage_Cost_Commercial_Only;
        public double? Average_Cost_Commercial_Only
        {
            get { return this._dblAverage_Cost_Commercial_Only; }
            set { this._dblAverage_Cost_Commercial_Only = value; }
        }

        private double? _dblCoefficients_of_Variation_Commercial_Only;
        public double? Coefficients_of_Variation_Commercial_Only
        {
            get { return this._dblCoefficients_of_Variation_Commercial_Only; }
            set { this._dblCoefficients_of_Variation_Commercial_Only = value; }
        }

        private double? _dblNormalized_Pricing_Episode_Count;
        public double? Normalized_Pricing_Episode_Count
        {
            get { return this._dblNormalized_Pricing_Episode_Count; }
            set { this._dblNormalized_Pricing_Episode_Count = value; }
        }

        private double? _dblNormalized_Pricing_Total_Cost;
        public double? Normalized_Pricing_Total_Cost
        {
            get { return this._dblNormalized_Pricing_Total_Cost; }
            set { this._dblNormalized_Pricing_Total_Cost = value; }
        }

        private double? _dblSpecialist_Episode_Count;
        public double? Specialist_Episode_Count
        {
            get { return this._dblSpecialist_Episode_Count; }
            set { this._dblSpecialist_Episode_Count = value; }
        }

        private double? _dblEpisode_Count;
        public double? Episode_Count
        {
            get { return this._dblEpisode_Count; }
            set { this._dblEpisode_Count = value; }
        }

        private string _strETGPreviousPopCostTreatmentIndicator;
        public string Pop_Cost_Previous_Treatment_Indicator
        {
            get { return this._strETGPreviousPopCostTreatmentIndicator; }
            set { this._strETGPreviousPopCostTreatmentIndicator = value; }
        }

        private string _strCurrentAttribution;
        public string Current_Attribution
        {
            get { return this._strCurrentAttribution; }
            set { this._strPreviousAttribution = this._strCurrentAttribution; this._strCurrentAttribution = value; notifyUpdates(); }
        }

        private string _strPreviousAttribution;
        public string Previous_Attribution
        {
            get { return this._strPreviousAttribution; }
            set { this._strPreviousAttribution = value; }
        }

        private string _strPopCostChangeComments;
        public string Pop_Cost_Change_Comments
        {
            get { return this._strPopCostChangeComments; }
            set { this._strPopCostChangeComments = value; notifyUpdates(); }
        }

        private string _strCurrentEpisodeCostTreatmentIndicator;
        public string Current_Episode_Cost_Treatment_Indicator
        {
            get { return this._strCurrentEpisodeCostTreatmentIndicator; }
            set { this._strPreviousEpisodeCostTreatmentIndicator = this._strCurrentEpisodeCostTreatmentIndicator; this._strCurrentEpisodeCostTreatmentIndicator = value; notifyUpdates(); }
        }

        private string _strPreviousEpisodeCostTreatmentIndicator;
        public string Previous_Episode_Cost_Treatment_Indicator
        {
            get { return this._strPreviousEpisodeCostTreatmentIndicator; }
            set { this._strPreviousEpisodeCostTreatmentIndicator = value; }
        }

        private string _strCurrentMapping;
        public string Current_Mapping
        {
            get { return this._strCurrentMapping; }
            set { this._strPreviousMapping = this._strCurrentMapping; this._strCurrentMapping = value; notifyUpdates(); }
        }
        private string _strPreviousMapping;
        public string Previous_Mapping
        {
            get { return this._strPreviousMapping; }
            set { this._strPreviousMapping = value; notifyUpdates(); }
        }



        private string _strCurrentMappingOriginal;
        public string Current_Mapping_Original
        {
            get { return this._strCurrentMappingOriginal; }
            set { this._strCurrentMappingOriginal = value; }
        }
        private string _strPreviousMappingOriginal;
        public string Previous_Mapping_Original
        {
            get { return this._strPreviousMappingOriginal; }
            set { this._strPreviousMappingOriginal = value; }
        }





        private string _strEpisodeCostChangeComments;
        public string Episode_Cost_Change_Comments
        {
            get { return this._strEpisodeCostChangeComments; }
            set { this._strEpisodeCostChangeComments = value; notifyUpdates(); }
        }

        private string _strPatientCentricChangeComments;
        public string Patient_Centric_Change_Comments
        {
            get { return this._strPatientCentricChangeComments; }
            set { this._strPatientCentricChangeComments = value; notifyUpdates(); }
        }



        private string _strCurrentPatientCentricMapping;
        public string Current_Patient_Centric_Mapping
        {
            get { return this._strCurrentPatientCentricMapping; }
            set { this._strPreviousPatientCentricMapping = this._strCurrentPatientCentricMapping; this._strCurrentPatientCentricMapping = value; notifyUpdates(); }
        }

        private string _strHas_nrx;
        public string has_nrx
        {
            get { return this._strHas_nrx; }
            set { this._strHas_nrx = value; }
        }


        private string _strHas_rx;
        public string has_rx
        {
            get { return this._strHas_rx; }
            set { this._strHas_rx = value; }
        }




        private string _strPreviousPatientCentricMapping;
        public string Previous_Patient_Centric_Mapping
        {
            get { return this._strPreviousPatientCentricMapping; }
            set { this._strPreviousPatientCentricMapping = value; }
        }
        private string _strData_Period;
        public string Data_Period
        {
            get { return this._strData_Period; }
            set { this._strData_Period = value; }
        }

        private DateTime _dtData_Date;
        public DateTime Data_Date
        {
            get { return this._dtData_Date; }
            set { this._dtData_Date = value; }
        }


        private double _dblSymmetry_Version;
        public double Symmetry_Version
        {
            get { return this._dblSymmetry_Version; }
            set { this._dblSymmetry_Version = value; }
        }





        private string _strLOBCurrentString;

        public string LOBCurrentString
        {
            get
            {
                return _strLOBCurrentString;
            }
            set
            {

                //base.NotifyPropertyChanged("LOBCurrentString", new Action<bool>((valid) => { AppMessages.ProjectIsValid.Send(valid); }));
                //base.NotifyPropertyChanged("LOBCurrentString", new Action<string>((valid) => { AppMessages.ProjectChangeTracking.Send("LOBCurrentString"); }));
                this._strLOBPreviousString = this._strLOBCurrentString;
                this._strLOBCurrentString = value;
                notifyUpdates();

            }
        }


        private string _strLOBPreviousString;
        public string LOBPreviousString
        {
            get { return this._strLOBPreviousString; }
            set { this._strLOBPreviousString = value; }
        }



        private string _strIs_Config;
        public string Is_Config
        {
            get { return this._strIs_Config; }
            set { this._strIs_Config = value; }
        }



        private string _strMeasure_Status;
        public string Measure_Status
        {
            get { return this._strMeasure_Status; }
            set { this._strMeasure_Status = value; }
        }



        private bool _blHasChanged;
        public bool HasChanged
        {
            get { return this._blHasChanged; }
            set { this._blHasChanged = value; }
        }

        private void notifyUpdates()
        {
            ETG_Fact_Symmetry_Update_Tracker am = new ETG_Fact_Symmetry_Update_Tracker() { ETG_Fact_Symmetry_id = this._intETGFactSymmetryid, ETG_Fact_Symmetry_id_Previous = this._intETGFactSymmetryidPrevious, Current_Patient_Centric_Mapping = this._strCurrentPatientCentricMapping, Previous_Patient_Centric_Mapping = this._strPreviousPatientCentricMapping, Current_Mapping = this._strCurrentMapping, Previous_Mapping = this._strPreviousMapping, Current_Episode_Cost_Treatment_Indicator = this._strCurrentEpisodeCostTreatmentIndicator, Previous_Episode_Cost_Treatment_Indicator = this._strPreviousEpisodeCostTreatmentIndicator, Current_Attribution = this._strCurrentAttribution, Previous_Attribution = this._strPreviousAttribution, Pop_Cost_Current_Treatment_Indicator = this._strETGCurrentPopCostTreatmentIndicator, Pop_Cost_Previous_Treatment_Indicator = this._strETGPreviousPopCostTreatmentIndicator, LOBCurrentString = this._strLOBCurrentString, LOBPreviousString = this._strLOBPreviousString, Pop_Cost_Change_Comments = this._strPopCostChangeComments, Episode_Cost_Change_Comments = this._strEpisodeCostChangeComments, Patient_Centric_Change_Comments = this._strPatientCentricChangeComments, Current_Mapping_Original = this._strCurrentMappingOriginal, Previous_Mapping_Original = this._strPreviousMappingOriginal };

            //2023
            base.NotifyPropertyChanged(new Action<ETG_Fact_Symmetry_Update_Tracker>((a) => { AppMessages.ProjectChangeTracking.Send(am); }), am);
        }





        private double? _dblPop_Cost_Episode_Distribution;
        public double? Pop_Cost_Episode_Distribution
        {
            get { return this._dblPop_Cost_Episode_Distribution; }
            set { this._dblPop_Cost_Episode_Distribution = value; }
        }


        private double? _dblPercent_of_Episodes;
        public double? Percent_of_Episodes
        {
            get { return this._dblPercent_of_Episodes; }
            set { this._dblPercent_of_Episodes = value; }
        }









        private double? _dblEpisode_Cost_Episode_Distribution;
        public double? Episode_Cost_Episode_Distribution
        {
            get { return this._dblEpisode_Cost_Episode_Distribution; }
            set { this._dblEpisode_Cost_Episode_Distribution = value; }
        }


        private double? _dblEpisode_Cost_Percent_of_Episodes;
        public double? Episode_Cost_Percent_of_Episodes
        {
            get { return this._dblEpisode_Cost_Percent_of_Episodes; }
            set { this._dblEpisode_Cost_Percent_of_Episodes = value; }
        }


        private double? _dblSpecialist_Total_Cost;
        public double? Specialist_Total_Cost
        {
            get { return this._dblSpecialist_Total_Cost; }
            set { this._dblSpecialist_Total_Cost = value; }
        }


        private double? _dblSpecialist_Average_Cost;
        public double? Specialist_Average_Cost
        {
            get { return this._dblSpecialist_Average_Cost; }
            set { this._dblSpecialist_Average_Cost = value; }
        }


        private double? _dblSpecialist_CV;
        public double? Specialist_CV
        {
            get { return this._dblSpecialist_CV; }
            set { this._dblSpecialist_CV = value; }
        }



        private string _strPop_Cost_Changes_Made;
        public string Pop_Cost_Changes_Made
        {
            get { return this._strPop_Cost_Changes_Made; }
            set { this._strPop_Cost_Changes_Made = value; }
        }



        private double? _dblEpisode_Cost_Total_Cost_Commercial_Only;
        public double? Episode_Cost_Total_Cost_Commercial_Only
        {
            get { return this._dblEpisode_Cost_Total_Cost_Commercial_Only; }
            set { this._dblEpisode_Cost_Total_Cost_Commercial_Only = value; }
        }


        private double? _dblEpisode_Cost_Average_Cost_Commercial_Only;
        public double? Episode_Cost_Average_Cost_Commercial_Only
        {
            get { return this._dblEpisode_Cost_Average_Cost_Commercial_Only; }
            set { this._dblEpisode_Cost_Average_Cost_Commercial_Only = value; }
        }


        private double? _dblEpisode_Cost_Coefficients_of_Variation_Commercial_Only;
        public double? Episode_Cost_Coefficients_of_Variation_Commercial_Only
        {
            get { return this._dblEpisode_Cost_Coefficients_of_Variation_Commercial_Only; }
            set { this._dblEpisode_Cost_Coefficients_of_Variation_Commercial_Only = value; }
        }





        private double? _dblEpisode_Cost_Normalized_Pricing_Episode_Count;
        public double? Episode_Cost_Normalized_Pricing_Episode_Count
        {
            get { return this._dblEpisode_Cost_Normalized_Pricing_Episode_Count; }
            set { this._dblEpisode_Cost_Normalized_Pricing_Episode_Count = value; }
        }

        private double? _dblEpisode_Cost_Normalized_Pricing_Total_Cost;
        public double? Episode_Cost_Normalized_Pricing_Total_Cost
        {
            get { return this._dblEpisode_Cost_Normalized_Pricing_Total_Cost; }
            set { this._dblEpisode_Cost_Normalized_Pricing_Total_Cost = value; }
        }

        private double? _dblEpisode_Cost_Episode_Count_Commercial_Only;
        public double? Episode_Cost_Episode_Count_Commercial_Only
        {
            get { return this._dblEpisode_Cost_Episode_Count_Commercial_Only; }
            set { this._dblEpisode_Cost_Episode_Count_Commercial_Only = value; }
        }



        private double? _dblEpisode_Cost_Total_Cost;
        public double? Episode_Cost_Total_Cost
        {
            get { return this._dblEpisode_Cost_Total_Cost; }
            set { this._dblEpisode_Cost_Total_Cost = value; }
        }

        private double? _dblEpisode_Cost_Average_Cost;
        public double? Episode_Cost_Average_Cost
        {
            get { return this._dblEpisode_Cost_Average_Cost; }
            set { this._dblEpisode_Cost_Average_Cost = value; }
        }

        private double? _dblEpisode_Cost_Specialist_CV;
        public double? Episode_Cost_Specialist_CV
        {
            get { return this._dblEpisode_Cost_Specialist_CV; }
            set { this._dblEpisode_Cost_Specialist_CV = value; }
        }



        private string _strEpisode_Cost_Changes_Made;
        public string Episode_Cost_Changes_Made
        {
            get { return this._strEpisode_Cost_Changes_Made; }
            set { this._strEpisode_Cost_Changes_Made = value; }
        }





    }




    public class ETG_Fact_Symmetry_Config_Model
    {
        public ETG_Fact_Symmetry_Config_Model()
        {

        }


        private string _strETGBaseClass;
        public string Base_ETG
        {
            get { return this._strETGBaseClass; }
            set { this._strETGBaseClass = value; }
        }

        private string _strETGDescription;
        public string ETG_Base_Class_Description
        {
            get { return this._strETGDescription; }
            set { this._strETGDescription = value; }
        }



        private string _strPremiumSpecialty;
        public string Premium_Specialty
        {
            get { return this._strPremiumSpecialty; }
            set { this._strPremiumSpecialty = value; }
        }



        private string _strTRT_CD;
        public string TRT_CD
        {
            get { return this._strTRT_CD; }
            set { this._strTRT_CD = value; }
        }

        private string _strAlways_Attributed;
        public string Always_Attributed
        {
            get { return this._strAlways_Attributed; }
            set { this._strAlways_Attributed = value; }
        }


        private string _strIf_Attributed;
        public string If_Attributed
        {
            get { return this._strIf_Attributed; }
            set { this._strIf_Attributed = value; }
        }

        private string _strRx;
        public string Rx
        {
            get { return this._strRx; }
            set { this._strRx = value; }
        }

        private string _strNRx;
        public string NRx
        {
            get { return this._strNRx; }
            set { this._strNRx = value; }
        }

        private string _strRisk_Model;
        public string Risk_Model
        {
            get { return this._strRisk_Model; }
            set { this._strRisk_Model = value; }
        }

        //private string _strCurrent_EC_Treatment_Indicator;
        //public string Current_EC_Treatment_Indicator
        //{
        //    get { return this._strCurrent_EC_Treatment_Indicator; }
        //    set { this._strCurrent_EC_Treatment_Indicator = value; }
        //}
    }



    public class ETG_Fact_Symmetry_PateintCentric
    {
        public ETG_Fact_Symmetry_PateintCentric()
        {

        }


        private string _strETGBaseClass;
        public string Base_ETG
        {
            get { return this._strETGBaseClass; }
            set { this._strETGBaseClass = value; }
        }

        private string _strETGDescription;
        public string ETG_Base_Class_Description
        {
            get { return this._strETGDescription; }
            set { this._strETGDescription = value; }
        }



        private string _strPremiumSpecialty;
        public string Premium_Specialty
        {
            get { return this._strPremiumSpecialty; }
            set { this._strPremiumSpecialty = value; }
        }



        private string _strTRT_CD;
        public string TRT_CD
        {
            get { return this._strTRT_CD; }
            set { this._strTRT_CD = value; }
        }


        private string _strCurrent_Rx_NRx;
        public string Current_Rx_NRx
        {
            get { return this._strCurrent_Rx_NRx; }
            set { this._strCurrent_Rx_NRx = value; }
        }


        private string _strRisk_Model;
        public string Risk_Model
        {
            get { return this._strRisk_Model; }
            set { this._strRisk_Model = value; }
        }


        private string _strCurrent_Pt_Centric_Mapping;
        public string Current_Pt_Centric_Mapping
        {
            get { return this._strCurrent_Pt_Centric_Mapping; }
            set { this._strCurrent_Pt_Centric_Mapping = value; }
        }


        private string _strPatient_Centric_Change_Comments;
        public string Patient_Centric_Change_Comments
        {
            get { return this._strPatient_Centric_Change_Comments; }
            set { this._strPatient_Centric_Change_Comments = value; }
        }

        //private string _strCurrent_EC_Treatment_Indicator;
        //public string Current_EC_Treatment_Indicator
        //{
        //    get { return this._strCurrent_EC_Treatment_Indicator; }
        //    set { this._strCurrent_EC_Treatment_Indicator = value; }
        //}
    }

    public class ETG_Fact_Symmetry_RxNrxConfig_Model
    {
        public ETG_Fact_Symmetry_RxNrxConfig_Model()
        {

        }


        private string _strETGBaseClass;
        public string Base_ETG
        {
            get { return this._strETGBaseClass; }
            set { this._strETGBaseClass = value; }
        }

        private string _strETGDescription;
        public string ETG_Base_Class_Description
        {
            get { return this._strETGDescription; }
            set { this._strETGDescription = value; }
        }



        private string _strPremiumSpecialty;
        public string Premium_Specialty
        {
            get { return this._strPremiumSpecialty; }
            set { this._strPremiumSpecialty = value; }
        }



        //private string _strTRT_CD;
        //public string TRT_CD
        //{
        //    get { return this._strTRT_CD; }
        //    set { this._strTRT_CD = value; }
        //}

        //private string _strAlways_Attributed;
        //public string Always_Attributed
        //{
        //    get { return this._strAlways_Attributed; }
        //    set { this._strAlways_Attributed = value; }
        //}


        //private string _strIf_Attributed;
        //public string If_Attributed
        //{
        //    get { return this._strIf_Attributed; }
        //    set { this._strIf_Attributed = value; }
        //}

        private string _strRx;
        public string Rx
        {
            get { return this._strRx; }
            set { this._strRx = value; }
        }

        private string _strNRx;
        public string NRx
        {
            get { return this._strNRx; }
            set { this._strNRx = value; }
        }

        //private string _strRisk_Model;
        //public string Risk_Model
        //{
        //    get { return this._strRisk_Model; }
        //    set { this._strRisk_Model = value; }
        //}

        //private string _strCurrent_EC_Treatment_Indicator;
        //public string Current_EC_Treatment_Indicator
        //{
        //    get { return this._strCurrent_EC_Treatment_Indicator; }
        //    set { this._strCurrent_EC_Treatment_Indicator = value; }
        //}
    }

    public class ETG_Fact_Symmetry_Export_Model
    {
        public ETG_Fact_Symmetry_Export_Model()
        {

        }



        private string _strETGBaseClass;
        public string ETG_Base_Class
        {
            get { return this._strETGBaseClass; }
            set { this._strETGBaseClass = value; }
        }





        private string _strETGDescription;
        public string ETG_Description
        {
            get { return this._strETGDescription; }
            set { this._strETGDescription = value; }
        }


        private string _strPremiumSpecialty;
        public string Premium_Specialty
        {
            get { return this._strPremiumSpecialty; }
            set { this._strPremiumSpecialty = value; }
        }


        private string _strNRx;
        public string Previous_Rx_NRx
        {
            get { return this._strNRx; }
            set { this._strNRx = value; }
        }


        private string _strRx;
        public string Current_Rx_NRx
        {
            get { return this._strRx; }
            set { this._strRx = value; }
        }

        private string _strETGPreviousLOB;
        public string Previous_LOB
        {
            get { return this._strETGPreviousLOB; }
            set { this._strETGPreviousLOB = value; }
        }

        private string _strETGCurrentLOB;
        public string Current_LOB
        {
            get { return this._strETGCurrentLOB; }
            set { this._strETGCurrentLOB = value; }
        }
        private string _strPreviousPopCostTreatmentIndicator;
        public string PC_Previous_Treatment_Indicator
        {
            get { return this._strPreviousPopCostTreatmentIndicator; }
            set { this._strPreviousPopCostTreatmentIndicator = value; }
        }


        private string _strCurrentPopCostTreatmentIndicator;
        public string PC_Current_Treatment_Indicator
        {
            get { return this._strCurrentPopCostTreatmentIndicator; }
            set { this._strCurrentPopCostTreatmentIndicator = value; }
        }

        private double? _dblPCEpisodeCnt;
        public double? PC_Episode_Cnt
        {
            get { return this._dblPCEpisodeCnt; }
            set { this._dblPCEpisodeCnt = value; }
        }

        private double? _dblTotalCost;
        public double? PC_Tot_Cost
        {
            get { return this._dblTotalCost; }
            set { this._dblTotalCost = value; }
        }

        private double? _dblAvgCost;
        public double? PC_Avg_Cost
        {
            get { return this._dblAvgCost; }
            set { this._dblAvgCost = value; }
        }

        private double? _dblCoefficientsOfVariation;
        public double? PC_CV
        {
            get { return this._dblCoefficientsOfVariation; }
            set { this._dblCoefficientsOfVariation = value; }
        }



        private double? _dblSpecialistEpisodeCnt;
        public double? PC_Spec_Episode_Cnt
        {
            get { return this._dblSpecialistEpisodeCnt; }
            set { this._dblSpecialistEpisodeCnt = value; }
        }




        private double? _dblPop_Cost_Episode_Distribution;
        public double? PC_Spec_Episode_Distribution
        {
            get { return this._dblPop_Cost_Episode_Distribution; }
            set { this._dblPop_Cost_Episode_Distribution = value; }
        }


        private double? _dblPercent_of_Episodes;
        public double? PC_Spec_Perc_of_Episodes
        {
            get { return this._dblPercent_of_Episodes; }
            set { this._dblPercent_of_Episodes = value; }
        }



        private double? _dblSpecialist_Total_Cost;
        public double? PC_Spec_Tot_Cost
        {
            get { return this._dblSpecialist_Total_Cost; }
            set { this._dblSpecialist_Total_Cost = value; }
        }


        private double? _dblSpecialist_Average_Cost;
        public double? PC_Spec_Avg_Cost
        {
            get { return this._dblSpecialist_Average_Cost; }
            set { this._dblSpecialist_Average_Cost = value; }
        }


        private double? _dblSpecialist_CV;
        public double? PC_Spec_CV
        {
            get { return this._dblSpecialist_CV; }
            set { this._dblSpecialist_CV = value; }
        }



        private string _strPreviousPCAttribution;
        public string PC_Prev_Attribution
        {
            get { return this._strPreviousPCAttribution; }
            set { this._strPreviousPCAttribution = value; }
        }

        private string _strCurrentPCAttribution;
        public string PC_Current_Attribution
        {
            get { return this._strCurrentPCAttribution; }
            set { this._strCurrentPCAttribution = value; }
        }





        private string _strPCChangeComments;
        public string PC_Change_Comments
        {
            get { return this._strPCChangeComments; }
            set { this._strPCChangeComments = value; }
        }

        private string _strPreviousEpisodeCostTreatmentIndicator;
        public string EC_Previous_Treatment_Indicator
        {
            get { return this._strPreviousEpisodeCostTreatmentIndicator; }
            set { this._strPreviousEpisodeCostTreatmentIndicator = value; }
        }

        private string _strCurrentEpisodeCostTreatmentIndicator;
        public string EC_Current_Treatment_Indicator
        {
            get { return this._strCurrentEpisodeCostTreatmentIndicator; }
            set { this._strCurrentEpisodeCostTreatmentIndicator = value; }
        }


        private double? _dblECEpisodeCnt;
        public double? EC_Episode_Count
        {
            get { return this._dblECEpisodeCnt; }
            set { this._dblECEpisodeCnt = value; }
        }


        private double? _dblEpisode_Cost_Total_Cost_Commercial_Only;
        public double? EC_Tot_Cost
        {
            get { return this._dblEpisode_Cost_Total_Cost_Commercial_Only; }
            set { this._dblEpisode_Cost_Total_Cost_Commercial_Only = value; }
        }


        private double? _dblEpisode_Cost_Average_Cost_Commercial_Only;
        public double? EC_Avg_Cost
        {
            get { return this._dblEpisode_Cost_Average_Cost_Commercial_Only; }
            set { this._dblEpisode_Cost_Average_Cost_Commercial_Only = value; }
        }


        private double? _dblEpisode_Cost_Coefficients_of_Variation_Commercial_Only;
        public double? EC_CV
        {
            get { return this._dblEpisode_Cost_Coefficients_of_Variation_Commercial_Only; }
            set { this._dblEpisode_Cost_Coefficients_of_Variation_Commercial_Only = value; }
        }



        //private double? _dblEpisode_Cost_Normalized_Pricing_Episode_Count;
        //public double? ECNormalizedPricingEpisodeCount
        //{
        //    get { return this._dblEpisode_Cost_Normalized_Pricing_Episode_Count; }
        //    set { this._dblEpisode_Cost_Normalized_Pricing_Episode_Count = value; }
        //}

        //private double? _dblEpisode_Cost_Normalized_Pricing_Total_Cost;
        //public double? ECNormalizedPricingTotalCost
        //{
        //    get { return this._dblEpisode_Cost_Normalized_Pricing_Total_Cost; }
        //    set { this._dblEpisode_Cost_Normalized_Pricing_Total_Cost = value; }
        //}

        private double? _dblEpisode_Cost_Episode_Count_Commercial_Only;
        public double? EC_Spec_Episode_Cnt
        {
            get { return this._dblEpisode_Cost_Episode_Count_Commercial_Only; }
            set { this._dblEpisode_Cost_Episode_Count_Commercial_Only = value; }
        }




        private double? _dblEpisode_Cost_Episode_Distribution;
        public double? EC_Spec_Episode_Distribution
        {
            get { return this._dblEpisode_Cost_Episode_Distribution; }
            set { this._dblEpisode_Cost_Episode_Distribution = value; }
        }

        private double? _dblEpisode_Cost_Percent_of_Episodes;
        public double? EC_Spec_Perc_of_Episodes
        {
            get { return this._dblEpisode_Cost_Percent_of_Episodes; }
            set { this._dblEpisode_Cost_Percent_of_Episodes = value; }
        }



        private double? _dblEpisode_Cost_Total_Cost;
        public double? EC_Spec_Tot_Cost
        {
            get { return this._dblEpisode_Cost_Total_Cost; }
            set { this._dblEpisode_Cost_Total_Cost = value; }
        }

        private double? _dblEpisode_Cost_Average_Cost;
        public double? EC_Spec_Avg_Cost
        {
            get { return this._dblEpisode_Cost_Average_Cost; }
            set { this._dblEpisode_Cost_Average_Cost = value; }
        }

        private double? _dblEpisode_Cost_Specialist_CV;
        public double? EC_Spec_CV
        {
            get { return this._dblEpisode_Cost_Specialist_CV; }
            set { this._dblEpisode_Cost_Specialist_CV = value; }
        }


        private string _strECPreviousMapping;
        public string EC_Previous_Mapping
        {
            get { return this._strECPreviousMapping; }
            set { this._strECPreviousMapping = value; }
        }

        private string _strECCurrentMapping;
        public string EC_Current_Mapping
        {
            get { return this._strECCurrentMapping; }
            set { this._strECCurrentMapping = value; }
        }




        private string _strECChangeComments;
        public string EC_Change_Comments
        {
            get { return this._strECChangeComments; }
            set { this._strECChangeComments = value; }
        }



        private string _strPreviousPatientCentricMapping;
        public string Previous_Pt_Centric_Mapping
        {
            get { return this._strPreviousPatientCentricMapping; }
            set { this._strPreviousPatientCentricMapping = value; }
        }




        private string _strCurrentPatientCentricMapping;
        public string Current_Pt_Centric_Mapping
        {
            get { return this._strCurrentPatientCentricMapping; }
            set { this._strCurrentPatientCentricMapping = value; }
        }




        private string _strPatientCentricChangeComments;
        public string Pt_Centric_Change_Comments
        {
            get { return this._strPatientCentricChangeComments; }
            set { this._strPatientCentricChangeComments = value; }
        }



        private string _strMeasureStatus;
        public string Measure_Status
        {
            get { return this._strMeasureStatus; }
            set { this._strMeasureStatus = value; }
        }


    }

    public class ETG_Fact_Symmetry_Export_Model2
    {
        public ETG_Fact_Symmetry_Export_Model2()
        {

        }


        public object Clone()
        {
            return this.MemberwiseClone();
        }



        private string _strETGBaseClass;
        public string ETG_Base_Class
        {
            get { return this._strETGBaseClass; }
            set { this._strETGBaseClass = value; }
        }





        private string _strETGDescription;
        public string ETG_Description
        {
            get { return this._strETGDescription; }
            set { this._strETGDescription = value; }
        }


        private string _strPremiumSpecialty;
        public string Premium_Specialty
        {
            get { return this._strPremiumSpecialty; }
            set { this._strPremiumSpecialty = value; }
        }


        //private string _strNRx;
        //public string Previous_Rx_NRx
        //{
        //    get { return this._strNRx; }
        //    set { this._strNRx = value; }
        //}


        private string _strRx;
        public string Current_Rx_NR
        {
            get { return this._strRx; }
            set { this._strRx = value; }
        }

        //private string _strETGPreviousLOB;
        //public string Previous_LOB
        //{
        //    get { return this._strETGPreviousLOB; }
        //    set { this._strETGPreviousLOB = value; }
        //}

        private string _strETGCurrentLOB;
        public string Current_LOB
        {
            get { return this._strETGCurrentLOB; }
            set { this._strETGCurrentLOB = value; }
        }

        //private double? _dblPCEpisodeCnt;
        //public double? PC_Episode_Cnt
        //{
        //    get { return this._dblPCEpisodeCnt; }
        //    set { this._dblPCEpisodeCnt = value; }
        //}

        //private double? _dblTotalCost;
        //public double? PC_Tot_Cost
        //{
        //    get { return this._dblTotalCost; }
        //    set { this._dblTotalCost = value; }
        //}

        //private double? _dblAvgCost;
        //public double? PC_Avg_Cost
        //{
        //    get { return this._dblAvgCost; }
        //    set { this._dblAvgCost = value; }
        //}

        //private double? _dblCoefficientsOfVariation;
        //public double? PC_CV
        //{
        //    get { return this._dblCoefficientsOfVariation; }
        //    set { this._dblCoefficientsOfVariation = value; }
        //}



        //private double? _dblSpecialistEpisodeCnt;
        //public double? PC_Spec_Episode_Cnt
        //{
        //    get { return this._dblSpecialistEpisodeCnt; }
        //    set { this._dblSpecialistEpisodeCnt = value; }
        //}

        //private double? _dblPop_Cost_Episode_Distribution;
        //public double? PC_Spec_Episode_Distribution
        //{
        //    get { return this._dblPop_Cost_Episode_Distribution; }
        //    set { this._dblPop_Cost_Episode_Distribution = value; }
        //}


        //private double? _dblPercent_of_Episodes;
        //public double? PC_Spec_Perc_of_Episodes
        //{
        //    get { return this._dblPercent_of_Episodes; }
        //    set { this._dblPercent_of_Episodes = value; }
        //}



        //private double? _dblSpecialist_Total_Cost;
        //public double? PC_Spec_Tot_Cost
        //{
        //    get { return this._dblSpecialist_Total_Cost; }
        //    set { this._dblSpecialist_Total_Cost = value; }
        //}


        //private double? _dblSpecialist_Average_Cost;
        //public double? PC_Spec_Avg_Cost
        //{
        //    get { return this._dblSpecialist_Average_Cost; }
        //    set { this._dblSpecialist_Average_Cost = value; }
        //}


        //private double? _dblSpecialist_CV;
        //public double? PC_Spec_CV
        //{
        //    get { return this._dblSpecialist_CV; }
        //    set { this._dblSpecialist_CV = value; }
        //}



        //private string _strPreviousPCAttribution;
        //public string PC_Prev_Attribution
        //{
        //    get { return this._strPreviousPCAttribution; }
        //    set { this._strPreviousPCAttribution = value; }
        //}

        //private string _strCurrentPCAttribution;
        //public string PC_Current_Attribution
        //{
        //    get { return this._strCurrentPCAttribution; }
        //    set { this._strCurrentPCAttribution = value; }
        //}


        //private string _strPCChangeComments;
        //public string PC_Change_Comments
        //{
        //    get { return this._strPCChangeComments; }
        //    set { this._strPCChangeComments = value; }
        //}


        //private string _strPreviousEpisodeCostTreatmentIndicator;
        //public string EC_Previous_Treatment_Indicator
        //{
        //    get { return this._strPreviousEpisodeCostTreatmentIndicator; }
        //    set { this._strPreviousEpisodeCostTreatmentIndicator = value; }
        //}

        private string _strCurrentEpisodeCostTreatmentIndicator;
        public string EC_Current_Treatment_Indicator
        {
            get { return this._strCurrentEpisodeCostTreatmentIndicator; }
            set { this._strCurrentEpisodeCostTreatmentIndicator = value; }
        }



        private double? _dblECEpisodeCnt;
        public double? EC_Episode_Count
        {
            get { return this._dblECEpisodeCnt; }
            set { this._dblECEpisodeCnt = value; }
        }


        private double? _dblEpisode_Cost_Total_Cost_Commercial_Only;
        public double? EC_Tot_Cost
        {
            get { return this._dblEpisode_Cost_Total_Cost_Commercial_Only; }
            set { this._dblEpisode_Cost_Total_Cost_Commercial_Only = value; }
        }


        private double? _dblEpisode_Cost_Average_Cost_Commercial_Only;
        public double? EC_Avg_Cost
        {
            get { return this._dblEpisode_Cost_Average_Cost_Commercial_Only; }
            set { this._dblEpisode_Cost_Average_Cost_Commercial_Only = value; }
        }


        private double? _dblEpisode_Cost_Coefficients_of_Variation_Commercial_Only;
        public double? EC_CV
        {
            get { return this._dblEpisode_Cost_Coefficients_of_Variation_Commercial_Only; }
            set { this._dblEpisode_Cost_Coefficients_of_Variation_Commercial_Only = value; }
        }



        //private double? _dblEpisode_Cost_Normalized_Pricing_Episode_Count;
        //public double? ECNormalizedPricingEpisodeCount
        //{
        //    get { return this._dblEpisode_Cost_Normalized_Pricing_Episode_Count; }
        //    set { this._dblEpisode_Cost_Normalized_Pricing_Episode_Count = value; }
        //}

        //private double? _dblEpisode_Cost_Normalized_Pricing_Total_Cost;
        //public double? ECNormalizedPricingTotalCost
        //{
        //    get { return this._dblEpisode_Cost_Normalized_Pricing_Total_Cost; }
        //    set { this._dblEpisode_Cost_Normalized_Pricing_Total_Cost = value; }
        //}

        private double? _dblEpisode_Cost_Episode_Count_Commercial_Only;
        public double? EC_Spec_Episode_Cnt
        {
            get { return this._dblEpisode_Cost_Episode_Count_Commercial_Only; }
            set { this._dblEpisode_Cost_Episode_Count_Commercial_Only = value; }
        }




        private double? _dblEpisode_Cost_Episode_Distribution;
        public double? EC_Spec_Episode_Distribution
        {
            get { return this._dblEpisode_Cost_Episode_Distribution; }
            set { this._dblEpisode_Cost_Episode_Distribution = value; }
        }

        private double? _dblEpisode_Cost_Percent_of_Episodes;
        public double? EC_Spec_Perc_of_Episodes
        {
            get { return this._dblEpisode_Cost_Percent_of_Episodes; }
            set { this._dblEpisode_Cost_Percent_of_Episodes = value; }
        }



        private double? _dblEpisode_Cost_Total_Cost;
        public double? EC_Spec_Tot_Cost
        {
            get { return this._dblEpisode_Cost_Total_Cost; }
            set { this._dblEpisode_Cost_Total_Cost = value; }
        }

        private double? _dblEpisode_Cost_Average_Cost;
        public double? EC_Spec_Avg_Cost
        {
            get { return this._dblEpisode_Cost_Average_Cost; }
            set { this._dblEpisode_Cost_Average_Cost = value; }
        }

        private double? _dblEpisode_Cost_Specialist_CV;
        public double? EC_Spec_CV
        {
            get { return this._dblEpisode_Cost_Specialist_CV; }
            set { this._dblEpisode_Cost_Specialist_CV = value; }
        }


        //private string _strECPreviousMapping;
        //public string EC_Previous_Mapping
        //{
        //    get { return this._strECPreviousMapping; }
        //    set { this._strECPreviousMapping = value; }
        //}

        private string _strECCurrentMapping;
        public string EC_Current_Mapping
        {
            get { return this._strECCurrentMapping; }
            set { this._strECCurrentMapping = value; }
        }






        private string _strECChangeComments;
        public string EC_Change_Comments
        {
            get { return this._strECChangeComments; }
            set { this._strECChangeComments = value; }
        }



        //private string _strPreviousPatientCentricMapping;
        //public string Previous_Pt_Centric_Mapping
        //{
        //    get { return this._strPreviousPatientCentricMapping; }
        //    set { this._strPreviousPatientCentricMapping = value; }
        //}




        //private string _strCurrentPatientCentricMapping;
        //public string Current_Pt_Centric_Mapping
        //{
        //    get { return this._strCurrentPatientCentricMapping; }
        //    set { this._strCurrentPatientCentricMapping = value; }
        //}




        //private string _strPatientCentricChangeComments;
        //public string Pt_Centric_Change_Comments
        //{
        //    get { return this._strPatientCentricChangeComments; }
        //    set { this._strPatientCentricChangeComments = value; }
        //}



        //private string _strMeasureStatus;
        //public string Measure_Status
        //{
        //    get { return this._strMeasureStatus; }
        //    set { this._strMeasureStatus = value; }
        //}



    }


    public class ETG_Fact_Symmetry_Export_ModelOLD
    {
        public ETG_Fact_Symmetry_Export_ModelOLD()
        {

        }

        private Int64 _intETGFactSymmetryid;
        public Int64 ETGFactSymmetryId
        {
            get { return this._intETGFactSymmetryid; }
            set { this._intETGFactSymmetryid = value; }
        }

        private string _strETGDescription;
        public string ETGDescription
        {
            get { return this._strETGDescription; }
            set { this._strETGDescription = value; }
        }


        private string _strETGBaseClass;
        public string ETGBaseClass
        {
            get { return this._strETGBaseClass; }
            set { this._strETGBaseClass = value; }
        }


        private string _strPremiumSpecialty;
        public string PremiumSpecialty
        {
            get { return this._strPremiumSpecialty; }
            set { this._strPremiumSpecialty = value; }
        }

        private string _strETGPreviousLOB;
        public string PreviousLOB
        {
            get { return this._strETGPreviousLOB; }
            set { this._strETGPreviousLOB = value; }
        }

        private string _strETGCurrentLOB;
        public string CurrentLOB
        {
            get { return this._strETGCurrentLOB; }
            set { this._strETGCurrentLOB = value; }
        }


        private string _strETGPreviousPopCostTreatmentIndicator;
        public string PreviousPCTreatmentIndicator
        {
            get { return this._strETGPreviousPopCostTreatmentIndicator; }
            set { this._strETGPreviousPopCostTreatmentIndicator = value; }
        }

        private string _strETGCurrentPopCostTreatmentIndicator;
        public string CurrentPCTreatmentIndicator
        {
            get { return this._strETGCurrentPopCostTreatmentIndicator; }
            set { this._strETGCurrentPopCostTreatmentIndicator = value; }
        }

        private string _strNRx;
        public string NRx
        {
            get { return this._strNRx; }
            set { this._strNRx = value; }
        }


        private string _strRx;
        public string Rx
        {
            get { return this._strRx; }
            set { this._strRx = value; }
        }



        private double? _dblPCEpisodeCnt;
        public double? PCEpisodeCnt
        {
            get { return this._dblPCEpisodeCnt; }
            set { this._dblPCEpisodeCnt = value; }
        }

        private double? _dblTotalCost;
        public double? TotalCost
        {
            get { return this._dblTotalCost; }
            set { this._dblTotalCost = value; }
        }

        private double? _dblAvgCost;
        public double? AvgCost
        {
            get { return this._dblAvgCost; }
            set { this._dblAvgCost = value; }
        }

        private double? _dblCoefficientsOfVariation;
        public double? CoefficientsOfVariation
        {
            get { return this._dblCoefficientsOfVariation; }
            set { this._dblCoefficientsOfVariation = value; }
        }

        //private double? _dblNormalizedPricingTotalCost;
        //public double? NormalizedPricingTotalCost
        //{
        //    get { return this._dblNormalizedPricingTotalCost; }
        //    set { this._dblNormalizedPricingTotalCost = value; }
        //}



        //private double? _dblNormalizedPricingEpisodeCnt;
        //public double? NormalizedPricingEpisodeCnt
        //{
        //    get { return this._dblNormalizedPricingEpisodeCnt; }
        //    set { this._dblNormalizedPricingEpisodeCnt = value; }
        //}


        private double? _dblSpecialistEpisodeCnt;
        public double? SpecialistEpisodeCnt
        {
            get { return this._dblSpecialistEpisodeCnt; }
            set { this._dblSpecialistEpisodeCnt = value; }
        }

        private double? _dblPop_Cost_Episode_Distribution;
        public double? PopCostEpisodeDistribution
        {
            get { return this._dblPop_Cost_Episode_Distribution; }
            set { this._dblPop_Cost_Episode_Distribution = value; }
        }


        private double? _dblPercent_of_Episodes;
        public double? PercentOfEpisodes
        {
            get { return this._dblPercent_of_Episodes; }
            set { this._dblPercent_of_Episodes = value; }
        }



        private double? _dblSpecialist_Total_Cost;
        public double? SpecialistTotalCost
        {
            get { return this._dblSpecialist_Total_Cost; }
            set { this._dblSpecialist_Total_Cost = value; }
        }


        private double? _dblSpecialist_Average_Cost;
        public double? SpecialistAverageCost
        {
            get { return this._dblSpecialist_Average_Cost; }
            set { this._dblSpecialist_Average_Cost = value; }
        }


        private double? _dblSpecialist_CV;
        public double? SpecialistCV
        {
            get { return this._dblSpecialist_CV; }
            set { this._dblSpecialist_CV = value; }
        }

        private string _strPop_Cost_Changes_Made;
        public string PopCostChangesMade
        {
            get { return this._strPop_Cost_Changes_Made; }
            set { this._strPop_Cost_Changes_Made = value; }
        }



        private string _strPreviousPCAttribution;
        public string PreviousPCAttribution
        {
            get { return this._strPreviousPCAttribution; }
            set { this._strPreviousPCAttribution = value; }
        }

        private string _strCurrentPCAttribution;
        public string CurrentPCAttribution
        {
            get { return this._strCurrentPCAttribution; }
            set { this._strCurrentPCAttribution = value; }
        }


        private string _strPCChangeComments;
        public string PCChangeComments
        {
            get { return this._strPCChangeComments; }
            set { this._strPCChangeComments = value; }
        }


        private string _strPreviousECTreatmentIndicator;
        public string PreviousECTreatmentIndicator
        {
            get { return this._strPreviousECTreatmentIndicator; }
            set { this._strPreviousECTreatmentIndicator = value; }
        }

        private string _strCurrentECTreatmentIndicator;
        public string CurrentECTreatmentIndicator
        {
            get { return this._strCurrentECTreatmentIndicator; }
            set { this._strCurrentECTreatmentIndicator = value; }
        }


        private string _strECPreviousMapping;
        public string ECPreviousMapping
        {
            get { return this._strECPreviousMapping; }
            set { this._strECPreviousMapping = value; }
        }

        private string _strECCurrentMapping;
        public string ECCurrentMapping
        {
            get { return this._strECCurrentMapping; }
            set { this._strECCurrentMapping = value; }
        }


        private double? _dblECEpisodeCnt;
        public double? ECEpisodeCnt
        {
            get { return this._dblECEpisodeCnt; }
            set { this._dblECEpisodeCnt = value; }
        }


        private double? _dblEpisode_Cost_Total_Cost_Commercial_Only;
        public double? ECTotalCostCommercialOnly
        {
            get { return this._dblEpisode_Cost_Total_Cost_Commercial_Only; }
            set { this._dblEpisode_Cost_Total_Cost_Commercial_Only = value; }
        }


        private double? _dblEpisode_Cost_Average_Cost_Commercial_Only;
        public double? ECAverageCostCommercialOnly
        {
            get { return this._dblEpisode_Cost_Average_Cost_Commercial_Only; }
            set { this._dblEpisode_Cost_Average_Cost_Commercial_Only = value; }
        }


        private double? _dblEpisode_Cost_Coefficients_of_Variation_Commercial_Only;
        public double? ECCoefficientsOfVariationCommercialOnly
        {
            get { return this._dblEpisode_Cost_Coefficients_of_Variation_Commercial_Only; }
            set { this._dblEpisode_Cost_Coefficients_of_Variation_Commercial_Only = value; }
        }



        //private double? _dblEpisode_Cost_Normalized_Pricing_Episode_Count;
        //public double? ECNormalizedPricingEpisodeCount
        //{
        //    get { return this._dblEpisode_Cost_Normalized_Pricing_Episode_Count; }
        //    set { this._dblEpisode_Cost_Normalized_Pricing_Episode_Count = value; }
        //}

        //private double? _dblEpisode_Cost_Normalized_Pricing_Total_Cost;
        //public double? ECNormalizedPricingTotalCost
        //{
        //    get { return this._dblEpisode_Cost_Normalized_Pricing_Total_Cost; }
        //    set { this._dblEpisode_Cost_Normalized_Pricing_Total_Cost = value; }
        //}

        private double? _dblEpisode_Cost_Episode_Count_Commercial_Only;
        public double? ECEpisodeCountCommercialOnly
        {
            get { return this._dblEpisode_Cost_Episode_Count_Commercial_Only; }
            set { this._dblEpisode_Cost_Episode_Count_Commercial_Only = value; }
        }




        private double? _dblEpisode_Cost_Episode_Distribution;
        public double? ECEpisodeDistribution
        {
            get { return this._dblEpisode_Cost_Episode_Distribution; }
            set { this._dblEpisode_Cost_Episode_Distribution = value; }
        }

        private double? _dblEpisode_Cost_Percent_of_Episodes;
        public double? ECPercentOfEpisodes
        {
            get { return this._dblEpisode_Cost_Percent_of_Episodes; }
            set { this._dblEpisode_Cost_Percent_of_Episodes = value; }
        }



        private double? _dblEpisode_Cost_Total_Cost;
        public double? ECTotalCost
        {
            get { return this._dblEpisode_Cost_Total_Cost; }
            set { this._dblEpisode_Cost_Total_Cost = value; }
        }

        private double? _dblEpisode_Cost_Average_Cost;
        public double? ECAverageCost
        {
            get { return this._dblEpisode_Cost_Average_Cost; }
            set { this._dblEpisode_Cost_Average_Cost = value; }
        }

        private double? _dblEpisode_Cost_Specialist_CV;
        public double? ECSpecialistCV
        {
            get { return this._dblEpisode_Cost_Specialist_CV; }
            set { this._dblEpisode_Cost_Specialist_CV = value; }
        }



        private string _strEpisode_Cost_Changes_Made;
        public string EC_ChangesMade
        {
            get { return this._strEpisode_Cost_Changes_Made; }
            set { this._strEpisode_Cost_Changes_Made = value; }
        }

        private string _strECChangeComments;
        public string ECChangeComments
        {
            get { return this._strECChangeComments; }
            set { this._strECChangeComments = value; }
        }



        private string _strPreviousPatientCentricMapping;
        public string PreviousPatientCentricMapping
        {
            get { return this._strPreviousPatientCentricMapping; }
            set { this._strPreviousPatientCentricMapping = value; }
        }




        private string _strCurrentPatientCentricMapping;
        public string CurrentPatientCentricMapping
        {
            get { return this._strCurrentPatientCentricMapping; }
            set { this._strCurrentPatientCentricMapping = value; }
        }




        private string _strPatientCentricChangeComments;
        public string PatientCentricChangeComments
        {
            get { return this._strPatientCentricChangeComments; }
            set { this._strPatientCentricChangeComments = value; }
        }







    }






























    //CHRIS ADDED
    [Table("ETG_Fact_Symmetry_Update_Tracker")]
    public class ETG_Fact_Symmetry_Update_Tracker
    {
        private Int64 _intETGFactSymmetryid;
        public Int64 ETG_Fact_Symmetry_id
        {
            get { return this._intETGFactSymmetryid; }
            set { this._intETGFactSymmetryid = value; }
        }

        private Int64 _intETGFactSymmetryidPrevious;
        public Int64 ETG_Fact_Symmetry_id_Previous
        {
            get { return this._intETGFactSymmetryidPrevious; }
            set { this._intETGFactSymmetryidPrevious = value; }
        }

        private string _strETGCurrentPopCostTreatmentIndicator;
        public string Pop_Cost_Current_Treatment_Indicator
        {
            get { return this._strETGCurrentPopCostTreatmentIndicator; }
            set { this._strETGCurrentPopCostTreatmentIndicator = value; }
        }

        private string _strETGPreviousPopCostTreatmentIndicator;
        public string Pop_Cost_Previous_Treatment_Indicator
        {
            get { return this._strETGPreviousPopCostTreatmentIndicator; }
            set { this._strETGPreviousPopCostTreatmentIndicator = value; }
        }



        private string _strCurrentAttribution;
        public string Current_Attribution
        {
            get { return this._strCurrentAttribution; }
            set { this._strCurrentAttribution = value; }
        }

        private string _strPreviousAttribution;
        public string Previous_Attribution
        {
            get { return this._strPreviousAttribution; }
            set { this._strPreviousAttribution = value; }
        }

        private string _strPopCostChangeComments;
        public string Pop_Cost_Change_Comments
        {
            get { return this._strPopCostChangeComments; }
            set { this._strPopCostChangeComments = value; }
        }

        private string _strCurrentEpisodeCostTreatmentIndicator;
        public string Current_Episode_Cost_Treatment_Indicator
        {
            get { return this._strCurrentEpisodeCostTreatmentIndicator; }
            set { this._strCurrentEpisodeCostTreatmentIndicator = value; }
        }

        private string _strPreviousEpisodeCostTreatmentIndicator;
        public string Previous_Episode_Cost_Treatment_Indicator
        {
            get { return this._strPreviousEpisodeCostTreatmentIndicator; }
            set { this._strPreviousEpisodeCostTreatmentIndicator = value; }
        }

        private string _strCurrentMapping;
        public string Current_Mapping
        {
            get { return this._strCurrentMapping; }
            set { this._strCurrentMapping = value; }
        }
        private string _strPreviousMapping;
        public string Previous_Mapping
        {
            get { return this._strPreviousMapping; }
            set { this._strPreviousMapping = value; }
        }

        private string _strEpisodeCostChangeComments;
        public string Episode_Cost_Change_Comments
        {
            get { return this._strEpisodeCostChangeComments; }
            set { this._strEpisodeCostChangeComments = value; }
        }

        private string _strCurrentPatientCentricMapping;
        public string Current_Patient_Centric_Mapping
        {
            get { return this._strCurrentPatientCentricMapping; }
            set { this._strCurrentPatientCentricMapping = value; }
        }

        private string _strPreviousPatientCentricMapping;
        public string Previous_Patient_Centric_Mapping
        {
            get { return this._strPreviousPatientCentricMapping; }
            set { this._strPreviousPatientCentricMapping = value; }
        }


        private string _strPatientCentricChangeComments;
        public string Patient_Centric_Change_Comments
        {
            get { return this._strPatientCentricChangeComments; }
            set { this._strPatientCentricChangeComments = value; }
        }


        private string _strCurrentMappingOriginal;
        public string Current_Mapping_Original
        {
            get { return this._strCurrentMappingOriginal; }
            set { this._strCurrentMappingOriginal = value; }
        }
        private string _strPreviousMappingOrigina;
        public string Previous_Mapping_Original
        {
            get { return this._strPreviousMappingOrigina; }
            set { this._strPreviousMappingOrigina = value; }
        }




        private string _strLOBCurrentString;

        public string LOBCurrentString
        {
            get
            {
                return _strLOBCurrentString;
            }
            set
            {
                this._strLOBCurrentString = value;
            }
        }


        private string _strLOBPreviousString;
        public string LOBPreviousString
        {
            get { return this._strLOBPreviousString; }
            set { this._strLOBPreviousString = value; }
        }



        private string _strUsername;
        public string username
        {
            get { return this._strUsername; }
            set { this._strUsername = value; }
        }
    }




    //public class ETGFactSymmetryFilters 
    //{
    //    public ETGFactSymmetryFilters()
    //    {

    //    }

    //    private string _strETGBaseClass;
    //    public string ETG_Base_Class
    //    {
    //        get { return this._strETGBaseClass; }
    //        set { this._strETGBaseClass = value; }
    //    }

    //    private string _strETGDescription;
    //    public string ETG_Description
    //    {
    //        get { return this._strETGDescription; }
    //        set { this._strETGDescription = value; }
    //    }


    //    private Int16 _intPremiumSpecialtyId;
    //    public Int16 Premium_Specialty_id
    //    {
    //        get { return this._intPremiumSpecialtyId; }
    //        set { this._intPremiumSpecialtyId = value; }
    //    }

    //    private string _strPremiumSpecialty;
    //    public string Premium_Specialty
    //    {
    //        get { return this._strPremiumSpecialty; }
    //        set { this._strPremiumSpecialty = value; }
    //    }

    //    private string _strData_Period;
    //    public string Data_Period
    //    {
    //        get { return this._strData_Period; }
    //        set { this._strData_Period = value; }
    //    }

    //    private DateTime _dtData_Date;
    //    public DateTime Data_Date
    //    {
    //        get { return this._dtData_Date; }
    //        set { this._dtData_Date = value; }
    //    }



    //}


    public class ETG_Data_Date
    {
        public ETG_Data_Date()
        {

        }

        private string _strData_Period;
        public string Data_Period
        {
            get { return this._strData_Period; }
            set { this._strData_Period = value; }
        }

        private DateTime _dtData_Date;
        public DateTime Data_Date
        {
            get { return this._dtData_Date; }
            set { this._dtData_Date = value; }
        }

    }




    public class ETG_Symmetry_Verion
    {
        public ETG_Symmetry_Verion()
        {

        }


        private double _dblSymmetry_Version;
        public double Symmetry_Version
        {
            get { return this._dblSymmetry_Version; }
            set { this._dblSymmetry_Version = value; }
        }

        private string _strData_Period;
        public string Data_Period
        {
            get { return this._strData_Period; }
            set { this._strData_Period = value; }
        }

        private DateTime _dtData_Date;
        public DateTime Data_Date
        {
            get { return this._dtData_Date; }
            set { this._dtData_Date = value; }
        }

    }

}
