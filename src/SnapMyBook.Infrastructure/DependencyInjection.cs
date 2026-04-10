using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SnapMyBook.Application.Abstractions;
using SnapMyBook.Application.Books;
using SnapMyBook.Application.Highlights;
using SnapMyBook.Infrastructure.Identity;
using SnapMyBook.Infrastructure.Persistence;
using SnapMyBook.Infrastructure.Services;

namespace SnapMyBook.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=snapmybook.db";

        services.AddDbContext<SnapMyBookDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<SnapMyBookDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IHighlightRepository, HighlightRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IImageStorageService, LocalImageStorageService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IHighlightService, HighlightService>();
        services.AddScoped<IOcrWorkflowService, OcrWorkflowService>();

        return services;
    }
}
