using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MxTWeb.Data;
using MxTWeb.Models;
using MxTWeb.Services;

namespace MxTWeb.Services
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiKeyAuth]
    public class ChipPressureController : ControllerBase
    {

        private readonly IDbContextFactory<MxTWebContext> _dbFactory;
        public ChipPressureController(IDbContextFactory<MxTWebContext> dbFactory) 
        {
            _dbFactory = dbFactory; 
        }

        [HttpGet]  // 임시 테스트용
        public IActionResult Get()
        {
            return Ok("ChipPressure Controller is working!");
        }

        [HttpPost] // HTTP POST 요청만 받음
        public async Task<IActionResult> PostLog([FromBody] ChipPressure data)
        {
            Console.WriteLine($"Received: {data?.Number}, {data?.WaferInfo}");

            if (data == null)
                return BadRequest("Data is null");

            // JSON 데이터가 자동으로 매핑
            var pressureData = new ChipPressure
            {
                Number = data.Number,
                WaferInfo = data.WaferInfo,
                ProduceDate = data.ProduceDate,
                TestDate = data.TestDate,
                Pressure = data.Pressure
            };

            try
            {
                using var context = _dbFactory.CreateDbContext();
                context.ChipPressure.Add(pressureData);
                await context.SaveChangesAsync();
                //return Ok("Chip pressure data saved successfully.");
                return Ok(pressureData);
            }
            catch (Exception ex)
            {
                //return StatusCode(500, "Internal server error while saving data.");
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
