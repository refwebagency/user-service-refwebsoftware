using AutoMapper;
using UserService.Models;
using UserService.Dtos;

namespace UserService.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserReadDto>();
            CreateMap<UserCreateDto, User>();
            CreateMap<UserUpdateDto, User>();

            CreateMap<Specialization, ReadSpecializationDTO>();
            CreateMap<CreateSpecializationDTO, Specialization>();
            CreateMap<UpdateSpecializationDTO, Specialization>();

            // Partie RabbitMQ
            CreateMap<User, UserUpdatedDto>();
            CreateMap<User, UserUpdateAsyncDto>();

            CreateMap<PublishedSpecializationDTO, Specialization>();
            CreateMap<Specialization, PublishedSpecializationDTO>();

            CreateMap<UpdatedSpecializationDTO, Specialization>();
        }
    }
}
