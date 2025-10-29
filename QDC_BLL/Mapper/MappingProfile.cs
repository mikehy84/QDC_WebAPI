using AutoMapper;
using QDC_DAL.Models;
using QDC_DML.Identity;
using QDC_DML.Portfolio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QDC_BLL.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Portfolio, PortfolioDTO>().ReverseMap();

            CreateMap<Employee, EmployeeDTO>().ReverseMap();

            CreateMap<AppUser, UserDTO>().ReverseMap();
        }
    }

}
