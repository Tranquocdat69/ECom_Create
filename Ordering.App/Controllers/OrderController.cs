using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTS.FIT.BDRD.Services.Ordering.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            OrderDTO orderDTO = await _mediator.Send(command);
            return Ok(orderDTO);
        }
    }
}
