
using System.Linq.Expressions;
using Marten.Linq;

namespace MartenBug;

public sealed class UserLookup : ICompiledQuery<User, User?>
{
    public string Name { get; set; } = null!;

    public Expression<Func<IMartenQueryable<User>, User?>> QueryIs()
    {
        return q => q.FirstOrDefault(x => x.Name == Name);
    }
}

public sealed class ShapeLookup : ICompiledQuery<ShapeBase, ShapeBase?>
{
    public string Color { get; set; } = null!;

    public Expression<Func<IMartenQueryable<ShapeBase>, ShapeBase?>> QueryIs()
    {
        return q => q.FirstOrDefault(x => x.Color == Color);
    }
}


