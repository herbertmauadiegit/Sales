using Microsoft.AspNetCore.Mvc;
using Sales.Application;
using Sales.Domain;

namespace Sales.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalesController(ISaleService _saleService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateSale([FromBody] SaleRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            try
            {
                return Ok(await _saleService.CreateSale(request));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{saleNumber}")]
        public async Task<IActionResult> UpdateSale(int saleNumber, [FromBody] UpdateSaleRequest request)
        {
            try
            {
                return Ok(await _saleService.UpdateSale(saleNumber, request));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{saleNumber}/cancel")]
        public async Task<IActionResult> CancelSale(int saleNumber)
        {
            try
            {
                await _saleService.CancelSale(saleNumber);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{saleNumber}")]
        public async Task<IActionResult> GetSale(int saleNumber)
        {
            try
            {
                return Ok(await _saleService.GetSale(saleNumber));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete()]
        [Route("DeQueue")]
        public async Task<IActionResult> DeQueue()
        {
            try
            {
                var queue = await _saleService.DeQueue();
                if (queue is not null)
                    return Ok(await _saleService.DeQueue());
                else
                    return BadRequest("Queue has no elements");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
