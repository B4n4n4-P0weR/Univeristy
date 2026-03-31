SELECT maker, model, type 
FROM Product 
WHERE (type='PC' AND maker='DEXP') OR maker='HP';
