//
// Created by Askat on 5/15/19.
// Modified by Zhanat on May 2022
//

#ifndef ROS_ARUCO_ROS_ARUCO_H
#define ROS_ARUCO_ROS_ARUCO_H

#include <ros/ros.h>
#include "opencv2/opencv.hpp"
#include <opencv2/aruco.hpp>
#include <map>
#include <Eigen/Dense>
#include <iostream>
#include <cmath>
#include <tf2/LinearMath/Transform.h>
#define deg2rad M_PI/180.0f


class ArUcoNode{
private:
    /*camera calibration parameters */
    cv::Mat cameraMatrix; // camera matrix
    cv::Mat distCoeffs; // distortion coefficients
    int width, height, fps; // camera parameters

    /*aruco module parameters*/
    cv::Ptr<cv::aruco::Dictionary> dictionary;
    cv::Ptr<cv::aruco::DetectorParameters> parameters; // detector parameters
    float aruco_len; //size of the marker (meter)
    float axis_len; //drawn axis length

    // ROS
    ros::NodeHandle n_h_; // ROS NodeHandle
    ros::Publisher pose_pub; // ROS topic publisher
    ros::Publisher pose_pub_user_position_data_for_hololens; // ROS topic publisher
    ros::ServiceClient gazebo_client;

    // Transformation map
    std::map<int,Eigen::Matrix4f> transMap;
    std::map<int,tf2::Quaternion> q_bm;
    float rtx[1000],rty[1000],rtz[1000];

    // text file name
    std::string  fileName = "/home/issai/catkin_ws/src/ros_aruco/aruco_marker_position_data_in_the_building.txt";

    float y_max = 2.3, y_min = 1.0;
    struct EulerAngles{
        double roll,yaw,pitch;
    };


public:
    ArUcoNode();        // constructor
    // to initialize homogeneous matrices
    void initialization();
    int arucoDetectZhanat();
    double datenum(int year, int mon, int day, int hour, int imin, int sec, int mil);
    std::string getTimestamp(); 
    Eigen::Matrix4f trMat(Eigen::Matrix4f &Hm, float &x, float &y, float &z, float &rx, float &ry, float &rz);        
    tf2::Quaternion bmQuat(tf2::Quaternion &q2, float &rx, float &ry, float &rz);
    // returns the rotation matrix: camera -> marker
    Eigen::Matrix3f rotMat(double qw, double qx, double qy, double qz);
    // Euclidean distance between two points
    static double dist(const cv::Point2f &p1, const cv::Point2f &p2);
    // Compute area in image of a fiducial, using Heron's formula to find the area of two triangles
    static double calcFiducialArea(const std::vector<cv::Point2f> &pts);
    EulerAngles ToEulerAngles(tf2::Quaternion q);
    int arucoDetect();

};

#endif //ROS_ARUCO_ROS_ARUCO_H
