using LearnApi.Helper;
using LearnApi.Modal;
using LearnApi.Repos.Models;

namespace LearnApi.Service
{
    public interface ICustomerService
    {
        Task <List<Customermodal>> GetAll();
        Task<Customermodal> GetByCode(string code);
        Task<APIResponse> Remove(string code);
        Task<APIResponse> Create(Customermodal data);

        Task<APIResponse> Update(Customermodal data,string code);

    }
}
