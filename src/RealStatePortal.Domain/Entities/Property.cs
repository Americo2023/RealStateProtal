using RealStatePortal.Domain.Common;
using RealStatePortal.Domain.Enums;
using RealStatePortal.Domain.ValueObjects;

namespace RealStatePortal.Domain.Entities;

#pragma warning disable CA1716
public sealed class Property : AggregateRoot
{
    private readonly List<PropertyImage> images = [];

    public Property(
        string referenceNumber,
        string title,
        string description,
        PropertyType propertyType,
        Money price,
        int bedrooms,
        int bathrooms,
        int rooms,
        decimal livingArea,
        decimal totalArea,
        int? floor,
        int? numberOfFloors,
        int? constructionYear,
        EnergyClass energyClass,
        Guid brokerId,
        DateTimeOffset? createdAt = null,
        Guid? id = null)
        : base(id)
    {
        ReferenceNumber = Guard.Required(referenceNumber, nameof(referenceNumber));
        Title = Guard.Required(title, nameof(title));
        Description = Guard.Required(description, nameof(description));
        PropertyType = propertyType;
        Price = price;
        Bedrooms = Guard.NonNegative(bedrooms, nameof(bedrooms));
        Bathrooms = Guard.NonNegative(bathrooms, nameof(bathrooms));
        Rooms = Guard.NonNegative(rooms, nameof(rooms));
        LivingArea = Guard.Positive(livingArea, nameof(livingArea));
        TotalArea = Guard.Positive(totalArea, nameof(totalArea));
        ValidateOptionalNonNegative(floor, nameof(floor));
        ValidateOptionalNonNegative(numberOfFloors, nameof(numberOfFloors));
        ValidateConstructionYear(constructionYear);
        Floor = floor;
        NumberOfFloors = numberOfFloors;
        ConstructionYear = constructionYear;
        EnergyClass = energyClass;
        BrokerId = Guard.Required(brokerId, nameof(brokerId));
        Status = PropertyStatus.Draft;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public string ReferenceNumber { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public PropertyStatus Status { get; private set; }
    public PropertyType PropertyType { get; private set; }
    public Money Price { get; private set; }
    public int Bedrooms { get; private set; }
    public int Bathrooms { get; private set; }
    public int Rooms { get; private set; }
    public decimal LivingArea { get; private set; }
    public decimal TotalArea { get; private set; }
    public int? Floor { get; private set; }
    public int? NumberOfFloors { get; private set; }
    public int? ConstructionYear { get; private set; }
    public EnergyClass EnergyClass { get; private set; }
    public DateTimeOffset? PublishedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid BrokerId { get; private set; }
    public PropertyAddress? Address { get; private set; }
    public IReadOnlyCollection<PropertyImage> Images => images.AsReadOnly();

    public void Publish(DateTimeOffset occurredAt)
    {
        EnsureStatus(PropertyStatus.Draft, "Only draft properties can be published.");
        Status = PropertyStatus.Published;
        PublishedAt = occurredAt;
        Touch(occurredAt);
    }

    public void Withdraw(DateTimeOffset occurredAt)
    {
        EnsureStatus(PropertyStatus.Published, "Only published properties can be withdrawn.");
        Status = PropertyStatus.Draft;
        PublishedAt = null;
        Touch(occurredAt);
    }

    public void MarkAsSold(DateTimeOffset occurredAt)
    {
        EnsureStatus(PropertyStatus.Published, "Only published properties can be marked as sold.");
        Status = PropertyStatus.Sold;
        Touch(occurredAt);
    }

    public void Delete(DateTimeOffset occurredAt)
    {
        EnsureStatus(PropertyStatus.Sold, "Only sold properties can be deleted.");
        Status = PropertyStatus.Deleted;
        Touch(occurredAt);
    }

    public void TransferTo(Guid brokerId, DateTimeOffset occurredAt)
    {
        EnsureNotDeleted();
        BrokerId = Guard.Required(brokerId, nameof(brokerId));
        Touch(occurredAt);
    }

    public void UpdateDetails(
        string title,
        string description,
        PropertyType propertyType,
        Money price,
        int bedrooms,
        int bathrooms,
        int rooms,
        decimal livingArea,
        decimal totalArea,
        int? floor,
        int? numberOfFloors,
        int? constructionYear,
        EnergyClass energyClass,
        DateTimeOffset occurredAt)
    {
        EnsureNotDeleted();
        ReferenceNumber = Guard.Required(ReferenceNumber, nameof(ReferenceNumber));
        Title = Guard.Required(title, nameof(title));
        Description = Guard.Required(description, nameof(description));
        Price = price;
        Bedrooms = Guard.NonNegative(bedrooms, nameof(bedrooms));
        Bathrooms = Guard.NonNegative(bathrooms, nameof(bathrooms));
        Rooms = Guard.NonNegative(rooms, nameof(rooms));
        LivingArea = Guard.Positive(livingArea, nameof(livingArea));
        TotalArea = Guard.Positive(totalArea, nameof(totalArea));
        ValidateOptionalNonNegative(floor, nameof(floor));
        ValidateOptionalNonNegative(numberOfFloors, nameof(numberOfFloors));
        ValidateConstructionYear(constructionYear);
        PropertyType = propertyType;
        Floor = floor;
        NumberOfFloors = numberOfFloors;
        ConstructionYear = constructionYear;
        EnergyClass = energyClass;
        Touch(occurredAt);
    }

    public void SetAddress(PropertyAddress address, DateTimeOffset occurredAt)
    {
        EnsureNotDeleted();
        Address = address ?? throw new ArgumentNullException(nameof(address));
        Touch(occurredAt);
    }

    public void AddImage(PropertyImage image, DateTimeOffset occurredAt)
    {
        EnsureNotDeleted();
        ArgumentNullException.ThrowIfNull(image);

        if (image.IsPrimary)
        {
            ClearPrimaryImage();
        }

        images.Add(image);
        Touch(occurredAt);
    }

    public void RemoveImage(Guid imageId, DateTimeOffset occurredAt)
    {
        EnsureNotDeleted();
        var image = images.SingleOrDefault(candidate => candidate.Id == imageId)
            ?? throw new DomainException("The property image does not belong to this property.");

        images.Remove(image);
        Touch(occurredAt);
    }

    public void SetPrimaryImage(Guid imageId, DateTimeOffset occurredAt)
    {
        EnsureNotDeleted();
        var image = images.SingleOrDefault(candidate => candidate.Id == imageId)
            ?? throw new DomainException("The property image does not belong to this property.");

        ClearPrimaryImage();
        image.Update(image.Url, image.AltText, image.SortOrder, true);
        Touch(occurredAt);
    }

    private void EnsureStatus(PropertyStatus expectedStatus, string message)
    {
        if (Status != expectedStatus)
        {
            throw new DomainException(message);
        }
    }

    private void EnsureNotDeleted()
    {
        if (Status == PropertyStatus.Deleted)
        {
            throw new DomainException("Deleted properties cannot be modified.");
        }
    }

    private void ClearPrimaryImage()
    {
        foreach (var image in images.Where(image => image.IsPrimary))
        {
            image.Update(image.Url, image.AltText, image.SortOrder, false);
        }
    }

    private void Touch(DateTimeOffset occurredAt)
    {
        UpdatedAt = occurredAt;
    }

    private static void ValidateOptionalNonNegative(int? value, string parameterName)
    {
        if (value is < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Value cannot be negative.");
        }
    }

    private static void ValidateConstructionYear(int? constructionYear)
    {
        if (constructionYear is < 1800 or > 3000)
        {
            throw new ArgumentOutOfRangeException(nameof(constructionYear), "Construction year is outside the supported range.");
        }
    }
}
#pragma warning restore CA1716