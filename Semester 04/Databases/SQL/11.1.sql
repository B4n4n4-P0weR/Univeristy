SELECT maker, 
Product.model AS model_1, 
PC.model AS model_2, price 
FROM Product INNER JOIN PC ON PC.model = Product.model 
ORDER BY maker, model_2;
