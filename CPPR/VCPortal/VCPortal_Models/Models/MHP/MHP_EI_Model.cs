using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.MHP;
public class MHP_EI_Model
{
    private int _intExcelRow;
    public int ExcelRow
    {
        get { return _intExcelRow; }
        set { _intExcelRow = value; }
    }


    private int? _int_cnt_in_ip;
    public int? cnt_in_ip
    {
        get { return _int_cnt_in_ip; }
        set { _int_cnt_in_ip = value; }
    }


    private int? _int_cnt_on_ip;
    public int? cnt_on_ip
    {
        get { return _int_cnt_on_ip; }
        set { _int_cnt_on_ip = value; }
    }


    private int? _int_cnt_in_op;
    public int? cnt_in_op
    {
        get { return _int_cnt_in_op; }
        set { _int_cnt_in_op = value; }
    }


    private int? _int_cnt_on_op;
    public int? cnt_on_op
    {
        get { return _int_cnt_on_op; }
        set { _int_cnt_on_op = value; }
    }

    private string _strStartDate;
    public string StartDate
    {
        get { return _strStartDate; }
        set { _strStartDate = value; }
    }


    private string _strEndDate;
    public string EndDate
    {
        get { return _strEndDate; }
        set { _strEndDate = value; }
    }


    private string _strState;
    public string State
    {
        get { return _strState; }
        set { _strState = value; }
    }


    private string _strLegalEntity;
    public string LegalEntity
    {
        get { return _strLegalEntity; }
        set { _strLegalEntity = value; }
    }

}
