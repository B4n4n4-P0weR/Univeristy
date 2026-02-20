import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from sklearn.ensemble import AdaBoostClassifier
from sklearn.tree import DecisionTreeClassifier
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder
from sklearn.metrics import accuracy_score
from sklearn.impute import SimpleImputer

# Загружаем данные
train_data = pd.read_csv('../titanic/train.csv')
test_data = pd.read_csv('../titanic/test.csv')
gender_submission_data = pd.read_csv('../titanic/gender_submission.csv')

# Определяем признаки для обучения
features = ['Pclass', 'Sex', 'Age', 'SibSp', 'Parch', 'Fare', 'Embarked']

# Предобработка данных
train_data['Age'] = train_data['Age'].fillna(train_data['Age'].median())
train_data['Fare'] = train_data['Fare'].fillna(train_data['Fare'].median())
test_data['Age'] = test_data['Age'].fillna(test_data['Age'].median())
test_data['Fare'] = test_data['Fare'].fillna(test_data['Fare'].median())

train_data['Embarked'] = train_data['Embarked'].fillna(train_data['Embarked'].mode()[0])
test_data['Embarked'] = test_data['Embarked'].fillna(test_data['Embarked'].mode()[0])

# Преобразуем категориальные признаки 'Sex' и 'Embarked' в числовые значения
le_sex = LabelEncoder()
le_embarked = LabelEncoder()

train_data['Sex'] = le_sex.fit_transform(train_data['Sex'])
train_data['Embarked'] = le_embarked.fit_transform(train_data['Embarked'])

test_data['Sex'] = le_sex.transform(test_data['Sex'])
test_data['Embarked'] = le_embarked.transform(test_data['Embarked'])

# Проверка на наличие NaN значений в тестовой выборке и их заполнение
imputer = SimpleImputer(strategy='most_frequent')
test_data[features] = imputer.fit_transform(test_data[features])

# Выбираем признаки для обучения
X_train = train_data[features]
y_train = train_data['Survived']

# Разделим тренировочные данные на обучающую и валидационную выборки
X_train_split, X_val_split, y_train_split, y_val_split = train_test_split(X_train, y_train, test_size=0.2, random_state=42)

# Модель AdaBoost с базовым классификатором в виде короткого дерева решений
ada_model = AdaBoostClassifier(DecisionTreeClassifier(max_depth=1), n_estimators=50, algorithm='SAMME', random_state=42)

# Обучаем модель
ada_model.fit(X_train_split, y_train_split)

# Оценка на валидационной выборке
y_val_pred = ada_model.predict(X_val_split)
accuracy = accuracy_score(y_val_split, y_val_pred)
print(f'Tочность на валидационной выборке: {accuracy * 100:.2f}%')

# Визуализация зависимости точности от количества итераций
n_estimators_range = np.arange(10, 200, 10)
accuracies = []

for n_estimators in n_estimators_range:
    model = AdaBoostClassifier(DecisionTreeClassifier(max_depth=1), n_estimators=n_estimators, algorithm='SAMME', random_state=42)
    model.fit(X_train_split, y_train_split)
    y_val_pred = model.predict(X_val_split)
    accuracies.append(accuracy_score(y_val_split, y_val_pred))

plt.figure(figsize=(10, 6))
plt.plot(n_estimators_range, accuracies, marker='o')
plt.title('Зависимость точности от количества итераций (n_estimators)')
plt.xlabel('Количество итераций (n_estimators)')
plt.ylabel('Точность')
plt.grid(True)
plt.show()

# Предсказание на тестовых данных
X_test = test_data[features]
test_predictions = ada_model.predict(X_test)

# Сохраняем результат
output = pd.DataFrame({'PassengerId': test_data['PassengerId'], 'Survived': test_predictions})
output.to_csv('submission.csv', index=False)

print("Файл 'submission.csv' с результатами сохранен.")

# Вывод первых 10 предсказанных значений
print("Первые 10 предсказаний и реальные значения:")
print(pd.DataFrame({'Predicted': test_predictions[:10], 'Real': gender_submission_data['Survived'][:10]}))