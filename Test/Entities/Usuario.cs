using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Test.Entities
{
    public class Usuario : IdentityUser
    {
        public Guid PersonaId { get; set; }
        public Persona Persona { get; set; } = null!;
    }
}