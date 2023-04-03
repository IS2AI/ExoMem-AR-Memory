![ExoMem_github_april](https://user-images.githubusercontent.com/7812207/229469811-f5c1cc70-4bdf-4e04-bbb3-cd88f6b08ce0.png)

This repository contains the source code developed for the AR-based human memory augmentation system, ExoMem. ExoMem is the first Augmented Reality (AR) and Artificial Intelligence (AI) enhanced head-worn system that constructs a holographic visuospatial memory in an indoor environment. Microsoft HoloLens AR Goggles sense the environment, exchange data over a wireless network with a computing module (PC with Ubuntu 16.04), and construct a holographic visuospatial memory. The computing module performs computer vision-based localization and object detection on first-person view (FPV) data received from the HoloLens.

# Dependencies

## AI environment perception

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

4. Download yolov3-tiny.weights.zip
https://drive.google.com/drive/folders/1d0y73MaLYDGnXHFyNfsM0MxcvIwlrBDm
  * Unzip and paste yolov3-tiny.weights to the directory ~/home/AI_Environment_Perception 

5. Check the system, 172.20.10.4 is the IP address of HoloLens:
  * $ cd AI_environment_perception/
  * $ sudo update-alternatives --config python
  * $ unset PYTHONPATH
  * $ python AI_environment_perception.py -a 172.20.10.4
 
## AR environment 

1. Visual Studio 2019 
https://visualstudio.microsoft.com/ru/vs/older-downloads/

* In the Visual Studio Installer select the following components
* .Net desktop development
* Universal Windows Platform development
* Desktop development with C++
* .Net Core cross-platform development

2. Unity 2018.4.28f1
https://unity3d.com/get-unity/download/archive

### Unity application for creating holographic visuospatial memory in AR

  * Download ExoMem_Record_Memory_HoloLens.zip from https://drive.google.com/drive/folders/1CuzTKzWoNJYEFS3oJBJ-ZdGjIIBZWmXQ
  * Go to the directory ~/ExoMem_Record_Memory_HoloLens/App/ExoMem_Record_Memory_App 
  * Rigth click ExoMem_Record_Memory_HoloLens.sln and open with Microsoft Visual Studio 2019 
  * Visual Studio 2019, select (Release, ARM, Remote Machine)
  * In Debug Properties, Machine Name: type the HoloLens' IP Address
  * In Debug, select Start Without Debugging

### Unity application for loading holographic visuospatial memory in AR

  * Download ExoMem_Load_Memory_HoloLens.zip from https://drive.google.com/drive/folders/1CuzTKzWoNJYEFS3oJBJ-ZdGjIIBZWmXQ
  * Go to the directory ~/ExoMem_Load_Memory_HoloLens/App/ExoMem_Load_Memory 
  * Rigth click ExoMem_Load_Memory_HoloLens.sln and open with Microsoft Visual Studio 2019 
  * Visual Studio 2019, select (Release, ARM, Remote Machine)
  * In Debug Properties, Machine Name: type the HoloLens' IP Address
  * In Debug, select Start Without Debugging

### Unity application for running the computer-based test during map-pointing activity

  * Download Retrieve_Memory_Computer_Based_Test.zip from https://drive.google.com/drive/folders/1CuzTKzWoNJYEFS3oJBJ-ZdGjIIBZWmXQ
  * Open Unity 2018.4.28f1, at the start menu select open the project and go to the directory Retrieve_Memory_Computer_Based_Test
  * Go to scenes, open input_user_data_scene and run the program 
  * Select AR:0, Day:1, ID: 1 (for example)
  * Go to scenes, open load_memory_desktop and run the program 
  * Select AR:0, Day:1, ID: 1 (for example)

# ExoMem 
* [Create the holographic visuospatial memory in AR](#create)
* [Load the holographic visuospatial memory in AR](#load)
* [Computer-based test](#test)

## [Create the holographic visuospatial memory in AR] (#create)
1. Start the ros master
  * Open a new terminal, roscore will use python 2.7
  * $ sudo update-alternatives --config python
  * Choose the version of python 2.7
  * $ unset PYTHONPATH
  * $ source /opt/ros/kinetic/setup.bash
  * $ roscore

2. Run the aruco_node 
  * Go to the catkin_ws directory in ros environment
  * $ cd catkin_ws/
  * $ source devel/setup.bash
  * $ rosrun ros_aruco aruco_node

3. Run the python code 
  * Go to the directory where the python code is stored
  * $ cd AI_environment_perception/
  * For the code the python 3.7 will be used, therefore run the following command
  * $ sudo update-alternatives --config python
  * Choose the version of python 3.7
  * $ unset PYTHONPATH
  * $ python AI_environment_perception.py -a 172.20.10.4

4. Run the Unity application in HoloLens 2 
  * ExoMem_Record_Memory_HoloLens
  

https://user-images.githubusercontent.com/7812207/229480773-01e7db9b-6f13-40bc-9992-045e1f6d5b11.mp4



https://user-images.githubusercontent.com/7812207/229480834-248c6552-36ff-4665-ac0e-8035936c000f.mp4


## Load the holographic visuospatial memory in AR 
1. Run the Unity application in HoloLens 2
  * ExoMem_Load_Memory_HoloLens

## Computer-based test
1. Run the Unity application in Desktop computer
  * Retrieve_Memory_Computer_Based_Test


https://user-images.githubusercontent.com/7812207/229480984-d8b9b8e9-7680-4ab7-a0b5-f48179f368e7.mp4

