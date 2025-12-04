using System.Web.Http;

[RoutePrefix("")]
public class RootController : ApiController
{
    [HttpGet]
    [Route("")]
    public IHttpActionResult Get()
    {
        return Ok("Projet exécuté avec succès ! Visitez /swagger pour tester les APIs.");
    }
}
