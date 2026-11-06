SELECT h.Id, h.Name, h.Capacity, h.PricePerHour, h.IsActive
FROM Halls h
WHERE h.IsActive = 1 AND h.Capacity >= @MinCapacity
  AND NOT EXISTS (SELECT 1 FROM Bookings b WHERE b.HallId = h.Id AND b.Start < @End AND @Start < b.[End])
ORDER BY h.Name, h.Id;

-- Repeats the hall filter, every service belongs to a hall from the first result
SELECT s.Id, s.HallId, s.Name, s.Price
FROM Services s
JOIN Halls h ON h.Id = s.HallId
WHERE h.IsActive = 1 AND h.Capacity >= @MinCapacity
  AND NOT EXISTS (SELECT 1 FROM Bookings b WHERE b.HallId = h.Id AND b.Start < @End AND @Start < b.[End])
ORDER BY s.Id;
