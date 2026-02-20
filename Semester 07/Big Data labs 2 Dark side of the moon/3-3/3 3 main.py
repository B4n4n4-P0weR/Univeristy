import pandas as pd
from sklearn.preprocessing import StandardScaler
from sklearn.decomposition import PCA
import matplotlib.pyplot as plt
import numpy as np

# Загрузка данных
data = pd.read_csv('Spotify Most Streamed Songs.csv', delimiter=',')

# Выбор числовых признаков для анализа (пропускаем категориальные)
numerical_features = ['released_year', 'released_month', 'released_day', 'in_spotify_playlists',
                      'in_spotify_charts', 'streams', 'in_apple_playlists', 'in_apple_charts',
                      'bpm', 'danceability_%', 'valence_%', 'energy_%', 'acousticness_%',
                      'instrumentalness_%', 'liveness_%', 'speechiness_%']

# Стандартизация данных
scaler = StandardScaler()
scaled_data = scaler.fit_transform(data[numerical_features])

# Применение PCA
pca = PCA(n_components=2)  # Сокращаем до 2 компонент для визуализации
pca_result = pca.fit_transform(scaled_data)

# Визуализация первых двух главных компонент
plt.figure(figsize=(8, 6))
plt.scatter(pca_result[:, 0], pca_result[:, 1], c='blue', label='Data Points')
plt.title('PCA of Music Dataset')

# Изменяем оси, чтобы указать главные компоненты с максимальным вкладом
component_names = np.array(numerical_features)[np.argmax(abs(pca.components_), axis=1)]

plt.xlabel(f'Principal Component 1 ({component_names[0]})')
plt.ylabel(f'Principal Component 2 ({component_names[1]})')

plt.legend()
plt.show()

# Вывод объяснённой дисперсии
print("Explained variance ratio: ", pca.explained_variance_ratio_)

# Вывод главных компонент
print("Principal Components:\n", pca.components_)

# Соответствие компонент исходным признакам
print("Corresponding features:\n", numerical_features)
