-- The copies in the bookings. What was sold, at the prices it was sold for
SELECT bs.Name, COUNT(*), SUM(bs.Price)
FROM BookedServices bs
JOIN Bookings b ON b.Id = bs.BookingId
WHERE b.Start >= @From AND b.Start < @To
GROUP BY bs.Name
ORDER BY COUNT(*) DESC, bs.Name
