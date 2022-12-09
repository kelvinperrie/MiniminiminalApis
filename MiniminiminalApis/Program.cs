using Microsoft.EntityFrameworkCore;
using MiniminiminalApis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PokemonDb>(opt => opt.UseInMemoryDatabase("PokemonDB"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

// can just return a string? with an accent
app.MapGet("/", () => "Hello Vorld!");
// second param is a delegate so you can just chuck a method in there if you want, you can't stop me
app.MapGet("/howdy", SayHello);

// list all of our pokemons
app.MapGet("/pokemon", async (PokemonDb db) =>
await db.Pokemons.ToListAsync());

// list all of our pokemons, but filtered!
app.MapGet("/pokemon/fire", async (PokemonDb db) =>
await db.Pokemons.Where(p => string.Equals(p.Type,"fire",StringComparison.InvariantCultureIgnoreCase)).ToListAsync());

// get a specific pokemon
app.MapGet("/pokemon/{id}", async (int id, PokemonDb db) =>
    await db.Pokemons.FindAsync(id)
        is Pokemon pokey
            ? Results.Ok(pokey)
            : Results.NotFound());

// can't do this, it's going to look like the same entry point as above. Everything's a string right, even when it's an int.
//app.MapGet("/pokemon/{name}", async (string name, PokemonDb db) =>
//    await db.Pokemons.FirstOrDefaultAsync(p => p.Name != null && p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
//        is Pokemon pokey
//            ? Results.Ok(pokey)
//            : Results.NotFound());

// make new pokemon
app.MapPost("/pokemon", async (Pokemon pokemon, PokemonDb db) =>
{
    db.Pokemons.Add(pokemon);
    await db.SaveChangesAsync();

    return Results.Created($"/pokemon/{pokemon.Id}",pokemon);
});

// update a pokemon
app.MapPut("/pokemon/{id}", async (int id, Pokemon updatedPokemon, PokemonDb db) =>
{
    var pokey = await db.Pokemons.FindAsync(id);

    if (pokey is null) return Results.NotFound();

    pokey.Name = updatedPokemon.Name;
    pokey.Type = updatedPokemon.Type;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

// why do we have to pass the id in the url, we are passing in the updated pokemon anyway! lets just use the id in the updatedPokemon and 
// since it is a put that should distinguish from the other entry points?? heck if i know
app.MapPut("/pokemon/", async (Pokemon updatedPokemon, PokemonDb db) =>
{
    var pokey = await db.Pokemons.FindAsync(updatedPokemon.Id);

    if (pokey is null) return Results.NotFound();

    pokey.Name = updatedPokemon.Name;
    pokey.Type = updatedPokemon.Type;

    await db.SaveChangesAsync();

    return Results.NoContent();
});



app.Run();

static async Task<IResult> SayHello()
{
    return Results.Ok("howdy; top level statements, feels a bit God class to me, I like it, just dump everything here.");
}

/*
{
    "name": "flameypop",
    "type": "fire"
}
{
    "name": "peeklechunk",
    "type": "electric"
}
{
    "name": "splashdon",
    "type": "water"
}

*/