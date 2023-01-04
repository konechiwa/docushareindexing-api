using System.Text;
using DocuShareIndexingAPI.Entities;
using DocuShareIndexingAPI.Interface;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System;


namespace DocuShareIndexingAPI.Service
{
    public class TokenService : ITokenService
    {

        /**
        * @notice readonly variables.
        */
        private readonly SymmetricSecurityKey _key;


        /**
        * @notice the constructor class.
        */
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }


        public string createToken(User user)
        {
            // 1. Create claims collection.
            var claims = new List<Claim> 
            {   
                new Claim(JwtRegisteredClaimNames.NameId, user.Username),
            };

            // 2. Create credentials from SymmetricSecurityKey.
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);


            // 3. Create Token Descriptor from claims and credentials.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddYears(2),
                SigningCredentials = creds,
            };

            // 4. Create token handler from JwtSecurityTokenHandler.
            var tokenHandler = new JwtSecurityTokenHandler();


            // 5. Create security token from token handler.
            var token = tokenHandler.CreateToken(tokenDescriptor);


            // Final the token string.
            return tokenHandler.WriteToken(token);
        }
    }
}