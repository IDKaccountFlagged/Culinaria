using System;
using Culinaria.Areas.Identity.Data;
using Culinaria.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Culinaria.Areas.Identity.IdentityHostingStartup))]
namespace Culinaria.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<CulinariaContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("CulinariaContextConnection")));

                services.AddDefaultIdentity<CulinariaUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<CulinariaContext>();
            });
        }
    }
}