using IOTCS.EdgeGateway.Domain.ValueObject;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Application
{
    public interface IAuthorizationService
    {
        UserDto GenerateToken(string name, string password);

        bool ValidateToken(JToken token);
    }
}
