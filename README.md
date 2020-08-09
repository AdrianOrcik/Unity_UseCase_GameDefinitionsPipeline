# :pencil: (Use Case) Game definitions pipeline
---------

:pushpin: Overview
---------
A showcase of game definitions pipeline with definitions on google drive and then download into unity for next data processing. Here is a tutorial use case that I solved for my project and I would like to share it with you.

![Architecture](https://user-images.githubusercontent.com/14979589/89738801-36e90300-da84-11ea-8ccb-c5c4273725ac.png)

:bulb: Idea
---------
Build a pipeline that will be cover whole process from creating definitions in google drive till I will able to process definitions into unity.

:white_check_mark: Goals
---------
* Google Drive sheet export to JSON
* Definitions version system with backups
* List definitions in unity
* Ability to download into unity

:rocket: Result
---------
Google Sheet Add-on<br>
![ExportAdd-on](https://user-images.githubusercontent.com/14979589/89738913-0190e500-da85-11ea-8a63-62151db6a106.png)

Unity Definition Downloader Editor<br>
![UnityEditor](https://user-images.githubusercontent.com/14979589/89739003-b3301600-da85-11ea-88d6-fe6ab3536d7d.png)

:pushpin: How these thigs works?
---------
**Google Drive Definitions Export** <br>
(Javascript Add-on)
* Convert and export sheet to JSON
* Create definitions pack from JSON data
* Create root sheet backup with version

**Definitions Structure**<br>
* RemoteDefinitions/
  * RootDefinitionSheet.sheet
  * DefintionV1 pack/
    * DefinitionV1/
      * Definition1.json
      * Definition2.json 
      * ....
    * DefinitionSheetbackup.sheet

**Google Drive API Downloader**<br>
(Python Script)
* Authentication by credential by google console
* Searching and listing between definitions
* Downloading files and dealing with folders

**Unity Editor Tool**<br>
(C# script)
* Creating a process for the external call of GoogleDriveAPIDownloader
* Dealing with output data from the google drive downloader
* Convert data into the usable structure and processing them

:page_facing_up: How to install
---------
* Download package
* Setup python environment variable as PYTHON_HOME
* Generate google console credential.json and put into plugin source
* Setup tool paths

:package: Package to download
---------
* Unity package
* Open source codes
  * ExportDefinition.gs
  * GoogleDriveDownloader.py
  * DefinitionDownloaderEditor.cs
