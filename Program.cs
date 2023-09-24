


using Microsoft.EntityFrameworkCore;
using NewLab1_MinimalAPI.Data;
using NewLab1_MinimalAPI.Models;
using NewLab1_MinimalAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBookRepository<Book>, BookRepository>();

//DB Conection
builder.Services.AddDbContext<LibDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("Minimal-Connection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


//EndPoints 

//GetAllBooks
app.MapGet("/books", async (IBookRepository<Book> repo) =>
{
    try
    {
        var book = await repo.GetAll();
        return Results.Ok(book);


    }
    catch (Exception)
    {

        return Results.StatusCode(500);
    }
}).WithName("GetAllBooks");

//GetSingelBook By ID 

app.MapGet("/books{id}", async (IBookRepository<Book> repo, int id) =>
{
    var book = await repo.GetBookById(id);
    try
    {
        if (book == null)
        {
            return Results.NotFound("Book with this ID does not exsist");
        }
        return Results.Ok(book);

    }
    catch (Exception)
    {

        return Results.StatusCode(500);
    }
}).WithName("GetBookById");

//Add Book 
app.MapPost("/book", async (IBookRepository<Book> repo, Book newBook) =>
{
    try
    {
        await repo.AddBook(newBook);
        if (newBook == null)
        {
            return Results.StatusCode(404);
        }
        return Results.Created($"/books/{newBook.Id}", newBook);
    }
    catch (Exception)
    {

        return Results.StatusCode(500);
    }

});

//Update Book
app.MapPut("/book/{id}", async (IBookRepository<Book> repo, int id, Book bookToUpdate) =>
{
    try
    {
        if (id != bookToUpdate.Id)
        {
            return Results.StatusCode(400);
        }
        var response = await repo.UpdateBook(bookToUpdate);
        if (response == null)
        {
            return Results.NotFound();
        }
        return Results.Ok(bookToUpdate);
    }
    catch (Exception)
    {
        return Results.StatusCode(500);
    }

});

//Delete a  book 
app.MapDelete("/book/{id}", async (IBookRepository<Book> repo, int id) =>
{
    try
    {
        var bookToDelete = await repo.DeleteBook(id);
        if (bookToDelete == null)
        {
            return Results.NotFound();
        }
        return Results.Ok(bookToDelete);
    }
    catch (Exception)
    {

        return Results.StatusCode(500);
    }

});

//Search Book Title
app.MapGet("/bookSearch/{string}", async (IBookRepository<Book> repo, string keyWord) =>
{
    try
    {
        var tempList = await repo.SearchForBook(keyWord);
        if (tempList == null)
        {
            return Results.StatusCode(404);
        }
        return Results.Ok(tempList);
    }
    catch (Exception)
    {
        return Results.StatusCode(500);
    }
});



app.Run();


