# ImageProcessing

Contains multiple examples on how images can be processed using C# .NET.

## Recognize Numbers by Signature

### Overview

In the first example we will try to identify numbers (0-9) in an image by using their signature.

These are the images with the numbers in their unprocessed from.

 1  |  2  |  3  |  4  |  5
--- | --- | --- | --- | ---
![1](./doc/RawNumbers/1.jpg) | ![2](./doc/RawNumbers/2.jpg) | ![3](./doc/RawNumbers/3.jpg) | ![4](./doc/RawNumbers/4.jpg) | ![5](./doc/RawNumbers/5.jpg)

 6  |  7  |  8  |  9  |  0
--- | --- | --- | --- | ---
![6](./doc/RawNumbers/6.jpg) | ![7](./doc/RawNumbers/7.jpg) | ![8](./doc/RawNumbers/8.jpg) | ![9](./doc/RawNumbers/9.jpg) | ![0](./doc/RawNumbers/0.jpg)

By applying different processing steps, we try to get the signature of the number displayed
in the image. The reason we want the signature is:

* It provides a stable result even if the number in the image is rotated.
* It can be scaled to a normalized graph, so that the scale of the number in the image can be neglected.


### Step 1: convertion to binary image

An image is represented by a matrix containing its color as a RGB (red, green, blue) value. We can convert
those pixels by applying a given threashold `t` to their color values. Is the average color value of a pixel
belove the threashold `t`, then we convert this pixel to black (represented by 1) otherwise to white
(represented by 0). By doing so we get a simpler matrix, which is more suitable for futher image processing.

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
<img src="./doc/Binary/binary_05.png" height="100px"> | => | <img src="./doc/Cropped/cropped_05.png" height="60px">


### Step 3: Shrinking

In this step we want to reduce the amout of pixels processed even further by shrinking the size of the
image. For this we consider the pixels in a `2x2` matrix. If the majority of the pixels is 1 (black)
in this matrix, the final pixel will also be 1 (black), otherwise 0 (white). By doing so we can shrink
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


### Application

And this is the result in the sample application (screen shot).

![screen shot](./doc/1-processing-steps.png)

## Remarks

### Image Icon

The Image Icon was taken from [iconfinder.com](https://www.iconfinder.com/icons/79825/compressed_image_svg+xml_icon).

![Image Icon](https://cdn1.iconfinder.com/data/icons/fs-icons-ubuntu-by-franksouza-/128/image-svg-plus-xml-compressed.png "Image Icon")

Author:  [Frank Souza](https://www.iconfinder.com/iconsets/fs-icons-ubuntu-by-franksouza-)  
Licence: Free for non commercial use