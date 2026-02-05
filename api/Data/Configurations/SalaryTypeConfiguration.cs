using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HRMS.api.Data.Configurations
{
    public class SalaryTypeConfiguration : IEntityTypeConfiguration<SalaryType>
    {
        public void Configure(EntityTypeBuilder<SalaryType> builder)
        {
            builder.ToTable("SalaryTypes");

            builder.HasKey(st => st.Id);

            builder.Property(st => st.Category)
                .HasMaxLength(20);

            builder.Property(st => st.SubType)
                .HasMaxLength(50)
                .IsRequired();

            var converter = new ValueConverter<SalaryCategory, string>(
                v => v.ToString(),
                v => (SalaryCategory)Enum.Parse(typeof(SalaryCategory), v));

        }
    }
}