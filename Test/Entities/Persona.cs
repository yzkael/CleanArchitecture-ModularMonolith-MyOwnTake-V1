using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Entities
{
    public class Persona : EntityBase
    {
        public string Nombre { get; set; } = null!;
        public string ApellidoPaterno { get; set; } = null!;
        public string ApellidoMaterno { get; set; } = null!;
        public string Carnet { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
    }
}