import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import seaborn as sns
from sklearn.preprocessing import StandardScaler, LabelEncoder
from sklearn.svm import SVC
from sklearn.model_selection import train_test_split, cross_val_score
from sklearn.metrics import accuracy_score, classification_report, confusion_matrix

# 1. Загрузка данных
train_data = pd.read_csv('../titanic/train.csv')
test_data = pd.read_csv('../titanic/test.csv')

# 2. Предобработка данных
# Заполнение пропущенных значений
train_data['Age'] = train_data['Age'].fillna(train_data['Age'].median())
train_data['Embarked'] = train_data['Embarked'].fillna(train_data['Embarked'].mode()[0])
train_data = train_data.drop(columns=['Cabin'])

test_data['Age'] = test_data['Age'].fillna(test_data['Age'].median())
test_data['Fare'] = test_data['Fare'].fillna(test_data['Fare'].median())
test_data = test_data.drop(columns=['Cabin'])

# Преобразование категориальных признаков в числовые
label_encoder = LabelEncoder()
train_data['Sex'] = label_encoder.fit_transform(train_data['Sex'])
train_data['Embarked'] = label_encoder.fit_transform(train_data['Embarked'])

test_data['Sex'] = label_encoder.fit_transform(test_data['Sex'])
test_data['Embarked'] = label_encoder.fit_transform(test_data['Embarked'])

# 3. Подготовка признаков и целевой переменной
features = ['Pclass', 'Sex', 'Age', 'SibSp', 'Parch', 'Fare', 'Embarked']
X = train_data[features]  # Признаки тренировочной выборки
y = train_data['Survived']  # Целевая переменная

X_test = test_data[features]  # Признаки тестовой выборки

# 4. Разделение на тренировочную и валидационную выборки
X_train, X_val, y_train, y_val = train_test_split(X, y, test_size=0.2, random_state=42)

# 5. Масштабирование признаков
scaler = StandardScaler()
X_train_scaled = scaler.fit_transform(X_train)
X_val_scaled = scaler.transform(X_val)
X_test_scaled = scaler.transform(X_test)

# 6. Построение и обучение модели SVM
svm_model = SVC(kernel='linear', C=1.0, random_state=42)
svm_model.fit(X_train_scaled, y_train)

# 7. Оценка модели на валидационной выборке
y_val_pred = svm_model.predict(X_val_scaled)
accuracy = accuracy_score(y_val, y_val_pred)
print(f"Точность модели на валидационной выборке: {accuracy:.2f}")
print("Отчет по классификации:\n", classification_report(y_val, y_val_pred))

# 8. Матрица ошибок
plt.figure(figsize=(6, 4))
conf_matrix = confusion_matrix(y_val, y_val_pred)
sns.heatmap(conf_matrix, annot=True, fmt="d", cmap="Blues")
plt.title("Матрица ошибок")
plt.xlabel("Предсказанные значения")
plt.ylabel("Истинные значения")
plt.show()

# 9. Визуализация зависимости точности от параметра C
C_values = [0.1, 1.0, 10.0, 100.0]
accuracies = []

for C in C_values:
    svm_model = SVC(kernel='linear', C=C, random_state=42)
    svm_model.fit(X_train_scaled, y_train)
    y_val_pred = svm_model.predict(X_val_scaled)
    accuracies.append(accuracy_score(y_val, y_val_pred))

plt.figure(figsize=(8, 5))
plt.plot(C_values, accuracies, marker='o')
plt.title("Зависимость точности от параметра C")
plt.xlabel("Значение C")
plt.ylabel("Точность")
plt.xscale('log')  # Логарифмическая шкала для оси C
plt.grid(True)
plt.show()

# 10. Предсказания на тестовой выборке
y_pred = svm_model.predict(X_test_scaled)

# 11. Сохранение предсказаний в файл submission.csv
submission_data = pd.read_csv("../titanic/gender_submission.csv")[['PassengerId']]
submission_data['Survived'] = y_pred
submission_data.to_csv('submission.csv', index=False)

print("Предсказания сохранены в файл submission.csv")

real_y = submission_data['Survived']
pred_vs_real = pd.DataFrame({
    'Predicted': y_pred[:10],
    'Real': real_y[:10]
})
print("Первые 10 предсказаний и реальные значения:")
print(pred_vs_real)