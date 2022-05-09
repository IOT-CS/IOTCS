using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.ValueObject
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
        public string Creator { get; set; }
        public string CreateTime { get; set; }
        public string Updator { get; set; }
        public string UpdatedTime { get; set; }
        public string DisplayName { get; set; }
    }
}
