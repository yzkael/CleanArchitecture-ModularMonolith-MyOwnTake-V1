using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Entities;

namespace Test.Data.Configuration
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasIndex(u => u.Email).IsUnique();

            builder.HasOne(u => u.Persona)
            .WithOne(p => p.Usuario)
            .HasForeignKey<Usuario>(u => u.PersonaId);
        }
    }
}