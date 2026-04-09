using Microsoft.Data.Sqlite;
using BookHighlights.Models;

namespace BookHighlights.Data;

public class AppDbContext : IDisposable
{
    private readonly SqliteConnection _connection;

    public AppDbContext()
    {
        _connection = new SqliteConnection("Data Source=bookhighlights.db");
        _connection.Open();
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Books (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Author TEXT,
                CreatedAt TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Highlights (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                BookId INTEGER NOT NULL,
                Text TEXT NOT NULL,
                ImagePath TEXT,
                PageNumber INTEGER,
                CreatedAt TEXT NOT NULL,
                FOREIGN KEY (BookId) REFERENCES Books(Id) ON DELETE CASCADE
            );
        ";
        cmd.ExecuteNonQuery();
    }

    // Books
    public IEnumerable<Book> GetAllBooks()
    {
        var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Books ORDER BY CreatedAt DESC";
        using var reader = cmd.ExecuteReader();
        var books = new List<Book>();
        while (reader.Read())
        {
            books.Add(new Book
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Author = reader.IsDBNull(2) ? null : reader.GetString(2),
                CreatedAt = DateTime.Parse(reader.GetString(3))
            });
        }
        return books;
    }

    public Book? GetBookById(int id)
    {
        var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Books WHERE Id = @Id";
        cmd.Parameters.AddWithValue("@Id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Book
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Author = reader.IsDBNull(2) ? null : reader.GetString(2),
                CreatedAt = DateTime.Parse(reader.GetString(3))
            };
        }
        return null;
    }

    public int AddBook(Book book)
    {
        var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Books (Title, Author, CreatedAt) 
            VALUES (@Title, @Author, @CreatedAt);
            SELECT last_insert_rowid();
        ";
        cmd.Parameters.AddWithValue("@Title", book.Title);
        cmd.Parameters.AddWithValue("@Author", (object?)book.Author ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CreatedAt", book.CreatedAt.ToString("o"));
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    // Highlights
    public IEnumerable<Highlight> GetHighlightsByBookId(int bookId)
    {
        var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Highlights WHERE BookId = @BookId ORDER BY CreatedAt DESC";
        cmd.Parameters.AddWithValue("@BookId", bookId);
        using var reader = cmd.ExecuteReader();
        var highlights = new List<Highlight>();
        while (reader.Read())
        {
            highlights.Add(new Highlight
            {
                Id = reader.GetInt32(0),
                BookId = reader.GetInt32(1),
                Text = reader.GetString(2),
                ImagePath = reader.IsDBNull(3) ? null : reader.GetString(3),
                PageNumber = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                CreatedAt = DateTime.Parse(reader.GetString(5))
            });
        }
        return highlights;
    }

    public int AddHighlight(Highlight highlight)
    {
        var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Highlights (BookId, Text, ImagePath, PageNumber, CreatedAt) 
            VALUES (@BookId, @Text, @ImagePath, @PageNumber, @CreatedAt);
            SELECT last_insert_rowid();
        ";
        cmd.Parameters.AddWithValue("@BookId", highlight.BookId);
        cmd.Parameters.AddWithValue("@Text", highlight.Text);
        cmd.Parameters.AddWithValue("@ImagePath", (object?)highlight.ImagePath ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PageNumber", highlight.PageNumber);
        cmd.Parameters.AddWithValue("@CreatedAt", highlight.CreatedAt.ToString("o"));
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }
}
