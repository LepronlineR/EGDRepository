# imports
import tensorflow as tf
import numpy as np
import os
import librosa
import librosa.display
from IPython.display import Audio
import warnings
from keras.models import Sequential
from keras.layers import Dense, LSTM, Dropout, Conv1D, MaxPooling1D, Flatten
warnings.filterwarnings('ignore')

def extract_mfcc(filename, length):
  y, sr = librosa.load(filename, duration=length, offset=0.5)
  mfcc = np.mean(librosa.feature.mfcc(y=y, sr=sr, n_mfcc=50).T, axis=0)
  return mfcc

def get_model():
  model = Sequential([
      Conv1D(256, kernel_size=5, strides=1, padding='same', activation='relu', input_shape=(50, 1)),
      MaxPooling1D(pool_size=5, strides = 2, padding = 'same'),
      Conv1D(256, kernel_size=5, strides=1, padding='same', activation='relu'),
      MaxPooling1D(pool_size=5, strides = 2, padding = 'same'),
      Conv1D(128, kernel_size=5, strides=1, padding='same', activation='relu'),
      MaxPooling1D(pool_size=5, strides = 2, padding = 'same'),
      Dropout(0.2),

      Conv1D(64, kernel_size=5, strides=1, padding='same', activation='relu'),
      MaxPooling1D(pool_size=5, strides = 2, padding = 'same'),
      LSTM(128, return_sequences=False),
      Flatten(),
      Dense(32, activation='relu'),
      Dropout(0.2),
      Dense(7, activation='softmax')
  ])
  model.compile(loss='categorical_crossentropy', optimizer='adam', metrics=['accuracy'])
  return model

list_labels = ['angry', 'disgust', 'fear', 'happy', 'neutral', 'ps', 'sad']

def main():
  model = get_model()
  model.load_weights('/content/Model/cp.ckpt')
  #while(True):
    # main loop, keep predicting

  # get the file
  # get the file
  path = '/content/03-01-04-02-02-01-02.wav'
  # transform
  X = extract_mfcc(path, librosa.get_duration(filename=path))
  # predict
  pred = model.predict(tf.expand_dims(X, axis=0))
  if len(pred[0]) > 1:
    pred_class = list_labels[tf.argmax(pred[0])]
  else:
    pred_class = list_labels[int(tf.round(pred[0]))]

    print(pred_class)
  # transform
  X = extract_mfcc(path, librosa.get_duration(filename=path))
  X = np.expand_dims(X, -1)
  # predict
  result = model.predict(X)
  print(result)

if __name__ == "__main__":
  main()