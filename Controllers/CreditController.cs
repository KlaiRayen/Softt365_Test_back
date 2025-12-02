using System.Web.Http;
using Softt365Assessment.Models.DTOs;
using Softt365Assessment.Services;

namespace Softt365Assessment.Controllers
{
    [RoutePrefix("api/credit")]
    public class CreditController : ApiController
    {
        private readonly ICalculCreditService _service;

        public CreditController()
        {
            _service = new CalculCreditService();
        }


        [HttpPost]
        [Route("calculer")]
        public IHttpActionResult Calculer([FromBody] CalculCreditRequestDto request)
        {
            if (request == null)
                return BadRequest("Les données envoyées sont vides.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _service.Calculer(request);

            return Ok(result);
        }
    }
}
