SELECT Id, Name, Capacity, PricePerHour, IsActive FROM Halls WHERE Id = @Id AND IsActive = 1;

SELECT Id, HallId, Name, Price FROM Services WHERE HallId = @Id ORDER BY Id;
