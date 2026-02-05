using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.api.Data.Configurations
{
    public class RequestBalanceConfiguration : IEntityTypeConfiguration<RequestBalance>
    {
        public void Configure(EntityTypeBuilder<RequestBalance> builder)
        {
            builder.ToTable("RequestBalances");
            
            builder.HasKey(rb => new { rb.EmployeeId, rb.RequestTypeId, rb.Year });
            
            builder.Property(rb => rb.TotalDays)
                .HasPrecision(18, 2)
                .IsRequired();
            
            builder.Property(rb => rb.UsedDays)
                .HasPrecision(18, 2)
                .IsRequired()
                .HasDefaultValue(0);
            
            builder.Property(rb => rb.RemainingDays)
                .HasPrecision(18, 2)
                .IsRequired();
            
            builder.ToTable(rb => rb.HasCheckConstraint(
                "CK_RequestBalance_RemainingDays", 
                "[RemainingDays] >= 0"));
            
            builder.ToTable(rb => rb.HasCheckConstraint(
                "CK_RequestBalance_UsedDays", 
                "[UsedDays] <= [TotalDays]"));
            
            builder.HasOne(rb => rb.Employee)
                .WithMany()  
                .HasForeignKey(rb => rb.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(rb => rb.RequestType)
                .WithMany()
                .HasForeignKey(rb => rb.RequestTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasIndex(rb => new { rb.EmployeeId, rb.Year });
            builder.HasIndex(rb => rb.Year);
        }
    }
}