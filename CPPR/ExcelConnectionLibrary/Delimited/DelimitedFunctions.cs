using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileParsingLibrary.Delimited;
public class DelimitedFunctions
{
    public List<KeyValuePair<string,string>> Mappings { get; set; }    

    public List<T> ImportDelimitedFile<T>(string filename, char delimiter)
    {
        List<T> list = new List<T>();
        List<string> lines = System.IO.File.ReadAllLines(filename).ToList();
        string headerLine = lines[0];
        var headerInfo = headerLine.Split(delimiter).ToList().Select((v,i) => new {ColName = v, ColIndex = i });

        Type type = typeof(T);
        var properties = type.GetProperties();

        var dataLines = lines.Skip(1);
        dataLines.ToList().ForEach(line =>
        {
            var values = line.Split(delimiter);
            T  obj = (T)Activator.CreateInstance(type);

            //SET VALUES TO OBJ PROPERTIES FROM DELIMITED COLUMNS
            foreach(var prop in properties)
            {
                //FIND MAPPING FOR THE PROP
                var mapping = Mappings.SingleOrDefault(m=>m.Value== prop.Name);
                var colName = mapping.Key;
                var colIndex = headerInfo.SingleOrDefault(s=>s.ColName == colName).ColIndex;
                var value = values[colIndex];
                var propType = prop.PropertyType;
                prop.SetValue(obj, Convert.ChangeType(value, propType));
            }
            list.Add(obj);

        });

        return list;
    }



}
