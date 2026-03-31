SELECT maker, Product.model AS model_1, pc.model AS model_2, price 
FROM Product LEFT JOIN PC ON PC.model = Product.model 
WHERE type = 'pc' 
ORDER BY maker, PC.model;
