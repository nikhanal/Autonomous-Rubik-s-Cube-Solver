import time
import os
from ultralytics import YOLO
import cv2
import kociemba
import socket
import numpy as np

host, port = "127.0.0.1", 25001

#Model path
model_path = os.path.join('.','detection_model', 'best.pt')

# Load YOLO model
model = YOLO(model_path)


# Open the webcam
  # Use 0 for the default webcam, change the index if you have multiple cameras

# Define color names based on class IDs
color_names = {
    0: 'Green',
    1: 'Red',
    2: 'White',
    3: 'Blue',
    4: 'Orange',
    5: 'Yellow',
    6: 'Cube'
    # Add more color names if needed
}
threshold = 0.7
cPress = 0

myObjects = []  # List to store detected objects
cubeData = {
    'color':"Cube",
    'x1':0,
    'y1':0,
    'x2':0,
    'y2':0
}


sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

connected = False
while not connected:
    try:
        sock.connect((host, port))
        connected = True
    except ConnectionRefusedError:
        print("Connection refused. Retrying...")



cubeState = {
'White':"",
'Red':"",
'Green':"",
'Yellow':"",
'Orange':"",
'Blue':""
}


currentFace = ""

displayText=""

def LaunchCamera(Auto):
    scanFail = False
    cPress = 0
    threshold = 0.5
    solvedString="UUUUUUUUURRRRRRRRRFFFFFFFFFDDDDDDDDDLLLLLLLLLBBBBBBBBB"
    myObjects = []  # List to store detected objects
    cubeData = {
        'color':"Cube",
        'x1':0,
        'y1':0,
        'x2':0,
        'y2':0
    }
    cubeState = {
    'White':"",
    'Red':"",
    'Green':"",
    'Yellow':"",
    'Orange':"",
    'Blue':""
    }
    cap = None

    # Check indices 1 to 10

    cap = cv2.VideoCapture(0)
    if Auto:
        cpressCounter = 0
        startTime = time.time()
    else:
        startTime = 0
    while True:
        ret, frame = cap.read()
        image = frame.copy()
        results = model(frame)[0]
        
        for result in results.boxes.data.tolist():
            x1,y1,x2,y2,score,class_id = result
            if score > 0.7:
                class_label = color_names.get(int(class_id), 'Unknown')
                if class_label != 'Cube':
                    continue
                cubeData = {
                    'color':"Cube",
                    'x1':0,
                    'y1':0,
                    'x2':0,
                    'y2':0
                }
                cubeData = {
                    'color': "Cube",
                    'x1': x1,
                    'y1': y1,
                    'x2': x2,
                    'y2': y2
                }

        # Annotate the image
        for result in results.boxes.data.tolist():
            x1, y1, x2, y2, score, class_id = result
            xCenter = int((x1+x2)/2)
            yCenter = int ((y1+y2)/2)
            if score > threshold:
                class_label = results.names[int(class_id)][0].upper()
                score_str = f"{score:.2f}"
                
                if class_label=="C":
                    width = int((x1+x2)/2)
                    width = int(width-0.1*width)
                    cv2.putText(frame, f"{class_label} {score_str}", (width, int(y1)),
                        cv2.FONT_HERSHEY_SIMPLEX, 0.6, (0, 0, 0), 3, cv2.LINE_AA)
                    cv2.putText(frame, f"{class_label} {score_str}", (width, int(y1)),
                        cv2.FONT_HERSHEY_SIMPLEX, 0.6, (255,255,255), 1, cv2.LINE_AA)
                    
                    
                else:
                    if (xCenter>cubeData['x1']-10) and (xCenter<cubeData['x2']+10) and (yCenter>cubeData['y1']-10) and (yCenter<cubeData['y2']+10):
                        cv2.putText(frame, f"{class_label}", (int((x1+x2)/2)-20, int((y1+y2)/2)),
                                    cv2.FONT_HERSHEY_SIMPLEX, 0.6, (0, 0, 0), 3, cv2.LINE_AA)
                        cv2.putText(frame, f"{score_str}", (int((x1+x2)/2)-20, int((y1+y2)/2)+20),
                                    cv2.FONT_HERSHEY_SIMPLEX, 0.6, (0, 0, 0), 3, cv2.LINE_AA)
                        cv2.putText(frame, f"{class_label}", (int((x1+x2)/2)-20, int((y1+y2)/2)),
                                    cv2.FONT_HERSHEY_SIMPLEX, 0.6, (255,255,255), 1, cv2.LINE_AA)
                        cv2.putText(frame, f"{score_str}", (int((x1+x2)/2)-20, int((y1+y2)/2)+20),
                                    cv2.FONT_HERSHEY_SIMPLEX, 0.6, (255,255,255), 1, cv2.LINE_AA)
            
        if cPress !=0:
            if len(currentFace) != 9:
                cv2.putText(frame, "Take Again!!!", (20,100), cv2.FONT_HERSHEY_SIMPLEX, 1, (0,0, 0), 5)
                cv2.putText(frame, "Take Again!!!", (20,100), cv2.FONT_HERSHEY_SIMPLEX, 1, (255, 255, 255), 3)
            else:
                cv2.putText(frame, currentFace, (20,100), cv2.FONT_HERSHEY_SIMPLEX, 1, (0,0,0), 5)
                cv2.putText(frame, currentFace, (20,100), cv2.FONT_HERSHEY_SIMPLEX, 1, (255, 255, 255), 3)
    
        else:
            if Auto==False:
                cv2.putText(frame, "Press c to read face", (20,100), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 0), 5)
                cv2.putText(frame, "Press c to read face", (20,100), cv2.FONT_HERSHEY_SIMPLEX, 1, (255, 255, 255), 3)

        y_value = 100
        for key,value in cubeState.items():
            if len(value) == 9:
                cv2.putText(frame, f"{key}", (500,y_value), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 0), 3)
                cv2.putText(frame, f"{key}", (500,y_value), cv2.FONT_HERSHEY_SIMPLEX, 1, (255,255,255), 1)
                y_value += 50
        
        
        if scanFail:
            cv2.putText(frame, "Scan Again", (240,450), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 0), 5)
            cv2.putText(frame, "Scan Again", (240,450), cv2.FONT_HERSHEY_SIMPLEX, 1, (255, 255, 255), 3)

        cv2.putText(frame,"Yolo Object Detection",(150,30),cv2.FONT_HERSHEY_SIMPLEX,1,(0,0,0),5)
        cv2.putText(frame,"Yolo Object Detection",(150,30),cv2.FONT_HERSHEY_SIMPLEX,1,(255,255,255),3)
        # Display the frame
        
        cv2.imshow('Real-time Object Detection', frame)

        # Check for 'c' key press to take a picture and analyze the color
        key = cv2.waitKey(1)
        if Auto:
            currentTime = time.time()
            elaspedTime = currentTime - startTime
        else:
            elaspedTime = 0
        if key == ord('c') or elaspedTime>=10:
            #cv2.imshow('Real-time Object Detection', frame)

            # Take a picture
            cPress = 1
            if Auto:
                startTime = time.time()
            scanFail = False
            #image = frame.copy()

            # Perform object detection
            results = model(image)[0]
            

            # Extract the color, x-coordinate, and y-coordinate from the detected objects
            for box in results.boxes.data.tolist():
                x1,y1,x2,y2,score,class_id = box
                if score>0.7:
                    class_label = color_names.get(int(class_id), 'Unknown')
                    if class_label != 'Cube':
                        continue
                    cubeData = {
                        'color':"Cube",
                        'x1':0,
                        'y1':0,
                        'x2':0,
                        'y2':0
                    }
                    cubeData = {
                        'color': "Cube",
                        'x1': x1,
                        'y1': y1,
                        'x2': x2,
                        'y2': y2
                    }
            for box in results.boxes.data.tolist():
                x1, y1, x2, y2, score, class_id = box
                xCenter = int((x1+x2)/2)
                yCenter = int ((y1+y2)/2)
                class_label = color_names.get(int(class_id), 'Unknown')

                # Skip processing if the class is 'Cube'
                if class_label == 'Cube':
                    continue
                else:
                    if (xCenter>cubeData['x1']) and (xCenter<cubeData['x2']) and (yCenter>cubeData['y1']) and (yCenter<cubeData['y2']):
                        myObjects.append({
                            'color': class_label[0],
                            'x_coord': xCenter,
                            'y_coord': yCenter,
                        })

  
            sorted_objects = sorted(myObjects, key=lambda x: x['y_coord'])

            # Divide the sorted list into groups of 3
            grouped_objects = [sorted_objects[i:i+3] for i in range(0, len(sorted_objects), 3)]

            # Print the result
            for group in grouped_objects:
                group.sort(key=lambda x: x['x_coord'])

            # Print the result
            currentFace = ""
            for group in grouped_objects:
                sorted_colors = [obj['color'] for obj in group]

                print(sorted_colors)
                for sorted_color in sorted_colors:
                    currentFace+=sorted_color  
                
            # Clear the list for the next capture
            if len(currentFace) == 9:
                if Auto:
                    cpressCounter = cpressCounter + 1
                if currentFace[4] =='G':
                    cubeState['Green'] = currentFace
                elif currentFace[4] =='B':
                    cubeState['Blue'] = currentFace
                elif currentFace[4] =='R':
                    cubeState['Red'] = currentFace
                elif currentFace[4] =='O':
                    cubeState['Orange'] = currentFace
                elif currentFace[4] =='W':
                    cubeState['White'] = currentFace
                elif currentFace[4] =='Y':
                    cubeState['Yellow'] = currentFace

                if Auto:
                    if cpressCounter <=3:
                        sock.sendall("qzf".encode("utf-8"))
                    elif cpressCounter == 4:
                        sock.sendall("qzRfr".encode("utf-8"))
                    elif cpressCounter == 5:
                        sock.sendall("qzffrr".encode("utf-8"))

            myObjects = []
            
            
        # Break the loop if 'q' key is pressed
        elif key == ord('q'):
            break

        
        
        if all(len(value) == 9 for value in cubeState.values()):
            print(cubeState)
            replacements = {
                'Y': 'D',
                'G': 'F',
                'O': 'L',
                'W': 'U'
            }


            for key, value in cubeState.items():
                # Perform replacements for each character in the value
                for old_char, new_char in replacements.items():
                    value = value.replace(old_char, new_char)
                # Update the value in the dictionary
                cubeState[key] = value

            cubeStateString = ""

            for value in cubeState.values():
                cubeStateString += value

            print (cubeState)
            print(cubeStateString)
                
            try:
                if Auto:
                    sock.sendall("qzrr".encode("utf-8"))
                unityData ="z"+(kociemba.solve(solvedString,cubeStateString))
                sock.sendall(unityData.encode("utf-8"))
                break
            except:
                cubeState = {
                'White':"",
                'Red':"",
                'Green':"",
                'Yellow':"",
                'Orange':"",
                'Blue':""
                }
                cPress = 0
                scanFail = True
                pass

            # Release the webcam and close all windows
    cap.release()
    cv2.destroyAllWindows()  
            
            
            
            
     
            
        

    
def GenerateKociembaSolution(unityCubeState):
    solvedString="UUUUUUUUURRRRRRRRRFFFFFFFFFDDDDDDDDDLLLLLLLLLBBBBBBBBB"
    unityData = "q"+(kociemba.solve(solvedString,unityCubeState))
    sock.sendall(unityData.encode("utf-8"))
    

while True:
    response = sock.recv(1024).decode("utf-8")
    print(response)
    if response == "Launch Camera":
        LaunchCamera(False)
    elif response == "Quit":
        sock.close()
        break
    elif response == "Launch Auto":
        LaunchCamera(True)
    elif response[0] == "G":
        GenerateKociembaSolution(response[1:])
    response = ""

