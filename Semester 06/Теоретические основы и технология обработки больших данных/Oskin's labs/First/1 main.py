import pandas as pd

values_df = pd.read_csv("/Users/vlad/Developer/Oskin's labs/First/Pokemon.csv", delimiter=',')

print('\nОбщая информаця о данных в таблице')
print(values_df.info())

print('\nВывод данных таблицы')
print(values_df)

print('\nВывод названий столбцов таблицы')
print(values_df.columns)

print('\nКоличество пустых строк')
print(values_df.isnull().sum())

print(f'\nКоличество строк-дубликатов: {values_df.duplicated().sum()}')


# Очистка данных и конвертация в числовой формат
#values_df['Height'] = values_df['Height'].str.extract(r'(\d+\.\d+)').astype(float)
#values_df['Weight'] = values_df['Weight'].str.extract(r'(\d+\.\d+)').astype(float)
#values_df['HP Base'] = values_df['HP Base'].astype(float)


# Сортировка по столбцам 'Height'
values_df.sort_values(by=['Height', 'Weight', 'HP Base'], inplace=True)

# Показать результаты
print(values_df.head())


print('Статистика по признакам')
print(values_df.describe())

print('\n1-е 10 значений таблицы')
print(values_df.head(10))


import numpy as np

values_arr_Height = np.array(values_df)[:, 3]
values_arr_Weight = np.array(values_df)[:, 4]
values_arr_HP_Base = np.array(values_df)[:, 14]

print('Вывод данных массива')
print(values_arr_Height)

import matplotlib
import matplotlib.pyplot as plt
from pylab import plot

matplotlib.style.use('ggplot')


plt.figure()

x = np.linspace(values_arr_Height[0], values_arr_Height[values_arr_Height.size - 1], values_arr_Height.size)

plt.plot(x, values_arr_Height,  'g', label='Height')
#plt.plot(x, values_arr_Weight,  'b', label='Weight')
#plt.plot(x, values_arr_HP_Base, 'r', label='HP Base')

plt.legend()
plt.xlabel('Height')
plt.ylabel('Values')
plt.title('Height and Average HP Base Comparison')

plt.show()
