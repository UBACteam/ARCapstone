import numpy as np 
import csv
import xml
from xml.dom import minidom as md
import xml.etree.ElementTree as ET
import string 


#Step 1: Pull Info from XML File


#Test 1 with minidom: Problem: It gets all of the X's, not just the position

#mydoc = md.parse('measuretest.xml')           #set up file parsing object 
#xitems = mydoc.getelementsbytagname('x')      #parse file by positions
#yitems = mydoc.getelementsbytagname('y')      #parse file by positions
#zitems = mydoc.getelementsbytagname('z')      #parse file by positions

##it's loading all x into the list, not just position. need to figure out how to intelligently only load position into list. 

##print specific item attributes
#print('position 1 name attribute: ')
#print(items[0].attributes['name'].value)

#print('printing entire list: ')
#print(xitems)
#for elem in range(0, len(xitems)):
#    print('\n x[{0}] data:'.format(elem))
#    print(xitems[elem].firstchild.data)
#    print(xitems[elem].childnodes[0].data)

#print('printing entire list: ')
#print(yitems)
#for elem in range(0, len(yitems)):
#    print('\n y[{0}] data:'.format(elem))
#    print(yitems[elem].firstchild.data)
#    print(yitems[elem].childnodes[0].data)

#print('printing entire list: ')
#print(zitems)
#for elem in range(0, len(xitems)):
#    print('\n z[{0}] data:'.format(elem))
#    print(zitems[elem].firstchild.data)
#    print(zitems[elem].childnodes[0].data)
    

#Option 2 with etree
def parseXML(xmlfile):
    #create element tree object
    tree = ET.parse(xmlfile)

    #Get Root Element
    root = tree.getroot()

    #create empty list for position items

    positionItems = []

    count = -1
    #iterate position items #See if I can grab just x or just y or just z 
    for position in root.findall('./MarkerData/position'):
        
        #empty x dictionary
        x = {}

        #empty y dictionary
        y = {}

        #empty z dictionary
        z = {}
        count  = count + 1
        print('count = {0}'.format(count))

        for child in position:
            
            if child.tag == 'x':
                key = (str(child.tag)+ str(count))
                x[key] = child.text
                

            elif child.tag == 'y':
                key = (str(child.tag)+ str(count))
                y[key] = child.text
                

            else:
                key = (str(child.tag)+ str(count))
                z[key] = child.text
                
                

        positionItems.append(x)
        positionItems.append(y)
        positionItems.append(z)
 

    return positionItems


#Step 2: Pull items into Array 

#Step 3: Find marker points with respect to origin reference point (x0, y0) (n = arbitrary marker point number)

"""
x1’ = (x1 - x0)
y1’ = (y1- y0)
xn’ = (xn - x0)
yn’ = (yn - y0)
x0 = 0
y0 = 0
"""

#Step 4: Convert points to inches: inches = meter/0.0254

#Step 5: Make Z Coordinate Normal to Deck by switching Y/Z Values 

#Step 6: Export to XML?
def main():
    print("Printing whole list")
    print(parseXML('measuretest.xml'))


if __name__ == "__main__":
    main()