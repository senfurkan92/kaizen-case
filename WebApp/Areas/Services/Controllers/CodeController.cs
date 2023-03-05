using CodeWorks;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Areas.Services.Controllers
{
    public class CodeController : ControllerBase
    {
        private readonly ICodeGenerator _codeGenerator;

        public CodeController(ICodeGenerator codeGenerator)
        {
            _codeGenerator= codeGenerator;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var code = _codeGenerator.GenerateToken();
            var result = new { code = code };
            return Ok(result);
        }

        [HttpGet]
        public IActionResult Verify(string code)
        {
            var isVerified = _codeGenerator.VerifyToken(code);
            var result = new { isVerified = isVerified };
            return Ok(result);
        }
    }
}
