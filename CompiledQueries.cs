
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