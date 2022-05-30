![ExoMem_github](https://user-images.githubusercontent.com/7812207/170980352-71c7c8d8-5d7a-40be-9828-f7b873b0b838.png)

This repository contains the source code developed for human memory augmentation system, ExoMem. ExoMem is the first Augemnted Reality (AR) and Artificial Intelligence (AI) enhanced cognitive assistant that constructs a synthetic spatiotemporal memory for objects in an indoor environment. Microsoft HoloLens AR Goggles sense the environment, exchange data over a wireless network and construct a spatiotemporal memory. A computing module performs computer vision-based localization and object detection on first-person view (FPV) data received from the HoloLens.

# Dependencies

## For AI environment perception

### CV-based user localization using ArUco fiducial markers

1. Ubuntu 16.04 
https://releases.ubuntu.com/16.04/

2. ROS Kinetic
http://wiki.ros.org/kinetic/Installation/Ubuntu

3. Create and build a catkin workspace:
  * $ mkdir -p ~/catkin_ws/src
  * $ cd ~/catkin_ws/
  * $ catkin_make
  
4. Copy and paste folder ros_aruco to the directory ~/catkin_ws/src:
  * $ sudo apt-get install ros-kinetic-fiducials
  * $ catkin_make
  
5. Update the system. If not updated remove the old key and import the new key:
  * $ sudo apt-get update
  * $ sudo apt-key del 421C365BD9FF1F717815A3895523BAEEB01FA116
  * $ sudo -E apt-key adv --keyserver 'hkp://keyserver.ubuntu.com:80' --recv-key C1CF6E31E6BADE8868B172B4F42ED6FBAB17C654
  * $ sudo apt-get update
  
6. Check the system:
  * roscore
  * cd catkin_ws/
  * source devel/setup.bash
  * rosrun ros_aruco aruco_node

### Object recognition using YOLO V3 object detector

1. Install Python 3.7 from Source Code (Python 2.7 is already installed during the ROS Kinetic installatipon in Step 2)
https://www.osetc.com/en/how-to-install-the-latest-python-3-7-on-ubuntu-16-04-or-18-04.html
  * $ sudo update-alternatives --install /usr/bin/python python /usr/bin/python2.7 2
  * $ sudo update-alternatives --install /usr/local/bin/python python /usr/local/bin/python3.7 3
 
2. Istall OpenCV to Python 3.7.x, but first upgrade pip3 and setuptools to the latest versions
  * $ pip3 install --upgrade pip setuptools
  * $ sudo pip3 install opencv-contrib-python
  
3. Copy and paste folder AI_Environment_Perception to the directory ~/home

4. Check the system:
  * $ cd AI_environment_perception/
  * $ sudo update-alternatives --config python
  * $ unset PYTHONPATH
  * $ python AI_environment_perception.py -a 172.20.10.4
  where 172.20.10.4 is IP address of HoloLens 

## For recording the spatiotemporal memory
