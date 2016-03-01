# EVE-IPH-Deployment-Program - User Instructions

This is the code for the program that builds the Database, imports images,  and deployes files for use with EVE IPH to the file server for user download and update (currently media fire) through the EVE-IPH-Updater program. These notes describe how to use the program and other functions for deploying the IPH program and updating for users.

I typically run this program in debug mode but it could be run in stand-alone if no changes to the DB or files are required. 

Steps to import an Static Data Export (SDE), build the new database, and export the files:
## Prepping the SQL Server Database
1. Download the CCP SDE from: https://developers.eveonline.com/resource/resources 
  1. Extract the files to a common folder name that you will remember. I use the name of the *.zip file for the folder and save all .yaml, .sql, and other files in a separate folder in a main folder (e.g. ...\SDE Working\CCP SDE Version XX)
  2. Extract the *.bak file to the C drive (or wherever you can get to it easily)
2. Download the Image Export Collection with the name "*_Types.zip". This is used to update program images.
  1. Extract the folder to a common location you can get to easily. I typically save this folder in the main folder as 1a (SDE Working).
3. Restore the backup SDE file to MS SQL Server. I utilize the MS SQL Server Express edition. 
  1. Log into your local server, right-click 'Databases' and select 'Restore Database'
  2. Select the 'Device' Radio button and hit the "..." to open the file explorer
  3. Select your *.bak file from step 1b above and hit OK
  4. Rename the "Database:" field (typically named 'ebs_DATADUMP') to the name of the *.zip file. For example, if the *.zip file is named 'YC-118-2_1.0_116998.zip', change the database name to 'YC-118-2_1.0_116998'. NOTE: This name is important and will be the name of the database used in the update program to correctly import the SDE data as well as the folder to import yaml and other files from.
  5. Hit OK to restore the database.
**NOTE: CCP has indicated that they will be migrating the entire SDE to YAML files in the coming months. There has been no date set but this program only does very simple YAML processing and will need to be updated to use the installed package YAML.net to process YAML files to continue working after this change.**
4. Open the EVE IPH Deployment Program (run in debug or run the exe)
5. Select the 'File Path Settings' tab and update the following:
  1. Database Name - Set to the name used to restore the database to the MS Sql Server - Example: 'YC-118-2_1.0_116998'
  2. Image Version - Set this name to the name of the *_Types.zip file. For example, 'YC-118-1_1.0_Types.zip' would be 'YC-118-1_1.0'
  3. Version Number - This is the Major and Minor version of the program and important for building a binary program. 
  4. Media Fire Deployment Folder - This is the folder where updates to the Media Fire server from the desktop app are stored so users can update the program. For this app, Media Fire will automatically update the files when replaced and maintain the same link locations used in the updater program.
  5. Working Folder - This is the folder where the image files folder ('Types') is stored along with the folder for the database version we are working with (YC-118-1_1.0_116998). 
  6. Root Debug Folder - This folder is where VB.net will run the debug executable, which is set in the EVE Isk Per Hour program properties -> Compile -> Build Output Path
  7. Save the settings.
  8. Select the 'DB Updater & Deployment' Tab
  9. If you have just restored a new DB file, you need to insert data from the yaml and UniverseDataDX.db file (sqlite). To do this, select the 'Update SQL Server DB' button and wait until it is complete. Note, some of the universe tables take a bit of time to import.

## Building the EVE IPH Database
1. If the SQL server database is updated, you can build the EVE IPH Database.
2. Hit the 'Build DB' button and wait until it completes
3. The resulting database will be stored in the Working Directory set in the program settings above.

## Updating the EVE IPH Images
1. Only after the final EVE IPH Database is built can you update the Images
2. Hit 'Image Copy' and the program will copy all the relevant images, zip them into a final file, and export the new images to the Root Directory set in the program settings above for use in debugging.

## Updating and Deploying EVE IPH files

The program allows easy deployment of the required files to update the program. Complete the following anytime you need to update a new file for user download. Note: If you delete and replace a file in the Media File folder, the links for downloading will change and require an update to the program, which you may not be able to push to users but instead require them to re-install. The following are details on updating the files through this process.

- The List shown is a current list of the file and file date for all the files in the Media Fire directory. Hitting 'Refresh List' will refresh any files updated.
- Selecting 'Update Files for Export' will copy the necessary files from the Working and Root Directories and build a new 'LatestVersionIPH.xml' file for updating the program. The 'Latest Version.xml' file is saved to the Root Directory and not the Media Fire directory.
- Selecting the 'Build Binary' button will package all the files into a zip file and deploy it to the Media Fire Directory for updating.
- After updating any files, wait until the Media Fire Destop app updates the files and then copy over the 'LatestVersionIPH.xml' file to the Media Fire Directory to upload the file users check for updates.
- For the 'Eve Isk per Hour 3.2.msi' file, I use a free version of Advanced Installer that is very minimal. This is because the VB.Net Express version does not make msi files. Feel free to use whatever will create a msi file without advertisements or malware (such as Adv. Installer or the built in installer with a full version of Visual Studio) to build the msi file, then save it in the Media Fire Directory for upload.


##Success!
