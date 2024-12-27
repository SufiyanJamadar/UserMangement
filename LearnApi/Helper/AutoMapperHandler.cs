using AutoMapper;
using LearnApi.Modal;
using LearnApi.Repos.Models;

namespace LearnApi.Helper
{
    public class AutoMapperHandler:Profile
    {
        public AutoMapperHandler()
        {
            CreateMap<TblCustomer, Customermodal>().ForMember(item => item.Statusname, opt => opt.MapFrom(
                item => (item.IsActive.HasValue && item.IsActive.Value)? "Active" : "In active")).ReverseMap();
        }
    }
}
