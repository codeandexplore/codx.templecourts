using Codx.Temple.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Codx.Temple.API.IntegrationTests.Controllers;

[ApiController]
[Route("test")]
public class TestQuestionsController : ControllerBase
{
    [HttpGet("question")]
    public IActionResult GetQuestion()
    {
        var question = Codx.Temple.Domain.Entities.Question.Create(
            Guid.NewGuid(),
            1,
            Codx.Temple.Domain.Enums.QuestionType.Essay,
            "What is the meaning of life?",
            referenceContext: System.Text.Json.JsonDocument.Parse("{\"answer\": \"42\"}"));

        return Ok(question);
    }
}
