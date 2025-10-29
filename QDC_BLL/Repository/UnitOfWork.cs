using QDC_BLL.Interfaces;
using QDC_DAL;
using QDC_DAL.Data;
using AutoMapper;
//using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using QDC_DAL.Models;
using System;

namespace QDC_BLL.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UnitOfWork(ApplicationDbContext db, 
            IConfiguration configuration,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper)
        {
            _db = db;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            Portfolio = new PortfolioRep(_db);
            Employee = new EmployeeRep(_db);
            User = new UserRep(_db, configuration, userManager, roleManager, mapper);
        }



        public IPortfolio Portfolio { get; private set; }
        public IEmployee Employee { get; private set; }
        public IUserRep User { get; private set; }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}
