using RealStatePortal.Domain.Common;

namespace RealStatePortal.Domain.Entities;

public sealed class PropertyImage : Entity
{
    public PropertyImage(Guid id, string url, string altText, int sortOrder, bool isPrimary)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("Image URL is required.", nameof(url));
        }

        if (sortOrder < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sortOrder));
        }

        Url = url.Trim();
        AltText = altText?.Trim() ?? string.Empty;
        SortOrder = sortOrder;
        IsPrimary = isPrimary;
    }

    private PropertyImage()
        : base(Guid.NewGuid())
    {
        Url = string.Empty;
        AltText = string.Empty;
    }

    public string Url { get; private set; }
    public string AltText { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsPrimary { get; private set; }
}
