namespace SnapMyBook.Domain.Entities;

public class Tag
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string NormalizedName { get; private set; } = string.Empty;

    private Tag()
    {
    }

    public Tag(string name)
    {
        Rename(name);
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tag name is required.", nameof(name));
        }

        Name = name.Trim();
        NormalizedName = Name.ToUpperInvariant();
    }
}
