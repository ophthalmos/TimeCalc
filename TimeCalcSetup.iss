#define MyAppName "TimeCalc"
#define MyAppVersion "2.0.0.0"
#pragma include __INCLUDE__ + ";" + "C:\Program Files (x86)\Inno Download Plugin"
#include <idp.iss>

[Setup]
AppName={#MyAppName}
AppVersion={#MyAppName}
AppVerName={#MyAppName} {#MyAppVersion}
VersionInfoVersion={#MyAppVersion}
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=admin
AppPublisher=Wilhelm Happe
VersionInfoCopyright=(C) 2023, W. Happe
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
MinVersion=0,6.0
AppMutex={#MyAppName}_MultiStartPrevent
SignTool=sha256

[Files]
Source: "bin\x64\Release\net6.0-windows\TimeCalc.exe"; DestDir: "{app}"; Permissions: users-modify; Flags: replacesameversion
Source: "bin\x64\Release\net6.0-windows\{#MyAppName}.dll"; DestDir: "{app}"; Permissions: users-modify; Flags: replacesameversion
Source: "bin\x64\Release\net6.0-windows\{#MyAppName}.runtimeconfig.json"; DestDir: "{app}"; Permissions: users-modify; Flags: ignoreversion
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
WinVersionTooLowError=This program requires Windows 2000 or higher.

[CustomMessages]
IDP_FormCaption=.NET 6 Desktop runtime is required.
IDP_FormDescription=%nPlease wait while downloading from Microsoft.
IDP_DownloadFailed=Download of .NET 6 failed. .NET 6 Desktop runtime is required to run TimeCalc.
IDP_RetryCancel=Click 'Retry' to try downloading the files again, or click 'Cancel' to terminate setup.
InstallingDotNetRuntime=Installing .NET 6 Desktop Runtime. This might take a few minutes...
DotNetRuntimeFailedToLaunch=Failed to launch .NET Runtime Installer with error "%1". Please fix the error then run this installer again.
DotNetRuntimeFailed1602=.NET Runtime installation was cancelled. This installation can continue, but be aware that this application may not run unless the .NET Runtime installation is completed successfully.
DotNetRuntimeFailed1603=A fatal error occurred while installing the .NET Runtime. Please fix the error, then run the installer again.
DotNetRuntimeFailed5100=Your computer does not meet the requirements of the .NET Runtime. Please consult the documentation.
DotNetRuntimeFailedOther=The .NET Runtime installer exited with an unexpected status code "%1". Please review any other messages shown by the installer to determine whether the installation completed successfully, and abort this installation and fix the problem if it did not.

[Code]
var
  requiresRestart: boolean;
  NetRuntimeInstaller: string;

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

function CompareVersion(V1, V2: string): Integer;
var
  P, N1, N2: Integer;
begin
  Result := 0;
  while (Result = 0) and ((V1 <> '') or (V2 <> '')) do
  begin
    P := Pos('.', V1);
    if P > 0 then
    begin
      N1 := StrToInt(Copy(V1, 1, P - 1));
      Delete(V1, 1, P);
    end
      else
    if V1 <> '' then
    begin
      N1 := StrToInt(V1);
      V1 := '';
    end
      else
    begin
      N1 := 0;
    end;
    P := Pos('.', V2);
    if P > 0 then
    begin
      N2 := StrToInt(Copy(V2, 1, P - 1));
      Delete(V2, 1, P);
    end
      else
    if V2 <> '' then
    begin
      N2 := StrToInt(V2);
      V2 := '';
    end
      else
    begin
      N2 := 0;
    end;
    if N1 < N2 then Result := -1
      else
    if N1 > N2 then Result := 1;
  end;
end;

function NetRuntimeIsMissing(): Boolean;
var
  runtimes: TArrayOfString;
  registryKey: string;
  I: Integer;
  meetsMinimumVersion: Boolean;
  meetsMaximumVersion: Boolean;
  minimumVersion: string;
  maximumExclusiveVersion: string;
begin
  Result := True;
  minimumVersion := '6.0.13';
  maximumExclusiveVersion := '6.9.9';
  registryKey := 'SOFTWARE\WOW6432Node\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App';
  if RegGetValueNames(HKLM, registryKey, runtimes) then
  begin
    for I := 0 to GetArrayLength(runtimes)-1 do
    begin
      meetsMinimumVersion := not (CompareVersion(runtimes[I], minimumVersion) = -1);
      meetsMaximumVersion := CompareVersion(runtimes[I], maximumExclusiveVersion) = -1;
      if meetsMinimumVersion and meetsMaximumVersion then
      begin
        Log(Format('[.NET] Selecting %s', [runtimes[I]]));
        Result := False;
          Exit;
      end;
    end;
  end;
end;

function InstallDotNetRuntime(): String;
var
  StatusText: string;
  ResultCode: Integer;
begin
  StatusText := WizardForm.StatusLabel.Caption;
  WizardForm.StatusLabel.Caption := CustomMessage('InstallingDotNetRuntime');
  WizardForm.ProgressGauge.Style := npbstMarquee;
  try
    if not Exec(ExpandConstant('{tmp}\' + NetRuntimeInstaller), '/passive /norestart /showrmui /showfinalerror', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then
    begin
      Result := FmtMessage(CustomMessage('DotNetRuntimeFailedToLaunch'), [SysErrorMessage(resultCode)]);
    end
    else
    begin
      // See https://msdn.microsoft.com/en-us/library/ee942965(v=vs.110).aspx#return_codes
      case resultCode of
        0: begin
          // Successful
        end;
        1602 : begin
          MsgBox(CustomMessage('DotNetRuntimeFailed1602'), mbInformation, MB_OK);
        end;
        1603: begin
          Result := CustomMessage('DotNetRuntimeFailed1603');
        end;
        1641: begin
          requiresRestart := True;
        end;
        3010: begin
          requiresRestart := True;
        end;
        5100: begin
          Result := CustomMessage('DotNetRuntimeFailed5100');
        end;
        else begin
          MsgBox(FmtMessage(CustomMessage('DotNetRuntimeFailedOther'), [IntToStr(resultCode)]), mbError, MB_OK);
        end;
      end;
    end;
  finally
    WizardForm.StatusLabel.Caption := StatusText;
    WizardForm.ProgressGauge.Style := npbstNormal;
    
    DeleteFile(ExpandConstant('{tmp}\NetRuntimeInstaller.exe'));
  end;
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
begin
  // 'NeedsRestart' only has an effect if we return a non-empty string, thus aborting the installation.
  // If the installers indicate that they want a restart, this should be done at the end of installation.
  // Therefore we set the global 'restartRequired' if a restart is needed, and return this from NeedRestart()

  if NetRuntimeIsMissing() then
  begin
    Result := InstallDotNetRuntime();
  end;
end;

function NeedRestart(): Boolean;
begin
  Result := requiresRestart;
end;

procedure InitializeWizard;
// begin
//   WizardForm.LicenseAcceptedRadio.Checked := True;
begin
  if NetRuntimeIsMissing() then
    begin
      idpSetOption('DetailsButton', '0'); //Controls availability of 'Details' button
      idpSetOption('DetailedMode', '1'); //If set to 1, download details will be visible by default
      idpSetOption('AllowContinue', '1'); //Allow user to continue installation if download fails.
      NetRuntimeInstaller := 'WindowsDesktop-Runtime-6.0.x-Win-x64.exe';
      idpAddFile('https://aka.ms/dotnet/6.0/windowsdesktop-runtime-win-x64.exe', ExpandConstant('{tmp}\' + NetRuntimeInstaller));
      idpDownloadAfter(wpReady);
    end;
  end; 
//end;