using Microsoft.EntityFrameworkCore;

namespace MiniminiminalApis
{
    public class PokemonDb : DbContext
    {
        public PokemonDb(DbContextOptions<PokemonDb> options)
    : base(options) { }

        public DbSet<Pokemon> Pokemons => Set<Pokemon>();
    }
}
