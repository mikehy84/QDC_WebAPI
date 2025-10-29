using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QDC_BLL.Interfaces;
using QDC_DAL.Data;
using QDC_DAL.Models;
using QDC_DML.Portfolio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;

namespace QDC_API.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/EmployeeAPI")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        protected ApiResponse _response;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _response = new ApiResponse();
        }


        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetEmployees()
        {
            IEnumerable<Employee> employee = await _unitOfWork.Employee.GetAllAsync();
            _response.Result = _mapper.Map<List<EmployeeDTO>>(employee);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }


        [HttpGet("{id:int}", Name = "GetEmployeeById")]
        [Authorize(Roles = Role.Owner)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> GetEmployeeById(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var employee = await _unitOfWork.Employee.GetAsync(p => p.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            _response.Result = _mapper.Map<EmployeeDTO>(employee);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }


        [HttpPost]
        [Authorize(Roles = Role.Owner)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> CreateEmployee([FromForm] EmployeeCreateDTO employeeCreateDTO)
        {
            try
            {
                // if not using [ApiController]
                //if(!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}

                // if a custom validation needed
                if (await _unitOfWork.Employee
                    .GetAsync(p => p.Name.ToLower() == employeeCreateDTO.Name.ToLower()) != null)
                {
                    //ModelState.AddModelError("CustomError", "name already exists!");
                    //_response.StatusCode = HttpStatusCode.BadRequest;


                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Project name already exists!");
                    return BadRequest(_response);
                }


                //if (categoryDto.Image == null && categoryDto.Image.Length == 0)
                //{
                //    _response.StatusCode = HttpStatusCode.BadRequest;
                //    return BadRequest(_response);
                //}


                byte[] imgBytes = null;
                if (ModelState.IsValid)
                {
                    if (employeeCreateDTO.Image != null)
                    {
                        var ms = new MemoryStream();
                        employeeCreateDTO.Image.CopyTo(ms);
                        imgBytes = ms.ToArray();
                    }
                }

                // manually mapping employeeCreateDTO to employee
                Employee employee = new()
                {
                    Name = employeeCreateDTO.Name,
                    Job = employeeCreateDTO.Job,
                    Description = employeeCreateDTO.Description,
                    Image = imgBytes,
                    IsArchive = employeeCreateDTO.IsArchive,
                };


                await _unitOfWork.Employee.CreateAsync(employee);

                _response.Result = _mapper.Map<EmployeeDTO>(employee);
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetEmployeeById", new { id = employee.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }


        [HttpDelete("{id:int}", Name = "DeleteEmployee")]
        [Authorize(Roles = Role.Owner)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // with IActionResult we do not need to define return type
        public async Task<ActionResult<ApiResponse>> DeleteEmployee(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var employee = await _unitOfWork.Employee.GetAsync(v => v.Id == id);

                if (employee == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                await _unitOfWork.Employee.RemoveAsync(employee);
                _response.StatusCode = HttpStatusCode.Accepted;
                _response.IsSuccess = true;
                return Accepted(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPut("{id:int}", Name = "UpdateEmployee")]
        [Authorize(Roles = Role.Owner)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> UpdateEmployee(int id, [FromForm] EmployeeUpdateDTO employeeUpdateDto)
        {
            try
            {
                if (employeeUpdateDto == null || id != employeeUpdateDto.Id)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var employeeFromDb = await _unitOfWork.Employee.GetAsync(v => v.Id == id, tracked: false);

                byte[]? imgBytes = null;
                if (ModelState.IsValid)
                {
                    if (employeeUpdateDto.Image != null)
                    {
                        var ms = new MemoryStream();
                        employeeUpdateDto.Image.CopyTo(ms);
                        imgBytes = ms.ToArray();
                    }
                }

                Employee employee = new()
                {
                    Id = id,
                    Name = employeeUpdateDto.Name,
                    Job= employeeUpdateDto.Job,
                    Description = employeeUpdateDto.Description,
                    Image = imgBytes ?? employeeFromDb.Image,
                    IsArchive = employeeUpdateDto.IsArchive,
                };

                await _unitOfWork.Employee.UpdateAsync(employee);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
    }
}
