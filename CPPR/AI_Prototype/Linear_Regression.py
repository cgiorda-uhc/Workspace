import numpy as np
import matplotlib.pyplot as plt
from sklearn.linear_model import LinearRegression
from sklearn.model_selection import train_test_split


#DATA TO TEACH MODEL
time_studied = np.array([20,50,32,65,23,43,10,5,22,35,29,5,56]).reshape(-1,1)
scores = np.array([56,83,47,93,47,82,45,23,55,67,57,4,89]).reshape(-1,1)

#USE sklearn.model_selection import train_test_split TO SPLIT 80/20 FOR TESTING
#time_train, time_test, score_train, score_test = train_test_split(time_studied,scores, test_size=0.3 )



#CREATE MODEL USING sklearn.linear_model import LinearRegression
#model = LinearRegression()
#model.fit(time_studied, scores)
#model.fit(time_train, score_train)


#VERIFY ACCURACY
for x in range(10):
    time_train, time_test, score_train, score_test = train_test_split(time_studied,scores, test_size=0.3 )
    model = LinearRegression()
    model.fit(time_train, score_train)
    print(model.score(time_test, score_test))
    print("--------------------------------")
#print("Test2 = " + str(model.predict(np.array([56]).reshape(-1,1))))





#GRAPH IT
#plt.scatter(time_studied, scores)
plt.scatter(time_train, score_train)
plt.plot(np.linspace(0,70,100).reshape(-1,1), model.predict(np.linspace(0,70,100).reshape(-1,1)), 'r') #np.linspace(start,end,num)
#plt.ylim(0,100)
plt.show()


