using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCS_Project_Manager
{

    public class MHP_Yearly_Universes_Reporting_Model 
    {
        public MHP_Yearly_Universes_Reporting_Model()
        {

        }

        private int _intExcelRow;
        public int ExcelRow
        {
            get { return this._intExcelRow; }
            set { this._intExcelRow = value; }
        }


        private int? _int_cnt_in_ip;
        public int? cnt_in_ip
        {
            get { return this._int_cnt_in_ip; }
            set { this._int_cnt_in_ip = value; }
        }


        private int? _int_cnt_on_ip;
        public int? cnt_on_ip
        {
            get { return this._int_cnt_on_ip; }
            set { this._int_cnt_on_ip = value; }
        }


        private int? _int_cnt_in_op;
        public int? cnt_in_op
        {
            get { return this._int_cnt_in_op; }
            set { this._int_cnt_in_op = value; }
        }


        private int? _int_cnt_on_op;
        public int? cnt_on_op
        {
            get { return this._int_cnt_on_op; }
            set { this._int_cnt_on_op = value; }
        }


        

        private string _strStartDate;
        public string StartDate
        {
            get { return this._strStartDate; }
            set { this._strStartDate = value; }
        }


        private string _strEndDate;
        public string EndDate
        {
            get { return this._strEndDate; }
            set { this._strEndDate = value; }
        }


        private string _strState;
        public string State
        {
            get { return this._strState; }
            set { this._strState = value; }
        }

        private string _strLegalEntity;
        public string LegalEntity
        {
            get { return this._strLegalEntity; }
            set { this._strLegalEntity = value; }
        }

    }

    public class MHPIFP_Yearly_Universes_Reporting_Model
    {
        public MHPIFP_Yearly_Universes_Reporting_Model()
        {

        }

        private int _intExcelRow;
        public int ExcelRow
        {
            get { return this._intExcelRow; }
            set { this._intExcelRow = value; }
        }


        private int? _int_cnt_in_ip;
        public int? cnt_in_ip
        {
            get { return this._int_cnt_in_ip; }
            set { this._int_cnt_in_ip = value; }
        }


        private int? _int_cnt_on_ip;
        public int? cnt_on_ip
        {
            get { return this._int_cnt_on_ip; }
            set { this._int_cnt_on_ip = value; }
        }


        private int? _int_cnt_in_op;
        public int? cnt_in_op
        {
            get { return this._int_cnt_in_op; }
            set { this._int_cnt_in_op = value; }
        }


        private int? _int_cnt_on_op;
        public int? cnt_on_op
        {
            get { return this._int_cnt_on_op; }
            set { this._int_cnt_on_op = value; }
        }




        private string _strStartDate;
        public string StartDate
        {
            get { return this._strStartDate; }
            set { this._strStartDate = value; }
        }


        private string _strEndDate;
        public string EndDate
        {
            get { return this._strEndDate; }
            set { this._strEndDate = value; }
        }


        private string _strState;
        public string State
        {
            get { return this._strState; }
            set { this._strState = value; }
        }




        private string _product;
        public string Product
        {
            get { return this._product; }
            set { this._product = value; }
        }
    }

    public class MHPCS_Yearly_Universes_Reporting_Model
    {
        public MHPCS_Yearly_Universes_Reporting_Model()
        {

        }

        private int _intExcelRow;
        public int ExcelRow
        {
            get { return this._intExcelRow; }
            set { this._intExcelRow = value; }
        }


        private int? _int_cnt_ip;
        public int? cnt_ip
        {
            get { return this._int_cnt_ip; }
            set { this._int_cnt_ip = value; }
        }


       
      

        private int? _int_cnt_op;
        public int? cnt_op
        {
            get { return this._int_cnt_op; }
            set { this._int_cnt_op = value; }
        }




        private string _strStartDate;
        public string StartDate
        {
            get { return this._strStartDate; }
            set { this._strStartDate = value; }
        }


        private string _strEndDate;
        public string EndDate
        {
            get { return this._strEndDate; }
            set { this._strEndDate = value; }
        }


        private string _strState;
        public string State
        {
            get { return this._strState; }
            set { this._strState = value; }
        }


        private string _strCS_TADM_PRDCT_MAP;
        public string CS_TADM_PRDCT_MAP
        {
            get { return this._strCS_TADM_PRDCT_MAP; }
            set { this._strCS_TADM_PRDCT_MAP = value; }
        }
    }


    public class MHP_Yearly_Universes_Details_Model
    {
        public MHP_Yearly_Universes_Details_Model()
        {

        }

        private string _strAuthorization;
        public string Authorization
        {
            get { return this._strAuthorization; }
            set { this._strAuthorization = value; }
        }



        private string _strRequest_Decision;
        public string Request_Decision
        {
            get { return this._strRequest_Decision; }
            set { this._strRequest_Decision = value; }
        }


        private string _strAuthorization_Type;
        public string Authorization_Type
        {
            get { return this._strAuthorization_Type; }
            set { this._strAuthorization_Type = value; }
        }


        private string _strPar_NonPar_Site;
        public string Par_NonPar_Site
        {
            get { return this._strPar_NonPar_Site; }
            set { this._strPar_NonPar_Site = value; }
        }


        private string _strInpatient_Outpatient;
        public string Inpatient_Outpatient
        {
            get { return this._strInpatient_Outpatient; }
            set { this._strInpatient_Outpatient = value; }
        }

        private string _strRequest_Date;
        public string Request_Date
        {
            get { return this._strRequest_Date; }
            set { this._strRequest_Date = value; }
        }


        private string _strState_of_Issue;
        public string State_of_Issue
        {
            get { return this._strState_of_Issue; }
            set { this._strState_of_Issue = value; }
        }

        private string _strFINC_ARNG_DESC;
        public string FINC_ARNG_DESC
        {
            get { return this._strFINC_ARNG_DESC; }
            set { this._strFINC_ARNG_DESC = value; }
        }


        private string _strDecision_Reason;
        public string Decision_Reason
        {
            get { return this._strDecision_Reason; }
            set { this._strDecision_Reason = value; }
        }

        //private string _strLegalEntity;
        //public string LegalEntity
        //{
        //    get { return this._strLegalEntity; }
        //    set { this._strLegalEntity = value; }
        //}


        private string _strLEG_ENTY_NBR;
        public string LEG_ENTY_NBR
        {
            get { return this._strLEG_ENTY_NBR; }
            set { this._strLEG_ENTY_NBR = value; }
        }

        private string _strLEG_ENTY_FULL_NM;
        public string LEG_ENTY_FULL_NM
        {
            get { return this._strLEG_ENTY_FULL_NM; }
            set { this._strLEG_ENTY_FULL_NM = value; }
        }




        private string _strMKT_SEG_RLLP_DESC;
        public string MKT_SEG_RLLP_DESC
        {
            get { return this._strMKT_SEG_RLLP_DESC; }
            set { this._strMKT_SEG_RLLP_DESC = value; }
        }


        private string _strMKT_TYP_DESC;
        public string MKT_TYP_DESC
        {
            get { return this._strMKT_TYP_DESC; }
            set { this._strMKT_TYP_DESC = value; }
        }




        private string _strCUST_SEG_NBR;
        public string CUST_SEG_NBR
        {
            get { return this._strCUST_SEG_NBR; }
            set { this._strCUST_SEG_NBR = value; }
        }


        private string _strCUST_SEG_NM;
        public string CUST_SEG_NM
        {
            get { return this._strCUST_SEG_NM; }
            set { this._strCUST_SEG_NM = value; }
        }




        private string _strEnrollee_First_Name;
        public string Enrollee_First_Name
        {
            get { return this._strEnrollee_First_Name; }
            set { this._strEnrollee_First_Name = value; }
        }



        private string _strEnrollee_Last_Name;
        public string Enrollee_Last_Name
        {
            get { return this._strEnrollee_Last_Name; }
            set { this._strEnrollee_Last_Name = value; }
        }



        private string _strCardholder_ID;
        public string Cardholder_ID
        {
            get { return this._strCardholder_ID; }
            set { this._strCardholder_ID = value; }
        }



        private string _strMember_Date_of_Birth;
        public string Member_Date_of_Birth
        {
            get { return this._strMember_Date_of_Birth; }
            set { this._strMember_Date_of_Birth = value; }
        }


        private string _strProcedure_Code_Description;
        public string Procedure_Code_Description
        {
            get { return this._strProcedure_Code_Description; }
            set { this._strProcedure_Code_Description = value; }
        }

        private string _strPrimary_Procedure_Code_Req;
        public string Primary_Procedure_Code_Req
        {
            get { return this._strPrimary_Procedure_Code_Req; }
            set { this._strPrimary_Procedure_Code_Req = value; }
        }

        private string _strPrimary_Diagnosis_Code;
        public string Primary_Diagnosis_Code
        {
            get { return this._strPrimary_Diagnosis_Code; }
            set { this._strPrimary_Diagnosis_Code = value; }
        }


        //private string _strDiagnosis_Code_Description;
        //public string Diagnosis_Code_Description
        //{
        //    get { return this._strDiagnosis_Code_Description; }
        //    set { this._strDiagnosis_Code_Description = value; }
        //}




    }



    public class MHPIFP_Yearly_Universes_Details_Model
    {
        public MHPIFP_Yearly_Universes_Details_Model()
        {

        }

        private string _strAuthorization;
        public string Authorization
        {
            get { return this._strAuthorization; }
            set { this._strAuthorization = value; }
        }



        private string _strRequest_Decision;
        public string Request_Decision
        {
            get { return this._strRequest_Decision; }
            set { this._strRequest_Decision = value; }
        }


        private string _strAuthorization_Type;
        public string Authorization_Type
        {
            get { return this._strAuthorization_Type; }
            set { this._strAuthorization_Type = value; }
        }


        private string _strPar_NonPar_Site;
        public string Par_NonPar_Site
        {
            get { return this._strPar_NonPar_Site; }
            set { this._strPar_NonPar_Site = value; }
        }


        private string _strInpatient_Outpatient;
        public string Inpatient_Outpatient
        {
            get { return this._strInpatient_Outpatient; }
            set { this._strInpatient_Outpatient = value; }
        }

        private string _strRequest_Date;
        public string Request_Date
        {
            get { return this._strRequest_Date; }
            set { this._strRequest_Date = value; }
        }


        private string _strState_of_Issue;
        public string State_of_Issue
        {
            get { return this._strState_of_Issue; }
            set { this._strState_of_Issue = value; }
        }

      
        private string _strDecision_Reason;
        public string Decision_Reason
        {
            get { return this._strDecision_Reason; }
            set { this._strDecision_Reason = value; }
        }

   
        private string _strEnrollee_First_Name;
        public string Enrollee_First_Name
        {
            get { return this._strEnrollee_First_Name; }
            set { this._strEnrollee_First_Name = value; }
        }



        private string _strEnrollee_Last_Name;
        public string Enrollee_Last_Name
        {
            get { return this._strEnrollee_Last_Name; }
            set { this._strEnrollee_Last_Name = value; }
        }



        private string _strCardholder_ID;
        public string Cardholder_ID
        {
            get { return this._strCardholder_ID; }
            set { this._strCardholder_ID = value; }
        }



        private string _strMember_Date_of_Birth;
        public string Member_Date_of_Birth
        {
            get { return this._strMember_Date_of_Birth; }
            set { this._strMember_Date_of_Birth = value; }
        }


        private string _strProcedure_Code_Description;
        public string Procedure_Code_Description
        {
            get { return this._strProcedure_Code_Description; }
            set { this._strProcedure_Code_Description = value; }
        }

        private string _strPrimary_Procedure_Code_Req;
        public string Primary_Procedure_Code_Req
        {
            get { return this._strPrimary_Procedure_Code_Req; }
            set { this._strPrimary_Procedure_Code_Req = value; }
        }

        private string _strPrimary_Diagnosis_Code;
        public string Primary_Diagnosis_Code
        {
            get { return this._strPrimary_Diagnosis_Code; }
            set { this._strPrimary_Diagnosis_Code = value; }
        }



        private string _PRDCT_CD;
        public string PRDCT_CD
        {
            get { return this._PRDCT_CD; }
            set { this._PRDCT_CD = value; }
        }


        private string _PRDCT_CD_DESC;
        public string PRDCT_CD_DESC
        {
            get { return this._PRDCT_CD_DESC; }
            set { this._PRDCT_CD_DESC = value; }
        }

    }









    public class MHPCS_Yearly_Universes_Details_Model
    {
        public MHPCS_Yearly_Universes_Details_Model()
        {

        }

        private string _strAuthorization;
        public string Authorization
        {
            get { return this._strAuthorization; }
            set { this._strAuthorization = value; }
        }



        private string _strRequest_Decision;
        public string Request_Decision
        {
            get { return this._strRequest_Decision; }
            set { this._strRequest_Decision = value; }
        }


        private string _strAuthorization_Type;
        public string Authorization_Type
        {
            get { return this._strAuthorization_Type; }
            set { this._strAuthorization_Type = value; }
        }


        private string _strPar_NonPar_Site;
        public string Par_NonPar_Site
        {
            get { return this._strPar_NonPar_Site; }
            set { this._strPar_NonPar_Site = value; }
        }


        private string _strInpatient_Outpatient;
        public string Inpatient_Outpatient
        {
            get { return this._strInpatient_Outpatient; }
            set { this._strInpatient_Outpatient = value; }
        }

        private string _strRequest_Date;
        public string Request_Date
        {
            get { return this._strRequest_Date; }
            set { this._strRequest_Date = value; }
        }


        private string _strState_of_Issue;
        public string State_of_Issue
        {
            get { return this._strState_of_Issue; }
            set { this._strState_of_Issue = value; }
        }

        


        private string _strDecision_Reason;
        public string Decision_Reason
        {
            get { return this._strDecision_Reason; }
            set { this._strDecision_Reason = value; }
        }

        //private string _strLegalEntity;
        //public string LegalEntity
        //{
        //    get { return this._strLegalEntity; }
        //    set { this._strLegalEntity = value; }
        //}




        private string _strEnrollee_First_Name;
        public string Enrollee_First_Name
        {
            get { return this._strEnrollee_First_Name; }
            set { this._strEnrollee_First_Name = value; }
        }



        private string _strEnrollee_Last_Name;
        public string Enrollee_Last_Name
        {
            get { return this._strEnrollee_Last_Name; }
            set { this._strEnrollee_Last_Name = value; }
        }



        private string _strCardholder_ID;
        public string Cardholder_ID
        {
            get { return this._strCardholder_ID; }
            set { this._strCardholder_ID = value; }
        }



        private string _strMember_Date_of_Birth;
        public string Member_Date_of_Birth
        {
            get { return this._strMember_Date_of_Birth; }
            set { this._strMember_Date_of_Birth = value; }
        }


        private string _strProcedure_Code_Description;
        public string Procedure_Code_Description
        {
            get { return this._strProcedure_Code_Description; }
            set { this._strProcedure_Code_Description = value; }
        }


        private string _strPrimary_Procedure_Code_Req;
        public string Primary_Procedure_Code_Req
        {
            get { return this._strPrimary_Procedure_Code_Req; }
            set { this._strPrimary_Procedure_Code_Req = value; }
        }



        private string _strPrimary_Diagnosis_Code;
        public string Primary_Diagnosis_Code
        {
            get { return this._strPrimary_Diagnosis_Code; }
            set { this._strPrimary_Diagnosis_Code = value; }
        }


        private string _strCS_TADM_PRDCT_MAP;
        public string CS_TADM_PRDCT_MAP
        {
            get { return this._strCS_TADM_PRDCT_MAP; }
            set { this._strCS_TADM_PRDCT_MAP = value; }
        }



        private string _strGroup_Number;
        public string Group_Number
        {
            get { return this._strGroup_Number; }
            set { this._strGroup_Number = value; }
        }


        private string _strPRDCT_CD_DESC;
        public string PRDCT_CD_DESC
        {
            get { return this._strPRDCT_CD_DESC; }
            set { this._strPRDCT_CD_DESC = value; }
        }


    }
    public class Group_State_Model
    {
        private string _State_of_Issue;
        public string State_of_Issue
        {
            get { return this._State_of_Issue; }
            set { this._State_of_Issue = value; }
        }


        private string _Group_Number;
        public string Group_Number
        {
            get { return this._Group_Number; }
            set { this._Group_Number = value; }
        }
    }



}
