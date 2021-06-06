
### Step 1: convertion to binary image

An image is represented by a matrix containing its color as a RGB (red, green, blue) value. We can convert
those pixels by applying a given threashold `t` to their color values. Is the average color value of a pixel
belove the threashold `t`, then we convert this pixel to black (represented by `1`) otherwise to white
(represented by `0`). By doing so we get a simpler matrix, which is more suitable for futher image processing.

```
 Threashold t: 120
 Color value of a pixel: (R + G + B)/3

 Conversion: ColorValueOfAPixel > t   --->   true: 0;  false: 1

 RGB image in hex values                          Binary image
 [ 0xFFFFFF 0x000000 0xFFFFFF 0xFFFFFF ]          [ 0 1 0 0 ]
 [ 0xFFFFFF 0x000000 0x000000 0xFFFFFF ]          [ 0 1 1 0 ]
 [ 0xFFFFFF 0x000000 0x111111 0x222222 ]   --->   [ 0 1 1 1 ]
 [ 0xFFFFFF 0x000000 0x000000 0xFFFFFF ]          [ 0 1 1 0 ]
 [ 0xFFFFFF 0xFFFFFF 0xFFFFFF 0xFFFFFF ]          [ 0 0 0 0 ]
```

Before (color) | . | After (binary)
---------------- | - | ------------------
<img src="./doc/RawNumbers/5.jpg" height="100px"> | => | <img src="./doc/Binary/binary_05.png" height="100px">


### Step 2: cropping

The goal of this step is to reduce the amount of pixels for further processing to speed up things. This
can easily be done by cropping all unnecessary areas around our object in the image. For this we determine
the black pixels at the edge (`top`, `right`, `bottom`, `left`) of our object in the image and only
consider the matrix range within these edges with a smal offset of 1 pixel.

```
Binary image   (for better readability:  0 -> . | 1 -> #)

      0 1 2 3 4 5 6 7 8 9 10
  . -------------------------> n (zero based)          Cropped Image
0 | [ . . . . . . . . . . . ]                          [ . . . . . . ]
1 | [ . . . . . . . . . . . ]  top edge: m=2           [ . # . . . . ]
2 | [ . . . # . . . . . . . ]  right edge: n=6   --->  [ . # # . . . ]
3 | [ . . . # # . . . . . . ]  bottom edge: m=5        [ . # # # . . ]
4 | [ . . . # # # . . . . . ]  left edge: n=3          [ . # # # # . ]
5 | [ . . . # # # # . . . . ]                          [ . . . . . . ]
6 | [ . . . . . . . . . . . ] 
7 | [ . . . . . . . . . . . ]
  v
m (zero based)
```

Before | . | After (cropped)
-------- | - | -------
<img src="./doc/Binary/binary_05.png" height="100px"> | => | <img src="./doc/Cropped/cropped_05.png" height="100px">


### Step 3: Shrinking

In this step we want to reduce the amout of pixels processed even further by shrinking the size of the
image. For this we consider the pixels in a `2x2` matrix. If the majority of the pixels is `1` (black)
in this matrix, the final pixel will also be `1` (black), otherwise `0` (white). By doing so we can shrink
the size of the image to half.

```
Binary image   (for better readability:  0 -> . | 1 -> #)

Original (16x16)            Shrinked (8x8)
[ .. .. .. .. .. .. .. .. ]   [ . . . . . . . . ]
[ .. .# ## ## ## ## #. .. ]   [ . # # # # # # . ]
[ .. .# ## ## ## ## ## .. ]   [ . # . . . . . . ]
[ .. ## ## ## ## ## #. .. ]   [ . # . . . . . . ]
[ .. ## #. .. .. .. .. .. ]   [ . # # # # # . . ]
[ .. ## #. .. .. .. .. .. ]   [ . # . . . . . . ]
[ .. ## #. .. .. .. .. .. ]   [ . # # # # # # . ]
[ .. ## ## ## ## ## #. .. ]   [ . . . . . . . . ]
[ .. .# ## ## ## ## #. .. ]
[ .. ## ## ## ## ## .. .. ]
[ .. ## #. .. .. .. .. .. ]
[ .. ## #. .. .. .. .. .. ]
[ .. ## ## ## ## ## #. .. ]
[ .. ## ## ## ## ## ## .. ]
[ .. .# ## ## ## ## #. .. ]
[ .. .. .. .. .. .. .. .. ]
```

Before   | . | After (shrinked)
-------- | - | -------
<img src="./doc/Cropped/cropped_05.png" height="100px"> | => | <img src="./doc/DownSizing/img/downSizedImage_05.png" height="100px">


### Step 4: Remove Noise (Smoothing)

Due to the previous shrinking process some pixels in the image seem off place. To remove unnecessary
noises in the image, the next step will be to apply a smoothing filter. The smoothing filter is
a `7x7` matrix where the center contains the average value of all pixels in the matrix.

```
7x7 smoothing filter
[ . . . . . . . ]
[ . . . . . . . ]
[ . . . . . . . ]
[ . . . X . . . ]  X = the center pixel contains the average value of the whole matrix
[ . . . . . . . ]
[ . . . . . . . ]
[ . . . . . . . ]
```

If the center value of the smoothing filter matrix is `>= 0.5` then the pixel will be set to `1`
(black), otherwise to `0` (white).

Before   | . | After (smoothed)
-------- | - | -------
<img src="./doc/DownSizing/img/downSizedImage_05.png" height="100px"> | => | <img src="./doc/Smoothing/img/smoothedImage_05.png" height="100px">

### Step 5: Getting the Skeleton

The purpose of this step is to reduce thick areas to its core skeleton so that only the relevant
pixel remain. The process of thinning is explained [here](./doc/Thinning/Readme.md)

Start | 2nd iteration | 4th iteration | 6th iteration | 8th iteration | 11th iteration (final)
----- | ------------- | ------------- | ------------- | ------------- | -------------
<img src="./doc/Thinning/img/thinndedImage_05_00.png" height="100px"> | <img src="./doc/Thinning/img/thinndedImage_05_02.png" height="100px"> | <img src="./doc/Thinning/img/thinndedImage_05_04.png" height="100px"> | <img src="./doc/Thinning/img/thinndedImage_05_06.png" height="100px"> | <img src="./doc/Thinning/img/thinndedImage_05_08.png" height="100px"> | <img src="./doc/Thinning/img/thinndedImage_05_11.png" height="100px">

### Step 6: Getting the Signature

Based on the previously calculated skeleton we can now finally create the signature of the object.
The signature is created by sampling the points from the center of the image and plot all the black
pixels (value `1`) found on a graph.

<img src="./doc/Signature/img/signatureExample.png" height="300px">

By applying the algorithm on the created skeleton we get the following result.

sampling of number | resulting signature
------------------ | -------------------
<img src="./doc/Signature/img/128/sampledSignature128_05.png" height="200px"> | <img src="./doc/Signature/img/360/signature360_05.png" >


### Step 7: Comparing with Signature Template

The last step is to compare the calculated signature with the predefined templates.

These are the templates used for the numbers.

 1  |  2  |  3 
--- | --- | ---
![1](./doc/SignatureTemplate/signTemplate360_01.png) | ![2](./doc/SignatureTemplate/signTemplate360_02.png) | ![3](./doc/SignatureTemplate/signTemplate360_03.png) | 

 4  |  5  |  6 
--- | --- | ---
![4](./doc/SignatureTemplate/signTemplate360_04.png) | ![5](./doc/SignatureTemplate/signTemplate360_05.png) | ![6](./doc/SignatureTemplate/signTemplate360_06.png)

 7  |  8  |  9  |  0
--- | --- | --- | ---
![7](./doc/SignatureTemplate/signTemplate360_07.png) | ![8](./doc/SignatureTemplate/signTemplate360_08.png) | ![9](./doc/SignatureTemplate/signTemplate360_09.png) | ![0](./doc/SignatureTemplate/signTemplate360_00.png)

If all pixels of a signature are within the predefined template, then we consider this a match.