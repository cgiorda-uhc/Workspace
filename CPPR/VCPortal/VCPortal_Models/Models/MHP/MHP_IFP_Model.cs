using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.MHP;
public class MHP_IFP_Model
{
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
