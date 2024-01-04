using System.Text.Json.Serialization;

namespace MartenBug;


[JsonDerivedType(typeof(Triangle), nameof(Triangle))]
[JsonDerivedType(typeof(Square), nameof(Square))]
public abstract class ShapeBase
{
    public Guid Id { get; set; }
    public string Color { get; set; }

    protected ShapeBase(Guid id, string color)
    {
        Id = id;
        Color = color;
    }
}

public sealed class Triangle : ShapeBase
{
    public Triangle(Guid id, string color) : base(id, color) { }
}

public sealed class Square : ShapeBase
{
    public Square(Guid id, string color) : base(id, color) { }
}
