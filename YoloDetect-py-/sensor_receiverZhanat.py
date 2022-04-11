#!/usr/bin/env python3
from __future__ import print_function


"""
 Copyright (c) Microsoft. All rights reserved.

 This code is licensed under the MIT License (MIT).
 THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
 ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
 IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
 PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
"""

""" Sample code to access HoloLens Research mode sensor stream """
# pylint: disable=C0103


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
import os, shutil
import os
import glob

PROCESS = True

# Constant parameters used in Aruco methods
ARUCO_PARAMETERS = aruco.DetectorParameters_create()
ARUCO_DICT = aruco.Dictionary_get(aruco.DICT_ARUCO_ORIGINAL)
# Definitions

# Protocol Header Format
# Cookie VersionMajor VersionMinor FrameType Timestamp ImageWidth
# ImageHeight PixelStride RowStride
SENSOR_STREAM_HEADER_FORMAT = "@IBBHqIIII"

SENSOR_FRAME_STREAM_HEADER = namedtuple(
    'SensorFrameStreamHeader',
    'Cookie VersionMajor VersionMinor FrameType Timestamp ImageWidth ImageHeight PixelStride RowStride'
)

# Each port corresponds to a single stream type
# Port for obtaining Photo Video Camera stream
PV_STREAM_PORT = 23940
HL2_BB_PORT=9090
HL2_BBZ_PORT=9097
# HL2_BBZM_PORT=9098
ROSCPP_PORT=10105
ROSCPP_PORTZ=10112

# Initializing video writer
fps=2
size=(896,504)
videoWriter = cv2.VideoWriter('MyOutput.avi', cv2.VideoWriter_fourcc('I','4','2','0'), fps, size)

# Create grid board object we're using in our stream
board = aruco.GridBoard_create(
        markersX=2,
        markersY=2,
        markerLength=0.09,
        markerSeparation=0.01,
        dictionary=ARUCO_DICT)
# Create vectors we'll be using for rotations and translations for postures
rvecs, tvecs = None, None

def main(argv):

    """Receiver main"""
    parser = argparse.ArgumentParser()
    required_named_group = parser.add_argument_group('named arguments')

    required_named_group.add_argument("-a", "--host",
                                      help="Host address to connect", required=True)
    args = parser.parse_args(argv)



    # Create a TCP Stream socket
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s2 = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s2Z = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    # s2ZM = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s3 = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    
    soc=socket.socket(socket.AF_INET,socket.SOCK_STREAM)
        
    # Try connecting to the address
    s.connect((args.host, PV_STREAM_PORT))
    s3.connect(('',ROSCPP_PORT))
    s2.connect((args.host,HL2_BB_PORT))
    s2Z.connect((args.host,HL2_BBZ_PORT))
    # s2ZM.connect((args.host,HL2_BBZM_PORT))
    
     
         #connect to the server 
    
    
    soc.connect(('',ROSCPP_PORTZ))    
    #deletes content of frames folder
    deleteContent()
    
    #creating files and writing headers
    filechik = open("Totaltime.txt","w")
    #filechik.write("Total Time"+"\n")
    #file1 = open("TimeForReceivingRGBframe.txt","w")
    #file1.write("Time for receiving a RGB frame"+"\n")
    #file2 = open("TimeForObjectRecognition.txt","w")
    #file2.write("Time for object recognition"+"\n")
    #filew = open("TimeForSendingDataToHL2.txt","w")
    #filew.write("Time for sending data to HL2"+"\n")
    #file3 = open("TimeForArucoMarkersDetection.txt","w")
    #file3.write("Time for Aruco markers detection in a frame"+"\n")
    #file4 = open("TimeForSendingAruco&BboxDataToROS.txt","w")
    #file4.write("Time for sending Aruco and Bbox data to ROS"+"\n")

   
    # Load Yolo
    net = cv2.dnn.readNet("yolov3-tiny.weights", "yolov3-tiny.cfg")
    classes = []
    with open("coco.names", "r") as f:
        classes = [line.strip() for line in f.readlines()]

    layer_names = net.getLayerNames()
   
    output_layers = [layer_names[i[0] - 1] for i in net.getUnconnectedOutLayers()]

    colors = np.random.uniform(0, 255, size=(len(classes), 3))
    
    count=0
    countZ1 = 5
    countZ2 = 7
    # Try receive data
    try:
        quit = False
        while not quit:
            tic= datetime.now()
            reply = s.recv(struct.calcsize(SENSOR_STREAM_HEADER_FORMAT))
            
            
            

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
                image_data_chunk = s.recv(remaining_bytes)

                if not image_data_chunk:
                    print('ERROR: Failed to receive image data')
                    sys.exit()
                image_data += image_data_chunk

            image_array = np.frombuffer(image_data, dtype=np.uint8).reshape((header.ImageHeight,
                                        header.ImageWidth, header.PixelStride))
            if PROCESS:
                # process image
                image_array = cv2.cvtColor(image_array,cv2.cv2.COLOR_RGBA2RGB)
                raw_image = cv2.cvtColor(image_array,cv2.cv2.COLOR_RGBA2RGB)
                gray = cv2.cvtColor(image_array, cv2.COLOR_BGR2GRAY)

            height, width, channels = image_array.shape
            #a=datetime.now() - tic
            #file1.writelines(str(a.seconds*1000000+a.microseconds)+"\n") 
            #print("Time for receiving a RGB frame:"+str(a))
            
            #tica=datetime.now()
            blob = cv2.dnn.blobFromImage(image_array, 0.00392, (416, 416), (0, 0, 0), True, crop=False)
            net.setInput(blob)
            outs = net.forward(output_layers)
            
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
            #b=datetime.now() - tica
            #print("Time for object recognition:"+str(b))
            #file2.writelines(str(b.seconds+b.microseconds)+"\n")
            #ticw=datetime.now()
            array="{}:".format(count)
            
            if len(boxes)==0:
                array="{}00:0".format(array)
                s2.sendall(array.encode("utf-8"))
                #s2Z.sendall(array.encode("utf-8"))
            else:
                for i in range(len(boxes)):
                    if i in indexes:
                        x, y, w, h = boxes[i]
                        label = str(classes[class_ids[i]])
                        color = colors[i]
                        cv2.rectangle(image_array, (x, y), (x + w, y + h), color, 2)
                        cv2.putText(image_array, label, (x, y + 30), font, 3, color, 3)
                        
                        array="{}{}:{}:{}:{}:{}:{}".format(array,class_ids[i],x*416/900,y*416/510,h*416/510,w*416/900,confidences[i])
                        #arrayZ="1:2:3:4:5"
                        if(i!=(len(boxes)-1)):
                            array="{}:".format(array)
                            
                        
            s2.sendall(str(array).encode("utf-8"))
            #s2Z.sendall(str(arrayZ).encode("utf-8"))
            
            #w=datetime.now() - ticw
            #print("Time for sending data to HL2:"+str(w))
            #filew.writelines(str(w.seconds+w.microseconds)+"\n")
            #ticb=datetime.now()
            
            # Detect Aruco markers
            ARUCO_PARAMETERS.cornerRefinementMethod=aruco.CORNER_REFINE_NONE
            ARUCO_PARAMETERS.adaptiveThreshWinSizeMin=7
            ARUCO_PARAMETERS.adaptiveThreshWinSizeMax=23
            ARUCO_PARAMETERS.minMarkerPerimeterRate=0.1
            corners, ids, rejectedImgPoints = aruco.detectMarkers(gray, ARUCO_DICT, parameters=ARUCO_PARAMETERS)
            
            #c=datetime.now() - ticb
            #print("Time for Aruco markers detection in a frame:"+str(c))
            #file3.writelines(str(c.seconds+c.microseconds)+"\n")
            
            size_of_marker  = 0.14
            dist = np.array([[-4.84508353e-02],  [2.79630248e-01], [6.80014833e-03],  [8.17392130e-04], [-9.64730774e-01]])
            mtx = np.array([[666.52349183,   0,         441.04189665],
            [  0,         659.37164362, 236.29693346],
            [  0,           0,           1        ]])
            
            aruco_list=[]
            aruco_list=detect_markers(gray, mtx, dist)
            #for i in aruco_list:
            #    image_array = drawCube(image_array, aruco_list, i[0], mtx, dist)
            ticc=datetime.now()
            array2=""
            if ids is None:
                array2="0:"
                arrayZM="0"
            else:
               array2="{}:".format(len(ids))  
               arrayZM="{}".format(len(ids))          
               for j in range(len(ids)):
                   id1=ids[j]
                   corner=corners[j]
                     
                   array2="{}{}:{}:{}:{}:{}:{}:{}:{}:{}:".format(array2,id1[0],corner[0][0][0],corner[0][0][1],corner[0][1][0],corner[0][1][1],corner[0][2][0],corner[0][2][1],corner[0][3][0],corner[0][3][1])
                   arrayZM="{}:{}".format(arrayZM, id1[0]) 
           
            array2="{}{}:".format(array2,len(class_ids))
            for i in range(len(class_ids)):
                array2="{}{}:{}:{}:{}:{}:".format(array2,class_ids[i],boxes[i][0],boxes[i][1],boxes[i][2],boxes[i][3])
         
            s3.sendall(str(array2).encode("utf-8"))
            
         
            buf=soc.recv(1024)
            #Print message
            #print(buf)
            
            arrayZ="{}".format(buf)
            s2Z.sendall(str(arrayZ).encode("utf-8"))
            
            # s2ZM.sendall(str(arrayZM).encode("utf-8"))
            
            #zhcom print("have sent the signal to HoloLens")
            
            d=datetime.now() - ticc
            print("Time for sending Aruco and Bbox data to ROS:"+str(d))
            #file4.writelines(str(d.seconds+d.microseconds)+"\n")
            
            # Outline all of the markers detected in our image
            image_array = aruco.drawDetectedMarkers(image_array, corners, borderColor=(0, 0, 255))   
                    
            cv2.imshow('Photo Video Camera Stream', image_array)
            cv2.imwrite("frames/frames%d.jpg"%count, raw_image)
            count=count+1

            videoWriter.write(image_array)
            ff=datetime.now()-tic
            filechik.writelines(str(ff.seconds*1000000+ff.microseconds)+"\n")
            

            if cv2.waitKey(1) & 0xFF == ord('q'):
                break
            
    except KeyboardInterrupt:
        pass

    s.close()
    cv2.destroyAllWindows()

def detect_markers(gray, camera_matrix, dist_coeff):
    #zhcom print("Hello")
    markerLength = 0.14
    aruco_list = []
    ######################## INSERT CODE HERE ########################
    #gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    # Select a predefined dictionary in the aruco module to create a dictionary object
             # This dictionary is composed of 250 markers, each of which is 5*5bits in size.
    aruco_dict = aruco.Dictionary_get(aruco.DICT_ARUCO_ORIGINAL)
             #Create default parameter: all specific options in the marker detection process, such as threshold, contour filtering, and default parameters for Birat extraction
    parameters = aruco.DetectorParameters_create()
    parameters.cornerRefinementMethod=aruco.CORNER_REFINE_NONE
    parameters.adaptiveThreshWinSizeMin=7
    parameters.adaptiveThreshWinSizeMax=23
    parameters.minMarkerPerimeterRate=0.1
             #Detect which markers are there. (original image, dictionary list,)
             #List of angles of the detected images (four corners arranged in the original order (clockwise from the upper left corner)), the list of all maker ids detected
    
    corners, ids, _ = aruco.detectMarkers(gray, aruco_dict, parameters = parameters)
    #zhcom print(corners)
    #zhcom print("\n")
    #zhcom print(ids)
             # rvecs and tvecs are the rotation and translation vectors for each marker angle. Camera_matrix and dist_coeff are camera calibration parameters that need to be solved
    #rvec, tvec,_= aruco.estimatePoseSingleMarkers(corners, markerLength, camera_matrix, dist_coeff)
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
    #for i in range(0,L):
    #        p=(ids[i][0],centrecoord[i],rvec[i],tvec[i])
            
    #        aruco_list.append(p)
    ##################################################################
    
    return aruco_list

def drawCube(img, ar_list, ar_id, camera_matrix, dist_coeff):
    for x in ar_list:
            if ar_id == x[0]:
                    rvec, tvec = x[2], x[3]
    markerLength = 0.14
    m = markerLength/2
    ######################## INSERT CODE HERE ########################
    pts = np.float32([[-m,m,m], [-m,-m,m], [m,-m,m], [m,m,m],[-m,m,0], [-m,-m,0], [m,-m,0], [m,m,0]])
    imgpts, _ = cv2.projectPoints(pts, rvec, tvec, camera_matrix, dist_coeff)
    imgpts = np.int32(imgpts).reshape(-1,2)
    img = cv2.drawContours(img, [imgpts[:4]],-1,(0,0,255),4)
    for i,j in zip(range(4),range(4,8)): img = cv2.line(img, tuple(imgpts[i]), tuple(imgpts[j]),(0,0,255),4);
    img = cv2.drawContours(img, [imgpts[4:]],-1,(0,0,255),4)
    ##################################################################
    return img

def deleteContent():
    files = glob.glob('frames/*')
    for f in files:
        os.remove(f)
        
if __name__ == "__main__":
    main(sys.argv[1:])
