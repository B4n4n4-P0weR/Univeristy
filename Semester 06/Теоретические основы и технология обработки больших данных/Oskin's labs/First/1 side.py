import pandas as pd
import numpy as np
import matplotlib.pyplot as plt

# Чтение данных
values_df = pd.read_csv("/Users/vlad/Developer/Oskin's labs/First/Pokemon.csv", delimiter=',')

# Очистка данных и конвертация в числовой формат
values_df['Height'] = values_df['Height'].str.extract(r'(\d+\.\d+)').astype(float)
values_df['Weight'] = values_df['Weight'].str.extract(r'(\d+\.\d+)').astype(float)
values_df['HP Base'] = values_df['HP Base'].astype(float)

# Группировка данных по росту и вычисление среднего значения для каждой группы
average_values_by_height = values_df.groupby('Height').agg({'Weight': 'mean', 'HP Base': 'mean'}).reset_index()

# Визуализация
plt.figure()

# Строим график зелёного цвета с отсортированными значениями роста
sorted_height = values_df['Height'].sort_values().unique()
plt.plot(sorted_height, 'g', label='Sorted Height')

# Строим графики средних значений веса и базового уровня HP
plt.plot(np.arange(len(average_values_by_height['Weight'])), average_values_by_height['Weight'], 'b', label='Average Weight')
plt.plot(np.arange(len(average_values_by_height['HP Base'])), average_values_by_height['HP Base'], 'r', label='Average HP Base')

plt.legend()
plt.xlabel('Unique Heights')
plt.ylabel('Values')
plt.title('Height and Average Values Comparison')

plt.show()
