using AutoMapper;

namespace HospitalMS.AutoMapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile() {
            CreateMap<RegisterViewModel, ApplicationUser>()
                .ForMember(dest => dest.PasswordHash, src => src.MapFrom(x => x.Password))
                .ForMember(dest => dest.PhoneNumber, src => src.MapFrom(x => x.Phone))
                .ForMember(dest => dest.UserName, src => src.MapFrom(x => x.UserName));
            CreateMap<Doctor, ApplicationUser>()
                .ForMember(dest => dest.PasswordHash, src => src.MapFrom(x => x.Password))
                .ForMember(dest => dest.PhoneNumber, src => src.MapFrom(x => x.Phone))
                .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToString()));
            CreateMap<Nurse, ApplicationUser>()
                .ForMember(dest => dest.PasswordHash, src => src.MapFrom(x => x.Password))
                .ForMember(dest => dest.PhoneNumber, src => src.MapFrom(x => x.Phone))
                .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToString()));

            CreateMap<Doctor, AdminNurseViewModel>();
            CreateMap<AdminNurseViewModel,Doctor>();
            CreateMap<AdminNurseViewModel, Nurse>();
            CreateMap<Nurse, AdminNurseViewModel>();
            CreateMap<ApplicationUser, Patient>()
                .ForMember(dest => dest.Password, src => src.MapFrom(x => x.PasswordHash))
                .ForMember(dest => dest.Phone, src => src.MapFrom(x => x.PhoneNumber))
                .ForMember(dest => dest.Username, src => src.MapFrom(x => x.UserName));
                

        }
    }
}
