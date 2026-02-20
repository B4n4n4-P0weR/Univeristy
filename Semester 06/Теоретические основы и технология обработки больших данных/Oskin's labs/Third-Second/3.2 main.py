import pandas as pd
import numpy as np
from sklearn.preprocessing import StandardScaler
from sklearn.model_selection import train_test_split
from sklearn.linear_model import LinearRegression
from sklearn.metrics import r2_score, root_mean_squared_error

# Загрузка данных
file_path = "/Users/vlad/Developer/Oskin's labs/Third-Second/AAPL.csv"
data = pd.read_csv(file_path)

# Преобразование даты в формат datetime и установка её в качестве индекса
data['Date'] = pd.to_datetime(data['Date'])
data.set_index('Date', inplace=True)

# Анализ и удаление выбросов с использованием межквартильного размаха (IQR)
Q1 = data.quantile(0.25)
Q3 = data.quantile(0.75)
IQR = Q3 - Q1
outliers = ((data < (Q1 - 1.5 * IQR)) | (data > (Q3 + 1.5 * IQR))).any(axis=1)
cleaned_data = data[~outliers]

# Стандартизация данных
scaler = StandardScaler()
scaled_data = scaler.fit_transform(cleaned_data)
scaled_data_df = pd.DataFrame(scaled_data, index=cleaned_data.index, columns=cleaned_data.columns)

# Добавление логарифмической доходности и скользящего среднего за 7 дней
positive_shift = scaled_data_df['Close'] + 1 - scaled_data_df['Close'].min()  # сдвиг всех значений в положительную область
returns = np.log(positive_shift / positive_shift.shift(1))
scaled_data_df['Log_Returns'] = returns.replace([np.inf, -np.inf], np.nan).fillna(returns.mean())
scaled_data_df['SMA_7'] = scaled_data_df['Close'].rolling(window=7).mean()

# Удаление строк с NaN значениями
final_data = scaled_data_df.dropna()

# Разделение данных на обучающую и тестовую выборки
X = final_data.drop('Close', axis=1)
y = final_data['Close']
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# Построение и оценка модели линейной регрессии
model = LinearRegression()
model.fit(X_train, y_train)
y_pred = model.predict(X_test)
r2 = r2_score(y_test, y_pred)
rmse = root_mean_squared_error(y_test, y_pred)  # Использование root_mean_squared_error для RMSE

# Вывод результатов
print(f"Коэффициент детерминации (R²): {r2}")
print(f"Среднеквадратичная ошибка (RMSE): {rmse}")
print(f"Коэффициенты модели: {model.coef_}")


import matplotlib.pyplot as plt

# Построение графика цены закрытия
plt.figure(figsize=(14, 7))
plt.plot(data.index, data['Close'], label='Цена закрытия (Close)', color='green')

plt.title('Динамика цены закрытия акций Apple')
plt.xlabel('Дата')
plt.ylabel('Цена закрытия')
plt.legend()
plt.grid(True)
plt.show()
