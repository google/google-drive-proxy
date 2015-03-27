# About Drive Proxy

Drive Proxy is a Windows Service that streamlines communication with Google
Drive. It is meant to facilitate the construction of tools that leverage Google
Drive's cloud storage capacity without burdening the hard drive and network with
unnecesary local copies. Drive Proxy handles authentication with Google Drive
and manages the cache where the needed files reside.

It uses a simple protocol to communicate with client applications over a pipe.
It is currently used by the [Google Drive Shell Extension]
(https://github.com/google/google-drive-shell-extension) project to provide a
transparent interface between Windows Explorer and Google Drive.

# Supported OS

- Windows 7 32-bit and 64-bit
- Windows 8
- Windows Server
- Citrix Server

# Contents

The bundled Visual Studio Solution file will work with Visual Studio 2010 and
later, and includes:
- the DriveProxy project
  This project builds the Drive Proxy background process, which is an interface
  to Drive API.
- the DriveProxy.Installer WiX project
  This project builds the installer for DriveProxy and DriveProxy Service. This
  only includes Drive Proxy, it will not install Google Drive Shell Extension.
- the DriveProxy.Service project
  This project builds the DriveProxy Service which manages Drive Proxy.
- the DriveProxy.Test project
  This project builds the GUI Application Test for the DriveProxy project.
- the GlobalAssemblyBuilder project
  This project centralizes the project name and the version number for all the
  projects.

# Build instructions

This github project is intended to be used as a component of your own project.
Since the service needs to talk with the Google APIs, you will need to setup a
Google API project in the Google Developers Console.

 1. Go to: https://console.developers.google.com/project 
 2. Click on “Create Project”
 3. Name your project and click on “Create”
 4. Wait for the project to be created.
 5. From the left hand side menu, click on “APIs & auth”.
 6. From the left hand side menu, Click on “APIs”
 7. You will need to enable the “Drive API” by toggling the switch to “on”
 8. From the left hand side menu, Click on “Credentials”
 9. Click on “Create new Client ID”
 10. Select “Installed application” and click on “Configure consent screen”
 11. Fill in the details for your consent screen and click on “Save”.
 12. A new form will be presented. Select “Installed application” and “Other”
     then click on “Create Client ID”
 13. You will be presented with a Client ID and Client Secret.
 14. Switch to the root of the git repository and using a text editor, open
     ProjectConfig.txt
 15. You will see a line “ClientID \<Your application google id here\>”.
     Replace “\<Your application google id here\>” by the Client ID in the
     developer console.  
     Example: “944352700820-eh520uo159llp750lf9jmn6srcm35r3j.apps.
     googleusercontent.com”.
 16. You will see a line “ClientSecret \<Your application google secret
     here\>”. Replace “\<Your application google secret here\>” by the Client
     Secret in the developer console.  
     Example: “BfI0jTaVzBAuRo9odDmheM2Z”
 17. You will see a line “UpgradeCode \<A GUID to identify your project
     here\>”. Generate a GUID and replace “\<A GUID to identify your project
     here\>” with the generated GUID.  
     Example: cb1ed02a-7233-4a67-a9f7-ad10a42a2082
 18. You will see a line “Company \<Your Company name here\>”. Replace “\<Your
     Company name here\>” with the company name you wish to appear in the
     “Add/Remove programs” window’s company column for Drive Proxy’s entry.  
     Example: “Initech, Inc.”
 19. You will see a line “CompanyPath \<Your Company here, must be a valid
     Windows folder name\>”. The installer will install to “%programfiles%\
     CompanyPath\Drive Proxy Service”. Replace “\<Your Company here, must be a
     valid Windows folder name\>” with the folder name under which you wish to
     group your programs.  
     Example: “Initech”
 20. You can then open DriveProxy.sln and compile the Installer project.

# Installation instructions

Executing the installer will install the service to “%programfiles%\
CompanyPath\Drive Proxy Service”.

# Usage

The service can not be used by itself, instead an application would use it to
communicate with Google Drive. The Google Drive Shell Extension project is an
example of such an application.

# Contact

For questions and answers join/view the [google-drive-proxy]
(https://groups.google.com/d/forum/google-drive-proxy) Google Group.
