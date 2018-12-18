# -*- coding: utf-8 -*-
"""
Created on Sat Dec 15 20:41:32 2018

@author: Gueguet
"""



import numpy as np
import cv2
import socket

cap = cv2.VideoCapture(0)

# socket configuration
UDP_IP = "127.0.0.1"
UDP_PORT = 5065

print ("UDP target IP:", UDP_IP)
print ("UDP target port:", UDP_PORT)

sock = socket.socket(socket.AF_INET, # Internet
                     socket.SOCK_DGRAM) # UDP



# define range of orange color in HSV
# change these values if you want to track another object
lower_orange = np.array([15,100,120])
upper_orange = np.array([35,255,255])

# Create empty points array
points = []

# Get default camera window size
ret, frame = cap.read()
Height, Width = frame.shape[:2]

frame_count = 0

while True:
    ret, frame = cap.read()
        
    hsv_img = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)

    # Threshold the HSV image to get only orange colors
    mask = cv2.inRange(hsv_img, lower_orange, upper_orange)
    #mask = cv2.morphologyEx(mask, cv2.MORPH_OPEN, kernel)
    
    # if you wanna see the result of the mask --> detection will be 
    # effective if you just see your tracked object
    #cv2.imshow("Mask", mask)
    
    # Find contours (OpenCV 3)
    _, contours, _ = cv2.findContours(mask.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    # uncomment the following line if you use OpenCV 2
    # contours, _ = cv2.findContours(mask.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    
    # store centroid center of mass
    center = int(Height/2), int(Width/2)
    
    radius = 0
    
    if len(contours) > 0:
        
        # get the largest contour and its center 
        c = max(contours, key=cv2.contourArea)
        (x, y), radius = cv2.minEnclosingCircle(c)
        
        #♀ extract moments of the circle
        M = cv2.moments(c)
        # extract the moment values and compute the center of mass
        try:
            center = (int(M["m10"] / M["m00"]), int(M["m01"] / M["m00"]))

        except:
            center =   int(Height/2), int(Width/2)

        # allow only countors that have a larger than 10 pixel radius
        if radius > 10:
            
            # Draw circle and leave the last center creating a trail
            cv2.circle(frame, (int(x), int(y)), int(radius),(10, 0, 0), 2)
            cv2.circle(frame, center, 10, (100, 0, 0), -1)
            
    # add center to the array
    points.append(center)
        
    
    # loop over the set of tracked points
    if radius > 10:
        for i in range(1, len(points)):
            try:
                cv2.line(frame, points[i - 1], points[i], (200, 0, 0), 3)
            except:
                pass
            
        # Make frame count zero
        frame_count = 0
    else:
        # Count frames 
        frame_count += 1
        
        # delete the trail after 10 frame without detection
        if frame_count == 10:
            points = []
            frame_count = 0
            
            
    # flip and display our object tracker
    frame = cv2.flip(frame, 1)
    # you can uncomment the following line if your GUI lib is correctly install
    #cv2.imshow("Object Tracker", frame)

    # send the data            
    sock.sendto(str(center).encode() , (UDP_IP, UDP_PORT))
    
    # press enter to close the camera rendering window
    # you can uncomment the following line if your GUI lib is correctly install
    #if cv2.waitKey(1) == 13: 
        #break
    
    
cap.release()
cv2.destroyAllWindows()