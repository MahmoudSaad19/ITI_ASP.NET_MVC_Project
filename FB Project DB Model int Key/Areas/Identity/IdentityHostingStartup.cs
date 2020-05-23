using System;
using FB_Project_DB_Model_int_Key.Data;
using FB_Project_DB_Model_int_Key.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(FB_Project_DB_Model_int_Key.Areas.Identity.IdentityHostingStartup))]
namespace FB_Project_DB_Model_int_Key.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}