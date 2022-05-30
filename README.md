![ExoMem_github](https://user-images.githubusercontent.com/7812207/170980352-71c7c8d8-5d7a-40be-9828-f7b873b0b838.png)

This repository contains the source code developed for human memory augmentation system, ExoMem. ExoMem is the first Augemnted Reality (AR) and Artificial Intelligence (AI) enhanced cognitive assistant that constructs a synthetic spatiotemporal memory for objects in an indoor environment. Microsoft HoloLens AR Goggles sense the environment, exchange data over a wireless network and construct a spatiotemporal memory. A computing module performs computer vision-based localization and object detection on first-person view (FPV) data received from the HoloLens.

# Dependencies

## For AI environment perception

### ArUco marker based user localization 

1. Ubuntu 16.04 
https://releases.ubuntu.com/16.04/

2. ROS Kinetic
http://wiki.ros.org/kinetic/Installation/Ubuntu

3. Create and build a catkin workspace:
$ mkdir -p ~/catkin_ws/src
$ cd ~/catkin_ws/
$ catkin_make

## For recording the spatiotemporal memory
