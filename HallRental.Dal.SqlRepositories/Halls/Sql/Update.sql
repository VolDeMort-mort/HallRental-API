UPDATE Halls
SET Name = @Name, Capacity = @Capacity, PricePerHour = @PricePerHour, IsActive = @IsActive
WHERE Id = @Id
