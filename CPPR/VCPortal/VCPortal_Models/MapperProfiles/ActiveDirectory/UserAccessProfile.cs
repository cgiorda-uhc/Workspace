using ActiveDirectoryLibrary;
using AutoMapper;
using VCPortal_Models.Models.ActiveDirectory;

namespace VCPortal_Models.MapperProfiles.ActiveDirectory;
public class UserAccessProfile : Profile
{
    public UserAccessProfile()
    {
        CreateMap<ADUserModel, UserAccessModel>();
    }

      
}
