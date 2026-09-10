using Microsoft.AspNetCore.Mvc;

namespace ProfanityService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProfanitiesController : ControllerBase
{
    // Filter out profanity words in comments
    
    //1. Takes the comment
    //2. Fetch profanity words
    //3. Filter out the banned words
    //4. Return the cleaned comment
    
}
