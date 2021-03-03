using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ChatHot.Model.EFModels.ChatHotDB
{
    public partial class ChatHotContext : DbContext
    {
        public ChatHotContext()
        {
        }

        public ChatHotContext(DbContextOptions<ChatHotContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Friend> Friends { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=192.168.0.111\\SQLSERVER;Initial Catalog=ChatHot;User ID=quxingbai;Password=quxingbai;MultipleActiveResultSets=True;");
                //optionsBuilder.UseSqlServer("Data Source="+StaticResource.ServerIpaddress.ToString()+"\\SQLSERVER;Initial Catalog=ChatHot;User ID=quxingbai;Password=quxingbai;MultipleActiveResultSets=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_PRC_CI_AS");

            modelBuilder.Entity<Friend>(entity =>
            {
                entity.HasKey(k => k.Fid);

                entity.ToTable("Friend");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Fid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("FID");

                entity.Property(e => e.LockDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(k => k.Uid);

                entity.ToTable("User");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UheadImage)
                    .HasMaxLength(100)
                    .HasColumnName("UHeadImage");

                entity.Property(e => e.Uid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("UID");

                entity.Property(e => e.UloginNumber).HasColumnName("ULoginNumber");

                entity.Property(e => e.Uname)
                    .HasMaxLength(50)
                    .HasColumnName("UName");

                entity.Property(e => e.Upassword)
                    .HasMaxLength(100)
                    .HasColumnName("UPassword");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Utag)
                    .HasMaxLength(100)
                    .HasColumnName("UTag");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
