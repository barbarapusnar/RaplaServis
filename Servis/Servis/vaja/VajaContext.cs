using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Servis.vaja;

public partial class VajaContext : DbContext
{
    public VajaContext()
    {
    }

    public VajaContext(DbContextOptions<VajaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Allocation> Allocations { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentException> AppointmentExceptions { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<DynamicType> DynamicTypes { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventAttributeValue> EventAttributeValues { get; set; }

    public virtual DbSet<EventPermission> EventPermissions { get; set; }

    public virtual DbSet<Preference> Preferences { get; set; }

    public virtual DbSet<RaplaConflict> RaplaConflicts { get; set; }

    public virtual DbSet<RaplaResource> RaplaResources { get; set; }

    public virtual DbSet<RaplaUser> RaplaUsers { get; set; }

    public virtual DbSet<RaplaUserGroup> RaplaUserGroups { get; set; }

    public virtual DbSet<ResourceAttributeValue> ResourceAttributeValues { get; set; }

    public virtual DbSet<ResourcePermission> ResourcePermissions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=localhost;database=vaja;user=root;password=");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Allocation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ALLOCATION", "Vaja");

            entity.HasIndex(e => e.AppointmentId, "KEY_ALLOCATION_APPOINTMENT_ID");

            entity.Property(e => e.AppointmentId).HasColumnName("APPOINTMENT_ID");
            entity.Property(e => e.ParentOrder).HasColumnName("PARENT_ORDER");
            entity.Property(e => e.ResourceId)
                .HasMaxLength(255)
                .HasColumnName("RESOURCE_ID");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("APPOINTMENT", "Vaja");

            entity.HasIndex(e => e.EventId, "KEY_APPOINTMENT_EVENT_ID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AppointmentEnd)
                .HasColumnType("datetime")
                .HasColumnName("APPOINTMENT_END");
            entity.Property(e => e.AppointmentStart)
                .HasColumnType("datetime")
                .HasColumnName("APPOINTMENT_START");
            entity.Property(e => e.EventId).HasColumnName("EVENT_ID");
            entity.Property(e => e.RepetitionEnd)
                .HasColumnType("datetime")
                .HasColumnName("REPETITION_END");
            entity.Property(e => e.RepetitionInterval).HasColumnName("REPETITION_INTERVAL");
            entity.Property(e => e.RepetitionNumber).HasColumnName("REPETITION_NUMBER");
            entity.Property(e => e.RepetitionType)
                .HasMaxLength(255)
                .HasColumnName("REPETITION_TYPE");
        });

        modelBuilder.Entity<AppointmentException>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("APPOINTMENT_EXCEPTION", "Vaja");

            entity.HasIndex(e => e.AppointmentId, "KEY_APPOINTMENT_EXCEPTION_APPOINTMENT_ID");

            entity.Property(e => e.AppointmentId).HasColumnName("APPOINTMENT_ID");
            entity.Property(e => e.ExceptionDate)
                .HasColumnType("datetime")
                .HasColumnName("EXCEPTION_DATE");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("CATEGORY", "Vaja");

            entity.HasIndex(e => e.ParentId, "KEY_CATEGORY_PARENT_ID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryKey)
                .HasMaxLength(255)
                .HasColumnName("CATEGORY_KEY");
            entity.Property(e => e.Definition).HasColumnName("DEFINITION");
            entity.Property(e => e.ParentId).HasColumnName("PARENT_ID");
            entity.Property(e => e.ParentOrder).HasColumnName("PARENT_ORDER");
        });

        modelBuilder.Entity<DynamicType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("DYNAMIC_TYPE", "Vaja");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Definition).HasColumnName("DEFINITION");
            entity.Property(e => e.TypeKey)
                .HasMaxLength(255)
                .HasColumnName("TYPE_KEY");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("EVENT", "Vaja");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreationTime)
                .HasColumnType("timestamp")
                .HasColumnName("CREATION_TIME");
            entity.Property(e => e.LastChanged)
                .HasColumnType("timestamp")
                .HasColumnName("LAST_CHANGED");
            entity.Property(e => e.LastChangedBy)
                .HasMaxLength(255)
                .HasColumnName("LAST_CHANGED_BY");
            entity.Property(e => e.OwnerId)
                .HasMaxLength(255)
                .HasColumnName("OWNER_ID");
            entity.Property(e => e.TypeKey)
                .HasMaxLength(255)
                .HasColumnName("TYPE_KEY");
        });

        modelBuilder.Entity<EventAttributeValue>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EVENT_ATTRIBUTE_VALUE", "Vaja");

            entity.HasIndex(e => e.EventId, "KEY_EVENT_ATTRIBUTE_VALUE_EVENT_ID");

            entity.Property(e => e.AttributeKey)
                .HasMaxLength(255)
                .HasColumnName("ATTRIBUTE_KEY");
            entity.Property(e => e.AttributeValue)
                .HasColumnType("varchar(20000)")
                .HasColumnName("ATTRIBUTE_VALUE");
            entity.Property(e => e.EventId).HasColumnName("EVENT_ID");
        });

        modelBuilder.Entity<EventPermission>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EVENT_PERMISSION", "Vaja");

            entity.HasIndex(e => e.EventId, "KEY_EVENT_PERMISSION_EVENT_ID");

            entity.Property(e => e.AccessLevel).HasColumnName("ACCESS_LEVEL");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("END_DATE");
            entity.Property(e => e.EventId).HasColumnName("EVENT_ID");
            entity.Property(e => e.GroupId)
                .HasMaxLength(255)
                .HasColumnName("GROUP_ID");
            entity.Property(e => e.MaxAdvance).HasColumnName("MAX_ADVANCE");
            entity.Property(e => e.MinAdvance).HasColumnName("MIN_ADVANCE");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("START_DATE");
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .HasColumnName("USER_ID");
        });

        modelBuilder.Entity<Preference>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("PREFERENCE", "Vaja");

            entity.HasIndex(e => e.UserId, "KEY_PREFERENCE_USER_ID");

            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .HasColumnName("ROLE");
            entity.Property(e => e.StringValue)
                .HasMaxLength(10000)
                .HasColumnName("STRING_VALUE");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");
            entity.Property(e => e.XmlValue).HasColumnName("XML_VALUE");
        });

        modelBuilder.Entity<RaplaConflict>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("RAPLA_CONFLICT", "Vaja");

            entity.Property(e => e.App1enabled).HasColumnName("APP1ENABLED");
            entity.Property(e => e.App2enabled).HasColumnName("APP2ENABLED");
            entity.Property(e => e.Appointment1)
                .HasMaxLength(255)
                .HasColumnName("APPOINTMENT1");
            entity.Property(e => e.Appointment2)
                .HasMaxLength(255)
                .HasColumnName("APPOINTMENT2");
            entity.Property(e => e.LastChanged)
                .HasColumnType("timestamp")
                .HasColumnName("LAST_CHANGED");
            entity.Property(e => e.ResourceId)
                .HasMaxLength(255)
                .HasColumnName("RESOURCE_ID");
        });

        modelBuilder.Entity<RaplaResource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("RAPLA_RESOURCE", "Vaja");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreationTime)
                .HasColumnType("timestamp")
                .HasColumnName("CREATION_TIME");
            entity.Property(e => e.LastChanged)
                .HasColumnType("timestamp")
                .HasColumnName("LAST_CHANGED");
            entity.Property(e => e.LastChangedBy)
                .HasMaxLength(255)
                .HasColumnName("LAST_CHANGED_BY");
            entity.Property(e => e.OwnerId)
                .HasMaxLength(255)
                .HasColumnName("OWNER_ID");
            entity.Property(e => e.TypeKey)
                .HasMaxLength(255)
                .HasColumnName("TYPE_KEY");
        });

        modelBuilder.Entity<RaplaUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("RAPLA_USER", "Vaja");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreationTime)
                .HasColumnType("timestamp")
                .HasColumnName("CREATION_TIME");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Isadmin).HasColumnName("ISADMIN");
            entity.Property(e => e.LastChanged)
                .HasColumnType("timestamp")
                .HasColumnName("LAST_CHANGED");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("NAME");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("USERNAME");
        });

        modelBuilder.Entity<RaplaUserGroup>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("RAPLA_USER_GROUP", "Vaja");

            entity.HasIndex(e => e.UserId, "KEY_RAPLA_USER_GROUP_USER_ID");

            entity.Property(e => e.CategoryId)
                .HasMaxLength(255)
                .HasColumnName("CATEGORY_ID");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");
        });

        modelBuilder.Entity<ResourceAttributeValue>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("RESOURCE_ATTRIBUTE_VALUE", "Vaja");

            entity.HasIndex(e => e.ResourceId, "KEY_RESOURCE_ATTRIBUTE_VALUE_RESOURCE_ID");

            entity.Property(e => e.AttributeKey)
                .HasMaxLength(255)
                .HasColumnName("ATTRIBUTE_KEY");
            entity.Property(e => e.AttributeValue)
                .HasColumnType("varchar(20000)")
                .HasColumnName("ATTRIBUTE_VALUE");
            entity.Property(e => e.ResourceId).HasColumnName("RESOURCE_ID");
        });

        modelBuilder.Entity<ResourcePermission>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("RESOURCE_PERMISSION", "Vaja");

            entity.HasIndex(e => e.ResourceId, "KEY_RESOURCE_PERMISSION_RESOURCE_ID");

            entity.Property(e => e.AccessLevel).HasColumnName("ACCESS_LEVEL");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("END_DATE");
            entity.Property(e => e.GroupId)
                .HasMaxLength(255)
                .HasColumnName("GROUP_ID");
            entity.Property(e => e.MaxAdvance).HasColumnName("MAX_ADVANCE");
            entity.Property(e => e.MinAdvance).HasColumnName("MIN_ADVANCE");
            entity.Property(e => e.ResourceId).HasColumnName("RESOURCE_ID");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("START_DATE");
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .HasColumnName("USER_ID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
