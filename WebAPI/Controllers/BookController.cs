using BoardCasterWebAPI.Data;
using BoardCasterWebAPI.Interfaces;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BoardCasterWebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        IBookDetails bookDetails = new BookDetails();
        /// <summary>
        /// get book details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getbookdetails")]
        public async Task<IActionResult> GetBookDetails(int id)
        {
           var dt= await bookDetails.GetBookDetails(id);
            return Ok(dt);
        }
    }
}
