SELECT h.Id, h.Name, h.IsActive,
       COUNT(b.Id),
       ISNULL(SUM(DATEDIFF(MINUTE, b.Start, b.[End])), 0),
       ISNULL(SUM(b.RentPrice), 0),
       ISNULL(SUM(b.ServicesPrice), 0)
FROM Halls h
LEFT JOIN Bookings b ON b.HallId = h.Id AND b.Start >= @From AND b.Start < @To
GROUP BY h.Id, h.Name, h.IsActive
HAVING h.IsActive = 1 OR COUNT(b.Id) > 0
ORDER BY h.Name
