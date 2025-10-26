using upgaming_luka_iadze.Models;

namespace upgaming_luka_iadze.Data;

public static class DataStore
{
    private static List<Author> _authors = new()
    {
        new Author { ID = 1, Name = "Robert C. Martin" },
        new Author { ID = 2, Name = "Jeffrey Richter" }
    };

    private static List<Book> _books = new()
    {
        new Book { ID = 1, Title = "Clean Code", AuthorID = 1, PublicationYear = 2008 },
        new Book { ID = 2, Title = "CLR via C#", AuthorID = 2, PublicationYear = 2012 },
        new Book { ID = 3, Title = "The Clean Coder", AuthorID = 1, PublicationYear = 2011 }
    };

    public static List<Author> Authors => _authors;
    public static List<Book> Books => _books;
}
