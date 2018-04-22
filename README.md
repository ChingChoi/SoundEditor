# SoundEditor

Basic feature
The program analyzes sound/music by calculating existing frequencies in a loaded sound file (currently wav file only).

Record feature
The program can record and playback audio.

Users feature
- process the sound file by copy, cut, and pasting the sound in time domain graph. 
- process the sound file by applying a filter to remove unwanted frequencies in frequencies domain graph (currently only has low-pass). 
- select different windowing techniques to enhance frequencies analysis results.

The program uses Discrete fourier transform to analyze sound frequencies. Windowing algorithms include "rectangle", "triangle", and "polynomial".

Filter is done using convolution

How to analyze and filter wav file
1) Open any wav file in File -> Open [Ctrl + Alt + O]
2) Select region you would like to analyze the frequencies on (click and drag)
3) Select types of windowing you would like to apply in Transformation -> Windowing (Default is rectangle)
4) Apply discrete fourier transform in Transformation -> Discrete Fourier Transform [Ctrl + Alt + D]
5) Filter result by selecting region of frequencies you would like to keep, and apply in Transformation -> Filter [Ctrl + Alt + F]
6) Save result if desired in File -> Save [Ctrl + S]

![Demo](https://github.com/ChingChoi/SoundEditor/blob/master/Resource/Demo.png)

How to record and playback
1) Open recorder panel in Tools -> Recorder [Ctrl + Alt + R]
2) To record, press the red circle
3) To end recording, press the red square
4) Playback features are performed using green buttons

![Recorder](https://github.com/ChingChoi/SoundEditor/blob/master/Resource/Recorder.png)

