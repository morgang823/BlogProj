using BlogProj.Data;
using BlogProj.Enums;
using BlogProj.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogProj.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IBlogFileService _fileSercvice;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IConfiguration _configuration;
        public DataService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, IBlogFileService fileSercvice, UserManager<BlogUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _roleManager = roleManager;
            _fileSercvice = fileSercvice;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task ManageDataAsync()
        {
            //Task 0: Make sure the DB is present by running through the migrations
            await _context.Database.MigrateAsync();
            //Task 1 SeedRoles  -Creating Roles and entering them into systems
            await SeedRolesAsync();

            //Task2 Seed a few users in teh system(ASPNetUsers)
            await SeedUsersAsync();

        }
        private async Task SeedRolesAsync()
        {
            if (_context.Roles.Any()) 
                return;

            //Spin through an enum
         foreach(var role in Enum.GetNames(typeof(BlogRole)))
            {
                //create role in teh system for each role as a string
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        private async Task SeedUsersAsync()
        {
            if (_context.Users.Any())
            {
                return;
            }
            var adminUser = new BlogUser()
            {
                Email = "Morgang823@gmail.com",
                UserName = "Morgang823",
                FirstName = "Gianni",
                LastName = "Morgan",
                PhoneNumber = "884-1428",
                EmailConfirmed = true,
                ImageData = await _fileSercvice.EncodeFileAsync("Propic.jpg"),
                ContentType = "jpg"
            };

            await _userManager.CreateAsync(adminUser, _configuration["AdminPassword"]);
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());
        }
    }
}
