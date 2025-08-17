using System.Security.Claims;
using System.Text.Json;
using Application.Events;
using Data.UnifiedOutbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Models.AttemptConnection;
using Models.Blockade;
using Models.Interest;
using Models.Match;
using Models.Message;
using Models.Notification;
using Models.Profiles;
using Models.Report;
using Models.Shared;
using Models.Users;


namespace Data.Context;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Interest> Interests => Set<Interest>();
    public DbSet<Mensaje> Messages => Set<Mensaje>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<Blockade> Blockades => Set<Blockade>();
    public DbSet<AttemptConnection> AttemptConnections => Set<AttemptConnection>();
    public DbSet<Models.ProfileInterest.PerfilInteres> ProfileInterests => Set<Models.ProfileInterest.PerfilInteres>();
    public DbSet<UnifiedOutboxMessage> OutboxMessages => Set<UnifiedOutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ignorar DomainEvents en todos los AggregateRoot
        foreach (var et in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(AggregateRoot).IsAssignableFrom(et.ClrType))
            {
                modelBuilder.Entity(et.ClrType).Ignore(nameof(AggregateRoot.DomainEvents));
            }
        }

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        modelBuilder.ApplyConfiguration(new InterestConfiguration());
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
        modelBuilder.ApplyConfiguration(new MatchConfiguration());
        modelBuilder.ApplyConfiguration(new ReportConfiguration());
        modelBuilder.ApplyConfiguration(new BlockadeConfiguration());
        modelBuilder.ApplyConfiguration(new AttemptConnectionConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileInterestConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());

        base.OnModelCreating(modelBuilder);
    }

    // Data/Context/AppDbContext.cs (tu DbContext)
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        // 1) recolecta DomainEvents
        var aggregates = ChangeTracker.Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();
        

        var domainEvents = aggregates.SelectMany(a => a.DomainEvents).ToList();

        if (domainEvents.Count > 0)
        {
            var mapper = this.GetService<IDomainToAppEventMapper>();
            var outbox = this.GetService<IUnifiedOutbox>();

            foreach (var de in domainEvents)
            {
                foreach (var appEvt in mapper.Map(de))
                {
                    await outbox.EnqueueAsync(
                        appEvt,
                        correlationId: null,
                        tenantId: null,
                        actorId: null,
                        ct);
                }
            }

            aggregates.ForEach(a => a.ClearDomainEvents());
        }

        return await base.SaveChangesAsync(ct);
    }
}


/* =========================
 * Converters / Comparers
 * ========================= */
internal static class EfConverters
{
    private static DateOnly MapDateBirthToProvider(DateBirth src) => src.Value;
    private static DateBirth MapDateBirthFromProvider(DateOnly v) => DateBirth.Create(v);

    public static readonly ValueConverter<DateBirth, DateOnly> DateBirthConv =
        new ValueConverter<DateBirth, DateOnly>(
            d => MapDateBirthToProvider(d),
            v => MapDateBirthFromProvider(v)
        );

    public static readonly ValueConverter<Email, string> EmailConv =
        new(e => e.Value, v => Email.Create(v));

    public static readonly ValueConverter<Alias, string> AliasConv =
        new(a => a.Value, v => Alias.Create(v));

    public static readonly ValueConverter<ApproximateCity, string> CityConv =
        new(c => c.Value, v => ApproximateCity.Create(v));
    
    public static readonly ValueConverter<Reason, string> ReasonConv =
        new(r => r.Value, v => Reason.Create(v));

    public static readonly ValueConverter<NotificationType, string> NotifTypeConv =
        new(t => t.Value, v => NotificationType.Create(v));

    public static readonly ValueConverter<NotificationData, string> NotifDataConv =
        new(d => d.Json, v => NotificationData.FromJson(v));

    // HashSet<PreferenceType> como JSON
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public static readonly ValueConverter<HashSet<PreferenceType>, string> PrefTypesConv =
        new(
            v => JsonSerializer.Serialize(v, JsonOpts),
            v => JsonSerializer.Deserialize<HashSet<PreferenceType>>(v, JsonOpts) ?? new()
        );

    public static readonly ValueComparer<HashSet<PreferenceType>> PrefTypesComp =
        new(
            (a, b) => a!.SetEquals(b!),
            a => a.Aggregate(0, (h, x) => HashCode.Combine(h, x.GetHashCode())),
            a => new HashSet<PreferenceType>(a!)
        );
}

/* =========================
 * User
 * ========================= */
internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("Users");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.Email)
            .HasConversion(EfConverters.EmailConv)
            .HasMaxLength(254)
            .IsRequired();

        b.HasIndex(x => x.Email).IsUnique();

        b.Property(x => x.EmailConfirmed).IsRequired();
        b.Property(x => x.UserRole).HasConversion<int>().IsRequired();
        
    }
}

/* =========================
 * Profile (owned Preferences)
 * ========================= */
internal sealed class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> b)
    {
        b.ToTable("Profiles");
        b.HasKey(p => p.Id);
        b.Property(p => p.Id).ValueGeneratedNever();
        

        b.Property(p => p.Alias)
            .HasConversion(EfConverters.AliasConv)
            .HasMaxLength(20)
            .IsRequired();

        b.Property(p => p.DateBirth)
            .HasConversion(EfConverters.DateBirthConv)
            .HasColumnType("date")
            .IsRequired();

        b.Property(p => p.City)
            .HasConversion(EfConverters.CityConv)
            .HasMaxLength(40)
            .IsRequired();

        b.Property(p => p.Bio).HasMaxLength(200);

        b.Property(p => p.Status).HasConversion<int>().IsRequired();
        b.Property(p => p.ShowCity).IsRequired();
        b.Property(p => p.HideOnline).IsRequired();

        b.Property(p => p.AvatarRoot).HasMaxLength(256);
        

        b.HasIndex(p => p.Alias);
        b.HasIndex(p => p.Status);

        // Owned: Preferences
        b.OwnsOne(p => p.Preferences, pref =>
        {
            pref.Property<HashSet<PreferenceType>>("_types")
                .HasColumnName("PreferenceTypes")
                .HasConversion(EfConverters.PrefTypesConv)
                .Metadata.SetValueComparer(EfConverters.PrefTypesComp);

            pref.Property(x => x.CommonInterestsOnly)
                .HasColumnName("CommonInterestsOnly")
                .IsRequired();

            pref.Property(x => x.WeeklySuggestions)
                .HasColumnName("WeeklySuggestions")
                .IsRequired();

            pref.OwnsOne(x => x.Range, r =>
            {
                r.Property(p => p.Min).HasColumnName("AgeMin").IsRequired();
                r.Property(p => p.Max).HasColumnName("AgeMax").IsRequired();
            });

            pref.WithOwner();
        });
    }
}

/* =========================
 * Notification
 * ========================= */
internal sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> b)
    {
        b.ToTable("Notifications");
        b.HasKey(n => n.Id);
        b.Property(n => n.Id).ValueGeneratedNever();

        b.Property(n => n.ProfileId).IsRequired();

        b.Property(n => n.Type)
            .HasConversion(
                t => t.Value,          
                v => NotificationType.Create(v)
            );

        b.Property(n => n.Data)
            .HasConversion(EfConverters.NotifDataConv)
            .HasColumnName("Payload")
            .HasColumnType("text") 
            .IsRequired();

        b.Property(n => n.IsRead).IsRequired();

        b.HasIndex(n => new { n.ProfileId, n.IsRead });
    }
}

/* =========================
 * Interest
 * ========================= */
internal sealed class InterestConfiguration : IEntityTypeConfiguration<Interest>
{
    public void Configure(EntityTypeBuilder<Interest> b)
    {
        b.ToTable("Interests");
        b.HasKey(i => i.Id);
        b.Property(i => i.Id).ValueGeneratedNever();

        b.Property(i => i.Name).HasMaxLength(100).IsRequired();
        b.Property(i => i.Slug).HasMaxLength(120).IsRequired();
        b.HasIndex(i => i.Slug).IsUnique();

        b.Property(i => i.Category).HasMaxLength(60);
        b.Property(i => i.Approved).IsRequired();
        b.Property(i => i.Sensitive).IsRequired();
        b.Property(i => i.Popularity).IsRequired();
        
    }
}

/* =========================
 * ProfileInterest (join)
 * ========================= */
internal sealed class ProfileInterestConfiguration : IEntityTypeConfiguration<Models.ProfileInterest.PerfilInteres>
{
    public void Configure(EntityTypeBuilder<Models.ProfileInterest.PerfilInteres> b)
    {
        b.ToTable("ProfileInterests");
        b.HasKey(x => new { x.ProfileId, x.InterestId });
        

        b.HasOne<Profile>()
         .WithMany()
         .HasForeignKey(x => x.ProfileId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne<Interest>()
         .WithMany()
         .HasForeignKey(x => x.InterestId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}

/* =========================
 * Messages
 * ========================= */
internal sealed class MessageConfiguration : IEntityTypeConfiguration<Mensaje>
{
    public void Configure(EntityTypeBuilder<Mensaje> b)
    {
        b.ToTable("Messages");
        b.HasKey(m => m.Id);
        b.Property(m => m.Id).ValueGeneratedNever();

        b.Property(m => m.MatchId).IsRequired();
        b.Property(m => m.SenderProfileId).IsRequired();

        b.Property(m => m.Content).HasMaxLength(2000).IsRequired();
        b.Property(m => m.MediaRef).HasMaxLength(256);

        b.Property(m => m.Status).HasMaxLength(20).IsRequired();
        

        b.HasIndex(m => new { m.MatchId, m.SendedIn });
    }
}

/* =========================
 * Match
 * ========================= */
internal sealed class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> b)
    {
        b.ToTable("Matches");
        b.HasKey(m => m.Id);
        b.Property(m => m.Id).ValueGeneratedNever();

        b.Property(m => m.ProfileAId).IsRequired();
        b.Property(m => m.ProfileBId).IsRequired();

        b.Property(m => m.Status).HasConversion<int>().IsRequired();
        

        b.HasIndex(m => new { m.ProfileAId, m.ProfileBId }).IsUnique(false);
    }
}

/* =========================
 * Reports
 * ========================= */
internal sealed class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> b)
    {
        b.ToTable("Reports");
        b.HasKey(r => r.Id);
        b.Property(r => r.Id).ValueGeneratedNever();

        b.Property(r => r.ReporterProfileId).IsRequired();
        b.Property(r => r.ObjectiveType).HasMaxLength(20).IsRequired();
        b.Property(r => r.TargetId).IsRequired();
        b.Property(r => r.Reason).HasMaxLength(200).IsRequired();
        b.Property(r => r.Details).HasMaxLength(2000);

        b.Property(r => r.Status).HasMaxLength(20).IsRequired();
    
    }
}

/* =========================
 * Blockade
 * ========================= */
internal sealed class BlockadeConfiguration : IEntityTypeConfiguration<Blockade>
{
    public void Configure(EntityTypeBuilder<Blockade> b)
    {
        b.ToTable("Blockades");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.BlockerProfileId).IsRequired();
        b.Property(x => x.BlockedProfileId).IsRequired();

        b.Property(x => x.Reason)
            .HasConversion(EfConverters.ReasonConv)
            .HasMaxLength(200)
            .IsRequired();
        

        b.HasIndex(x => new { x.BlockerProfileId, x.BlockedProfileId }).IsUnique();
    }
}

/* =========================
 * AttemptConnection
 * ========================= */
internal sealed class AttemptConnectionConfiguration : IEntityTypeConfiguration<AttemptConnection>
{
    public void Configure(EntityTypeBuilder<AttemptConnection> b)
    {
        b.ToTable("AttemptConnections");
        b.HasKey(a => a.Id);
        b.Property(a => a.Id).ValueGeneratedNever();

        b.Property(a => a.FromProfileId).IsRequired();
        b.Property(a => a.ToProfileId).IsRequired();
        b.Property(a => a.Status).HasConversion<int>().IsRequired();
        b.Property(a => a.CreatedAt).HasColumnType("timestamp with time zone").IsRequired(); // cambiado

        b.HasIndex(a => new { a.FromProfileId, a.ToProfileId, a.Status });
    }
}

/* =========================
 * OutboxMessage
 * ========================= */
internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<UnifiedOutboxMessage>
{
    public void Configure(EntityTypeBuilder<UnifiedOutboxMessage> b)
    {
        b.ToTable("OutboxMessages");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.Discriminator).HasMaxLength(300).IsRequired();
        b.Property(x => x.PayloadJson).HasColumnType("text").IsRequired();
        b.Property(x => x.Error).HasMaxLength(2000);

        b.HasIndex(x => x.ProcessedOn);
    }
}