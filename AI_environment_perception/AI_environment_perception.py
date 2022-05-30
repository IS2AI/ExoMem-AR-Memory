

#Code for object detection and user localization using image data registered by HoloLens
#Author: Zhanat Makhataeva
#Date: May 2022

#!/usr/bin/env python3

from __future__ import print_function
import argparse
import socket
import sys
import binascii
import struct
from collections import namedtuple
import cv2
import numpy as np
from datetime import datetime
import numpy
import cv2.aruco as aruco
import os

PROCESS = True

# Constant parameters used in AruCo methods
# This is need to register ArUco markers
ARUCO_PARAMETERS = aruco.DetectorParameters_create()
ARUCO_DICT = aruco.Dictionary_get(aruco.DICT_ARUCO_ORIGINAL)

# Needed to receive images from the HoloLens 2
SENSOR_STREAM_HEADER_FORMAT = "@IBBHqIIII"

SENSOR_FRAME_STREAM_HEADER = namedtuple(
    'SensorFrameStreamHeader',
    'Cookie VersionMajor VersionMinor FrameType Timestamp ImageWidth ImageHeight PixelStride RowStride'
)

# Each port corresponds to a single stream type
# Port for obtaining Photo Video Camera stream
PORT_to_receive_PV_STREAM_from_hololens = 23940

# Port for sending detected object data to hololens
PORT_to_send_detected_object_data_to_hololens=9090

# Port for sending detected aruno marker data to aruco node in ROS environment 
PORT_to_send_detected_aruco_marker_data_to_aruco_node_in_ros=10105

# Port for receiving user postion data calculated in the aruco node in ROS environment 
PORT_to_receive_user_position_data_from_aruco_node_in_ros=10112

# Port for sending user position data to hololens
PORT_to_send_user_position_data_to_hololens=9097

# Create grid board object we're using in our stream
board = aruco.GridBoard_create(
        markersX=2,
        markersY=2,
        markerLength=0.09,
        markerSeparation=0.01,
        dictionary=ARUCO_DICT)
        
def main(argv):

    """Receiver main"""
    parser = argparse.ArgumentParser()
    required_named_group = parser.add_argument_group('named arguments')
    required_named_group.add_argument("-a", "--host", help="Host address to connect", required=True)
    args = parser.parse_args(argv)

    # Create a TCP Stream socket
    receive_rgb_frame_from_hololens = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    send_detected_object_data_to_hololens = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    send_detected_aruco_marker_data_to_aruco_node_in_ros = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    receive_user_position_data_from_aruco_node_in_ros = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
    send_user_position_data_to_hololens = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    
    # Connecting to the address
    receive_rgb_frame_from_hololens.connect((args.host, PORT_to_receive_PV_STREAM_from_hololens))
    send_detected_object_data_to_hololens.connect((args.host,PORT_to_send_detected_object_data_to_hololens))
    send_detected_aruco_marker_data_to_aruco_node_in_ros.connect(('',PORT_to_send_detected_aruco_marker_data_to_aruco_node_in_ros))
    receive_user_position_data_from_aruco_node_in_ros.connect(('',PORT_to_receive_user_position_data_from_aruco_node_in_ros))  
    send_user_position_data_to_hololens.connect((args.host,PORT_to_send_user_position_data_to_hololens))
    
    # Load Yolo object detector
    object_detector_net = cv2.dnn.readNet("yolov3-tiny.weights", "yolov3-tiny.cfg")
    classes = []
    with open("coco.names", "r") as f:
        classes = [line.strip() for line in f.readlines()]
    layer_names = object_detector_net.getLayerNames()
    output_layers = [layer_names[i[0] - 1] for i in object_detector_net.getUnconnectedOutLayers()]
    colors = np.random.uniform(0, 255, size=(len(classes), 3))
    received_image_number=0
    
    # Receive data
    try:
        quit = False
        while not quit:
            reply = receive_rgb_frame_from_hololens.recv(struct.calcsize(SENSOR_STREAM_HEADER_FORMAT))
            if not reply:
                print('ERROR: Failed to receive data')
                sys.exit()

            data = struct.unpack(SENSOR_STREAM_HEADER_FORMAT, reply)

            # Parse the header
            header = SENSOR_FRAME_STREAM_HEADER(*data)
            
            # read the image in chunks
            image_size_bytes = header.ImageHeight * header.RowStride
            
            image_data = b''

            while len(image_data) < image_size_bytes:
                remaining_bytes = image_size_bytes - len(image_data)
                image_data_chunk = receive_rgb_frame_from_hololens.recv(remaining_bytes)

                if not image_data_chunk:
                    print('ERROR: Failed to receive image data')
                    sys.exit()
                image_data += image_data_chunk

            array_with_image_data = np.frombuffer(image_data, dtype=np.uint8).reshape((header.ImageHeight,
                                        header.ImageWidth, header.PixelStride))
            if PROCESS:
                # process image
                array_with_image_data = cv2.cvtColor(array_with_image_data,cv2.cv2.COLOR_RGBA2RGB)
                gray_image = cv2.cvtColor(array_with_image_data, cv2.COLOR_BGR2GRAY)

            height, width, channels = array_with_image_data.shape
            
            blob = cv2.dnn.blobFromImage(array_with_image_data, 0.00392, (416, 416), (0, 0, 0), True, crop=False)
            object_detector_net.setInput(blob)
            outs = object_detector_net.forward(output_layers)
            
            class_ids = []
            confidences = []
            boxes = []
            for out in outs:
                for detection in out:
                    scores = detection[5:]
                    class_id = np.argmax(scores)
                    confidence = scores[class_id]
                    if confidence > 0.5:
                        # Object detected
                        center_x = int(detection[0] * width)
                        center_y = int(detection[1] * height)
                        w = int(detection[2] * width)
                        h = int(detection[3] * height)
                        
                        # Rectangle coordinates
                        x = int(center_x - w / 2)
                        y = int(center_y - h / 2)
                        boxes.append([x, y, w, h])
                        confidences.append(float(confidence))
                        class_ids.append(class_id)
                        
            indexes = cv2.dnn.NMSBoxes(boxes, confidences, 0.5, 0.4)

            font = cv2.FONT_HERSHEY_PLAIN
            
            array_with_detected_objects_data="{}:".format(received_image_number)
            
            if len(boxes)==0:
                array_with_detected_objects_data ="{}00:0".format(array_with_detected_objects_data)
                send_detected_object_data_to_hololens.sendall(array_with_detected_objects_data.encode("utf-8"))
                
            else:
                for i in range(len(boxes)):
                    if i in indexes:
                        x, y, w, h = boxes[i]
                        label = str(classes[class_ids[i]])
                        color = colors[i]
                        cv2.rectangle(array_with_image_data, (x, y), (x + w, y + h), color, 2)
                        cv2.putText(array_with_image_data, label, (x, y + 30), font, 3, color, 3)
                        
                        array_with_detected_objects_data ="{}{}:{}:{}:{}:{}:{}".format(array_with_detected_objects_data,class_ids[i],x*416/900,y*416/510,h*416/510,w*416/900,confidences[i])
                        
                        if(i!=(len(boxes)-1)):
                            array_with_detected_objects_data ="{}:".format(array_with_detected_objects_data)
                            
            send_detected_object_data_to_hololens.sendall(str(array_with_detected_objects_data).encode("utf-8"))
            
            # Detect Aruco markers
            ARUCO_PARAMETERS.cornerRefinementMethod=aruco.CORNER_REFINE_NONE
            ARUCO_PARAMETERS.adaptiveThreshWinSizeMin=7
            ARUCO_PARAMETERS.adaptiveThreshWinSizeMax=23
            ARUCO_PARAMETERS.minMarkerPerimeterRate=0.1
            corners, ids, rejectedImgPoints = aruco.detectMarkers(gray_image, ARUCO_DICT, parameters=ARUCO_PARAMETERS)
            
            size_of_marker  = 0.14
            dist = np.array([[-4.84508353e-02],  [2.79630248e-01], [6.80014833e-03],  [8.17392130e-04], [-9.64730774e-01]])
            mtx = np.array([[666.52349183,   0,         441.04189665],
            [  0,         659.37164362, 236.29693346],
            [  0,           0,           1        ]])
            
            aruco_list=[]
            aruco_list=detect_markers(gray_image, mtx, dist)
            
            array_with_aruco_marker_data=""
            if ids is None:
                array_with_aruco_marker_data="0:"
            else:
               array_with_aruco_marker_data="{}:".format(len(ids))         
               for j in range(len(ids)):
                   id1=ids[j]
                   corner=corners[j]
                     
                   array_with_aruco_marker_data="{}{}:{}:{}:{}:{}:{}:{}:{}:{}:".format(array_with_aruco_marker_data,id1[0],corner[0][0][0],corner[0][0][1],corner[0][1][0],corner[0][1][1],corner[0][2][0],corner[0][2][1],corner[0][3][0],corner[0][3][1])
           
            array_with_aruco_marker_data="{}{}:".format(array_with_aruco_marker_data,len(class_ids))
            for i in range(len(class_ids)):
                array_with_aruco_marker_data="{}{}:{}:{}:{}:{}:".format(array_with_aruco_marker_data,class_ids[i],boxes[i][0],boxes[i][1],boxes[i][2],boxes[i][3])
         
            send_detected_aruco_marker_data_to_aruco_node_in_ros.sendall(str(array_with_aruco_marker_data).encode("utf-8"))
            
            user_position_data=receive_user_position_data_from_aruco_node_in_ros.recv(1024)
            
            array_with_user_position_data="{}".format(user_position_data)
            send_user_position_data_to_hololens.sendall(str(array_with_user_position_data).encode("utf-8"))
            
            # Outline all of the markers detected in our image
            array_with_image_data = aruco.drawDetectedMarkers(array_with_image_data, corners, borderColor=(0, 0, 255))   
                    
            cv2.imshow('Photo Video Camera Stream', array_with_image_data)
            
            received_image_number=received_image_number+1

            if cv2.waitKey(1) & 0xFF == ord('q'):
                break
            
    except KeyboardInterrupt:
        pass

    receive_rgb_frame_from_hololens.close()
    cv2.destroyAllWindows()

def detect_markers(gray_image, camera_matrix, dist_coeff):
    markerLength = 0.14
    aruco_list = []
    aruco_dict = aruco.Dictionary_get(aruco.DICT_ARUCO_ORIGINAL)
    
    #Create default parameter: all specific options in the marker detection process, such as threshold, 
    #contour filtering, and default parameters 
    parameters = aruco.DetectorParameters_create()
    parameters.cornerRefinementMethod=aruco.CORNER_REFINE_NONE
    parameters.adaptiveThreshWinSizeMin=7
    parameters.adaptiveThreshWinSizeMax=23
    parameters.minMarkerPerimeterRate=0.1
    
    #Detect which markers are there. (original image, dictionary list,)
    #List of angles of the detected images (four corners arranged in the original order (clockwise from the upper left corner)), 
    #the list of all maker ids detected
    corners, ids, _ = aruco.detectMarkers(gray_image, aruco_dict, parameters = parameters)
    
    centrecoord=[]
    if ids is None:
        L=0
    else:
        L=len(ids)
    
             # Calculate the center point of the marker
    for i in range(0,L):
            x=int(corners[i][0][0][0]+corners[i][0][1][0]+corners[i][0][3][0]+corners[i][0][2][0])/4
            y=int(corners[i][0][0][1]+corners[i][0][1][1]+corners[i][0][3][1]+corners[i][0][2][1])/4
            p=(x,y)
            centrecoord.append(p)
    
    return aruco_list

        
if __name__ == "__main__":
    main(sys.argv[1:])
