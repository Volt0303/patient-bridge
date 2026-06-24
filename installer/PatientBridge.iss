; Inno Setup script for PatientBridge
; Build with: "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" installer\PatientBridge.iss
; (run from the repository root)

#define MyAppName "PatientBridge"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "PatientBridge"
#define MyAppExeName "PatientBridge.exe"

[Setup]
AppId={{B7E4F2A1-9C3D-4E8B-A1F6-2D5C8E9A4B70}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
; Install per-machine (requires admin)
PrivilegesRequired=admin
; The app is a 64-bit self-contained build; install into the real Program Files
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
OutputDir=..\dist
OutputBaseFilename=PatientBridge-Setup-{#MyAppVersion}
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "..\publish\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\PatientBridge\config.json"; DestDir: "{app}"; Flags: ignoreversion onlyifdoesntexist

[Dirs]
; Ensure the output folder the clinic software reads from exists
Name: "C:\bdt"

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Uninstall {#MyAppName}"; Filename: "{uninstallexe}"

[Run]
; Launch after install so the app registers itself for startup
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName} now"; Flags: nowait postinstall skipifsilent

[UninstallRun]
; Stop the running app before uninstalling
Filename: "{cmd}"; Parameters: "/C taskkill /IM {#MyAppExeName} /F"; Flags: runhidden; RunOnceId: "KillPatientBridge"
