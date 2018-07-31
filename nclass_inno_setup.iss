; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "NClass"
#define MyAppVersion "2.12"
#define MyAppPublisher "Alex Graciano"
#define MyAppURL "alexgracianoarj@gmail.com"
#define MyAppExeName "NClass.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{8E3A3D2E-D6B7-4A6A-AD95-B165276D9B82}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DisableDirPage=no
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=no
OutputDir="."
OutputBaseFilename=NClass_v{#MyAppVersion}_setup
SetupIconFile=".\NClass\src\icons\nclass.ico"
Compression=lzma
SolidCompression=yes
LicenseFile=".\NClass\doc\LICENSE.TXT"
ChangesEnvironment=yes

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
;Name: "nl"; MessagesFile: "compiler:Languages\Dutch.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Dirs]
Name: {app}\Connections; Permissions: users-modify

[Files]
Source: ".\NClass\src\GUI\bin\Release\NClass.exe"; DestDir: "{app}";
Source: ".\NClass\src\GUI\bin\Release\NClass.exe.config"; DestDir: "{app}";
Source: ".\NClass\src\GUI\bin\Release\NClass.Translations.dll"; DestDir: "{app}\Lang";
Source: ".\NClass\src\GUI\bin\Release\*.dll"; DestDir: "{app}"; Excludes: "NClass.Translations.dll,*.pdb,*.config,*.xml,*.manifest";
Source: ".\NClass\doc\README.TXT"; DestDir: "{app}\doc"; Flags: isreadme
Source: ".\NClass\doc\LICENSE.TXT"; DestDir: "{app}\doc";
Source: ".\NClass\src\icons\diagram.ico"; DestDir: "{app}";
Source: ".\NClass\src\GUI\bin\Release\Plugins\de\*"; DestDir: "{app}\Plugins\de";
Source: ".\NClass\src\GUI\bin\Release\Plugins\AssemblyImport.dll"; DestDir: "{app}\Plugins";
Source: ".\NClass\src\GUI\bin\Release\Plugins\NReflect.dll"; DestDir: "{app}\Plugins";
Source: ".\NClass\src\GUI\bin\Release\Plugins\PDFExport.dll"; DestDir: "{app}\Plugins";
Source: ".\NClass\src\GUI\bin\Release\Plugins\PdfSharp.dll"; DestDir: "{app}\Plugins";
Source: ".\NClass\src\GUI\bin\Release\Templates\*"; DestDir: "{app}\Templates"; Permissions: users-modify
Source: ".\NClass\src\GUI\bin\Release\de\*"; DestDir: "{app}\Lang\de"
Source: ".\NClass\src\GUI\bin\Release\es\*"; DestDir: "{app}\Lang\es"
Source: ".\NClass\src\GUI\bin\Release\fr\*"; DestDir: "{app}\Lang\fr"
Source: ".\NClass\src\GUI\bin\Release\hu\*"; DestDir: "{app}\Lang\hu"
Source: ".\NClass\src\GUI\bin\Release\pt-BR\*"; DestDir: "{app}\Lang\pt-BR"
Source: ".\NClass\src\GUI\bin\Release\ru\*"; DestDir: "{app}\Lang\ru"
Source: ".\NClass\src\GUI\bin\Release\zh-CN\*"; DestDir: "{app}\Lang\zh-CN"
Source: ".\NClass\examples\*.ncp"; DestDir: "{app}\examples"; Permissions: users-modify
Source: ".\NClass\styles\*"; DestDir: "{app}\styles"; Permissions: users-modify
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: {app}\{#MyAppExeName}; Parameters: """{app}\examples\shapes.ncp"" ""{app}\examples\Northwind.ncp"""; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Registry]

Root: HKCR; Subkey: ".ncp"; ValueType: string; ValueName: ""; ValueData: "{#MyAppName}"; Flags: uninsdeletevalue
Root: HKCR; Subkey: "{#MyAppName}"; ValueType: string; ValueName: ""; ValueData: "{#MyAppName}"; Flags: uninsdeletekey
Root: HKCR; Subkey: "{#MyAppName}\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\diagram.ico,0"
Root: HKCR; Subkey: "{#MyAppName}\shell\open\command";  ValueData: """{app}\{#MyAppExeName}"" ""%1""";  ValueType: string;  ValueName: ""
