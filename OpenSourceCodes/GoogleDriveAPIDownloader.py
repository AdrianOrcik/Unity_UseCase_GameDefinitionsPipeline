from __future__ import print_function
import pickle
import os.path
import os
import errno
import io
import sys
from googleapiclient.discovery import build
from google_auth_oauthlib.flow import InstalledAppFlow
from google.auth.transport.requests import Request
from googleapiclient.http import MediaFileUpload, MediaIoBaseDownload
from apiclient import errors, http
from google_auth import auth

# If modifying these scopes, delete the file token.pickle.
SCOPES = ['https://www.googleapis.com/auth/drive']
CREDENTIAL_PATH = 'credentials.json'

FOLDER_MIME_TYPE = "application/vnd.google-apps.folder"
JSON_MIME_TYPE = "application/json"

AuthInstance = auth(SCOPES, CREDENTIAL_PATH)
Credential = AuthInstance.getCredential()
Service = build('drive', 'v3', credentials=Credential)

class DefinitionDownloader():
    def getRootFolderId(self, rootFolderName):
        query = "name contains '{0}'".format(rootFolderName)
        result = Service.files().list(
        pageSize=1,fields="nextPageToken, files(id,name, kind, mimeType)",q=query).execute()
        items = result.get('files', [])
        if len(items) > 0:
            for item in items:
                if item['name'] == rootFolderName and item['mimeType'] == FOLDER_MIME_TYPE:                
                    return items[0]['id']


    def downloadFile(self, fileId, filepath, folderName):
        self.__createOrPassFolder(folderName)
        print('filePath: ' + filepath)
        request = Service.files().get_media(fileId=fileId)
        fh = io.BytesIO()
        
        downloader = MediaIoBaseDownload(fh, request)
        done = False
        while done is False:
            done = downloader.next_chunk()
        with io.open(filepath, 'wb') as f:
            fh.seek(0)
            f.write(fh.read())

    def __createOrPassFolder(self, folderName):
        print('Exist: ' + folderName)
        if not os.path.exists(os.path.dirname(folderName)):
            try:
                os.makedirs(os.path.dirname(folderName))
            except OSError as exc:
                if exc.errno != errno.EEXIST:
                    raise

    def getSubFolders(self, rootFolderId):
        query = "'{0}' in parents".format(rootFolderId)
        
        result = Service.files().list(
        pageSize=100,fields="nextPageToken, files(id,name, mimeType)",q=query).execute()
        items = result.get('files', [])

        if len(items) > 0:
          for item in items:
                if item['mimeType'] == FOLDER_MIME_TYPE:
                    print(item) #gettings unity as output
    
    parentFolder = ""
    def downloadSubfolderJson(self, folderId, parentFolderName = ""):
        self.parentFolder = parentFolderName
        query = "'{0}' in parents".format(folderId)

        result = Service.files().list(
        pageSize=100,fields="nextPageToken, files(id,name, kind, mimeType)",q=query).execute()
        items = result.get('files', [])

        if items:
            for item in items:
                if item['mimeType'] == FOLDER_MIME_TYPE:
                    self.downloadSubfolderJson(item['id'], item['name'])
                elif item['mimeType'] == JSON_MIME_TYPE: 
                    self.filePath = "{0}/{1}/{2}.json".format(downloadFolder, self.parentFolder,item['name'])
                    self.folderPath = "{0}/{1}/".format(downloadFolder,self.parentFolder)
                    self.downloadFile(item['id'], self.filePath, self.folderPath)


# definitionFolder = "Assets/Definitions"
# downloader = DefinitionDownloader()
# downloader.downloadSubfolderJson('1Rl2qvoLTwQt6Skzh9BMuUyNbaJDwZ64N')

command = sys.argv[1]
definitionFolder = sys.argv[2]
folderId = sys.argv[3]
downloadFolder = sys.argv[4]

downloader = DefinitionDownloader()
if command == 'showDefinitions':
    rootFolderId = downloader.getRootFolderId(definitionFolder)
    downloader.getSubFolders(rootFolderId)
elif command == 'downloadDefinitions':
    downloader.downloadSubfolderJson(folderId)