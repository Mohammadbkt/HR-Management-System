using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.api.Data.Configurations
{
    public class OtpConfiguration : IEntityTypeConfiguration<Otp>
    {
        public void Configure(EntityTypeBuilder<Otp> builder)
        {
           builder.ToTable("Otps");
            
            builder.HasKey(o => o.Id);
            

            builder.Property(o => o.OtpHash)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(o => o.CreatedAt)
                .IsRequired();
                
            builder.Property(o => o.ExpiresAt)
                .IsRequired();
                
            builder.Property(o => o.IsUsed)
                .IsRequired()
                .HasDefaultValue(false);
            

            builder.HasOne(o => o.user)           
                .WithMany(u => u.Otps)       
                .HasForeignKey(o => o.UserId)    
                .OnDelete(DeleteBehavior.Cascade);  
                
            builder.HasIndex(o => new { o.UserId, o.CreatedAt })
                .HasDatabaseName("IX_Otps_UserId_CreatedAt");
                
            builder.HasIndex(o => o.OtpHash)
                .HasDatabaseName("IX_Otps_OtpHash");
                
            builder.HasIndex(o => o.ExpiresAt)
                .HasDatabaseName("IX_Otps_ExpiresAt");
                
            builder.HasIndex(o => o.IsUsed)
                .HasDatabaseName("IX_Otps_IsUsed");
        }
    }
}