import numpy as np 
import math
import csv
import xml
import xml.dom.minidom
import xml.etree.ElementTree as ET
import string 
import pprint

#Step 1: Pull Info from XML File

def parseXML(xmlfile):
    #Finds root of XML
    tree = ET.parse(xmlfile)
    root = tree.getroot()

    # Initializations of lists
    listOfFile = []
    titleArr = []
    xArr = []
    yArr = []
    zArr = []
    Ref1 = []

    # Adds each title element to title list
    for title in root.findall('./MarkerData/title'):
        titleArr.append(title.text)

    # Adds each x,y,z element to corrosponding list
    for position in root.findall('./MarkerData/position'):
      
        for child in position:
            
            if child.tag == 'x':
                x = float(child.text)

            if child.tag == 'y':
                y = float(child.text)

            if child.tag == 'z':
                z = float(child.text)

        xArr.append(x)
        yArr.append(y)
        zArr.append(z) 
   # Combines title, x, y, and z lists all into a single list
    listOfFile.extend(titleArr)
    listOfFile.extend(xArr)
    listOfFile.extend(yArr)
    listOfFile.extend(zArr)
    # Master List = [(title1, title2, titleRef1, titleRef2), (x1, x2, xRef1, xRef2), (y1, y2, yRef, yRef2), (z1, z2, zRef, zRef2)]

    #Finds length
    Partiallength = len(titleArr)       # Partial length refers to length of one type of element, e.g. how many points there are
    lengthOfList = len(listOfFile)

    # x,y,z of Ref1
    Ref1.append(listOfFile[2*Partiallength - 2])
    Ref1.append(listOfFile[3*Partiallength - 2])
    Ref1.append(listOfFile[lengthOfList - 2])
    
    print("Format is [(title1, title2, titleRef1, titleRef2), (x1, x2, xRef1, xRef2), (y1, y2, yRef, yRef2), (z1, z2, zRef, zRef2)]")
    print("Data from XML file: " +str(listOfFile))

    return listOfFile, Partiallength, Ref1



    
    


#Step 2: Find point with respect to ref1
def RespectToRef(List, Partiallength):
    # For better readability
    length = Partiallength
    
   
    # Assume ref1 point is 2nd last from end of list
    lengthOfList = len(List)
    xRef1 = List[2*length-2]
    yRef1 = List[3*length-2]
    zRef1 = List[4*length-2]
    
    i = 0

    # Initializations of lists
    # w.r.t Reference point 1
    newRef2Arr = [] 
    newxArr = []
    newyArr = []
    newzArr = []
    newList = []

    # Finds each point w.r.t Ref1 (not including ref2)
    while (i <= length-3):
        # Master List = [(title1, title2, titleRef1, titleRef2), (x1, x2, xRef1, xRef2), (y1, y2, yRef, yRef2), (z1, z2, zRef, zRef2)]
        # x = oldx - refx
        # y = oldy - refy
        # refx is last x comp
        # oldx starts at first x comp
        # newx = oldx - refx
        newx = List[length+i] - xRef1
        newxArr.append(newx)                # x component

        newy = List[2*length + i] - yRef1
        newyArr.append(newy)                # y component

        newz = List[3*length + i] - zRef1
        newzArr.append(newz)                # z component

        i += 1

    # Ref2 with repsect to Ref1
    xRef2 = List[2*length - 1] - xRef1
    yRef2 = List[3*length - 1] - yRef1
    zRef2 = List[4*length - 1] - zRef1
    newRef2Arr.append(xRef2)
    newRef2Arr.append(yRef2)
    newRef2Arr.append(zRef2)

    # Adds new x,y,z coords from each point
    newList.extend(newxArr)
    newList.extend(newyArr)
    newList.extend(newzArr)

    # New list includes marker points x,y,z. No titles, no references
    print("\nFormat is [x1, x2, x3, ... xn, y1, y2, y3, ... yn, z1, z2, z3, .... zn]")
    print("Repsect to reference point: " +str(newList))
    return newList, newRef2Arr




#Step 3: Conver to Inches 
def ConvertToInches(List, Ref2):
    # Ref2 conversion
    Ref2[0] = Ref2[0] / 0.0254
    Ref2[1] = Ref2[1] / 0.0254
    Ref2[2] = Ref2[2] / 0.0254

    # Marker points conversion
    length = len(List)
    i = 0
    # meters / 0.0254
    while (i < length):
        List[i] = List[i] / 0.0254
        i += 1

    print("Converted to inches: " +str(List))
    return List, Ref2


#Step 4: Make Z Coordinate Normal to Deck by switching Y/Z Values 
def NormalToDeck(List, Ref2):
    # Ref2 Conversion
    Temp = Ref2[1]
    Ref2[1] = Ref2[2]
    Ref2[2] = Temp

    
    length = len(List)
    #length of one points coords
    partialLength = int (length / 3)
    i = 0
    
    # Marker points conversion
    while (i < partialLength):
        Temp = List[partialLength + i]
        List[partialLength + i] = List[2*partialLength + i]
        List[2*partialLength + i] = Temp
        i += 1
    
    print("Normalized to deck: " +str(List))
    return List, Ref2


# Step 5: Transform
def Transform1(List, Ref2):
    # Initialize
    length = len(List)
    partialLength = int(length / 3)
    i = 0
    Point = []
    ListOfPoints = []

    #get angle using reference point 2
    # Angle = tan(y/x)
    # y = ref pt2 y
    # x = ref pt2 x
    angle = math.tan(Ref2[1] / Ref2[0])
    
    #Angle matrix
    cos = math.cos(angle)
    sin = math.sin(angle)
    sin0 = -math.sin(angle)

    # Form matrix
    T = np.array([[cos, sin], [sin0, cos]])

    # Marker Points Transformation
    while (i < partialLength):
        x = List[i]
        y = List[partialLength + i]

        Fem = np.array([x, y])
        FemT = np.transpose(Fem)
        TransformPt = T.dot(FemT)
        NewTransformPt = np.append(TransformPt, [List[2*partialLength + i]])
        Point = NewTransformPt.tolist()
        ListOfPoints.append(Point)
        i += 1
    print("Transformation Points: " +str(ListOfPoints))

    return ListOfPoints
    

# Step 6: Translate Ref1 to desired location
def Translate(List, Ref1):
    # Initialization
    lengthOfList = len(List)
    i = 0
    WorldCoordinates = []
    Ref1Translate = []

    # Get world coords of Ref1 from user
    print("\nReference Point = " +str(Ref1))
    print("Enter World coordinates of Reference Point")
    Ref1x = input("x-coordinate: ")
    Ref1y = input("y-coordinate: ")
    Ref1z = input("z-coordinate: ")
    Ref1x = float(Ref1x)
    Ref1y = float(Ref1y)
    Ref1z = float(Ref1z)

    # Convert each point to World Coords
    while (i < lengthOfList):
        Point = List[i]
        
        Translatex = Point[0] + Ref1x
        WorldCoordinates.append(Translatex)

        Translatey = Point[1] + Ref1y
        WorldCoordinates.append(Translatey)

        Translatez = Point[2] + Ref1z
        WorldCoordinates.append(Translatez)
        i += 1

    print("\nFormat is [x1, y1, z1, x2, y2, z2, ... xn, yn, zn]")
    print("World Coordinates: " +str(WorldCoordinates))
    return WorldCoordinates


    

# -----------------WIP----------------------
# Step 7: Export to XML
def Export(List):
    data = ET.Element("ArrayOfMarkerData")
    Markers = ET.SubElement(data, "MarkerData")
    Position = ET.SubElement(Markers, "Position")
    i = 0
    lengthOfList = len(List)

    while (i < lengthOfList):
        x = List[i]
        y = List[i+1]
        z = List[i+2]

        
        i += 3

    mydata = ET.tostring(data)
    myfile = open("WorldCoordinates.xml", "w")
    myfile.write(mydata)











def main():
    # Assumes XML format is pt1, pt2, pt3 ... ptn, ref1, ref2
    Ref1 = []
    Ref2 = []

    #Grab data from XML
    List, length, Ref1 = parseXML('measuretest2.xml')

    #Repsect to ref. point
    ToRef, Ref2 = RespectToRef(List, length)

    #Convert to inches
    ToInches, Ref2Inches = ConvertToInches(ToRef, Ref2)

    #Normalize Z to Y
    NormalZ, Ref2NormalZ = NormalToDeck(ToInches, Ref2Inches)

    #Transform Points
    TransformPts = Transform1(NormalZ, Ref2NormalZ)

    #Translate Ref1 to Ship coords.
    ShipCoordinates = Translate(TransformPts, Ref1)

    #Export to XML (WIP)
    #Export(ShipCoordinates)
    



if __name__ == "__main__":
    main()
    



