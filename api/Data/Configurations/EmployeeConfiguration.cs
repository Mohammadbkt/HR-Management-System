using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.api.Data.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.HasKey(e => e.UserId);

            builder.Property(e => e.SSN)
                .HasMaxLength(20)
                .IsRequired();

        
            
            builder.Property(e => e.WorkEmail)
                .HasMaxLength(255);

            
            builder.Property(e => e.WorkPhone)
                .HasMaxLength(20);

            builder.Property(e => e.Gender)
                .HasMaxLength(1);


            // Indexes
            builder.HasIndex(e => e.SSN).IsUnique();
            builder.HasIndex(e => e.WorkEmail).IsUnique();
            builder.HasIndex(e => e.WorkPhone).IsUnique();

            builder.ToTable(t => t.HasCheckConstraint("CK_Employee_Gender", "[Gender]='F' OR [Gender]='M'"));
            builder.ToTable(t => t.HasCheckConstraint("CK_Employee_StartDate", "[StartDate] > DATEADD(year, 16, [BirthDate])"));



            builder.HasOne(e => e.Manager)
                .WithMany()
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Position)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.NoAction);





        }
    }
}