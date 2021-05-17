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
                UserName = "Morgang823@gmail.com",
                DisplayName = "Gianni The Boss",
                Email = "Morgang823@gmail.com",
                FirstName = "Gianni",
                LastName = "Morgan",
                Password = "AdminPassword",
                PhoneNumber = "884-1428",
                EmailConfirmed = true,
                ImageData = await _fileSercvice.EncodeFileAsync("Propic.jpg"),
                ContentType = "jpg"
            };
            var modUser = new BlogUser()
            {
                UserName = "cfPersonnel@mailinator.com",
                DisplayName = "Moderator",
                Email = "cfPersonnel@mailinator.com",
                FirstName = "Moderator",
                LastName = "Moderator User",
                Password = "modPassword",
                PhoneNumber = "867-5309",
                EmailConfirmed = true,
                ImageData = await _fileSercvice.EncodeFileAsync("Gman.png"),
                ContentType = "png"
            };
            await _userManager.CreateAsync(adminUser, _configuration["AdminPassword"]);
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());
            await _userManager.CreateAsync(modUser, _configuration["modPassword"]);
            await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());

        }
    }
}
