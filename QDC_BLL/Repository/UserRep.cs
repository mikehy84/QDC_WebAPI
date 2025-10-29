using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QDC_BLL.Interfaces;
using QDC_DAL.Data;
using QDC_DAL.Models;
using QDC_DML.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace QDC_BLL.Repository
{

    public class UserRep : Repository<AppUser>, IUserRep
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private string? secretKey;
        //private string? issuer;
        //private string? audience;




        public UserRep(ApplicationDbContext db, 
            IConfiguration configuration,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper) : base(db)
        {
            _db = db;
            _userManager = userManager;
            secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
            //issuer = configuration.GetValue<string>("ApiSettings:Issuer");
            //audience = configuration.GetValue<string>("ApiSettings:Audience");
            _mapper = mapper;
            _roleManager= roleManager;
        }

        public bool IsUniqueUser(string userName)
        {
            var user = _db.AppUsers.FirstOrDefault(u => u.UserName == userName);
            if (user == null)
            {
                return true;
            }
            return false;
        }


        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.AppUsers
                .FirstOrDefault(u => u.UserName.ToLower().Trim() == loginRequestDTO.UserName.ToLower().Trim());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
            if (user == null || isValid == false)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null
                };
            }

            //if user was not found generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);

            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescreptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("id" , user.Id.ToString()),
                    new Claim("fullName" , user.FullName.ToString()),
                    new Claim(ClaimTypes.Email, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                //Issuer = issuer,
                //Audience= audience,
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescreptor);

            LoginResponseDTO loginResponseDTO = new()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDTO>(user),
            };
            return loginResponseDTO;
        }

        

        public async Task<UserDTO> Register(RegisterRequestDTO registerRequestDTO)
        {
            AppUser user = new()
            {
                UserId = _userManager.Users.Count() + 1,
                UserName = registerRequestDTO.Email.Trim(),
                FullName = registerRequestDTO.FullName.Trim(),
                Email = registerRequestDTO.Email.Trim(),
                PhoneNumber = registerRequestDTO.PhoneNumber.Trim(),
                NormalizedEmail = registerRequestDTO.Email.ToUpper().Trim(),
                Role = registerRequestDTO.Role.Trim() ?? "employee"
            };


            try
            {
                var result = await _userManager.CreateAsync(user, registerRequestDTO.Password.Trim());
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole("owner"));
                        await _roleManager.CreateAsync(new IdentityRole("admin"));
                        await _roleManager.CreateAsync(new IdentityRole("employee"));
                        await _roleManager.CreateAsync(new IdentityRole("viewer"));
                    }
                    await _userManager.AddToRoleAsync(user, registerRequestDTO.Role);
                    var userToReturn = _db.AppUsers
                        .FirstOrDefault(u => u.UserName == registerRequestDTO.Email);
                    return _mapper.Map<UserDTO>(user);
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return new UserDTO();
        }
    }
}
