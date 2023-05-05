using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Shared;


public enum ProjectType
{
    Report,
    DataLoad,
    Misc

}


public enum FileFormat
{
    Delimited,
    Excel
}

public enum SearchType
{
    AD,
    File,
    DB
}

public enum DBType
{
    MSSQL,
    Oracle,
    Teradata,
    DB2,
    Snowflake,
    SAS
}

public enum SQLType
{
    TableName,
    Text,
    StoredProcedure
}

public enum SQLAction
{
    Select,
    Update,
    Append,
    Refresh,
    DropCreate,
    Delete
}


public enum Status
{
    Success,
    Failure,
    Information,
    Warning

}

