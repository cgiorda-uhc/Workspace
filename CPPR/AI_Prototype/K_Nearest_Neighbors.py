import numpy as np
from collections import Counter
# from sklearn.model_selection import train_test_split
# from sklearn.datasets import load_breast_cancer
# from sklearn.neighbors import KNeighborsClassifier


# data = load_breast_cancer()
# print(data)
# print(data.feature_names)
# print(data.target_names)

# x_train, x_test, y_train, y_test = train_test_split(np.array(data.data), np.array(data.target), test_size=0.2)

# clf = KNeighborsClassifier(n_neighbors=3)
# clf.fit(x_train,y_train)

# print(clf.score(x_test, y_test))

# print(clf.predict(np.array(["mean smoothness","compactness error","fractal dimension error", "worst radius"]).reshape(-1,1)))

#GLOBAL FUNCTION
def euclidean_distance(x1,x2):
    distance = np.sqrt(np.sum((x1-x2)**2)) #a ** b  =  pow(a,b)
    return distance

class KNN:
    def __init__(self, k=3):
       self.k = k
        
    def fit(self, X,y):
       self.X_train = X
       self.y_train = y
     
    def predict(self, X):
       predictions = [self._predict(x) for x in X]
       return predictions

    def _predict(self,x):
       #COMPUTE THE DISTANCE
       distances = [euclidean_distance(x,x_train) for x_train in self.X_train]
       #GET THE CLOSEST k
       k_indices = np.argsort(distances)[:self.k]
       k_nearest_labels = [self.y_train[i] for i in k_indices]
       #MAJORITY VOTE
       most_common = Counter(k_nearest_labels).most_common()
       return most_common

