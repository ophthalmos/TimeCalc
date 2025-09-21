#define MyAppName "TimeCalc"
#define MyAppVersion "2.0.1"

[Setup]
AppName={#MyAppName}
AppVersion={#MyAppName}
AppVerName={#MyAppName} {#MyAppVersion}
VersionInfoVersion={#MyAppVersion}
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=admin
AppPublisher=Wilhelm Happe
VersionInfoCopyright=(C) 2025, W. Happe
LicenseFile=Lizenzvereinbarung.txt
AppPublisherURL=https://www.ophthalmostar.de/
;DefaultDirName={reg:HKCU\Software\TimeCalc,InstallPath|{autopf}\{#MyAppName}}
DefaultDirName={autopf}\{#MyAppName}
DisableWelcomePage=yes
DisableDirPage=no
DisableReadyPage=yes
CloseApplications=yes
WizardStyle=modern
WizardSizePercent=100
SetupIconFile=TimeCalc.ico
UninstallDisplayIcon={app}\TimeCalc.exe
DefaultGroupName=TimeCalc
AppId=TimeCalc
OutputDir=.
OutputBaseFilename={#MyAppName}Setup
SolidCompression=yes
ChangesAssociations=yes
DirExistsWarning=no
MinVersion=0,7.0
AppMutex={#MyAppName}_MultiStartPrevent

[Files]
Source: "bin\x64\Release\net8.0-windows7.0\TimeCalc.exe"; DestDir: "{app}"; Permissions: users-modify; Flags: replacesameversion
Source: "bin\x64\Release\net8.0-windows7.0\{#MyAppName}.dll"; DestDir: "{app}"; Permissions: users-modify; Flags: replacesameversion
Source: "bin\x64\Release\net8.0-windows7.0\{#MyAppName}.runtimeconfig.json"; DestDir: "{app}"; Permissions: users-modify; Flags: ignoreversion
Source: "Lizenzvereinbarung.txt"; DestDir: "{app}"; Permissions: users-modify;
Source: "TimeCalc.pdf"; DestDir: "{app}"; Permissions: users-modify;

[Icons]
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppName}.exe"
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppName}.exe"
Name: "{group}\Remove {#MyAppName}"; Filename: "{uninstallexe}"

[Registry]
Root: HKCR; Subkey: ".tcf";                            ValueData: "{#MyAppName}";          Flags: uninsdeletevalue; ValueType: string;  ValueName: ""
Root: HKCR; Subkey: "{#MyAppName}";                    ValueData: "Program {#MyAppName}";  Flags: uninsdeletekey;   ValueType: string;  ValueName: ""
Root: HKCR; Subkey: "{#MyAppName}\DefaultIcon";        ValueData: "{app}\{#MyAppName}.exe,0";                       ValueType: string;  ValueName: ""
Root: HKCR; Subkey: "{#MyAppName}\shell\open\command"; ValueData: """{app}\{#MyAppName}.exe"" ""%1""";              ValueType: string;  ValueName: ""

[InstallDelete]
Type: filesandordirs; Name: "{group}"

[Run]
Filename: "{app}\{#MyAppName}.exe"; Description: "{#MyAppName} starten"; Flags: postinstall nowait skipifsilent runasoriginaluser
Filename: "{app}\{#MyAppName}.pdf"; Description: "Hinweise anzeigen"; Flags: postinstall shellexec runasoriginaluser

[Languages]
Name: "de"; MessagesFile: "compiler:languages\German.isl"; LicenseFile: "Lizenzvereinbarung.txt"

[Messages]
BeveledLabel=
WinVersionTooLowError=This program requires Windows 10 or higher.

[Code]
procedure DeinitializeSetup();
var
  FilePath: string;
  BatchPath: string;
  S: TArrayOfString;
  ResultCode: Integer;
begin
  if ExpandConstant('{param:deleteSetup|false}') = 'true' then
  begin
    FilePath := ExpandConstant('{srcexe}');
    begin
      BatchPath := ExpandConstant('{%TEMP}\') + 'delete_' + ExtractFileName(ExpandConstant('{tmp}')) + '.bat';
      SetArrayLength(S, 7);
      S[0] := ':loop';
      S[1] := 'del "' + FilePath + '"';
      S[2] := 'if not exist "' + FilePath + '" goto end';
      S[3] := 'goto loop';
      S[4] := ':end';
      S[5] := 'rd "' + ExpandConstant('{tmp}') + '"';
      S[6] := 'del "' + BatchPath + '"';
      if SaveStringsToFile(BatchPath, S, True) then
      begin
        Exec(BatchPath, '', '', SW_HIDE, ewNoWait, ResultCode)
      end;
    end;
  end;
end;
