using System;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ReaderBackend.DTOs;
using ReaderBackend.Models;
using ReaderBackend.Services;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ReaderBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WebPagesController : ControllerBase
    {
        private readonly IWebPageService _webPageService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public WebPagesController(IHttpContextAccessor contextAccessor, IWebPageService webPageService, IMapper mapper)
        {
            _webPageService = webPageService;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        [HttpGet("article")]
        [AllowAnonymous]
        public async Task<ActionResult<WebPage>> ConvertToArticle(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
                return BadRequest("Absolute uri is required");

            var result = await _webPageService.GetArticle(uri);

            if (result == null)
                return BadRequest("Failed to create article");

            return Ok(result);
        }

        [HttpGet]
        public ActionResult<IEnumerable<WebPageReadDto>> GetAllUserWebPages()
        {
            string userId = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = _webPageService.GetWebPagesByUserId(Guid.Parse((ReadOnlySpan<char>) userId));

            if (!string.IsNullOrEmpty(result.error))
                return BadRequest(result.error);

            //200
            return Ok(_mapper.Map<IEnumerable<WebPageReadDto>>(result.webPages));
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<WebPageReadDto>> GetAllWebPages()
        {
            var result = _webPageService.GetAllWebPages();
        
            if (!string.IsNullOrEmpty(result.error))
                return BadRequest(result.error); //400
        
            //200
            return Ok(_mapper.Map<IEnumerable<WebPageReadDto>>(result.webPages));
        }
        
        [HttpGet("{id}", Name = "GetWebPageById")]
        public ActionResult<WebPageReadDto> GetWebPageById(Guid id)
        {
            var result = _webPageService.GetWebPageById(id);

            if (!string.IsNullOrEmpty(result.error))
                return BadRequest(result.error);

            if (result.webPage == null)
                return NotFound(); //404

            return Ok(_mapper.Map<WebPageReadDto>(result.webPage));
        }

        [HttpPost]
        public ActionResult<WebPageReadDto> AddWebPage(WebPageCreateDto webPageAddDto)
        {
            string userId = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var webPageModel = _mapper.Map<WebPage>(webPageAddDto);
            
            webPageModel.UserId = Guid.Parse((ReadOnlySpan<char>) userId);
            
            string error = _webPageService.AddWebPage(webPageModel);

            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);
            
            var webPageReadDto = _mapper.Map<WebPageReadDto>(webPageModel);

            return CreatedAtRoute(
                nameof(GetWebPageById), 
                new { webPageModel.Id }, 
                webPageReadDto);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateWebPage(Guid id, WebPageUpdateDto webPageUpdateDto)
        {
            var result = _webPageService.GetWebPageById(id);

            if (!string.IsNullOrEmpty(result.error))
                return BadRequest(result.error);

            if (result.webPage == null)
                return NotFound();

            _mapper.Map(webPageUpdateDto, result.webPage);
            _webPageService.UpdateWebPage(result.webPage);

            //204
            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult PartialWebPageUpdate(Guid id, JsonPatchDocument<WebPageUpdateDto> patchDoc)
        {
            var result = _webPageService.GetWebPageById(id);

            if (!string.IsNullOrEmpty(result.error))
                return BadRequest(result.error);

            if (result.webPage == null)
                return NotFound();

            var webPageToPatch = _mapper.Map<WebPageUpdateDto>(result);
            patchDoc.ApplyTo(webPageToPatch, ModelState);

            if (!TryValidateModel(webPageToPatch)) 
                return ValidationProblem(ModelState);

            _mapper.Map(webPageToPatch, result.webPage);

            _webPageService.UpdateWebPage(result.webPage);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteWebPage(Guid id)
        {
            var result = _webPageService.GetWebPageById(id);

            if (!string.IsNullOrEmpty(result.error))
                return BadRequest(result.error);

            if (result.webPage == null)
                return NotFound();

            _webPageService.DeleteWebPage(result.webPage);

            return NoContent();
        }
    }
}
