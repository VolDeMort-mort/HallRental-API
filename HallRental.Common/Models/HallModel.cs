namespace HallRental.Common.Models;

public class HallModel
{
    public Guid Id { get; set; }
 
    public string Name { get; set; } = string.Empty;
 
    public int Capacity { get; set; }
 
    public decimal PricePerHour { get; set; }
 
    public bool IsActive { get; set; }
 
    public List<ServiceModel> Services { get; set; } = new();
}