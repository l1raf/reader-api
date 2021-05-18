using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ReaderBackend.DTOs;
using ReaderBackend.Models;
using ReaderBackend.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
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
        public async Task<ActionResult> ConvertToArticle(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
                return BadRequest("Absolute uri is required");

            var result = await _webPageService.GetArticle(uri);

            if (result.error is not null)
                return BadRequest(result.error);

            return Ok(result.article);
        }

        [HttpGet("articles")]
        public async Task<ActionResult> GetAllUserArticles()
        {
            string userId = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var (error, webPages) = await _webPageService.GetWebPagesByUserId(Guid.Parse((ReadOnlySpan<char>)userId));

            if (error is not null)
                return BadRequest(error);

            if (webPages is null)
                return NoContent();

            return Ok(await _webPageService.GetAllUserArticles(webPages));
        }

        [HttpGet]
        public async Task<ActionResult> GetAllUserWebPages()
        {
            string userId = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _webPageService.GetWebPagesByUserId(Guid.Parse((ReadOnlySpan<char>)userId));

            if (!string.IsNullOrEmpty(result.error))
                return BadRequest(result.error);

            return Ok(_mapper.Map<IEnumerable<WebPageReadDto>>(result.webPages));
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult> GetAllWebPages()
        {
            var result = await _webPageService.GetAllWebPages();

            if (!string.IsNullOrEmpty(result.error))
                return BadRequest(result.error);

            return Ok(_mapper.Map<IEnumerable<WebPageReadDto>>(result.webPages));
        }

        [HttpGet("{id}", Name = "GetWebPageById")]
        public async Task<ActionResult> GetWebPageById(Guid id)
        {
            var result = await _webPageService.GetWebPageById(id);

            if (!string.IsNullOrEmpty(result.error))
                return BadRequest(result.error);

            if (result.webPage is null)
                return NotFound();

            return Ok(_mapper.Map<WebPageReadDto>(result.webPage));
        }

        [HttpPost]
        public async Task<ActionResult> AddWebPage(WebPageCreateDto webPageAddDto)
        {
            string userId = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var webPageModel = _mapper.Map<WebPage>(webPageAddDto);

            webPageModel.UserId = Guid.Parse((ReadOnlySpan<char>)userId);

            string error = await _webPageService.AddWebPage(webPageModel);

            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);

            var webPageReadDto = _mapper.Map<WebPageReadDto>(webPageModel);

            return CreatedAtRoute(
                nameof(GetWebPageById),
                new { webPageModel.Id },
                webPageReadDto);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateWebPage(WebPageUpdateDto updateDto)
        {
            string userId = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var (error, webPage) = await _webPageService.GetUserWebPageByUri(updateDto.Uri, Guid.Parse((ReadOnlySpan<char>)userId));

            if (error is not null)
                return BadRequest(error);

            if (webPage is null)
                return NotFound();

            updateDto.Title ??= webPage.Title;

            _mapper.Map(updateDto, webPage);
            await _webPageService.UpdateWebPage(webPage);

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateWebPage(Guid id, WebPageUpdateDto webPageUpdateDto)
        {
            var result = await _webPageService.GetWebPageById(id);

            if (!string.IsNullOrEmpty(result.error))
                return BadRequest(result.error);

            if (result.webPage is null)
                return NotFound();

            _mapper.Map(webPageUpdateDto, result.webPage);
            await _webPageService.UpdateWebPage(result.webPage);

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialWebPageUpdate(Guid id, JsonPatchDocument<WebPageUpdateDto> patchDoc)
        {
            var result = await _webPageService.GetWebPageById(id);

            if (!string.IsNullOrEmpty(result.error))
                return BadRequest(result.error);

            if (result.webPage is null)
                return NotFound();

            var webPageToPatch = _mapper.Map<WebPageUpdateDto>(result);
            patchDoc.ApplyTo(webPageToPatch, ModelState);

            if (!TryValidateModel(webPageToPatch))
                return ValidationProblem(ModelState);

            _mapper.Map(webPageToPatch, result.webPage);
            await _webPageService.UpdateWebPage(result.webPage);

            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteWebPage(Uri uri)
        {
            string userId = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _webPageService.GetUserWebPageByUri(uri, Guid.Parse((ReadOnlySpan<char>)userId));

            if (!string.IsNullOrEmpty(result.error))
                return BadRequest(result.error);

            if (result.webPage is null)
                return NotFound();

            await _webPageService.DeleteWebPage(result.webPage);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWebPage(Guid id)
        {
            var result = await _webPageService.GetWebPageById(id);

            if (!string.IsNullOrEmpty(result.error))
                return BadRequest(result.error);

            if (result.webPage is null)
                return NotFound();

            await _webPageService.DeleteWebPage(result.webPage);

            return NoContent();
        }
    }
}
