; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "HST Desktop Tester"
#define MyAppVersion GetFileVersion("C:\AAS_SDK\Projects\HGA\HGA.HST\DesktopTester\bin\Debug\DesktopTester.exe")
#define MyAppPublisher "Seagate Systems Sdn Bhd"
#define MyAppURL "http://www.seagate.com/"
#define MyAppExeName "DesktopTester.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)

AppId={{73A18CDC-6DB3-41B5-8B0A-40204935F964}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename=HST Desktop Tester Setup v{#MyAppVersion}
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "C:\AAS_SDK\Projects\HGA\HGA.HST\DesktopTester\bin\Debug\DesktopTester.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\AAS_SDK\Projects\HGA\HGA.HST\DesktopTester\Resources\PCB.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\AAS_SDK\Projects\HGA\HGA.HST\DesktopTester\bin\Debug\*"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\AAS_SDK\Projects\HGA\HGA.HST\DesktopTester\Build\Seagate\*"; DestDir: "C:\Seagate"; Flags: recursesubdirs createallsubdirs onlyifdoesntexist 
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\PCB.ico"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\PCB.ico"
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\PCB.ico"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

