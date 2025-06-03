using System.Data;

namespace BoardCasterWebAPI.Interfaces
{
    public interface IBookDetails
    {
        public Task<Book> GetBookDetails(int id);
        
    }
}
