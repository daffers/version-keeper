using System.Collections.Generic;
using System.Web.Http;
using WebApi.Hal;
using WebApi.Hal.JsonConverters;

namespace VersionKeeperWebApi.Controllers
{
    public class RootController : ApiController
    {
        
        public Representation Get()
        {
            return new Blank()
            {
                Links =
                {
                    new Link("VersionedApplications", "~/applications")
                }
            };
        }

    }

    public class Blank : Representation
    {
        public Blank()
        {
            Rel = "Root";
        }

        protected override void CreateHypermedia()
        {
            Href = "~";
        }
    }
}
