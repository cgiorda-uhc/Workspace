#https://github.com/jonkrohn/ML-foundations/blob/master/notebooks/1-intro-to-linear-algebra.ipynb
# -
from asyncio.windows_events import NULL
from pyexpat import XMLParserType
import sys
import numpy as np
import torch #The advantage of PyTorch tensors relative to NumPy arrays is that they easily be used for operations on GPU (see here for example).
#import tensorflow as tf



#Segment 2: Common Tensor Operations
#Segment 2: Common Tensor Operations
#Segment 2: Common Tensor Operations
#Segment 2: Common Tensor Operations
#Segment 2: Common Tensor Operations
#Segment 2: Common Tensor Operations


#Tensor Transposition
#Transpose of scalar is itself, e.g.: = xT= x
#Transpose of vector, converts column to row (and vice versa)
#Scalar and vector tranposition as special cases matrix transposition:
#   Flip of axes over main diagnol such that row i column j:
#   (XT)i,j = Xj,i
X = np.array([[25, 2], [5, 26], [3, 7]])
X_pt = torch.tensor([[25, 2], [5, 26], [3, 7]])
#X_tf = tf.Variable([[25, 2], [5, 26], [3, 7]])
# print("X = " + str(X.T)) #NumPy
# print("X_pt = " + str(X_pt.T)) #PyTorch
# print("tf.transpose(X_tf) = " + str(tf.transpose(X_tf))) #TensorfFlow

#Scalar Operations 
#Basic Arithmetical Properties
#Adding or multiplying with scalar applies operation to all elements and tensor shape is retained:
# print("X * 2 = " + str(X * 2))
# print("X + 2 = " + str(X + 2))
# print("X*2+2 = " + str(X*2+2))
# print("X_pt*2+2 = " + str(X_pt*2+2))# Python operators are overloaded; could alternatively use torch.mul() or torch.add()
# print("torch.add(torch.mul(X_pt, 2), 2) = " + str(torch.add(torch.mul(X_pt, 2), 2)))
# print("X_tf*2+2 = " + str(X_tf*2+2))# Operators likewise overloaded; could equally use tf.multiply() tf.add()
# print("tf.add(tf.multiply(X_tf, 2), 2) = " + str(tf.add(tf.multiply(X_tf, 2), 2)))



# Hadamard product Operations
# If two tensors have the same size, operations are often by default applied element-wise. 
# This is not matrix multiplication, which we'll cover later, but is rather called the Hadamard product or simply the element-wise product.
A = X+2
# print("A = " + str(A))
# print("A + X = " + str(A + X))
# print("A * X = " + str(A * X))
A_pt = X_pt + 2
# print("X * 2 = " + str(X * 2))
# print("A_pt * X_pt = " + str(A_pt * X_pt))
# A_tf = X_tf + 2
# print("A_tf + X_tf = " + str(A_tf + X_tf))
# print("A_tf * X_tf = " + str(A_tf * X_tf))


# Reduction
# Calculating the sum across all elements of a tensor is a common operation. For example:
# For vector x of length n, we calculate 
# For matrix X with m by n dimensions, we calculate 
# print("X.sum() = " + str(X.sum()))
# print("torch.sum(X_pt) = " + str(torch.sum(X_pt)))
# print("tf.reduce_sum(X_tf) = " + str(tf.reduce_sum(X_tf)))
# Can also be done along one specific axis alone, e.g.:
# print("X.sum(axis=0) = " + str(X.sum(axis=0))) # summing over all rows
# print("X.sum(axis=1) = " + str(X.sum(axis=1)))# summing over all columns
# print("torch.sum(X_pt, 0) = " + str(torch.sum(X_pt, 0)))
# print("tf.reduce_sum(X_tf, 1) = " + str(tf.reduce_sum(X_tf, 1)))
# Many other operations can be applied with reduction along all or a selection of axes, e.g.:
# maximum
# minimum
# mean
# product
# They're fairly straightforward and used less often than summation, so you're welcome to look them up in library docs if you ever need them.


# The Dot Product
# If we have two vectors (say, x and y) with the same length n, we can calculate the dot product between them. This is annotated several different ways, including the following:
# x . y
# xTy
# (x,y)
# Regardless which notation you use (I prefer the first), the calculation is the same; we calculate products in an element-wise fashion and then sum reductively across the products to a scalar value. That is, 
# The dot product is ubiquitous in deep learning: It is performed at every artificial neuron in a deep neural network, which may be made up of millions (or orders of magnitude more) of these neurons.
# x = np.array([25, 2, 5])
# y = np.array([0, 1, 2])
# x_pt = torch.tensor([25, 2, 5])
# print("x = " + str(x))
# print("y = " + str(y))
# print("25*0 + 2*1 + 5*2 = " + str(25*0 + 2*1 + 5*2))
# print("np.dot(x, y) = " + str(np.dot(x, y)))
# print("x_pt = " + str(x_pt))
# y_pt = torch.tensor([0, 1, 2])
# print("y_pt = " + str(y_pt))
# print("np.dot(x_pt, y_pt) = " + str(np.dot(x_pt, y_pt))) # NumPy can process PyTorch Tensors
# print("torch.dot(x_pt, y_pt) = " + str(torch.dot(x_pt, y_pt))) # !! PyTorch Requires Float in at least on element
# print("torch.dot(torch.tensor([25, 2, 5.]), torch.tensor([0, 1, 2.])) = " + str(torch.dot(torch.tensor([25, 2, 5.]), torch.tensor([0, 1, 2.])))) # !! PyTorch Requires Float in at least on element
# x_tf = tf.Variable([25, 2, 5])
# print("x_tf = " + str(x_tf))
# y_tf = tf.Variable([0, 1, 2])
# print("y_tf = " + str(y_tf))
# print("tf.reduce_sum(tf.multiply(x_tf, y_tf)) = " + str(tf.reduce_sum(tf.multiply(x_tf, y_tf))))  # 2 steps for TensorFlow 1: Multiply all elements 2: Sum final Scalar value of DOT


#Exercise
Y = np.array([[42,4,7,99], [-99, -3, 17,22]])
print("Y.T = " + str(Y.T))

X_pt = np.array([[25, 10], [-2, 1]])
Y_pt = np.array([[-1,7], [10, 8]]) 
print("X_pt * Y_pt = " + str(X_pt * Y_pt))
# print("X_pt + Y_pt = " + str(X_pt + Y_pt))


w = np.array([-1,2,-2])
x = np.array([5,10,0])
print("np.dot(w, x) = " + str(np.dot(w, x)))



# Method 1: Substitution
# - Solving for unknowns in linear equations
# - Use whenever there's a variable in system with coefficient of 1 (y*1 --- (1) is assumed) 
# - For example, when solving for x and y in the following systems (2):
# y = 3x
# -5x + 2y = 2

# Solution x:
# -5x + 2(3x) = 2
# -5x + 6x = 2
# 1x aka x = 2

# Solution y:
# y = 3x
# y = 3(2)
# y = 6

# Final Solve:
# (x,y) = (2,6)


# Solve for the unknowns in the following systems of equations:

# 1. x + y = 6 and 2x + 3y = 16    answer:  (x,y) =  (2,4)


# 2. -x + 4y = 0 and 2x - 5y = -6   answer:  (x,y) =  (-8,-2)


# 3. y = 4x + 1 and -4x + y = 2     answer:  (x,y) = unknown 



sys.exit()


row_1 = [1,2,3,4,5]
row_2 = [6,7,8,9,10]
row_3 = [11,12,13,14,15]
row_4 = [16,17,18,19,20]
row_5 = [21,22,23,24,25]
test_data = np.array([row_1,row_2,row_3,row_4,row_5])
print(test_data)
print(test_data[:,2:5:1]) #Slice
print(test_data[:,-2:-4:-1]) #Negative index
greater_than_five = test_data > 5 #Boolean
print(greater_than_five)
print(test_data[greater_than_five])
drop_under_5_array = np.where(test_data > 5, test_data,NULL) # Where
print(drop_under_5_array)


sys.exit()




X = np.array([[25, 2], [5, 26], [3, 7]])
print("-----------------------------------------")
print("-----------------------------------------")
print("X = " + str(X))
print("-----------------------------------------")
print("-----------------------------------------")
print("X[:,0] = " + str(X[:,0])) # Select left column of matrix X (zero-indexed)
print("-----------------------------------------")
print("-----------------------------------------")
print("X[1,:] = " + str(X[1,:])) # Select middle row of matrix X:
print("-----------------------------------------")
print("-----------------------------------------")
print("X[0:2, 0:2] = " + str(X[0:3, 0:2])) # Another slicing-by-index example:0:2 = fistrow:secondrow 0:2  = fistcol:secondcol ]
print("-----------------------------------------")
print("-----------------------------------------")


sys.exit()


#What is the transpose of this vector
# [25
#  2
#  -3
#  -23]
X = np.array([[25],[2],[-3],[-23]])
#X = np.array([[25,2,-3,-23]])  # = [[ 25   2  -3 -23]]
print(X)
print(X.T) # = [[ 25   2  -3 -23]]

#Using algebraic notation, what are the dimensions of this matrix Y?
#Y = [42 4 7 99]
#    [-99 -3 17 22]   
Y = np.array([[42, 4, 7, 99],[-99, -3, 17, 22] ])
print(Y)
print(Y.shape)


#Using algebraic notation, what is the position of the element in this martrix Y with the value of 17?
print(Y[1,2])

sys.exit()

# Higher-Rank Tensors
# As an example, (Rank 4 tensors) are common for images, where each dimension corresponds to:
# Number of images in training batch, e.g., 32
# Image height in pixels, e.g., 28 for MNIST digits
# Image width in pixels, e.g., 28
# Number of color channels, e.g., 3 for full-color images (RGB)
images_pt = torch.zeros([32, 28, 28, 3])
print(images_pt)
images_tf = tf.zeros([32, 28, 28, 3])
print(images_tf)

sys.exit()



#Matrices (Rank 1 Tensors)
# Two-dimensional array of numbers
# Denoted in uppercase, italics, bold, e.g.: X
# Height given priority ahead of width in notation, i.e: (n row, n col)
#   -If X has three rows and two columns its shape is (3,2)
# Individual scalar elements denoted in uppercase, italics only
#   -Element in top right corner of matrix X(bold, italic) above would be X(not bold) X 1,2(row,col)
# Colon represents an entire row or column
#   -Left column of matrix X is X:,1
#   -Middle row of matrix X is X2,:
print('----------Matrices ----------')
# Use array() with nested brackets:
X = np.array([[25, 2], [5, 26], [3, 7]])
print(X)
print(X.shape)
print(X.size)
print(X[:,0])# Select left column of matrix X (zero-indexed)
print(X[1,:])# Select middle row of matrix X:
print(X[0:2, 0:2])# Another slicing-by-index example:0:2 = fistrow:secondrow 0:2  = fistcol:secondcol ]


print('----------#Matrices in PyTorch ----------')
#Matrices in PyTorch and TensorFlow
X_pt = torch.tensor([[25, 2], [5, 26], [3, 7]])
print(X_pt)
print(X_pt.shape)# pythonic relative to TensorFlow
print(X_pt[1,:] )# N.B.: Python is zero-indexed; written algebra is one-indexed

# print('----------#Matrices in TensorFlow ----------')
X_tf = tf.Variable([[25, 2], [5, 26], [3, 7]])#TENSOR FLOW IS SLOW AND CLUNKY SO COMMENT
print(X_tf)
print(tf.rank(X_tf))
print(tf.shape(X_tf))
print(X_tf[1,:])



sys.exit()



#Vectors (Rank 1 Tensors)
# -One-dimensional array of numbers
# -Denoted in lowercase, italics, bold, e.g.: X
# -Arranged in an order, so element can be accessed by its index
#   -Elements are scalars so not bold, e.g., seconde elemnet of X is x2
# -Representing a point in space:
#   -Vector of length two represents location in 2D matrix
#   -Length three represents location in 3D cube
#   -Length of n (any > 3) represents location in n-dimensional tensor
print('----------NumPy---1D----------')
x = np.array([25, 2, 5]) # type argument is optional, e.g.: dtype=np.float16
print(x)
print(len(x))
print(x.shape)
print(type(x))

print(x[0])# zero-indexed
print(type(x[0]))

print('--------1D---------')
print('--------1D---------')
print('--------1D---------')


# Transposing a regular 1-D array has no effect!!!!!!!!!!!!!!!!!!!!!!!!!!
x_t = x.T
print('---------1D--------')
print(x_t)
print(x_t.shape)
# ...but it does we use nested "matrix-style" brackets:
y = np.array([[25, 2, 5]]) #MULTIDIMENSIONAL ARRAY
print('--------2D---------')
print(y)
print(y.shape)
# ...but can transpose a matrix with a dimension of length 1, which is mathematically equivalent:
y_t = y.T
print('---------2D--------')
print(y_t)
print(y_t.shape)
# Column vector can be transposed back to original row vector:
print('---------2D-------')
print(y_t.T)
print(y_t.T.shape)


print('-----------------')
print('-----------------')
print('-----------------')

print('-------Zero Vectors-------')
#Zero Vectors
z = np.zeros(3)
print(z)

print('-----------------')
print('-----------------')
print('-------Vectors in PyTorch and TensorFlow------')


#Vectors in PyTorch and TensorFlow
x_pt = torch.tensor([25, 2, 5])
print(x_pt)

x_tf = tf.Variable([25, 2, 5]) #TENSOR FLOW IS SLOW AND CLUNKY SO COMMENT
print(x_tf)


#Vectors represent a point in space
#Vectors can also represent a magnitude and direction from origin
#Norms are functions that quantify vector magnitude
#L2 Norm measures simple (Euclidean) distance from origin
#L2 is most common norm in ML - instead of ||X||2  ||X|| (2 is assumed default)
print('----------L2 Norm----------')
x = np.array([25, 2, 5]) # type argument is optional, e.g.: dtype=np.float16
print(x)
print((25**2 + 2**2 + 5**2)**(1/2))# = np.linalg.norm(x) **2 = squared **(1/2) = sqrt
print(np.sqrt((np.square(25) + np.square(2) + np.square(5))))
print(np.linalg.norm(x))

print('----------L1 Norm----------')
#L1 is another common norm for ML
#L1 varies linearly at all locations whether near or far from origin
#L1 is used whenever difference between zero and non-zero is key
print(np.abs(25) + np.abs(2) + np.abs(5)) #abs turns all negative to postive
print(np.linalg.norm(x, 1))

print('----------L2 Squared Norm----------')
#Squared L2 Remove final sqrt
#Squared L2 Computationally cheaper to use than L2 norm
#Squared L2 norm equals simply X tranpose of time X
#Derivative of element x requires that element alone, whereas L2 norm requires X vector
#Downside is it grows slowly near origin so cant be used if distinguishing between zero and near zero is important
print((25**2 + 2**2 + 5**2))
# we'll cover tensor multiplication more soon but to prove point quickly:
print(np.dot(x, x))

#Max Norm returns the absolute value of the largest magnitude element
print(np.max(np.abs(25) + np.abs(2) + np.abs(5)))


print('----------Orthogonal Vectors----------')
#Basis Vectors can be scaled to represent any vector in a given vector space
#Basis Vectors typically use unit vectors along axes of vector space
#Orthogonal Vectors X and Y are orthogonal vectors if X tranpose of time Y = 0
#Orthogonal Vectors are at 90 degree angle to each other (assuming non-zero norms)
#Orthogonal Vectors n-dimensional space has max n mutually orthogonal vectors (assuming non-zero norms)
#Orthonormal vectors are orthogonal and have unit norm
#   -Basis vectors are an example
i = np.array([1, 0])
print(i)
j = np.array([0, 1])
print(j)
print(np.dot(i, j)) # detail on the dot operation coming up...



sys.exit()

#Scalars (Rank 0 Tensors)
# -No dimensions
# -Single Number 
# -Denoted in lowercase, italics, e.g.:x
# -Should be typed, like all other tensors: e.g.:int, float
print('----------NumPy---------------')
x =  25
x =  np.uint8(25) # USE NUMPY
print(x)
t= type(x) # if we'd like more specificity (e.g., int16, uint8), we need NumPy or another numeric library
print(t)

y =  3
py_sum = np.uint8(x + y)
print(type(py_sum))
print(py_sum)

x_float = 25.0
float_sum = x_float + y
print(type(float_sum))
print(float_sum)

print('----------PyTorch---------------') #MORE INTUITIVE
print('torch.tensor(25): ')
x_pt = torch.tensor(25)
print(x_pt)
print(x_pt.shape)

print('----------TensorFlow---------------') #MORE SUPPORT AND ROBUST
print('tf.Variable(25, dtype=tf.int16): ')
x_tf = tf.Variable(25, dtype=tf.int16) # dtype is optional
print(x_tf)
print(x_tf.shape)


print('tf.Variable(3, dtype=tf.int16): ')
y_tf = tf.Variable(3, dtype=tf.int16)
print(x_tf + y_tf)

print('tf.add(x_tf, y_tf): ')
tf_sum = tf.add(x_tf, y_tf)

print(tf_sum)
print(tf_sum.numpy()) # note that NumPy operations automatically convert tensors to NumPy arrays, and vice versa

print('tf.Variable(25., dtype=tf.float16): ')
tf_float = tf.Variable(25., dtype=tf.float16)
print(tf_float)


