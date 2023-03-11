@echo off

@REM !!!! Update this path to your game folder path !!!!
set KSP_ROOT_FOLDER=T:\SteamGames\steamapps\common\Kerbal Space Program 2

set MOD_NAME=AUI
set KSP_MOD_FOLDER=%KSP_ROOT_FOLDER%\BepInEx\plugins\%MOD_NAME%
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

for %%I in ("bin\%build_configuration%\%MOD_NAME%.dll", "bin\%build_configuration%\%MOD_NAME%.pdb") do (
  copy /Y /B %%I "%KSP_MOD_FOLDER%"
)
if %ERRORLEVEL% NEQ 0 (
  echo Failed to copy the mod's binaries to the destination folder.
  set \A ret_value = %ERRORLEVEL%
  goto exit_deploy
)

:copy_swinfo
echo Deploying swinfo.json to %KSP_MOD_FOLDER%
copy /Y ..\mod_files\swinfo.json "%KSP_MOD_FOLDER%"

:exit_deploy
echo %0 done
exit /B %ret_value%