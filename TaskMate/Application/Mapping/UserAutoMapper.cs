using AutoMapper;
using TaskMate.Core.DTO.AuthDTO;
using TaskMate.Core.Models;

namespace TaskMate.Application.Mapping
{
    public class UserAutoMapper : Profile
    {
        public UserAutoMapper()
        {
            //Adding Mappings
            CreateMap<RegisterDTO, User>(); //DTO -> Model
            CreateMap<LoginDTO, User>(); //DTO -> Model
        }

    }
}
