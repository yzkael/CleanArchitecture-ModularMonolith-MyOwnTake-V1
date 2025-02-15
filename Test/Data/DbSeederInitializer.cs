using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Test.Configurations.Models;
using Test.Entities;

namespace Test.Data
{
    public static class InitialiserExtensions
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var initialiser = scope.ServiceProvider.GetRequiredService<DbSeederInitializer>();

            await initialiser.InitialiseAsync();

            await initialiser.SeedAsync();
        }
    }

    public class DbSeederInitializer
    {
        private readonly ILogger<DbSeederInitializer> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SudoData _sudoData;

        public DbSeederInitializer(ILogger<DbSeederInitializer> logger, AppDbContext context, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, IOptions<SudoData> sudoData)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _sudoData = sudoData.Value;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                await _context.Database.MigrateAsync();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }
        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }

        }

        public async Task TrySeedAsync()
        {
            if (_sudoData == null)
            {
                _logger.LogError("Sudo data is not configured.");
                throw new ArgumentNullException(nameof(_sudoData));
            }

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
                    var result = await _context.Users.AddAsync(newUsuario);
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
                if (usuario != null && role != null && !await _userManager.IsInRoleAsync(usuario, role.Name!))
                {
                    await _userManager.AddToRoleAsync(usuario, role.Name!);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

    }
}