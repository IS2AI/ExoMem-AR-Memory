![ExoMem_github](https://user-images.githubusercontent.com/7812207/170980352-71c7c8d8-5d7a-40be-9828-f7b873b0b838.png)

This repository contains the source code developed for human memory augmentation system, ExoMem. ExoMem is the first Augemnted Reality (AR) and Artificial Intelligence (AI) enhanced cognitive assistant that constructs a synthetic spatiotemporal memory for objects in an indoor environment. Microsoft HoloLens AR Goggles sense the environment, exchange data over a wireless network and construct a spatiotemporal memory. A computing module performs computer vision-based localization and object detection on first-person view (FPV) data received from the HoloLens.

# Dependencies

## For AI environment perception

### ArUco marker based user localization 

1. Ubuntu 16.04 
https://releases.ubuntu.com/16.04/

2. ROS Kinetic
http://wiki.ros.org/kinetic/Installation/Ubuntu

3. In the terminal create and build a catkin workspace:
  * $ mkdir -p ~/catkin_ws/src
  * $ cd ~/catkin_ws/
  * $ catkin_make
  
4. Copy and paste folder ros_aruco to the directory ~/catkin_ws/src

5. In the terminal run the following commands 
  * $ sudo apt-get install ros-kinetic-fiducials
  * $ catkin_make

6. Copy and paste folder AI_Environment_Perception to the directory ~/home

7. In the terminal run
  * $ sudo apt-get update

8. If not updated run the following commands:
  * Remove the old key $ sudo apt-key del 421C365BD9FF1F717815A3895523BAEEB01FA116
  * Import the new key $ sudo -E apt-key adv --keyserver 'hkp://keyserver.ubuntu.com:80' --recv-key C1CF6E31E6BADE8868B172B4F42ED6FBAB17C654
  * $ sudo apt-get update

7. Python 3.7.x 
  * Installing Python 3.7 from Source Code https://www.osetc.com/en/how-to-install-the-latest-python-3-7-on-ubuntu-16-04-or-18-04.html


## For recording the spatiotemporal memory
