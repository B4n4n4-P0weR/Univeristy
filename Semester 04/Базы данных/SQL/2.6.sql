SELECT maker, model, type 
FROM Product 
WHERE type='PC' AND (maker='MSI' OR maker='HyperPC');
