import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder, StandardScaler
from sklearn.neural_network import MLPClassifier
from sklearn.metrics import accuracy_score, mean_squared_error, mean_absolute_error

# Чтение файлов
train_data = pd.read_csv('../titanic/train.csv')
test_data = pd.read_csv('../titanic/test.csv')
gender_submission_data = pd.read_csv('../titanic/gender_submission.csv')

# Удалим ненужные для модели столбцы
train_data_clean = train_data.drop(columns=['PassengerId', 'Name', 'Ticket', 'Cabin'])
test_data_clean = test_data.drop(columns=['PassengerId', 'Name', 'Ticket', 'Cabin'])

# Обработка пропущенных значений
train_data_clean['Age'] = train_data_clean['Age'].fillna(train_data_clean['Age'].median())
train_data_clean['Embarked'] = train_data_clean['Embarked'].fillna(train_data_clean['Embarked'].mode()[0])
test_data_clean['Age'] = test_data_clean['Age'].fillna(test_data_clean['Age'].median())
test_data_clean['Fare'] = test_data_clean['Fare'].fillna(test_data_clean['Fare'].median())

# Объединение тренировочного и тестового наборов для кодирования категориальных переменных
combined_data = pd.concat([train_data_clean, test_data_clean], axis=0, ignore_index=True)

# Преобразование категориальных переменных в числовые
label_encoder = LabelEncoder()
combined_data['Sex'] = label_encoder.fit_transform(combined_data['Sex'])
combined_data['Embarked'] = label_encoder.fit_transform(combined_data['Embarked'])

# Разделение обратно на тренировочную и тестовую выборки
train_data_clean = combined_data[:len(train_data_clean)]
test_data_clean = combined_data[len(train_data_clean):]

# Масштабирование данных
scaler = StandardScaler()
X_train = scaler.fit_transform(train_data_clean.drop(columns=['Survived']))
y_train = train_data_clean['Survived']
X_test = scaler.transform(test_data_clean.drop(columns=['Survived']))

# Обучение нейронной сети
mlp = MLPClassifier(hidden_layer_sizes=(10,), max_iter=1000, random_state=42)
mlp.fit(X_train, y_train)

# Предсказания на тестовой выборке
y_pred = mlp.predict(X_test)

# Приведение предсказаний к типу int
y_pred = y_pred.astype(int)

# Сравнение с реальными ответами
real_y = gender_submission_data['Survived']
accuracy = accuracy_score(real_y, y_pred)

# Метрики ошибки
mse = mean_squared_error(real_y, y_pred)
mae = mean_absolute_error(real_y, y_pred)
rmse = np.sqrt(mse)

# Вывод метрик
print(f"Средняя точность модели: {accuracy * 100:.2f}%")
print(f"MSE: {mse:.4f}")
print(f"MAE: {mae:.4f}")
print(f"RMSE: {rmse:.4f}")

# Визуализация процесса обучения
plt.plot(mlp.loss_curve_)
plt.title('Кривая обучения нейронной сети')
plt.xlabel('Итерации')
plt.ylabel('Ошибка')
plt.grid(True)
plt.show()

# Запись предсказаний в файл "submission.csv"
submission = pd.DataFrame({
    'PassengerId': test_data['PassengerId'],
    'Survived': y_pred
})
submission.to_csv('submission.csv', index=False)

# Вывод первых 10 предсказаний и реальных значений
pred_vs_real = pd.DataFrame({
    'Predicted': y_pred[:10],
    'Real': real_y[:10]
})

print("Первые 10 предсказаний и реальные значения:")
print(pred_vs_real)