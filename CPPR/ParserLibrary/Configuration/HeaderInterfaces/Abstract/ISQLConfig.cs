namespace ProjectManagerLibrary.Configuration.HeaderInterfaces.Abstract
{
    public interface ISQLConfig
    {
         string Name { get; set; }
       string ConnectionString { get; set; }
       DBType DBType { get; set; }

        List<string> SQL { get; set; }

         SQLType SQLType { get; set; }

         SQLAction SQLAction { get; set; }

        string Schema { get; set; }

        int Limit { get; set; }
    }

}