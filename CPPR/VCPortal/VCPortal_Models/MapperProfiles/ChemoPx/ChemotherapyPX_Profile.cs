using AutoMapper;
using VCPortal_Models.Dtos.ChemoPx;
using VCPortal_Models.Models.ChemoPx;
using VCPortal_Models.Models.Shared;

namespace VCPortal_API.MapperProfiles.ChemoPx;

public class ChemotherapyPX_Profile : Profile
{
    public ChemotherapyPX_Profile()
    {


        //MAP OUR DTO TO OUR MODEL VIA AuotMapper
        //SOURCE -> TARGET
        CreateMap<ChemotherapyPXModel, ChemotherapyPX_ReadDto>();
        CreateMap<ChemotherapyPX_CreateDto, ChemotherapyPXModel>();
        CreateMap<ChemotherapyPX_UpdateDto, ChemotherapyPXModel>();
        CreateMap<ChemotherapyPX_ReadDto, ChemotherapyPX_UpdateDto>();
        CreateMap<ChemotherapyPX_UpdateDto, ChemotherapyPX_ReadDto>();
        CreateMap<ChemotherapyPX_CreateDto, ChemotherapyPX_ReadDto>();


    }
}

