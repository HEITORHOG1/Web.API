using Microsoft.AspNetCore.Mvc;
using Web.Application.Interfaces;
using Web.Domain.Entities;
using Web.Domain.Paginacao;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controller for handling error log operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorLogController : ControllerBase
    {
        private readonly IErrorLogService _errorLogService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorLogController"/> class.
        /// </summary>
        /// <param name="errorLogService">The error log service.</param>
        public ErrorLogController(IErrorLogService errorLogService)
        {
            _errorLogService = errorLogService;
        }

        /// <summary>
        /// Endpoint to log a new error.
        /// </summary>
        /// <param name="errorLog">The error log details.</param>
        /// <returns>Action result indicating the result of the operation.</returns>
        [HttpPost("log-error")]
        public async Task<IActionResult> LogError([FromBody] ErrorLog errorLog)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _errorLogService.LogErrorAsync(errorLog);
            return Ok(new { Message = "Error logged successfully" });
        }

        /// <summary>
        /// Endpoint to search errors by keyword.
        /// </summary>
        /// <param name="keyword">The keyword to search for.</param>
        /// <param name="paginationParameters">Pagination parameters.</param>
        /// <returns>Paged result of matching error logs.</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchErrors(string keyword, [FromQuery] PaginationParameters paginationParameters)
        {
            var result = await _errorLogService.SearchErrorsByKeywordAsync(keyword, paginationParameters);
            return Ok(result);
        }

        /// <summary>
        /// Endpoint to get all error logs ordered by date.
        /// </summary>
        /// <returns>List of all error logs ordered by date.</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllErrorsOrderedByDate()
        {
            var result = await _errorLogService.GetAllErrorsOrderedByDateAsync();
            return Ok(result);
        }
    }
}