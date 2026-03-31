SELECT * 
FROM product 
WHERE maker = 'MSI' 
UNION 
SELECT * 
FROM product 
WHERE maker = 'Xerox'; 
