using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Imagegram.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace Imagegram.Helpers
{
    public static class AuthHelper
    {
        public static int logged_in_userid;
        public static string logged_in_username;

        public static string GenerateToken(Account account)
        {

            try
            {

                var tokenHandler = new JwtSecurityTokenHandler();

                var tokenKey = Encoding.ASCII.GetBytes(ConfigHelper.GetValue("JwtKey"));

                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(
                        new Claim[]
                        {
                        new Claim("id", account.Id.ToString()),
                        new Claim("name", account.Name),
                        }),

                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static JwtSecurityToken ValidateToken(HttpRequest request)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var key = Encoding.ASCII.GetBytes(ConfigHelper.GetValue("JwtKey"));

                string token = request.Headers["Authorization"];

                token = token != null ? token.Split(new char[] { ' ' })[1] : "";

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken rawValidatedToken);

                JwtSecurityToken validatedToken = (JwtSecurityToken)rawValidatedToken;

                logged_in_userid = Convert.ToInt32(validatedToken.Claims.ToList().Find(x => x.Type == "id").Value);

                logged_in_username = validatedToken.Claims.ToList().Find(x => x.Type == "name").Value;

                return validatedToken;
            }
            catch (SecurityTokenValidationException ex)
            {
                throw new SecurityTokenValidationException(ex.Message);
            }
            catch (ArgumentException ex)
            {
                throw new SecurityTokenValidationException(ex.Message);
            }
        }
    }
}
