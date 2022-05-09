using IOTCS.EdgeGateway.Core.Security;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace IOTCS.EdgeGateway.Application.Imps
{
    public class AuthorizationService : IAuthorizationService
    {
        private JwtSettingsDto jwtSettings;
        private readonly ILogger _logger;
        private readonly IUserService _service;

        public AuthorizationService(IServiceProvider services
            , IConfiguration config
            , ILogger logger
            , IUserService service)
        {
            var jwtSettings = new JwtSettingsDto 
            {
                SecurityKey = config["Jwt:SecurityKey"],
                Issuer = config["Jwt:Issuer"],
                Audience = config["Jwt:Audience"],
                LifeTime = 12
            };
            
            this.jwtSettings = jwtSettings;
            this._logger = logger;
            this._service = service;
        }

        public UserDto GenerateToken(string name, string password)
        {
            UserDto userInfo = null;

            try
            {
                userInfo = _service.GetUserInfoByName(name).ConfigureAwait(false).GetAwaiter().GetResult();
                var enPassword = MD5Helper.GenerateMd5String(password);
                if (userInfo == null || userInfo.Password != password)
                {
                    return null;
                }

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim("name", name.ToString()),
                    new Claim("id",userInfo.Id.ToString()),
                    new Claim("admin","true",ClaimValueTypes.Boolean)
                 };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expires = DateTime.Now.AddHours(jwtSettings.LifeTime);
                var jwttoken = new JwtSecurityToken(
                    issuer: jwtSettings.Issuer,
                    audience: jwtSettings.Audience,
                    claims: claims,
                    notBefore: DateTime.Now,
                    expires: expires,
                    signingCredentials: creds);
                userInfo.Token = new JwtSecurityTokenHandler().WriteToken(jwttoken);
            }
            catch (Exception ex)
            {
                _logger.Error($"GenerateToken 失败：{ex.Message},{ex.StackTrace}");                
            }

            return userInfo;
        }

        public bool ValidateToken(JToken token)
        {
            var result = false;
            string tokens = token.ToString();
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(tokens);
            var id = jwtToken.Claims.FirstOrDefault(m => m.Type == "id");
            var name = jwtToken.Claims.FirstOrDefault(m => m.Type == "name");
            var iss = jwtToken.Claims.FirstOrDefault(m => m.Type == "iss");
            var aud = jwtToken.Claims.FirstOrDefault(m => m.Type == "aud");
            var exp = jwtToken.Claims.FirstOrDefault(m => m.Type == "exp");
            if (null == id || null == name || null == iss || null == aud || null == exp
                || aud.Value.ToString() != jwtSettings.Audience || iss.Value.ToString() != jwtSettings.Issuer
                )
            {
                result = false;
            }
            else
            {
                result = true;
            }
            _logger.Info("user name={0},id={1} in token", name.Value.ToString(), id.Value.ToString());
            return result;
        }
    }
}
