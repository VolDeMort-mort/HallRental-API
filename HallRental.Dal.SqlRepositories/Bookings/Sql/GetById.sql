SELECT b.Id, b.HallId, b.ClientId, b.Start, b.[End], b.RentPrice, b.ServicesPrice, h.Name
FROM Bookings b
JOIN Halls h ON h.Id = b.HallId
WHERE b.Id = @Id;

SELECT ServiceId, Name, Price FROM BookedServices WHERE BookingId = @Id ORDER BY Id;
