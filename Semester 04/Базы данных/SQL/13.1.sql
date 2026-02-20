SELECT DISTINCT maker 
FROM Product AS lap_product 
WHERE type = 'pc' AND 
EXISTS (SELECT maker 
FROM Product 
WHERE type = 'printer' AND 
maker = lap_product.maker );
