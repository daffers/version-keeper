using System.Collections.Generic;
using System.Web.Http;

namespace VersionKeeperWebApi.Controllers
{
    public class RootController : ApiController
    {
        
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

    }
}
