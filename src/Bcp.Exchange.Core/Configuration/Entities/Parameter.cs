namespace Bcp.Exchange.Core.Configuration.Entities;

public sealed class Parameter
{
    private Parameter() { }

    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? LongDescription { get; private set; }
    public Guid? ParentId { get; private set; }
    public Parameter? Parent { get; private set; }
    public ICollection<Parameter> Children { get; private set; } = [];
    public bool IsActive { get; private set; } = true;
    public int DisplayOrder { get; private set; }
    public decimal? NumericValue { get; private set; }
    public string? TextValue { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public string? ModifiedBy { get; private set; }
    public bool IsParent => ParentId == null;

    public static Parameter Create(
        string code,
        string description,
        string? longDescription,
        Guid? parentId,
        int displayOrder,
        decimal? numericValue,
        string? textValue,
        string? createdBy
    )
    {
        return new Parameter
        {
            Id = Guid.NewGuid(),
            Code = code,
            Description = description,
            LongDescription = longDescription,
            ParentId = parentId,
            DisplayOrder = displayOrder,
            NumericValue = numericValue,
            TextValue = textValue,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
        };
    }

    public void Update(
        string? description,
        string? longDescription,
        int? displayOrder,
        decimal? numericValue,
        string? textValue,
        string? modifiedBy
    )
    {
        if (description is not null)
            Description = description;

        if (longDescription is not null)
            LongDescription = longDescription;

        if (displayOrder.HasValue)
            DisplayOrder = displayOrder.Value;

        if (numericValue.HasValue)
            NumericValue = numericValue;

        if (textValue is not null)
            TextValue = textValue;

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Delete(string? modifiedBy)
    {
        IsActive = false;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Reactivate(
        string description,
        string? longDescription,
        Guid? parentId,
        int displayOrder,
        decimal? numericValue,
        string? textValue,
        string? modifiedBy
    )
    {
        Description = description;
        LongDescription = longDescription;
        ParentId = parentId;
        DisplayOrder = displayOrder;
        NumericValue = numericValue;
        TextValue = textValue;
        IsActive = true;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}
