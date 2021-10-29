using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdOps.IdentityServer.RabbitMQ
{
    public class RabbitMqOptions
    {
        public string Host { get; set; } = default!;

        public string Username { get; set; } = default!;

        public string Password { get; set; } = default!;
    }
}
