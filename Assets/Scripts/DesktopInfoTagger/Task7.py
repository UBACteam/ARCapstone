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
    tree = ET.parse(xmlfile)
    root = tree.getroot()

    listOfFile = []
    titleArr = []
    xArr = []
    yArr = []
    zArr = []

    for title in root.findall('./MarkerData/title'):
        titleArr.append(title.text)

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
   
    listOfFile.extend(titleArr)
    listOfFile.extend(xArr)
    listOfFile.extend(yArr)
    listOfFile.extend(zArr)

    length = len(titleArr)

    return listOfFile, length



    
    


#Step 2: Find point with respect to ref
def RespectToRef(List, length):
    i = 0
    n = length - 1 
    newxArr = []
    newyArr = []
    newzArr = []
    newList = []

    # x component
    while (i <= length-2):
        # Master List = [(title1, title2, titleRef), (x1, x2, xRef), (y1, y2, yRef), (z1, z2, zRef)]
        # x = oldx - refx
        # refx is last x comp
        # oldx starts at first x comp
        # newx = oldx - refx
        newx = List[length+i] - List[length+n]
        newxArr.append(newx)

        newy = List[2*length + i] - List[2*length + n]
        newyArr.append(newy)

        newz = List[3*length + i] - List[3*length + n]
        newzArr.append(newz)

        i += 1

    newList.extend(newxArr)
    newList.extend(newyArr)
    newList.extend(newzArr)

    return newList




#Step 3: Conver to Inches 
def ConvertToInches(List):
    length = len(List)
    i = 0
    # meters / 0.0254
    while (i < length):
        List[i] = List[i] / 0.0254
        i += 1
    return List


#Step 4: Make Z Coordinate Normal to Deck by switching Y/Z Values 
def NormalToDeck(List):
    length = len(List)
    #length of one component (x,y,or z)
    length2 = int (length / 3)
    i = 0
    
    while (i < length2):
        Temp = List[length2 + i]
        List[length2 + i] = List[2*length2 + i]
        List[2*length2 + i] = Temp
        i += 1

    return List


#Step 5: Transform
def Transform(List):
    length = len(List)
    length2 = int(length / 3)

    # The code below only works for this specific XML file
    # We need to make it universal
    # was going to do that after adding a 2nd ref point
    x1 = List[0]
    y1 = List[length2]
    x2 = List[1]
    y2 = List[length2+1]
   
    #get angle using vector 1
    angle = math.tan(y1/x1)
    
    #Angle matrix
    cos = math.cos(angle)
    sin = math.sin(angle)
    sin0 = -math.sin(angle)

    # cos, sin, -sin, cos
    # Form matrix
    T = np.array([[cos, sin], [sin0, cos]])

    # Form matrix
    Fem1 = np.array([[x1], [y1]])
    Fem2 = np.array([[x2], [y2]])

    #Matrix multiplication
    TransformPt1 = T.dot(Fem1)
    TransformPt2 = T.dot(Fem2)

    #Transpose them
    TransformPt1T = np.transpose(TransformPt1) #This step might not be needed
    TransformPt2T = np.transpose(TransformPt2)

    #Add z comp
    NewTransformPt1 = np.append(TransformPt1T, [List[2*length2]])
    NewTransformPt2 = np.append(TransformPt2T, [List[2*length2+1]])
 


    return NewTransformPt1, NewTransformPt2










def main():
    
    List, length = parseXML('measuretest2.xml')
    ToRef = RespectToRef(List, length)
    ToInches = ConvertToInches(ToRef)
    NormalZ = NormalToDeck(ToInches)
    TransformPt1, TransformPt2 = Transform(NormalZ)

    print(List)
    print(TransformPt1)
    print(TransformPt2)



if __name__ == "__main__":
    main()
    



