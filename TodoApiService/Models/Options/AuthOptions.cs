using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TodoApiService.Models.Options
{
    public class JWTAuthOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
        ///<summary>Token lifetime property in seconds</summary>
        public int TokenLifeTime { get; set; }
        public SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));
    }
}