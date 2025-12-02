using System.Web.Http;

[RoutePrefix("")]
public class RootController : ApiController
{
    [HttpGet]
    [Route("")]
    public IHttpActionResult Get()
    {
        return Ok("Utilisez /api/credit/calculer pour tester.");
    }
}
