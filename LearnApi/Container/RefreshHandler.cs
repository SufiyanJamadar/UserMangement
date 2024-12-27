using LearnApi.Repos;
using LearnApi.Repos.Models;
using LearnApi.Service;
using System.Security.Cryptography;

namespace LearnApi.Container
{
    public class RefreshHandler : IRefreshHandler
    {
        private readonly LearndataContext context;
        public RefreshHandler(LearndataContext context) 
        {
          this.context = context;
        }

        public async Task<string> GenerateToken(string username)
        {
            var randomnumber = new byte[32];
            using(var randomnumbergenerate=RandomNumberGenerator.Create())
            {
                randomnumbergenerate.GetBytes(randomnumber);
                string refreshtoken=Convert.ToBase64String(randomnumber);
                var Existtoken = this.context.TblRefreshtokens.FirstOrDefault(item => item.Userid == username);
                 if(Existtoken is not null)
                 {
                    Existtoken.Refreshtoken = refreshtoken;
                 }
                else
                {
                     await this.context.TblRefreshtokens.AddAsync(new TblRefreshtoken{
                         Userid=username,
                         Tokenid=new Random().Next().ToString(),
                         Refreshtoken=refreshtoken
                     });
                }
                await this.context.SaveChangesAsync();

                return refreshtoken;
            }
        }
    }
}
