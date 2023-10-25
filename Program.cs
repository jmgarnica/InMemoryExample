using Microsoft.EntityFrameworkCore;

var entity1 = new Entity(default, new OwnedEntity(true));
var entity2 = new Entity(default, new OwnedEntity(true));

var contextOptions = new DbContextOptionsBuilder<Context>()
    .UseInMemoryDatabase(Guid.NewGuid().ToString())
    .LogTo(Console.WriteLine);
var context = new Context(contextOptions.Options);

context.AddRange(entity1, entity2);
context.SaveChanges();

var query = context.Set<Entity>().Where(e => e.Owned.Property2);
var total = query.Union(query).Select(e => e.Owned.Property2);
var result = total.ToList();

Console.WriteLine(string.Join(',', result));

record Entity(int Id, OwnedEntity Owned)
{
    private Entity() : this(default, new OwnedEntity(default))
    {
    }
}

record OwnedEntity(bool Property2);

class Context : DbContext
{
    public Context(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Entity>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.OwnsOne(e => e.Owned);
        });
    }
}
