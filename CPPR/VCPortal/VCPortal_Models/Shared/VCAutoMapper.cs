using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPortal_API.MapperProfiles.ChemoPx;
using VCPortal_Models.Dtos.ChemoPx;
using VCPortal_Models.MapperProfiles.ActiveDirectory;

namespace VCPortal_Models.Shared;
public class VCAutoMapper
{
    public static U AutoMapChemotherapyPX<T, U>(T input)
    {

        //SETUP MAPPING FOR DTO
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ChemotherapyPX_Profile());
        });
        var mapper = mapperConfig.CreateMapper();

        var output = mapper.Map<U>(input);
        return output;
    }



    public static U AutoMapUserAccess<T, U>(T input)
    {

        //SETUP MAPPING FOR DTO
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new UserAccessProfile());
        });
        var mapper = mapperConfig.CreateMapper();

        var output = mapper.Map<U>(input);
        return output;
    }



    public static List<U> AutoMapChemotherapyPX<T, U>(List<T> input)
    {

        //SETUP MAPPING FOR DTO
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ChemotherapyPX_Profile());
        });
        var mapper = mapperConfig.CreateMapper();

        var output = mapper.Map<List<U>>(input);
        return output;
    }


}
