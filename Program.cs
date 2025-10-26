using upgaming_luka_iadze.Data;
using upgaming_luka_iadze.Dtos;
using upgaming_luka_iadze.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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

app.MapGet("Books", () => BookList);
app.MapGet("Authors/{id:int}/books", (int id, ILogger<Program> logger) =>
{
    logger.LogInformation($"Get Books with id {id}");
   var books = BookList.Where(b => b.AuthorID == id).ToList();

   if (!books.Any()) return Results.NotFound($"No books found for author with ID {id}.");
   
   return Results.Ok(books);    
});
app.MapPost("Books", (BookDto newBookDto) =>
{
    if (string.IsNullOrWhiteSpace(newBookDto.Title)) return Results.BadRequest("Title is required.");
    if (newBookDto.PublicationYear > DateTime.Now.Year) return Results.BadRequest("The PublicationYear cannot be in the future.");
    if (!AuthorList.Any(a => a.ID == newBookDto.AuthorID)) return Results.BadRequest("No Author found with this id.");
    
    var newBook = new Book
    {
        ID = BookList.Max(b => b.ID + 1),
        Title = newBookDto.Title,
        AuthorID = AuthorList.FirstOrDefault(a => a.Name == newBookDto.AuthorName)?.ID ?? 0,
        PublicationYear = newBookDto.PublicationYear,
    };
    
    BookList.Add(newBook);
    
    return Results.Created($"Books/{newBook.ID}", newBook);
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();