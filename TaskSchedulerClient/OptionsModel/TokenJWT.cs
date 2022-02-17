using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSchedulerClient.OptionsModel
{
    public class TokenJWT
    {

        public const string NameItem = "TokenStr";

        public TokenJWT(){ }

        public string Content { get; set; } = String.Empty;
        public string Status { get; set; } = String.Empty;

    }
}
