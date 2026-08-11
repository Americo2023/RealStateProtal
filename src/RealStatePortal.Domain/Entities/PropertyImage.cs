using RealStatePortal.Domain.Common;

namespace RealStatePortal.Domain.Entities;

public sealed class PropertyImage : Entity
{
    public PropertyImage(string url, string altText, int sortOrder, bool isPrimary, Guid? id = null)
        : base(id)
    {
        Update(url, altText, sortOrder, isPrimary);
    }

    public string Url { get; private set; } = null!;
    public string AltText { get; private set; } = null!;
    public int SortOrder { get; private set; }
    public bool IsPrimary { get; private set; }

    internal void Update(string url, string altText, int sortOrder, bool isPrimary)
    {
        Url = Guard.Required(url, nameof(url));
        AltText = Guard.Required(altText, nameof(altText));
        SortOrder = Guard.NonNegative(sortOrder, nameof(sortOrder));
        IsPrimary = isPrimary;
    }
}