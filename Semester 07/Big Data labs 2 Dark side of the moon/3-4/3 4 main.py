
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import seaborn as sns
from sklearn.model_selection import train_test_split
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.naive_bayes import MultinomialNB
from sklearn.metrics import classification_report, accuracy_score, confusion_matrix

# Загрузка данных
data = pd.read_csv('Movie Reviews.csv')

# Преобразуем метки: 'pos' = 1 (положительный отзыв), 'neg' = 0 (отрицательный отзыв)
data['label'] = data['tag'].apply(lambda x: 1 if x == 'pos' else 0)

# Разделение данных на обучающую и тестовую выборки
X_train, X_test, y_train, y_test = train_test_split(data['text'], data['label'], test_size=0.2, random_state=42)

# Преобразование текстов в векторное представление (TF-IDF)
vectorizer = TfidfVectorizer(stop_words='english', lowercase=True)
X_train_vectorized = vectorizer.fit_transform(X_train)
X_test_vectorized = vectorizer.transform(X_test)

# Функция для обучения модели и вывода метрик
def train_and_evaluate(alpha):
    model = MultinomialNB(alpha=alpha)
    model.fit(X_train_vectorized, y_train)
    y_pred = model.predict(X_test_vectorized)
    accuracy = accuracy_score(y_test, y_pred)
    print(f"Alpha: {alpha} - Точность модели: {accuracy:.4f}")
    return accuracy, y_pred

# Подбор оптимального значения alpha
alphas = np.linspace(0.1, 1.0, 10)
accuracies = []
best_alpha = None
best_accuracy = 0

for alpha in alphas:
    accuracy, y_pred = train_and_evaluate(alpha)
    accuracies.append(accuracy)
    if accuracy > best_accuracy:
        best_alpha = alpha
        best_accuracy = accuracy

# Визуализация зависимости точности от alpha
plt.figure(figsize=(8, 5))
plt.plot(alphas, accuracies, marker='o')
plt.title("Зависимость точности от параметра alpha")
plt.xlabel("alpha")
plt.ylabel("Точность")
plt.grid(True)
plt.show()

# Обучение модели с лучшим alpha и вывод отчёта классификации
print(f"\nЛучший параметр alpha: {best_alpha}, Точность: {best_accuracy:.4f}")
model = MultinomialNB(alpha=best_alpha)
model.fit(X_train_vectorized, y_train)
y_pred = model.predict(X_test_vectorized)

print("Отчёт классификации:\n", classification_report(y_test, y_pred))

# Построение и вывод матрицы ошибок (Confusion Matrix)
conf_matrix = confusion_matrix(y_test, y_pred)
plt.figure(figsize=(6, 4))
sns.heatmap(conf_matrix, annot=True, fmt="d", cmap="Blues")
plt.title("Матрица ошибок")
plt.xlabel("Предсказанные значения")
plt.ylabel("Истинные значения")
plt.show()