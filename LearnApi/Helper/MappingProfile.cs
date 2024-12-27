using AutoMapper;
using LearnApi.Modal;
using LearnApi.Repos.Models;

namespace LearnApi.Helper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<TblUser, UserModel>().
                ForMember(dest=>dest.Statusname,opt=>opt.MapFrom(src=>src.Isactive.HasValue && src.Isactive.Value ? "Active" : "InActive" ));
        }
    }
}
