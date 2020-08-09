# :pencil: (Use Case) Game definitions pipeline
---------

:pushpin: Overview
---------
Game definitions pipeline show case what is dealing with json definitions on google drive and then download into unity for next data processing.Here is a tutorial use case that I solve for my project and I would like to share it with you.

![Architecture](https://user-images.githubusercontent.com/14979589/89738801-36e90300-da84-11ea-8ccb-c5c4273725ac.png)

:bulb: Idea
---------
Pipeline which will be cover whole process from create definitions in google drive till I will able to process definitions into unity.

:white_check_mark: Goals
---------
* Google Drive sheet export to json
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
* Convert and export sheet to json
* Create definitions pack from json data
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
* Authentification by generated credential by google console
* Searching and listing between definitions
* Downloading files and dealing with folders

**Unity Editor Tool**<br>
(C# script)
* Creating a process for external call of GoogleDriveAPIDownloader
* Dealing with output data from stage before
* Convert data into usable structure and processing them

:page_facing_up: How to install
---------
* Download package
* Setup python enviroment variable as PYTHON_HOME
* Generate google console credential.json and put into plugin source
* Setup tool paths

:package: Package to download
---------
* Unity package
* Open source codes
 * ExportDefinition.json
 * GoogleDriveDownloader.py
 * DefinitionDownloaderEditor.cs
