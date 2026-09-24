using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;

namespace WebAPI_simple.Repositories
{
    public interface IBookRepository
    {
        List<BookWithAuthorAndPublisherDTO> GetAllBooks();
        BookWithAuthorAndPublisherDTO GetBookById(int id);
        addBookRequestDTO AddBook(addBookRequestDTO addBookRequestDTO);
        addBookRequestDTO? UpdateBookById(int id, addBookRequestDTO bookDTO);
        Book? DeleteBookById(int id);
    }
}