using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedFunctionsLibrary.Properties;

public static class AutoMapping<TSource, TDestination>
{
    private static Mapper _mapper = new Mapper(new MapperConfiguration(
        cfg => cfg.CreateMap<TSource, TDestination>()
        ));

    public static TDestination Map(TSource source)
    {
        return _mapper.Map<TDestination>(source);
    }


    public static List<TDestination> MapList(List<TSource> source)
    {
        
        var destination = new List<TDestination>();
        try
        {
            source.ForEach(x => { destination.Add(Map(x)); });
        }
        catch (Exception ex)
        {
            var s = ex.ToString();
        }

        return _mapper.Map<List<TDestination>>(source);
    }

}
