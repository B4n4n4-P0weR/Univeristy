SELECT COUNT(*) AS Qty 
FROM Laptop 
WHERE model IN
(
SELECT model 
FROM Product 
WHERE maker = 'MSI' 
);