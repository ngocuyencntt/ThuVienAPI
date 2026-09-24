using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;
namespace WebAPI_simple.Repositories
{
    public interface IAuthorRepository
    {
        List<AuthorDTO> GellAllAuthors();
        AuthorNoIdDTO GetAuthorById(int id);
        AddAuthorRequestDTO AddAuthor(AddAuthorRequestDTO addAuthorRequestDTO);
        AuthorNoIdDTO UpdateAuthorById(int id, AuthorNoIdDTO authorNoIdDTO);
        Author? DeleteAuthorById(int id);
        //bai tap : Lấy danh sách sách mà tác giả này viết.
        List<BookWithAuthorAndPublisherDTO> GetBooksByAuthorId(int authorId);
    }
}