# imports
import tensorflow as tf
import numpy as np
import os
import librosa
import librosa.display
import pyaudio
import wave
from IPython.display import Audio
import warnings
from keras.models import Sequential
from keras.layers import Dense, LSTM, Dropout, Conv1D, MaxPooling1D, Flatten

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

def predict(model):
  chunk = 1024  # Record in chunks of 1024 samples
  sample_format = pyaudio.paInt16  # 16 bits per sample
  channels = 2
  fs = 44100  # Record at 44100 samples per second
  seconds = 3
  filename = "output.wav"

  p = pyaudio.PyAudio()  # Create an interface to PortAudio

  print('Recording')

  stream = p.open(format=sample_format,
                  channels=channels,
                  rate=fs,
                  frames_per_buffer=chunk,
                  input=True)

  frames = []  # Initialize array to store frames

  # Store data in chunks for 3 seconds
  for i in range(0, int(fs / chunk * seconds)):
      data = stream.read(chunk)
      frames.append(data)

  # Stop and close the stream 
  stream.stop_stream()
  stream.close()
  # Terminate the PortAudio interface
  p.terminate()

  # Save the recorded data as a WAV file
  wf = wave.open(filename, 'wb')
  wf.setnchannels(channels)
  wf.setsampwidth(p.get_sample_size(sample_format))
  wf.setframerate(fs)
  wf.writeframes(b''.join(frames))
  wf.close()

  # transform
  X = extract_mfcc(filename, librosa.get_duration(filename=filename))
  # predict
  pred = model.predict(tf.expand_dims(X, axis=0))
  if len(pred[0]) > 1:
    pred_class = list_labels[tf.argmax(pred[0])]
  else:
    pred_class = list_labels[int(tf.round(pred[0]))]
  
  print(pred_class)

def main():
  model = get_model()
  model.load_weights('./cp.ckpt')
  while(True):
    input("Press enter to start recording: ")
    predict(model)
  

if __name__ == "__main__":
  main()