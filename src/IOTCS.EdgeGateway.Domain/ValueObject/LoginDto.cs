using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.ValueObject
{
    public class LoginDto
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string ValidCode { get; set; }

        public string Token { get; set; }

        public string DisplayName { get; set; }

    }
}
