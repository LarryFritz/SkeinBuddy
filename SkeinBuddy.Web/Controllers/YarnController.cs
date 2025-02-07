using Microsoft.AspNetCore.Mvc;
using SkeinBuddy.DataAccess.Repositories;
using SkeinBuddy.Models;
using SkeinBuddy.Queries;
using SkeinBuddy.Web.ViewModels.Yarn;

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

        [HttpGet("get-all")]
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

        [HttpGet]
        public async Task<PagedResult<Yarn>> QueryAsync([FromQuery]YarnQuery? query)
        {
            return await _yarnRepository.QueryAsync(query);
        }

        [HttpPost]
        public async Task<ActionResult<Yarn>> CreateAsync(CreateYarnViewModel model, CancellationToken cancellationToken)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Yarn yarn = new Yarn();
            yarn.Weight = model.Weight;
            yarn.Name = model.Name;
            yarn.BrandId = model.BrandId;

            await _yarnRepository.CreateAsync(yarn, cancellationToken);

            return yarn;
        }

        [HttpGet("ai-search")]
        public async Task<List<Yarn>> AiSearch([FromQuery] string text, CancellationToken cancellationToken)
        {
            return await _yarnRepository.QueryNearestNeighborsByName(text, cancellationToken);
        }
    }
}
