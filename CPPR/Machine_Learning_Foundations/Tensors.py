#https://github.com/jonkrohn/ML-foundations/blob/master/notebooks/1-intro-to-linear-algebra.ipynb
# -
import sys
import numpy as np
import torch #The advantage of PyTorch tensors relative to NumPy arrays is that they easily be used for operations on GPU (see here for example).
import tensorflow as tf



#Vectors (Rank 1 Tensors)
# -One-dimensional array of numbers
# -Denoted in lowercase, italics, bold, e.g.: X
# -Arranged in an order, so element can be accessed by its index
#   -Elements are scalars so not bold, e.g., seconde elemnet of X is x2
# -Representing a point in space:
#   -Vector of length two represents location in 2D matrix
#   -Length three represents location in 3D cube
#   -Length of n (any > 3) represents location in n-dimensional tensor
print('----------NumPy---------------')
x = np.array([25, 2, 5]) # type argument is optional, e.g.: dtype=np.float16
print(x)
print(len(x))
print(x.shape)
print(type(x))

print(x[0])# zero-indexed
print(type(x[0]))

print('-----------------')
print('-----------------')
print('-----------------')


# Transposing a regular 1-D array has no effect!!!!!!!!!!!!!!!!!!!!!!!!!!
x_t = x.T
print('-----------------')
print(x_t)
print(x_t.shape)
# ...but it does we use nested "matrix-style" brackets:
y = np.array([[25, 2, 5]]) #MULTIDIMENSIONAL ARRAY
print('-----------------')
print(y)
print(y.shape)
# ...but can transpose a matrix with a dimension of length 1, which is mathematically equivalent:
y_t = y.T
print('-----------------')
print(y_t)
print(y_t.shape)
# Column vector can be transposed back to original row vector:
print('-----------------')
print(y_t.T)
print(y_t.T.shape)


print('-----------------')
print('-----------------')
print('-----------------')


#Zero Vectors
z = np.zeros(3)
print(z)

print('-----------------')
print('-----------------')
print('-----------------')


#Vectors in PyTorch and TensorFlow
x_pt = torch.tensor([25, 2, 5])
print(x_pt)

x_tf = tf.Variable([25, 2, 5])
print(x_tf)

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


