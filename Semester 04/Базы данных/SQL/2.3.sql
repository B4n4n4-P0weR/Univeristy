SELECT maker, model, type 
FROM Product 
WHERE NOT (type='PC' OR type='Printer');
