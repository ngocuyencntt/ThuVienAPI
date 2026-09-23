using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_simple.Data;

namespace WebAPI_simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        // viết các action Post, Get, Update, Detele
        private readonly AppDbContext _dbContext;
        public BookController(AppDbContext dbContext)
        {
                _dbContext = dbContext;
        }
    }
}
