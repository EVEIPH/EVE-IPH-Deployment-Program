# EVE-IPH-Deployment-Program

This is the code for the program that builds the Database, imports images,  and deployes files for use with EVE IPH to the file server for user download and update (currently media fire) through the EVE-IPH-Updater program. These notes describe how to use the program and other functions for deploying the IPH program and updating for users.

I typically run this program in debug mode but it could be run in stand-alone if no changes to the DB or files are required. 

Steps to import an Static Data Export (SDE), build the new database, and export the files:
1 - Download the CCP SDE from: https://developers.eveonline.com/resource/resources 
    a - Extract the files to a common folder name that you will remember. I use the name of the *.zip file for the folder and save all .yaml, .sql, and other files in a separate folder in a main folder (e.g. ...\SDE Working\CCP SDE Version XX)
    b - Extract the *.bak file to the C drive (or wherever you can get to it easily)
2 - Download the Image Export Collection with the name "*_Types.zip". This is used to update program images.
    a - Extract the folder to a common location you can get to easily. I typically save this folder in the main folder as 1a (SDE Working).
3 - Restore the backup SDE file to MS SQL Server. I utilize the MS SQL Server Express edition. 
    a - Log into your local server, right-click 'Databases' and select 'Restore Database'
    b - Select the 'Device' Radio button and hit the "..." to open the file explorer
    c - Select your *.bak file from step 1b above and hit OK
    d - Rename the "Database:" field (typically named 'ebs_DATADUMP') to the name of the *.zip file. For example, if the *.zip file is named 'YC-118-2_1.0_116998.zip', change the database name to 'YC-118-2_1.0_116998'. NOTE: This name is important and will be the name of the database used in the update program to correctly import the SDE data as well as the folder to import yaml and other files from.
    f - Hit OK to restore the database.
4 - Open the EVE IPH Deployment Program (run in debug or run the exe)
5 - Select the 'File Path Settings' tab and update the following:
    a - Database Name - Set to the name used to restore the database to the MS Sql Server - Example: 'YC-118-2_1.0_116998'
    b - Image Version - Set this name to the name of the *_Types.zip file. For example, 'YC-118-1_1.0_Types.zip' would be 'YC-118-1_1.0'
    c - Version Number - This is the Major and Minor version of the program and important for building a binary program. 
    d - Media Fire Deployment Folder - This is the folder where updates to the Media Fire server from the desktop app are stored so users can update the program. For this app, Media Fire will automatically update the files when replaced and maintain the same link locations used in the updater program.
    e - Working Folder - This is the folder where the image files folder ('Types') is stored along with the folder for the database version we are working with (YC-118-1_1.0_116998). 
