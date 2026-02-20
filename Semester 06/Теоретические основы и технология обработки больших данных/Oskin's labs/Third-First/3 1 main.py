import pandas as pd
import numpy as np
import matplotlib.pyplot as plt

# Чтение данных
values_df = pd.read_csv("/Users/vlad/Developer/Oskin's labs/Third-First/pokemonDB_dataset.csv", delimiter=',')

# Очистка данных и конвертация в числовой формат
values_df['Height'] = values_df['Height'].str.extract(r'(\d+\.\d+)').astype(float)
values_df['Weight'] = values_df['Weight'].str.extract(r'(\d+\.\d+)').astype(float)
values_df['HP Base'] = values_df['HP Base'].astype(float)

# Сортировка по столбцам 'Height'
values_df.sort_values(by=['Height', 'Weight'], inplace=True)

# Пересчет среднего значения и стандартного отклонения для всего датасета
mean_weight = values_df['Weight'].mean()
std_weight = values_df['Weight'].std()

# Установка порога для определения аномалий на уровне среднее значение + 3 стандартных отклонения
weight_threshold = mean_weight + 5 * std_weight

# Применение этого порога для фильтрации данных
simple_filtered_data = values_df[(values_df['Weight'] <= weight_threshold) & (values_df['Weight'] != '—')]

# Просмотр результатов после применения упрощенного метода фильтрации
print(simple_filtered_data[['Pokemon', 'Height', 'Weight']].head())


# Визуализация
plt.figure()

values_arr_Height = np.array(simple_filtered_data)[:, 3]
values_arr_Weight = np.array(simple_filtered_data)[:, 4]

x = np.linspace(values_arr_Height[0], values_arr_Height[values_arr_Height.size - 1], values_arr_Height.size)

plt.plot(x, values_arr_Height,  'g', label='Height')

plt.legend()
plt.xlabel(' ')
plt.ylabel(' ')
plt.title('1')

plt.show()

plt.figure()
plt.plot(x, values_arr_Weight,  'b', label='Weight')

plt.legend()
plt.xlabel(' ')
plt.ylabel(' ')
plt.title('2')

plt.show()