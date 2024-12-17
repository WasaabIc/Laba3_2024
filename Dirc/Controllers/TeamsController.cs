using Dirc.Domain.Entities;
using Dirc.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dirc.WebApi.Controllers
{
    [ApiController]
    [Route("teams")]
    public class TeamsController : ControllerBase
    {
        private readonly ILogger<TeamsController> logger;
        private readonly IRepository<Team> teamRepository;

        public TeamsController(ILogger<TeamsController> logger, IRepository<Team> teamRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> Get()
        {
            logger.LogInformation("Getting all teams");
            return Ok(await teamRepository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> Get(Guid id)
        {
            var team = await teamRepository.GetByIdAsync(id);
            if (team == null) return NotFound($"Команда с ID {id} не найдена.");

            return Ok(team);
        }

        [HttpPost]
        public async Task<ActionResult> Insert([FromBody] Team team)
        {
            if (string.IsNullOrWhiteSpace(team?.TeamName))
                return BadRequest("Имя команды не может быть пустым.");

            if (!team.Id.HasValue)
                team.Id = Guid.NewGuid();

            var result = await teamRepository.InsertAsync(team);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] Team team)
        {
            if (string.IsNullOrWhiteSpace(team?.TeamName))
                return BadRequest("Имя команды не может быть пустым.");

            if (await teamRepository.GetByIdAsync(id) == null)
                return NotFound($"Команда с ID {id} не существует.");

            await teamRepository.UpdateAsync(id, team);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            if (await teamRepository.GetByIdAsync(id) == null)
                return NotFound($"Команда с ID {id} не существует.");

            await teamRepository.DeleteAsync(id);
            return Ok();
        }
    }
}
