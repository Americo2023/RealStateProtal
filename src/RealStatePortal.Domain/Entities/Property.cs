using RealStatePortal.Domain.Common;
using RealStatePortal.Domain.Enums;
using RealStatePortal.Domain.Events;
using RealStatePortal.Domain.ValueObjects;

namespace RealStatePortal.Domain.Entities;

public sealed class Property : AggregateRoot
{
    private readonly List<PropertyImage> images = [];

    private Property(
        Guid id,
        string referenceNumber,
        string title,
        string description,
        PropertyType propertyType,
        Money price,
        Guid brokerId,
        DateTimeOffset createdAt)
        : base(id)
    {
        ReferenceNumber = Required(referenceNumber, nameof(referenceNumber));
        Title = Required(title, nameof(title));
        Description = Required(description, nameof(description));
        PropertyType = propertyType;
        Price = price ?? throw new ArgumentNullException(nameof(price));
        BrokerId = RequiredId(brokerId, nameof(brokerId));
        Status = PropertyStatus.Draft;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    private Property()
        : base(Guid.NewGuid())
    {
        ReferenceNumber = string.Empty;
        Title = string.Empty;
        Description = string.Empty;
        Price = new Money(0, "USD");
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
    public decimal? LivingArea { get; private set; }
    public decimal? TotalArea { get; private set; }
    public int? Floor { get; private set; }
    public int? NumberOfFloors { get; private set; }
    public int? ConstructionYear { get; private set; }
    public EnergyClass? EnergyClass { get; private set; }
    public DateTimeOffset? PublishedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid BrokerId { get; private set; }
    public PropertyAddress? Address { get; private set; }
    public IReadOnlyCollection<PropertyImage> Images => images.AsReadOnly();

    public static Property Create(
        string referenceNumber,
        string title,
        string description,
        PropertyType propertyType,
        Money price,
        Guid brokerId,
        DateTimeOffset createdAt)
    {
        return new Property(Guid.NewGuid(), referenceNumber, title, description, propertyType, price, brokerId, createdAt);
    }

    public void UpdateDetails(
        string title,
        string description,
        PropertyType propertyType,
        Money price,
        DateTimeOffset updatedAt)
    {
        EnsureNotDeleted();
        Title = Required(title, nameof(title));
        Description = Required(description, nameof(description));
        PropertyType = propertyType;
        Price = price ?? throw new ArgumentNullException(nameof(price));
        Touch(updatedAt);
    }

    public void UpdateCharacteristics(
        int bedrooms,
        int bathrooms,
        int rooms,
        decimal? livingArea,
        decimal? totalArea,
        int? floor,
        int? numberOfFloors,
        int? constructionYear,
        EnergyClass? energyClass,
        DateTimeOffset updatedAt)
    {
        EnsureNotDeleted();

        if (bedrooms < 0 || bathrooms < 0 || rooms < 0)
        {
            throw new DomainException("Room counts cannot be negative.");
        }

        if (livingArea is < 0 || totalArea is < 0)
        {
            throw new DomainException("Areas cannot be negative.");
        }

        if (floor is < 0 || numberOfFloors is < 0 || constructionYear is < 0)
        {
            throw new DomainException("Property characteristics cannot be negative.");
        }

        Bedrooms = bedrooms;
        Bathrooms = bathrooms;
        Rooms = rooms;
        LivingArea = livingArea;
        TotalArea = totalArea;
        Floor = floor;
        NumberOfFloors = numberOfFloors;
        ConstructionYear = constructionYear;
        EnergyClass = energyClass;
        Touch(updatedAt);
    }

    public void SetAddress(PropertyAddress address, DateTimeOffset updatedAt)
    {
        EnsureNotDeleted();
        Address = address ?? throw new ArgumentNullException(nameof(address));
        Touch(updatedAt);
    }

    public void AddImage(PropertyImage image, DateTimeOffset updatedAt)
    {
        EnsureNotDeleted();
        ArgumentNullException.ThrowIfNull(image);
        images.Add(image);
        Touch(updatedAt);
    }

    public void RemoveImage(Guid imageId, DateTimeOffset updatedAt)
    {
        EnsureNotDeleted();
        var image = images.SingleOrDefault(item => item.Id == imageId)
            ?? throw new DomainException("The image does not belong to this property.");
        images.Remove(image);
        Touch(updatedAt);
    }

    public void TransferTo(Guid brokerId, DateTimeOffset updatedAt)
    {
        EnsureNotDeleted();
        BrokerId = RequiredId(brokerId, nameof(brokerId));
        Touch(updatedAt);
    }

    public void Publish(DateTimeOffset publishedAt)
    {
        if (Status != PropertyStatus.Draft)
        {
            throw new DomainException("Only draft properties can be published.");
        }

        Status = PropertyStatus.Published;
        PublishedAt = publishedAt;
        Touch(publishedAt);
        AddDomainEvent(new PropertyPublishedEvent(Id, publishedAt));
    }

    public void Withdraw(DateTimeOffset updatedAt)
    {
        if (Status != PropertyStatus.Published)
        {
            throw new DomainException("Only published properties can be withdrawn.");
        }

        Status = PropertyStatus.Draft;
        Touch(updatedAt);
    }

    public void MarkAsSold(DateTimeOffset updatedAt)
    {
        if (Status != PropertyStatus.Published)
        {
            throw new DomainException("Only published properties can be marked as sold.");
        }

        Status = PropertyStatus.Sold;
        Touch(updatedAt);
        AddDomainEvent(new PropertySoldEvent(Id, updatedAt));
    }

    public void Delete(DateTimeOffset deletedAt)
    {
        if (Status != PropertyStatus.Sold)
        {
            throw new DomainException("Only sold properties can be deleted.");
        }

        Status = PropertyStatus.Deleted;
        Touch(deletedAt);
        AddDomainEvent(new PropertyDeletedEvent(Id, deletedAt));
    }

    private void EnsureNotDeleted()
    {
        if (Status == PropertyStatus.Deleted)
        {
            throw new DomainException("Deleted properties cannot be modified.");
        }
    }

    private void Touch(DateTimeOffset timestamp)
    {
        UpdatedAt = timestamp;
    }

    private static string Required(string value, string name)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Value is required.", name)
            : value.Trim();
    }

    private static Guid RequiredId(Guid value, string name)
    {
        return value == Guid.Empty
            ? throw new ArgumentException("Id is required.", name)
            : value;
    }
}
