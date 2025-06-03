using BoardCasterWebAPI.Interfaces;
using BoardCasterWebAPI.Model;
using DocumentFormat.OpenXml.InkML;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BoardCasterWebAPI.Data
{
    /// <summary>
    /// BookDetails
    /// </summary>
    public class BookDetails : IBookDetails
    {
        /// <summary>
        /// GetBookDetails
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Book> GetBookDetails(int id) {
            var MSSQLDBConnection = "Server=DINESHKUMAR\\DEVSERVER;Database=LMS;User Id=sa;Password=Password#1;TrustServerCertificate=True;";
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter($"Select * from Books where Id={id}", MSSQLDBConnection))
            {
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                Book book = new Book();
                if (dataTable.Rows.Count > 0 )                    
                    foreach (DataRow row in dataTable.Rows)
                    {
                        book.Id = Convert.ToInt32(row[0]);
                        book.Title = row[1].ToString();
                        book.ISBN = row[2].ToString();
                        book.CopiesAvailable = Convert.ToInt32(row[3]);
                        book.AuthorId = Convert.ToInt32(row[4]);                       
                    }
                return book;
            }
        }

    }
}
