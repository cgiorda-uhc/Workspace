using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.MHP;
public class MHPCSDetails_Model
{


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
