# Thinning

Thinning describes the process of reducing the shapes in an images to its core skeleton.
By applying thinning to a shape only its characteristic attributes remain, which can be
used for futher processing steps such as object recognition.

![Structural Elements B1 to B8](./img/ImageAndSkeleton.png)

The starting point in this project is a binary image with a broad area. To get the skeleton
of this area, structural elements will be applied using the hit-and-miss transformation.
Here we are using the strucutral elements B1 to B8.

![Structural Elements B1 to B8](./img/StructuralElementsB1ToB8.png)

This is the result of applying the structural elements in sequence through 10 iterations.

![0](./img/thinndedImage_09_00.png)
![1](./img/thinndedImage_09_01.png)
![2](./img/thinndedImage_09_02.png)
![3](./img/thinndedImage_09_03.png)
![4](./img/thinndedImage_09_04.png)
![5](./img/thinndedImage_09_05.png)
![6](./img/thinndedImage_09_06.png)
![7](./img/thinndedImage_09_07.png)
![8](./img/thinndedImage_09_08.png)
![9](./img/thinndedImage_09_09.png)
![10](./img/thinndedImage_09_10.png)
![11](./img/thinndedImage_09_11.png)

A step by step description can be found in the following excel sheet:
[ThinningTestCases.xlsx](./ThinningTestCases.xlsx)