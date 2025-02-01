using Microsoft.AspNetCore.Mvc;
using SkeinBuddy.DataAccess.Repositories;
using SkeinBuddy.Models;

namespace SkeinBuddy.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class YarnController : ControllerBase
    {
        private readonly ILogger<YarnController> _logger;
        private readonly YarnRepository _yarnRepository;

        public YarnController(ILogger<YarnController> logger, YarnRepository yarnRepository)
        {
            _logger = logger;
            _yarnRepository = yarnRepository;
        }

        [HttpGet(Name = "GetAll")]
        public async Task<IEnumerable<Yarn>> GetAllAsync()
        {
            var test = await _yarnRepository.GetAllAsync();
            return test;
        }

        [HttpGet("{id}")]
        public async Task<Yarn> GetByIdAsync(Guid id)
        {
            return await _yarnRepository.GetByIdAsync(id);
        }
    }
}
