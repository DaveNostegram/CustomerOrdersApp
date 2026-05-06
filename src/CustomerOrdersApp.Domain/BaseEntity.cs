namespace CustomerOrdersApp.Domain;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public int PublicId { get; set; }
}