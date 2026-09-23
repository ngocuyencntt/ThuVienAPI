namespace WebAPI_simple.Models.Domain
{
    public class Book_Author
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        //navigation Properties - One book has many book_author
        public Book Book { get; set; }

        public int AuthorId { get; set; }
        //navigation Properties - One author has many book_author
        public Author Author { get; set; }
    }
}