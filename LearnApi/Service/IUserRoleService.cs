using LearnApi.Helper;
using LearnApi.Modal;
using LearnApi.Repos.Models;

namespace LearnApi.Service
{
    public interface IUserRoleService
    {
        Task<APIResponse> AssignRolePermission(List<TblRolepermission> _data);

        Task<List<TblRole>> GetAllRoles();

        Task<List<TblMenu>> GetAllMenus();

        Task<List<Appmenu>> GetAllMenubyRole(string userrole);

        Task<Menupermission> GetMenupermissionbyrole(string userrole,string menucode);

    }
}
