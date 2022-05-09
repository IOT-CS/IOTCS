using System;

namespace IOTCS.EdgeGateway.Infrastructure.WebApi
{
    public class CustomerException:SystemException
    {
        public CustomerException():base()
        {

        }
        public CustomerException(string message):base(message)
        {

        }

        public CustomerException(string message, Exception innerException):base(message,innerException)
        {

        }

    }
}
