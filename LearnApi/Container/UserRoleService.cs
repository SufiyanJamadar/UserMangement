using LearnApi.Helper;
using LearnApi.Modal;
using LearnApi.Repos;
using LearnApi.Repos.Models;
using LearnApi.Service;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace LearnApi.Container
{
    public class UserRoleService : IUserRoleService
    {
        private readonly LearndataContext context;
        public UserRoleService(LearndataContext learndata)
        {
            this.context = learndata;
        }
        public async Task<APIResponse> AssignRolePermission(List<TblRolepermission> _data)
        {
            APIResponse response = new APIResponse();
            int processcount = 0;
            try
            {
                using(var dbtransactions=await this.context.Database.BeginTransactionAsync())
                {
                    if (_data.Count > 0)
                    {
                        _data.ForEach(item =>
                        {
                            var userdata=this.context.TblRolepermissions.FirstOrDefault(item1=>item1.Userrole==item.Userrole &&
                            item1.Menucode==item.Menucode);
                             if(userdata!=null)
                             {
                                userdata.Haveview = item.Haveview;
                                userdata.Haveadd = item.Haveadd;
                                userdata.Haveedit = item.Haveedit;
                                userdata.Havedelete= item.Havedelete;
                                processcount++;
                             }
                            else
                            {
                                this.context.TblRolepermissions.Add(item);
                                processcount++;
                            }
                        });


                        // check count is equal to process count
                        if (_data.Count == processcount)
                        {
                            await this.context.SaveChangesAsync();
                            await dbtransactions.CommitAsync();
                            response.Result = "Pass";
                            response.Message = "Saved Sucessfully";
                        }
                        else
                        {
                            await dbtransactions.RollbackAsync();
                        }

                    }
                    else
                    {
                        response.Result = "Fail";
                        response.Message = "Failed";
                    }
                }
                
            }
            catch(Exception ex)
            {
                 response= new APIResponse();
            }
            return response;
        }

        public async Task<List<Appmenu>> GetAllMenubyRole(string userrole)
        {
           List<Appmenu> appmenu=new List<Appmenu>();

            var accessdata = (from menu in this.context.TblRolepermissions.Where(o => o.Userrole == userrole && o.Haveview)
                              join m in this.context.TblMenus on menu.Menucode equals m.Code into _jointable
                              from p in _jointable.DefaultIfEmpty()
                              select new { code = menu.Menucode, name = p.Name }).ToList();

            if (accessdata.Any())
            {
                accessdata.ForEach(item =>
                {
                    appmenu.Add(new Appmenu()
                    {
                        code = item.code,
                        Name = item.name
                    });
                });
            }

            return appmenu;

        }

        public async Task<List<TblMenu>> GetAllMenus()
        {
            return await this.context.TblMenus.ToListAsync();
        }

        public async Task<List<TblRole>> GetAllRoles()
        {
            return await this.context.TblRoles.ToListAsync();
        }

        public async Task<Menupermission> GetMenupermissionbyrole(string userrole, string menucode)
        {
            Menupermission menupermission = new Menupermission();
            var _data = await this.context.TblRolepermissions.FirstOrDefaultAsync(o => o.Userrole == userrole && o.Haveview
             && o.Menucode == menucode);
            if (_data != null)
            {
                menupermission.code = _data.Menucode;
                menupermission.Haveview = _data.Haveview;
                menupermission.Haveadd = _data.Haveadd;
                menupermission.Haveedit = _data.Haveedit;
                menupermission.Havedelete = _data.Havedelete;
            }
            return menupermission;
        }
    }
}
