import pandas as pd
import numpy as np
from sklearn.preprocessing import StandardScaler
from sklearn.cluster import KMeans
from sklearn.mixture import GaussianMixture
from sklearn.metrics import silhouette_score, davies_bouldin_score
import matplotlib.pyplot as plt
from sklearn.decomposition import PCA

# Чтение данных
train_df = pd.read_csv('../titanic/train.csv')
test_df = pd.read_csv('../titanic/test.csv')

# Убираем ненужные для кластеризации столбцы
train_data = train_df.drop(columns=['PassengerId', 'Name', 'Ticket', 'Cabin', 'Embarked', 'Survived'])
test_data = test_df.drop(columns=['PassengerId', 'Name', 'Ticket', 'Cabin', 'Embarked'])

# Обработка пропущенных данных
train_data.fillna(train_data.median(numeric_only=True), inplace=True)
test_data.fillna(test_data.median(numeric_only=True), inplace=True)

# Преобразование категориальных данных
train_data['Sex'] = train_data['Sex'].map({'male': 0, 'female': 1})
test_data['Sex'] = test_data['Sex'].map({'male': 0, 'female': 1})

# Нормализация данных
scaler = StandardScaler()
train_scaled = scaler.fit_transform(train_data)
test_scaled = scaler.transform(test_data)

# Применение KMeans
kmeans_inertia = []
silhouette_scores = []
davies_bouldin_scores = []

for k in range(2, 11):
    kmeans = KMeans(n_clusters=k, random_state=42)
    kmeans.fit(train_scaled)
    kmeans_inertia.append(kmeans.inertia_)

    labels = kmeans.labels_
    silhouette_scores.append(silhouette_score(train_scaled, labels))
    davies_bouldin_scores.append(davies_bouldin_score(train_scaled, labels))

# График метода локтя для K-means
plt.figure(figsize=(8, 6))
plt.plot(range(2, 11), kmeans_inertia, marker='o', label='Inertia')
plt.title('Метод локтя для K-means')
plt.xlabel('Количество кластеров')
plt.ylabel('Сумма квадратов расстояний (инерция)')
plt.grid(True)
plt.show()

# Применение EM (Gaussian Mixture)
gm_bic = []
silhouette_gm = []
davies_bouldin_gm = []

for k in range(2, 11):
    gm = GaussianMixture(n_components=k, random_state=42)
    gm.fit(train_scaled)
    gm_bic.append(gm.bic(train_scaled))

    labels = gm.predict(train_scaled)
    silhouette_gm.append(silhouette_score(train_scaled, labels))
    davies_bouldin_gm.append(davies_bouldin_score(train_scaled, labels))

# График BIC для Gaussian Mixture
plt.figure(figsize=(8, 6))
plt.plot(range(2, 11), gm_bic, marker='o', color='r')
plt.title('Выбор числа компонент смеси для EM (по BIC)')
plt.xlabel('Количество компонент')
plt.ylabel('BIC')
plt.grid(True)
plt.show()

# График для сравнения K-means и EM по метрикам качества кластеров
plt.figure(figsize=(10, 6))

# Силуэтный коэффициент
plt.plot(range(2, 11), silhouette_scores, marker='o', label='K-means Silhouette Score', color='blue')
plt.plot(range(2, 11), silhouette_gm, marker='o', label='EM Silhouette Score', color='green')

# Индекс Дэвиса-Боулдина
plt.plot(range(2, 11), davies_bouldin_scores, marker='o', linestyle='--', label='K-means Davies-Bouldin Index',
         color='blue')
plt.plot(range(2, 11), davies_bouldin_gm, marker='o', linestyle='--', label='EM Davies-Bouldin Index', color='green')

plt.title('Сравнение метрик качества кластеров для K-means и EM')
plt.xlabel('Количество кластеров/компонент')
plt.ylabel('Значение метрики')
plt.legend()
plt.grid(True)
plt.show()

# Определение оптимального количества кластеров
optimal_k = 3  # Пример значения, выберите оптимальный K по графику
kmeans = KMeans(n_clusters=optimal_k, random_state=42)
kmeans.fit(train_scaled)

# Применение PCA для визуализации кластеров K-means
pca = PCA(n_components=2)
train_pca = pca.fit_transform(train_scaled)

# Используем цвета для кластеров. Если какие-то переменные используются в других графиках, используем те же цвета
colors = ['blue', 'green', 'red']  # Пример: blue и green уже есть в графиках выше, red для остальных кластеров

plt.figure(figsize=(8, 6))
plt.scatter(train_pca[:, 0], train_pca[:, 1], c=kmeans.labels_, cmap=plt.cm.get_cmap('Set1', optimal_k))
plt.title('Визуализация кластеров для K-means')
plt.colorbar(label='Кластеры')
plt.show()

# Выводим центры кластеров для K-means
print("Центры кластеров для K-means:")
print(kmeans.cluster_centers_)

# Применение EM с оптимальным количеством компонент
gm = GaussianMixture(n_components=optimal_k, random_state=42)
gm.fit(train_scaled)

# Выводим центры компонент для EM
print("Центры компонент для EM:")
print(gm.means_)