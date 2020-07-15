using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HiringManagerServer.Application
{
    public class TokenConfiguration
    {
        public TokenConfiguration(IConfiguration configuration)
        {
            this.SignKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["AuthToken:secretKey"]));
            this.Credentials = new SigningCredentials(this.SignKey, SecurityAlgorithms.HmacSha256);
            this.ExpiryInMinutes = int.Parse(configuration["AuthToken:expiry"]);
        }

        public SymmetricSecurityKey SignKey { get; }

        public SigningCredentials Credentials { get; }

        public int ExpiryInMinutes { get; }
    }
}