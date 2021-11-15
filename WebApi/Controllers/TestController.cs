using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Common;
using Core.Dto;
using Core.Entity;
using Core.Interfaces.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IBaseService<Test, TestDto> _testService;

        public TestController(IBaseService<Test, TestDto> testService)
        {
            _testService = testService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var test = await _testService.GetAllAsync();
            return Ok(new ApiResponse<IList<TestDto>>(test));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var test = await _testService.GetByIdAsync(id);
            return Ok(new ApiResponse<TestDto>(test));
        }

        [HttpPost]
        public async Task<IActionResult> Create(TestDto testDto)
        {
            var test = await _testService.AddAsync(testDto);
            return Ok(new ApiResponse<TestDto>(test));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var test = await _testService.RemoveAsync(id);
            return Ok(new ApiResponse<bool>(test));
        }
    }
}
