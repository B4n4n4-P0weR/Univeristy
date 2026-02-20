<!DOCTYPE html>
<html>
<head>
    <title>Результат конвертации</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            text-align: center;
        }
        .result {
            font-size: 24px;
            margin: 20px;
        }
    </style>
</head>
<body>
<?php
if ($_SERVER["REQUEST_METHOD"] == "POST") {

    // Получить значение выбранного товара из формы
    $selectedTech = $_POST['tech'];

    // Здесь вы можете определить цены для каждого товара
    $priceList = [
        "TV" => 500, // Здесь цена для телевизора
        "Printer" => 150, // Здесь цена для принтера
        "M_Center" => 300, // Здесь цена для музыкального центра
        "Mouse" => 20, // Здесь цена для радио-мыши
        "Monitor" => 250, // Здесь цена для монитора
    ];

    // Получить цену для выбранного товара
    $selectedPrice = $priceList[$selectedTech];

    // Вернуть цену как ответ на запрос

    ?>
    <div class="result">
        Цена выбранного товара: <b>
        <?php echo $selectedPrice; ?> ₽</b>
    </div>
<?php } ?>
</body>
</html>


