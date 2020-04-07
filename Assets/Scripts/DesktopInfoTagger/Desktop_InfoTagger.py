import numpy as np 
import math
import csv
import xml
from xml.dom import minidom as md
import xml.etree.ElementTree as ET
import string 
import pprint

#Step 1: Pull Info from XML File




#Ref: https://stackabuse.com/reading-and-writing-xml-files-in-python/   

#Option 2 with etree
#Ref: https://www.geeksforgeeks.org/xml-parsing-python/
#Ref: https://docs.python.org/2/library/xml.etree.elementtree.html
def parseXML(xmlfile):
    #create element tree object
    tree = ET.parse(xmlfile)

    #Get Root Element
    root = tree.getroot()

    #create empty list for position items

    #positionItems = []

    #create empyt dictionary for position items
    positionItems = {}

    count = -1
    #iterate position items #See if I can grab just x or just y or just z 
    for position in root.findall('./MarkerData/position'):
        
        #empty x dictionary
        x = {}          #Unused if using positionItems

        #empty y dictionary
        y = {}          #Unused if using positionItems

        #empty z dictionary
        z = {}          #Unused if using positionItems
        count  = count + 1
        #print('count = {0}'.format(count))              #Debug print to ensure count was working 

        for child in position:
            
            if child.tag == 'x':
                key = (str(child.tag)+ str(count))
                positionItems[key] = float(child.text)
                

            elif child.tag == 'y':
                key = (str(child.tag)+ str(count))
                positionItems[key] = float(child.text)
                

            else:
                key = (str(child.tag)+ str(count))
                positionItems[key] = float(child.text)
                
                

       # positionItems.append(x)
        #positionItems.append(y)
        #positionItems.append(z)
 
    #print('x[x0] + x[x1] = {0}'.format(positionItems[0]['x0'] + positionItems[3]['x1']))        #Test to see if the numbers are workable. It looks like they're all going into separate dictionaries though so it's a list of like 
    print('positionItems[x0] + positionItems[x1] = {0}'.format((positionItems['x0'] + positionItems['x1'])))                                                                                     #30 dictionaries which each hold 3 values (x, y, z)
    positionItems['ref'] = 0.0      #hard code reference point to 0. 
                                    #Can be changed later`
    return positionItems

#This is current setup of the positionItems dictionary. One dictionary with multiple entries

#positionItems[x0] + positionItems[x1] = 0.47828006700000003
#{'x0': 0.172116011,
# 'x1': 0.306164056,
# 'x10': 0.6675978,
# 'x11': -0.514232934,
# 'x2': -0.9749752,
# 'x3': -1.06811023,
# 'x4': -1.16732335,
# 'x5': 0.0318306834,
# 'x6': -0.260851145,
# 'x7': -1.77072668,
# 'x8': -1.34240985,
# 'x9': -1.19217134,
# 'y0': -0.8078765,
# 'y1': -0.8078765,
# 'y10': -1.53544211,
# 'y11': 0.967834,
# 'y2': -0.8078765,
# 'y3': -0.8078765,
# 'y4': -0.735115767,
# 'y5': -0.9416122,
# 'y6': -0.7500954,
# 'y7': -0.781697035,
# 'y8': -0.8078765,
# 'y9': -0.881747842,
# 'z0': -0.646990061,
# 'z1': -1.48822832,
# 'z10': -1.15751064,
# 'z11': 0.50214386,
# 'z2': -0.772379637,
# 'z3': -0.45301944,
# 'z4': 0.373182356,
# 'z5': 0.6804357,
# 'z6': 1.07569611,
# 'z7': 0.85308516,
# 'z8': -1.73690677,
# 'z9': -0.01901257}


#Step 2: Pull items into Array 
#Ref: https://www.geeksforgeeks.org/numpy-mathematical-function/

"""
Arrays should look like:


T: 
[   Cos(Theta)          Sin(Theta)              ]
[   -Sin(Theta)         Cos(Theta               ]

fFEM
[   Fx          Fz          ] (Transposed)
[                           ]



"""

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
    pprint.pprint(parseXML('measuretest.xml'))


if __name__ == "__main__":
    main()


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