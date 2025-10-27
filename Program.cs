using upgaming_luka_iadze.Data;
using upgaming_luka_iadze.Dtos;
using upgaming_luka_iadze.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var AuthorList = DataStore.Authors;
var BookList = DataStore.Books;

app.MapGet("/api/books", (int? publicationYear, string? sortBy, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Fetching books with filters: year={Year}, sort={Sort}", publicationYear, sortBy);

        var booksQuery = BookList.AsQueryable();

        if (publicationYear.HasValue)
            booksQuery = booksQuery.Where(b => b.PublicationYear == publicationYear);

        if (!string.IsNullOrWhiteSpace(sortBy) && sortBy.Equals("title", StringComparison.OrdinalIgnoreCase))
            booksQuery = booksQuery.OrderBy(b => b.Title);

        var result = (from b in booksQuery join a in AuthorList on b.AuthorID equals a.ID
                select new BookDto{ID = b.ID, AuthorName = a.Name, PublicationYear = b.PublicationYear, Title = b.Title}).ToList();

        if (!result.Any())  return Results.Ok(Array.Empty<BookDto>());

        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error occurred while fetching books.");
        return Results.Problem("An unexpected error occurred while retrieving books.");
    }
});
app.MapGet("/api/authors/{id:int}/books", (int id, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Get books for author {AuthorId}", id);
        var author = AuthorList.FirstOrDefault(a => a.ID == id);
        
        if (author == null) return Results.NotFound($"Author with ID {id} was not found.");
        
        var books = BookList.Where(b => b.AuthorID == id).Select(b => new BookDto
        {
            ID = b.ID,
            Title = b.Title,
            PublicationYear = b.PublicationYear,
            AuthorName = author.Name
        }).ToList();
        
        return Results.Ok(books);    
    }
    catch (Exception ex)
    {
        logger.LogError(ex, $"Error while getting a books for author with  ID {id}.");
        return Results.Problem("An unexpected error occurred while retrieving books for the author.");
    }
    
});
app.MapPost("/api/books", (BookCreateDto dto, ILogger<Program> logger) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(dto.Title)) return Results.BadRequest("Title is required.");
        if (dto.PublicationYear > DateTime.Now.Year) return Results.BadRequest("The PublicationYear cannot be in the future.");
        
        var author = AuthorList.FirstOrDefault(a => a.ID == dto.AuthorID);
        if (author == null) return Results.BadRequest("No Author found with this id.");
        
        var newId = (BookList.Any() ? BookList.Max(b => b.ID) : 0) + 1;
        
        var newBook = new Book
        {
            ID = newId,
            Title = dto.Title,
            AuthorID = dto.AuthorID,
            PublicationYear = dto.PublicationYear,
        };
    
        BookList.Add(newBook);
        logger.LogInformation("Book '{Title}' added successfully.", newBook.Title);

        var responseDto = new BookDto
        {
            ID = newBook.ID,
            Title = newBook.Title,
            PublicationYear = newBook.PublicationYear,
            AuthorName = author.Name
        };
        
        return Results.Created($"/api/books/{newBook.ID}", responseDto);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error while creating a new book.");
        return Results.Problem("An unexpected error occurred while creating the book.");
    }
});

app.UseHttpsRedirection();

app.Run();