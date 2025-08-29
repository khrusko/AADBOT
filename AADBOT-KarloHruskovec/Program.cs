using AADBOT_KarloHruskovec.Data;
using AADBOT_KarloHruskovec.Events;
using AADBOT_KarloHruskovec.Models;
using AADBOT_KarloHruskovec.Repositories;
using AADBOT_KarloHruskovec.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Prometheus;
using AADBOT_KarloHruskovec.Aspects;



var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	if (builder.Environment.IsEnvironment("Testing"))
		options.UseInMemoryDatabase("testdb");
	else
		options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure());
});


builder.Services.AddDatabaseDeveloperPageExceptionFilter();



builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = "JwtBearer";
	options.DefaultChallengeScheme = "JwtBearer";
})
	.AddJwtBearer("JwtBearer", options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
			RoleClaimType = ClaimTypes.Role
		};
	})
	.AddGoogle(options =>
	{
		options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
		options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
		options.Events.OnCreatingTicket = async ctx =>
		{
			var userManager = ctx.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
			var db = ctx.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

			var email = ctx.Principal.FindFirstValue(ClaimTypes.Email);
			var user = await userManager.FindByEmailAsync(email);
			if (user == null)
			{
				user = new ApplicationUser
				{
					UserName = email,
					Email = email,
					EmailConfirmed = true,
					Package = "FREE",
					LastPackageChange = DateTime.UtcNow
				};
				await userManager.CreateAsync(user);
				await userManager.AddToRoleAsync(user, "RegisteredUser");

				db.Logs.Add(new LogEntry
				{
					UserId = user.Id,
					Action = "Registered via Google",
					Timestamp = DateTime.UtcNow
				});
				await db.SaveChangesAsync();
			}
		};
	})
	.AddGitHub(options =>
	{
		options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"];
		options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];
		options.Scope.Add("user:email");
		options.Events.OnCreatingTicket = async ctx =>
		{
			var userManager = ctx.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
			var db = ctx.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

			var email = ctx.Principal.FindFirstValue(ClaimTypes.Email);
			var user = await userManager.FindByEmailAsync(email);
			if (user == null)
			{
				user = new ApplicationUser
				{
					UserName = email,
					Email = email,
					EmailConfirmed = true,
					Package = "FREE",
					LastPackageChange = DateTime.UtcNow
				};
				await userManager.CreateAsync(user);
				await userManager.AddToRoleAsync(user, "RegisteredUser");

				db.Logs.Add(new LogEntry
				{
					UserId = user.Id,
					Action = "Registered via GitHub",
					Timestamp = DateTime.UtcNow
				});
				await db.SaveChangesAsync();
			}
		};
	});


builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
	options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.Events.OnRedirectToLogin = context =>
	{
		context.Response.StatusCode = 401;
		return Task.CompletedTask;
	};

	options.Cookie.SameSite = SameSiteMode.None;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddHttpContextAccessor();

//Services
builder.Services.AddLogging();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<LogPhotoUploadHandler>();

//Singleton
builder.Services.AddSingleton<IEventBus>(_ => LoggingService.Instance);
builder.Services.AddSingleton<IJwtService, JwtTokenService>();



LoggingService.Instance.Subscribe<PhotoUploadedEvent>(
	async evt => Console.WriteLine($"Photo uploaded: {evt.FileName}")
);



builder.Services.AddControllers(options =>
{
	options.Filters.Add<LoggingActionFilter>();
	options.Filters.Add<ValidationActionFilter>();
});


builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowReactDev", policy =>
	{
		policy.WithOrigins("http://localhost:3000")
			  .AllowAnyHeader()
			  .AllowAnyMethod()
			  .AllowCredentials();
	});
});

//Decorator
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IUserRepository>(provider =>
{
	var repo = provider.GetRequiredService<UserRepository>();
	var logger = provider.GetRequiredService<ILogger<LoggingUserRepository>>();
	return new LoggingUserRepository(repo, logger);
});



//Strategy - Default
builder.Services.AddScoped<IResizeStrategy>(_ => new ResizeStrategy(500));

var app = builder.Build();

app.UseHttpMetrics();
// map the /metrics endpoint
app.MapMetrics();


//Observer
using (var scope = app.Services.CreateScope())
{
	var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
	var logHandler = scope.ServiceProvider.GetRequiredService<LogPhotoUploadHandler>();
	eventBus.Subscribe<PhotoUploadedEvent>(logHandler.Handle);
}

app.Use(async (context, next) =>
{
	context.Response.OnStarting(() =>
	{
		context.Response.Headers.Remove("X-Powered-By");
		return Task.CompletedTask;
	});
	await next.Invoke();
});

//if (!app.Environment.IsDevelopment()) //Temporary fix, for development only
	app.UseHsts();



if (!app.Environment.IsEnvironment("Testing"))
{
	using (var scope = app.Services.CreateScope())
	{
		var services = scope.ServiceProvider;
		var db = services.GetRequiredService<ApplicationDbContext>();
		await db.Database.MigrateAsync();
		var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
		var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
		string[] roles = new[] { "Admin", "RegisteredUser" };
		foreach (var role in roles)
			if (!await roleManager.RoleExistsAsync(role))
				await roleManager.CreateAsync(new IdentityRole(role));
		var adminEmail = "admin@site.com";
		var adminUser = await userManager.FindByEmailAsync(adminEmail);
		if (adminUser == null)
		{
			adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true, Package = "GOLD" };
			await userManager.CreateAsync(adminUser, "Admin123!");
			await userManager.AddToRoleAsync(adminUser, "Admin");
		}
	}
	app.UseHsts();
	app.UseHttpsRedirection();
}



if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}
else
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowReactDev");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();


public partial class Program { }
