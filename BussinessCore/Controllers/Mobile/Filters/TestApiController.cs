using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DAL.Data;
using BussinessCore.API.Filters;
using DAL.Models;

namespace BussinessCore.API.Controllers
{
    [TypeFilter(typeof(ChequeaUatApiAttribute))]
    [ApiController]
    [Route("api/[controller]")]
    public class TestApiController : BaseApiController
    {

        public TestApiController(FINADIContext context) : base(context)
        {
        }

        [HttpGet]
        [Route("Obtener")]
        public async Task<IActionResult> Get()
        {
            return Ok(new JsonResult(new { p1 = 2, p2 = "aaa" }));
        }

        [HttpPost]
        [Route("Grabar")]
        public async Task<IActionResult> Post([FromBody] RespuestaAPI objeto)
        {
            return Ok(new JsonResult(objeto));
        }

    }
}
