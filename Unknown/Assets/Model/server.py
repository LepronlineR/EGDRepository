#
#   Hello World server in Python
#   Binds REP socket to tcp://*:5555
#   Expects b"Hello" from client, replies with b"World"
#

import tensorflow as tf
import numpy as np
import scipy
import os
import librosa
import librosa.display
import pyaudio
import wave
from IPython.display import Audio
import warnings
from keras.models import Sequential
from keras.layers import Dense, LSTM, Dropout, Conv1D, MaxPooling1D, Flatten
import time
import zmq
import io
import wave
import scipy.io.wavfile
import soundfile as sf
from scipy.io.wavfile import write

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

def convert_bytearray_to_wav_ndarray(input_bytearr: bytes, sampling_rate=44100):
    bytes_wav = bytes()
    byte_io = io.BytesIO(bytes_wav)
    write(byte_io, sampling_rate, np.frombuffer(input_bytearr, dtype=np.int32))
    output_wav = byte_io.read()
    output, sample_r = sf.read(io.BytesIO(output_wav))
    return output

def predict(model, bytes_wav, labels, sampling_rate = 44100, filename = "output.wav"):

    output = convert_bytearray_to_wav_ndarray(bytes_wav, sampling_rate)
    scipy.io.wavfile.write(filename, sampling_rate, output)

    # transform
    X = extract_mfcc(filename, librosa.get_duration(filename=filename))
    # predict
    pred = model.predict(tf.expand_dims(X, axis=0))
    if len(pred[0]) > 1:
        pred_class = labels[tf.argmax(pred[0])]
    else:
        pred_class = labels[int(tf.round(pred[0]))]
    
    return pred_class

def predict_t(model, labels, filename = "output.wav"):

    # transform
    X = extract_mfcc(filename, librosa.get_duration(filename=filename))
    # predict
    pred = model.predict(tf.expand_dims(X, axis=0))
    if len(pred[0]) > 1:
        pred_class = labels[tf.argmax(pred[0])]
    else:
        pred_class = labels[int(tf.round(pred[0]))]
    
    return pred_class

list_labels = ['angry', 'disgust', 'fear', 'happy', 'neutral', 'ps', 'sad']

model = get_model()
model.load_weights('./cp.ckpt')

context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5555")
    
while True:
    #  Wait for next request from client
    message = socket.recv()
    # print("Received request: %s" % message)

    pred = predict(model, message, list_labels)
    # pred = predict_t(model, list_labels)

    #  Send reply back to client
    #  In the real world usage, after you finish your work, send your output here
    print(pred)
    socket.send_string(pred)
