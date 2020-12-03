using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2C_DRP.WebApi.Models
{
    public class IdentityClaims
    {

        public string signInName { get; set; }
        public string password { get; set; }
        public string displayName { get; set; }
        public string givenName { get; set; }
        public string surName { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static IdentityClaims Parse(string JSON)
        {
            return JsonConvert.DeserializeObject(JSON, typeof(IdentityClaims)) as IdentityClaims;
        }
    }
}
