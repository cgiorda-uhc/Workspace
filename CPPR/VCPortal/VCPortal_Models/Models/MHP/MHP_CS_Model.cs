using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.MHP;
public class MHP_CS_Model
{

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
