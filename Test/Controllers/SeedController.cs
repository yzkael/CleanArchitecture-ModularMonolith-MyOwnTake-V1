using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Test.Configurations.Models;
using Test.Data;
using Test.Entities;

namespace Test.Controllers
{
    [ApiController]
    [Route("api/seed")]
    public class SeedController : ControllerBase
    {
        private readonly ILogger<DbSeederInitializer> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SudoData _sudoData;
        public SeedController(ILogger<DbSeederInitializer> logger, AppDbContext context, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, IOptions<SudoData> sudoData)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _sudoData = sudoData.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Test()
        {
            try
            {
                var newPersona = new Persona
                {
                    Nombre = _sudoData.Nombre,
                    ApellidoPaterno = _sudoData.ApellidoPaterno,
                    ApellidoMaterno = _sudoData.ApellidoMaterno,
                    Carnet = _sudoData.Carnet,
                    Telefono = _sudoData.Telefono
                };
                if (await _context.Personas.FirstOrDefaultAsync(p => p.Carnet == newPersona.Carnet) == null)
                {
                    var result = await _context.Personas.AddAsync(newPersona);
                    await _context.SaveChangesAsync();
                }
                var personaId = await _context.Personas.Where(p => p.Carnet == _sudoData.Carnet).Select(p => p.Id).FirstOrDefaultAsync();

                var passwordHasher = new PasswordHasher<Usuario>();
                var newUsuario = new Usuario
                {
                    UserName = _sudoData.Username,
                    Email = _sudoData.Email,
                    PasswordHash = passwordHasher.HashPassword(null!, _sudoData.Password),
                    PersonaId = personaId
                };
                if (await _userManager.FindByEmailAsync(_sudoData.Email) == null)
                {
                    var result = await _userManager.CreateAsync(newUsuario);
                    await _context.SaveChangesAsync();
                }
                var newRole = new IdentityRole
                {
                    Name = "Sudo",
                    NormalizedName = "SUDO"
                };
                if (await _roleManager.FindByNameAsync(newRole.Name) == null)
                {
                    var result = await _roleManager.CreateAsync(newRole);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"{result.Errors}");
                    }
                    await _context.SaveChangesAsync();
                }
                var usuario = await _userManager.FindByEmailAsync(_sudoData.Email);
                var role = await _roleManager.FindByNameAsync(newRole.Name);
                if (usuario != null && role != null)
                {
                    await _userManager.AddToRoleAsync(usuario, role.Name!);
                    await _context.SaveChangesAsync();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                return StatusCode(500, "damn");
            }
        }
    }
}