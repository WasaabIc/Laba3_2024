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
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> logger;
        private readonly IRepository<User> userRepository;

        public UsersController(ILogger<UsersController> logger, IRepository<User> userRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            logger.LogInformation("Getting all users");
            return Ok(await userRepository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(Guid id)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null) return NotFound($"Пользователь с ID {id} не найден.");

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult> Insert([FromBody] User user)
        {
            var validationResult = ValidateUser(user);
            if (!string.IsNullOrWhiteSpace(validationResult))
                return BadRequest(validationResult);

            if (!user.Id.HasValue)
                user.Id = Guid.NewGuid();

            user.CreatedAt = DateTime.UtcNow;

            var result = await userRepository.InsertAsync(user);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] User user)
        {
            var validationResult = ValidateUser(user);
            if (!string.IsNullOrWhiteSpace(validationResult))
                return BadRequest(validationResult);

            if (await userRepository.GetByIdAsync(id) == null)
                return NotFound($"Пользователь с ID {id} не существует.");

            await userRepository.UpdateAsync(id, user);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            if (await userRepository.GetByIdAsync(id) == null)
                return NotFound($"Пользователь с ID {id} не существует.");

            await userRepository.DeleteAsync(id);
            return Ok();
        }

        private string ValidateUser(User user)
        {
            if (user == null) return "Пользователь не может быть пустым.";
            if (string.IsNullOrWhiteSpace(user.Username)) return "Имя пользователя обязательно.";
            if (string.IsNullOrWhiteSpace(user.Password)) return "Пароль обязателен.";
            if (string.IsNullOrWhiteSpace(user.Role)) return "Роль обязательна.";

            return string.Empty;
        }
    }
}
