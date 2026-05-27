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
        private readonly ILogger<ChipPressureController> _logger;

        public ChipPressureController(IDbContextFactory<MxTWebContext> dbFactory, ILogger<ChipPressureController> logger)
        {
            _dbFactory = dbFactory;
            _logger = logger;
        }

        [HttpGet]  // 임시 테스트용
        public IActionResult Get()
        {
            return Ok("ChipPressure Controller is working!");
        }

        [HttpPost] // HTTP POST 요청만 받음
        public async Task<IActionResult> PostLog([FromBody] ChipPressure data)
        {
            if (data == null)
                return BadRequest("Data is null");

            // JSON 데이터가 자동으로 매핑
            var pressureData = new ChipPressure
            {
                ProduceDate = data.ProduceDate,
                TestDate = data.TestDate,
                WaferInfo = data.WaferInfo,
                Lot = data.Lot,
                Number = data.Number,
                Pressure = data.Pressure
            };

            try
            {
                using var context = _dbFactory.CreateDbContext();
                context.ChipPressure.Add(pressureData);
                await context.SaveChangesAsync();
                return Ok(pressureData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChipPressure 저장 실패. Wafer={WaferInfo}, Lot={Lot}", data.WaferInfo, data.Lot);
                return StatusCode(500, "Internal server error while saving data.");
            }
        }
    }
}
