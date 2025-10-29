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
using System.Net;

namespace QDC_API.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/PortfolioAPI")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        protected ApiResponse _response;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PortfolioController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _response = new ApiResponse();
        }


        [HttpGet]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse>> GetProtfolios()
        {
            IEnumerable<Portfolio> portfolio = await _unitOfWork.Portfolio.GetAllAsync();
            _response.Result = _mapper.Map<List<PortfolioDTO>>(portfolio);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }


        [HttpGet("{id:int}", Name = "GetProtfolioById")]
        [Authorize(Roles = Role.Owner)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> GetProtfolioById(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var portfolio = await _unitOfWork.Portfolio.GetAsync(p => p.Id == id);
            if (portfolio == null)
            {
                return NotFound();
            }
            _response.Result = _mapper.Map<PortfolioDTO>(portfolio);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }


        [HttpPost]
        [Authorize(Roles = Role.Owner)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> CreatePortfolio([FromForm] PortfolioCreateDTO portfolioCreateDTO)
        {
            try
            {
                // if not using [ApiController]
                //if(!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}

                // if a custom validation needed
                if (await _unitOfWork.Portfolio
                    .GetAsync(p => p.Name.ToLower() == portfolioCreateDTO.Name.ToLower()) != null)
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
                    if (portfolioCreateDTO.Image != null)
                    {
                        var ms = new MemoryStream();
                        portfolioCreateDTO.Image.CopyTo(ms);
                        imgBytes = ms.ToArray();
                    }
                }

                // manually mapping portfolioCreateDTO to portfolio
                Portfolio portfolio = new()
                {
                    Name = portfolioCreateDTO.Name,
                    Description = portfolioCreateDTO.Description,
                    Image = imgBytes,
                    IsArchive = portfolioCreateDTO.IsArchive,
                };


                await _unitOfWork.Portfolio.CreateAsync(portfolio);

                _response.Result = _mapper.Map<PortfolioDTO>(portfolio);
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetProtfolioById", new { id = portfolio.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }


        [HttpDelete("{id:int}", Name = "DeletePortfolio")]
        [AcceptVerbs("DELETE")]
        [Authorize(Roles = Role.Owner)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // with IActionResult we do not need to define return type
        public async Task<ActionResult<ApiResponse>> DeletePortfolio(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var portfolio = await _unitOfWork.Portfolio.GetAsync(v => v.Id == id);

                if (portfolio == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                await _unitOfWork.Portfolio.RemoveAsync(portfolio);
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

        [HttpPut("{id:int}", Name = "UpdatePortfolio")]
        [Authorize(Roles = Role.Owner)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> UpdatePortfolio(int id, [FromForm] PortfolioUpdateDTO portfolioUpdateDto)
        {
            try
            {
                if (portfolioUpdateDto == null || id != portfolioUpdateDto.Id)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var portfolioFromDb = await _unitOfWork.Portfolio.GetAsync(v => v.Id == id, tracked: false);

                byte[]? imgBytes = null;
                if (ModelState.IsValid)
                {
                    if (portfolioUpdateDto.Image != null)
                    {
                        var ms = new MemoryStream();
                        portfolioUpdateDto.Image.CopyTo(ms);
                        imgBytes = ms.ToArray();
                    }
                }

                Portfolio portfolio= new()
                {
                    Id = id,
                    Name = portfolioUpdateDto.Name,
                    Description = portfolioUpdateDto.Description,
                    Image = imgBytes ?? portfolioFromDb.Image,
                    IsArchive = portfolioUpdateDto.IsArchive,
                };

                await _unitOfWork.Portfolio.UpdateAsync(portfolio);
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
