using Api.User;
using Data.Context;                    
using Microsoft.EntityFrameworkCore;
using MediatR;
using FluentValidation;
using Application.Abstractions;
using Application.Abstractions.Services;
using Application.Events;
using Application.Identity;
using Application.User.Command;
using Data.Auth;
using Data.Events;
using Data.Repoository.UnitOfWork;
using Data.Repository.Notification;
using Data.Repository.Profile;
using Data.Repository.User;
using Data.Services.UserService;
using Data.UnifiedOutbox;
using Microsoft.AspNetCore.Identity;
using Models.Notifications.Interface;
using Models.Profiles.Interface;
using Models.Users.Interface;
using Tick.Components;                       

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("TickDb")!;
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(cs));

// API Controllers
builder.Services.AddControllers();

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommand).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Repos e infraestructura
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPerfilRepository, ProfileRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUnifiedOutbox, UnifiedOutbox>();
builder.Services.AddScoped<IEventTypeRegistry, EventTypeRegistry>();
builder.Services.AddScoped<IDomainToAppEventMapper, DomainToAppEventMapper>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHostedService<UnifiedOutboxDispatcherService>();

builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(UserController).Assembly);


builder.Services
    .AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>()     
    .AddEntityFrameworkStores<AuthDbContext>()                 
    .AddDefaultTokenProviders();
builder.Services.AddDbContext<AuthDbContext>(o => o.UseNpgsql(cs));
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStaticFiles();  
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
// API Routes
app.MapControllers();



// Blazor UI
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
