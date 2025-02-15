using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Entities;

namespace Test.Data.Configuration
{
    public class PersonaConfiguration : IEntityTypeConfiguration<Persona>
    {
        public void Configure(EntityTypeBuilder<Persona> builder)
        {
            builder.HasIndex(p => p.Carnet).IsUnique();

            builder.Property(p => p.Nombre).IsRequired();
            builder.Property(p => p.ApellidoPaterno).IsRequired();
            builder.Property(p => p.ApellidoMaterno).IsRequired();
            builder.Property(p => p.Carnet).IsRequired();
            builder.Property(p => p.Telefono).IsRequired();
        }
    }
}