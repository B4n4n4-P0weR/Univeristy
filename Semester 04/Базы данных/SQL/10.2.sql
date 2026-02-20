SELECT TOP 3 WITH ties model, price
FROM PC
WHERE price IS NOT NULL 
ORDER BY price;
