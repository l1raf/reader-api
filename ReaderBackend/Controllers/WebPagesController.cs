using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ReaderBackend.Dtos;
using ReaderBackend.Models;
using ReaderBackend.Services;
using System.Collections.Generic;

namespace ReaderBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebPagesController : ControllerBase
    {
        private readonly IWebPageService _webPageService;
        private readonly IMapper _mapper;

        public WebPagesController(IWebPageService webPageService, IMapper mapper)
        {
            _webPageService = webPageService;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<WebPageReadDto>> GetAllWebPages()
        {
            var result = _webPageService.GetAllWebPages();

            if (result.error != null)
                return BadRequest(result.error);

            //200
            return Ok(_mapper.Map<IEnumerable<WebPageReadDto>>(result.webPages));
        }

        [HttpGet("{id}", Name = "GetWebPageById")]
        public ActionResult<WebPageReadDto> GetWebPageById(int id)
        {
            var result = _webPageService.GetWebPageById(id);

            if (result.error != null)
                return BadRequest(result.error);

            if (result.webPage == null)
                return NotFound(); //404

            return Ok(_mapper.Map<WebPageReadDto>(result.webPage));
        }

        [HttpPost]
        public ActionResult<WebPageReadDto> AddWebPage(WebPageCreateDto webPageAddDto)
        {
            var webPageModel = _mapper.Map<WebPage>(webPageAddDto);
            var webPageReadDto = _mapper.Map<WebPageReadDto>(webPageModel);

            string error = _webPageService.AddWebPage(webPageModel);

            if (error != null)
                return BadRequest(error);

            return CreatedAtRoute(nameof(GetWebPageById), new { webPageReadDto.Id }, webPageReadDto );
        }

        [HttpPut("{id}")]
        public ActionResult UpdateWebPage(int id, WebPageUpdateDto webPageUpdateDto)
        {
            var result = _webPageService.GetWebPageById(id);

            if (result.error != null)
                return BadRequest(result.error);

            if (result.webPage == null)
                return NotFound();

            _mapper.Map(webPageUpdateDto, result.webPage);
            _webPageService.UpdateWebPage(result.webPage);

            //204
            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult PartialWebPageUpdate(int id, JsonPatchDocument<WebPageUpdateDto> patchDoc)
        {
            var result = _webPageService.GetWebPageById(id);

            if (result.error != null)
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
        public ActionResult DeleteWebPage(int id)
        {
            var result = _webPageService.GetWebPageById(id);

            if (result.error != null)
                return BadRequest(result.error);

            if (result.webPage == null)
                return NotFound();

            _webPageService.DeleteWebPage(result.webPage);

            return NoContent();
        }
    }
}
