@echo off

if defined CI_BUILD (
  echo Detected that this is a CI_BUILD. Not performing a local deployment.
  exit /B 0
)

setlocal enabledelayedexpansion

@REM !!!! Update this path to your game folder path !!!!
set KSP_ROOT_FOLDER=T:\SteamGames\steamapps\common\Kerbal Space Program 2

set MOD_NAME=AUI
set KSP_MOD_FOLDER=%KSP_ROOT_FOLDER%\BepInEx\plugins\%MOD_NAME%
set bin_folder=bin
set mod_files_folder=..\mod_files
set build_configuration=Debug
set \A ret_value = 0


echo %0 for %MOD_NAME%

:check_args
if "%1" == "Release" (
  set build_configuration=Release
) else (
  if "%1" == "Debug" (
    set build_configuration=Debug
  ) else (
    if "%1" == "" (
      @REM Use default build configuration.
      goto copy_bins
    )
    echo Unrecognized %0 argument: %1
    set \A ret_value = 1
    goto exit_deploy
  )
)

:find_bins
set dll_path="%bin_folder%\%build_configuration%\%MOD_NAME%.dll"
if not exist "%dll_path%" (
  echo The output bin folder was not in the expected location: %dll_path%
  set bin_folder=alternative_ui\bin
  set dll_path="!bin_folder!\%build_configuration%\%MOD_NAME%.dll"
  echo Trying alternative path: !dll_path!
  if not exist "!dll_path!" (
    echo Failed to find the output bin folder.
    set \A ret_value = 1
    goto exit_deploy
  ) else (
    echo Found the output bin path. Continuing on.
  )
)

:copy_bins
echo Deploying %MOD_NAME% - %build_configuration% to %KSP_MOD_FOLDER%
if not exist "%KSP_MOD_FOLDER%" (
  mkdir "%KSP_MOD_FOLDER%"
  if %ERRORLEVEL% NEQ 0 (
    echo Failed to make a destination folder for the mod. Check the permissions of your KSP folder.
    set \A ret_value = %ERRORLEVEL%
    goto exit_deploy
  )
)

for %%I in ("!bin_folder!\%build_configuration%\%MOD_NAME%.dll") do (
  copy /Y /B %%I "%KSP_MOD_FOLDER%"
)
if %ERRORLEVEL% NEQ 0 (
  echo Failed to copy the mod's binaries to the destination folder.
  set \A ret_value = %ERRORLEVEL%
  goto exit_deploy
)

:copy_swinfo
echo Deploying swinfo.json to %KSP_MOD_FOLDER%
set swinfo_path="%mod_files_folder%\swinfo.json"
if not exist "%swinfo_path%" (
  echo The swinfo.json file was not in the expected location: %swinfo_path%
  set mod_files_folder=mod_files
  set swinfo_path="!mod_files_folder!\swinfo.json"
  echo Trying alternative path: !swinfo_path!
  if not exist "!swinfo_path!" (
    echo Failed to find the swinfo.json file.
    set \A ret_value = 1
    goto exit_deploy
  ) else (
    echo Found the swinfo.json file. Continuing on.
  )
)
copy /Y "!swinfo_path!" "%KSP_MOD_FOLDER%"

:exit_deploy
echo %0 done
exit /B %ret_value%