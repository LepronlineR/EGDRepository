#
#   takes audio file and predicts
#   Binds REP socket to tcp://*:5555
#

import tensorflow as tf
import os
import librosa
import librosa.display
import numpy as np
import zmq
import io
import scipy.io.wavfile
import soundfile as sf
from scipy.io.wavfile import write

def extract_features(data, sample_rate):
    # ZCR
    result = np.array([])
    zcr = np.mean(librosa.feature.zero_crossing_rate(y=data).T, axis=0)
    result=np.hstack((result, zcr)) # stacking horizontally

    # Chroma_stft
    stft = np.abs(librosa.stft(data))
    chroma_stft = np.mean(librosa.feature.chroma_stft(S=stft, sr=sample_rate).T, axis=0)
    result = np.hstack((result, chroma_stft)) # stacking horizontally

    # MFCC
    mfcc = np.mean(librosa.feature.mfcc(y=data, sr=sample_rate).T, axis=0)
    result = np.hstack((result, mfcc)) # stacking horizontally

    # Root Mean Square Value
    rms = np.mean(librosa.feature.rms(y=data).T, axis=0)
    result = np.hstack((result, rms)) # stacking horizontally

    # MelSpectogram
    mel = np.mean(librosa.feature.melspectrogram(y=data, sr=sample_rate).T, axis=0)
    result = np.hstack((result, mel)) # stacking horizontally
    
    return result

def convert_bytearray_to_wav_ndarray(input_bytearr: bytes, sampling_rate=44100):
    bytes_wav = bytes()
    byte_io = io.BytesIO(bytes_wav)
    write(byte_io, sampling_rate, np.frombuffer(input_bytearr, dtype=np.int32))
    output_wav = byte_io.read()
    output, sample_r = sf.read(io.BytesIO(output_wav))
    return output

def predict(model, bytes_wav, labels, sampling_rate = 44100, filename = "output.wav"):

    if os.path.exists(filename):
        os.remove(filename)
    with open(filename, mode='bx') as f:
        f.write(bytes_wav)

    # transform
    data, sample_rate = librosa.load(filename)
    X = extract_features(data, sample_rate)
    # predict
    pred = model.predict(tf.expand_dims(X, axis=0))
    if len(pred[0]) > 1:
        pred_class = list_labels[tf.argmax(pred[0])]
    else:
        pred_class = list_labels[int(tf.round(pred[0]))]

    return pred_class

list_labels = ['angry', 'fear', 'happy', 'neutral', 'sad']

model = tf.keras.models.load_model('results.h5')

context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5555")
    
while True:
    #  Wait for next request from client
    message = socket.recv()

    pred = predict(model, message, list_labels)

    #  Send reply back to client
    print(pred)
    socket.send_string(pred)
