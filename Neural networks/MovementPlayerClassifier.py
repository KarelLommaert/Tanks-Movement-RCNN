#!/usr/bin/env python
# coding: utf-8

# In[1]:


import tensorflow as tf
from keras.models import Sequential
from keras import optimizers
from keras.layers.convolutional import Conv3D
from keras.layers.convolutional_recurrent import ConvLSTM2D
from keras.layers.normalization import BatchNormalization
from keras.layers import Dense
from keras.layers import Flatten
import numpy as np
import matplotlib.pyplot as plt
from keras.preprocessing.image import ImageDataGenerator
from keras.preprocessing import image
import os
from sklearn.preprocessing import LabelEncoder
from sklearn.preprocessing import OneHotEncoder
from keras.utils import to_categorical
from random import randint
from keras.layers import GaussianNoise
import gc

# Classes to evaluate
path = r"D:\Work\Data\SortedData\Player"
classDirs = os.listdir(path)
nClasses = len(classDirs)

# Create neural network
seq = Sequential()
#seq.add(GaussianNoise(0.1, input_shape=(None, 100, 100, 3)))
seq.add(ConvLSTM2D(filters=50, kernel_size=(3, 3),
                   input_shape=(None, 100, 100, 3),
                   padding='same', return_sequences=True))
seq.add(BatchNormalization())

seq.add(ConvLSTM2D(filters=50, kernel_size=(3, 3),
                   padding='same', return_sequences=True))
seq.add(BatchNormalization())

#seq.add(ConvLSTM2D(filters=100, kernel_size=(3, 3),
#                   padding='same', return_sequences=True))
#seq.add(BatchNormalization())

seq.add(ConvLSTM2D(filters=50, kernel_size=(3, 3),
                   padding='same', return_sequences=False))
seq.add(BatchNormalization())

seq.add(Flatten())

seq.add(Dense(nClasses, activation='softmax'))

adadelta = optimizers.adadelta(lr=0.0001)
seq.compile(loss='categorical_crossentropy', optimizer=adadelta, metrics=['accuracy'])


# In[2]:


def sequence_generator(bs = 4, nFrames = 15, nSeqsPerPlayer = 30, shouldAugment = True, printLabel = False):
    width = 100
    height = 100
    
    label_encoder = LabelEncoder()
    #image_gen = ImageDataGenerator()
    while (True):
        gc.collect()
        sequences = np.zeros((bs, nFrames, height, width, 3), dtype = 'int')
        #labels = []
        names = np.empty(bs, dtype = 'str')
        currentSeqID = 0
        
        while (currentSeqID < bs):
            # Read sequence
            randPlayerID = randint(0, nClasses - 1)
            if (printLabel):
                print(randPlayerID)
                
            randSeqID = randint(0, nSeqsPerPlayer - 1)
            
            playerDirPath = path + "\\" + classDirs[randPlayerID]
            seqDirs = os.listdir(playerDirPath)
            
            seqDirPath = playerDirPath + "\\" + seqDirs[randSeqID]
            imgs = os.listdir(seqDirPath)
            
            randStartID = randint(0, len(imgs) - nFrames - 1)

            randShiftRange = randint(-0, 0)
            randRotAmount = randint(-180, 180)

            for imgID in range(nFrames):
                imgName = imgs[randStartID + imgID]
                img = image.load_img(seqDirPath + "\\" + imgName)
                if (shouldAugment):
                    # Rotate randomly
                    img.rotate(randRotAmount)

                    # Convert to array
                    imgArray = image.img_to_array(img, dtype = 'int')

                    # Recolor enemies and bullets to while
                    for x in range(len(imgArray)):
                        for y in range(len(imgArray[x])):
                            if (int(imgArray[x, y, 0]) == 255 and int(imgArray[x, y, 1])== 0 and int(imgArray[x, y, 2]) == 0):
                                imgArray[x, y] = [255, 255, 255]
                            elif (int(imgArray[x, y, 0]) == 214 and int(imgArray[x, y, 1])== 134 and int(imgArray[x, y, 2]) == 0):
                                imgArray[x, y] = [255, 255, 255]
                    
                    # Shift randomly
                    shiftedImgArray = imgArray.copy()
                    #for imgH in range(len(imgArray)):
                    #    for imgW in range(len(imgArray[imgH])):
                    #        shiftPos = (imgW + randShiftRange) % len(imgArray[imgH])
                    #        shiftedImgArray[imgH, imgW] = imgArray[imgH, shiftPos]
                    sequences[currentSeqID, imgID] = shiftedImgArray
                else:
                    imgArray = image.img_to_array(img, dtype = 'int')
                    # Recolor enemies and bullets to while
                    for x in range(len(imgArray)):
                        for y in range(len(imgArray[x])):
                            if (int(imgArray[x, y, 0]) == 255 and int(imgArray[x, y, 1])== 0 and int(imgArray[x, y, 2]) == 0):
                                imgArray[x, y] = [255, 255, 255]
                            elif (int(imgArray[x, y, 0]) == 214 and int(imgArray[x, y, 1])== 134 and int(imgArray[x, y, 2]) == 0):
                                imgArray[x, y] = [255, 255, 255]
                    sequences[currentSeqID, imgID] = imgArray    
        
            # Add label
            names[currentSeqID] = classDirs[randPlayerID]
            
            # Increment seqID
            currentSeqID += 1
        
        # one-hot encode the labels
        integer_encoded = label_encoder.fit_transform(names)
        labels = to_categorical(integer_encoded, num_classes=nClasses)
 

 
        # yield the batch to the calling function
        yield (sequences, labels)


# In[3]:


seq.fit_generator(sequence_generator(), steps_per_epoch = 200, epochs = 80, validation_data = sequence_generator(shouldAugment = False), validation_steps = 20)#, validation_data = (x_val, y_val))


# In[ ]:





# In[ ]:




