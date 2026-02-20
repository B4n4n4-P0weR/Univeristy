SELECT * 
FROM product 
WHERE maker = 'DEXP' 
INTERSECT 	
SELECT * 
FROM product 
WHERE type = 'pc'; 
