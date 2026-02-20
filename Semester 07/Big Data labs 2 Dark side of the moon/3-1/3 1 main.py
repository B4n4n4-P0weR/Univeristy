import pandas as pd
import matplotlib.pyplot as plt

# Чтение данных
data = pd.read_csv("pokemonDB_dataset.csv", delimiter=',')

# Очистка данных и конвертация в числовой формат
data['Height'] = data['Height'].str.extract(r'(\d+\.\d+)').astype(float)
data['Weight'] = data['Weight'].str.extract(r'(\d+\.\d+)').astype(float)

# Сортировка по столбцам 'Height'
data.sort_values(by=['Height', 'Weight'], inplace=True)

# Оригинальные данные для сравнения
original_data = data.copy()

# Пересчет среднего значения и стандартного отклонения для всего датасета
mean_weight = data['Weight'].mean()
std_weight = data['Weight'].std()

# Установка порога для определения аномалий на уровне среднее значение + 3 стандартных отклонения
weight_threshold = mean_weight + 5 * std_weight

# Применение этого порога для фильтрации данных
simple_filtered_data = data[(data['Weight'] <= weight_threshold) & (data['Weight'] != '—')]

# Просмотр результатов после применения упрощенного метода фильтрации
print(simple_filtered_data[['Pokemon', 'Height', 'Weight']].head())
print(original_data[['Pokemon', 'Height', 'Weight']].head())

# Визуализация
plt.figure(figsize=(12, 6))

# График без аномалий
plt.plot(simple_filtered_data['Height'], simple_filtered_data['Weight'], 'g', label='Без аномалий')
plt.title('График без аномалий')
plt.xlabel('Height')
plt.ylabel('Weight')
plt.legend()
plt.show()

# График с аномалиями
plt.plot(original_data['Height'], original_data['Weight'], 'b', label='С аномалиями')
plt.title('График с аномалиями')
plt.xlabel('Height')
plt.ylabel('Weight')
plt.legend()
plt.show()