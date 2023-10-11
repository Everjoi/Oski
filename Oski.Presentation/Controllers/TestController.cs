using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oski.Application.Features.Tests.Queries;
using Oski.Application.Features.Tests.Queries.DTOsQueries;
using Oski.Application.Interfaces;
using Oski.Domain.Entities;
using System.Security.Claims;

namespace Oski.Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private readonly ITestService _testService;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public TestController(ITestService testService,IMapper mapper, IMediator mediator)
        {
            _testService = testService;
            _mapper = mapper;
            _mediator = mediator;
        }

        [HttpPost("start/{testId}")]
        public IActionResult StartTest(Guid testId)
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

            var attemptId = _testService.StartTest(userId,testId);

            if(attemptId == Guid.Empty)
                return BadRequest("Could not start the test or test is already completed.");

            return Ok(new { AttemptId = attemptId });
        }

        [HttpPost("answer")]
        public IActionResult AnswerQuestion(Guid attemptId,Guid questionId,Guid answerId)
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

            var success = _testService.AnswerQuestion(userId,attemptId,questionId,answerId);

            if(!success)
                return BadRequest("Could not save the answer or invalid attempt.");

            return Ok();
        }

        [HttpPost("finish/{attemptId}")]
        public IActionResult FinishTest(Guid attemptId)
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

            var score = _testService.FinishTest(userId,attemptId);

            if(score < 0)
                return BadRequest("Could not finish the test or invalid attempt.");

            return Ok(new { Score = score });
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAllTests()
        {
            var tests = await _mediator.Send(new GetAllTestsQuery());
  
            if(tests == null || !tests.Any())
                return NotFound("No tests found.");

            return Ok(tests);
        }


    }
}
