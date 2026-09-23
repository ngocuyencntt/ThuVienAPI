using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_simple.Data;
using WebAPI_simple.Models.DTO;

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

        //GET http://localhost:port/api/get-all-books
        [HttpGet("get-all-books")]
        public IActionResult GetAll()
        {
            // var allBooksDomain = _dbContext.Books.ToList();
            //Get Data From Database -Domain Model
            var allBooksDomain = _dbContext.Books;
            //Map domain models to DTOs
            var allBooksDTO = allBooksDomain.Select(Books => new BookWithAuthorAndPublisherDTO()
            {
                Id = Books.Id,
                Title = Books.Title,
                Description = Books.Description,
                IsRead = Books.IsRead,
                DateRead = Books.IsRead ? Books.DateRead.Value : null,
                Rate = Books.IsRead ? Books.Rate.Value : null,
                Genre = Books.Genre,
                CoverUrl = Books.CoverUrl,
                PublisherName = Books.Publisher.Name,
                AuthorNames = Books.book_Authors.Select(n => n.Author.FullName).ToList()
            }).ToList();
            //return DTOs
            return Ok(allBooksDTO);
        }

        //Xây dựng action GetBookById (request HTTP Get) 
        [HttpGet]
        [Route("get-book-by-id/{id:int}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            //get bookDomain object from DB
            var bookDomain = _dbContext.Books.Include(b => b.Publisher).Include(b =>
            b.book_Authors).ThenInclude(ba => ba.Author).FirstOrDefault(b => b.Id == id);
            if (bookDomain == null)
            {
                return NotFound();
            }

            //map bookDomain object to DTO
            var bookDTO = new Models.DTO.BookWithAuthorAndPublisherDTO()
            {
                Id = bookDomain.Id,
                Title = bookDomain.Title,
                Description = bookDomain.Description,
                IsRead = bookDomain.IsRead,
                DateRead = bookDomain.DateRead,
                Rate = bookDomain.Rate,
                Genre = bookDomain.Genre,
                CoverUrl = bookDomain.CoverUrl,
                DateAdded = bookDomain.DateAdded,
                PublisherName = bookDomain.Publisher != null ? bookDomain.Publisher.Name : "Unknown",
                AuthorNames = bookDomain.book_Authors?.Where(y => y.Author != null).Select(y =>
                y.Author.FullName).ToList() ?? new List<string>()
            };
            return Ok(bookDTO);
        }

        //Xây dựng action AddBook (request HTTP Post)
        [HttpPost("add-book")]
        public IActionResult AddBook([FromBody] addBookRequestDTO addBookRequestDTO)
        {
            // check if publisher exists or not
            var publisherDomain = _dbContext.Publishers.FirstOrDefault(x => x.Id == addBookRequestDTO.PublisherID);
            if (publisherDomain == null)
            {
                return NotFound(new { message = "Không tìm thấy NXB" });
            }

            // create a new book domain object
            var bookDomain = new Models.Domain.Book()
            {
                Title = addBookRequestDTO.Title,
                Description = addBookRequestDTO.Description,
                IsRead = addBookRequestDTO.IsRead,
                DateRead = addBookRequestDTO.DateRead,
                Rate = addBookRequestDTO.Rate,
                Genre = addBookRequestDTO.Genre,
                CoverUrl = addBookRequestDTO.CoverUrl,
                DateAdded = addBookRequestDTO.DateAdded,
                PublisherID = publisherDomain.Id
            };

            _dbContext.Books.Add(bookDomain);
            _dbContext.SaveChanges();

            // check authors if they exist and then add to Book_Author table
            foreach (var authorId in addBookRequestDTO.AuthorIds)
            {
                var authorDomain = _dbContext.Authors.FirstOrDefault(x => x.Id == authorId);
                if (authorDomain == null)
                {
                    return NotFound(new { message = "Không tìm thấy tác giả" });
                }

                var bookAuthorDomain = new Models.Domain.Book_Author()
                {
                    BookId = bookDomain.Id,
                    AuthorId = authorDomain.Id
                };

                _dbContext.Books_Authors.Add(bookAuthorDomain);
                _dbContext.SaveChanges();
            }

            return Ok();
        }

        //Xây dựng action UpdateBookById (request HTTP Put)
        [HttpPut ("update-book-by-id/{id:int}")]
        public IActionResult UpdateBookById(int id, [FromBody] addBookRequestDTO addBookRequestDTO)
        {
            var bookDomain = _dbContext.Books.FirstOrDefault(x => x.Id == id);
            if (bookDomain == null)
            {
                bookDomain.Title = addBookRequestDTO.Title;
                bookDomain.Description = addBookRequestDTO.Description;
                bookDomain.IsRead = addBookRequestDTO.IsRead;
                bookDomain.DateRead = addBookRequestDTO.DateRead;
                bookDomain.Rate = addBookRequestDTO.Rate;
                bookDomain.Genre = addBookRequestDTO.Genre;
                bookDomain.CoverUrl = addBookRequestDTO.CoverUrl;
                bookDomain.DateAdded = addBookRequestDTO.DateAdded;
                bookDomain.PublisherID = addBookRequestDTO.PublisherID;
                _dbContext.SaveChanges ();
            }
            var existingBookAuthors = _dbContext.Books_Authors.Where(x => x.BookId == id).ToList();
            if(existingBookAuthors!=null && existingBookAuthors.Count >0)
            {
                _dbContext.Books_Authors.RemoveRange(existingBookAuthors);
                _dbContext.SaveChanges();
            }
            foreach (var authorId in addBookRequestDTO.AuthorIds)
            {
                var authorDomain = _dbContext.Authors.FirstOrDefault(x => x.Id == authorId);
                if(authorDomain == null)
                {
                    return NotFound(new { message = "Thông báo lỗi" });
                }
                var bookAuthorDomain = new Models.Domain.Book_Author()
                {
                    BookId = bookDomain.Id,
                    AuthorId = authorDomain.Id
                };
                _dbContext.Books_Authors.Add(bookAuthorDomain);
                _dbContext.SaveChanges();
            }
            return Ok(addBookRequestDTO);
        }

        //Xây dựng action DeleteById (request HTTP Delete)
        [HttpDelete("delete-book-by-id/{id:int}")]
        public IActionResult DeleteBookById(int id)
        {
            var bookDomain = _dbContext.Books.FirstOrDefault(x => x.Id == id);
            if (bookDomain == null)
            {
                return NotFound();
            }

            var existingBookAuthors = _dbContext.Books_Authors.Where(x => x.BookId == id).ToList();
            if (existingBookAuthors != null && existingBookAuthors.Count > 0)
            {
                _dbContext.Books_Authors.RemoveRange(existingBookAuthors);
                _dbContext.SaveChanges();
            }

            _dbContext.Books.Remove(bookDomain);
            _dbContext.SaveChanges();
            return Ok(bookDomain);
        }
    }
}
