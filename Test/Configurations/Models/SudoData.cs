using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Configurations.Models
{
    public class SudoData
    {
        public const string Section = "SudoData";
        public string Nombre { get; set; } = null!;
        public string ApellidoPaterno { get; set; } = null!;
        public string ApellidoMaterno { get; set; } = null!;
        public string Carnet { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}