using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QDC_BLL.Interfaces;
using QDC_DAL.Models;
using QDC_DML.Identity;
using System.Data;
using System.Net;

namespace QDC_API.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        protected ApiResponse _response;
        private readonly IMapper _mapper;
        private readonly IRecaptchaService _recaptchaService;

        public UserController(IMapper mapper, IUnitOfWork unitOfWork, IRecaptchaService recaptchaService)
        {
            _mapper = mapper;
            this._response = new();
            _unitOfWork = unitOfWork;
            _recaptchaService = recaptchaService;
        }

        [HttpGet]
        [Authorize(Roles = Role.Owner)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse>> GetUsers()
        {
            try
            {

                var usersList = await _unitOfWork.User.GetAllAsync();
                _response.Result = _mapper.Map<List<UserDTO>>(usersList.OrderByDescending(u => u.UserId));
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }


        [HttpGet("{userId:int}", Name = "GetUser")]
        [Authorize(Roles = Role.Owner)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> GetUser(int userId)
        {
            try
            {
                if (userId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var user = await _unitOfWork.User.GetAsync(u => u.UserId == userId);

                if (user == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("User Not Found!");
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<UserDTO>(user);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }




        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            if (!await _recaptchaService.VerifyAsync(model.RecaptchaToken))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("reCAPTCHA verification failed");
                return BadRequest(_response);
            }

            var loginResponse = await _unitOfWork.User.Login(model);
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Incorrect Username or Password");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result= loginResponse;
            return Ok(_response);
        }




        [HttpPost("Register")]
        [Authorize(Roles = Role.Owner)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO model)
        {
            if (!await _recaptchaService.VerifyAsync(model.RecaptchaToken))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("reCAPTCHA verification failed");
                return BadRequest(_response);
            }

            bool ifUserNameUnique = _unitOfWork.User.IsUniqueUser(model.Email);
            if (!ifUserNameUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username already exists");
                return BadRequest(_response);
            }

            var user = await _unitOfWork.User.Register(model);
            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error while registering");
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }
    }
}
