[![.NET](https://github.com/mmarkovic/ImageProcessing/actions/workflows/dotnet.yml/badge.svg)](https://github.com/bbvch-academy/CleanCpp.Academy.Coronan/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

# ImageProcessing

Contains multiple examples on how images can be processed using C# .NET.

## Recognize Numbers by Signature

### Overview

The goal is to identify numbers (0-9) in an image by using their signature.

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

### Processing Steps

A more detailed explanation of the processing steps can be found [here](https://github.com/mmarkovic/ImageProcessing/tree/main/doc#readme).

The following steps will be performed to get the signature of the number in an image.

  0  |  1.  |  2.  |  3.  |  4.  |  5.  |  6. 
---- | ---- | ---- | ---- | ---- | ---- | ----
<img src="./doc/RawNumbers/5.jpg" height="100px" /> | <img src="./doc/Binary/binary_05.png" height="100px" /> | <img src="./doc/Cropped/cropped_05.png" height="100px" /> | <img src="./doc/DownSizing/img/downSizedImage_05.png" height="100px" /> | <img src="./doc/Smoothing/img/smoothedImage_05.png" height="100px" /> | <img src="./doc/Thinning/img/thinndedImage_05_11.png" height="100px" /> | <img src="./doc/Signature/img/360/signature360_05.png" />
original | binary image convertion | corpping | shrinking | smoothing | skeleton extraction | signature calculation

### UI

The calculated signature will be displayed in the UI as well as the maching signature template.

![UI](./doc/UI-screen-shot.png)


## Remarks

### Image Icon

The Image Icon was taken from [iconfinder.com](https://www.iconfinder.com/icons/79825/compressed_image_svg+xml_icon).

![Image Icon](https://cdn1.iconfinder.com/data/icons/fs-icons-ubuntu-by-franksouza-/128/image-svg-plus-xml-compressed.png "Image Icon")

Author:  [Frank Souza](https://www.iconfinder.com/iconsets/fs-icons-ubuntu-by-franksouza-)  
Licence: Free for non commercial use