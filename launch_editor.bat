@echo off

REM # Update the bin folder's content and binaries files
echo GameBuilder.exe -debug -out "ZeldaOracle\ConscriptDesigner\bin" -game "ZeldaOracle\Game\bin" -editor "ZeldaOracle\GameEditor\bin" -content "ZeldaOracle\GameContent\ZeldaContent.contentproj"
GameBuilder.exe -debug -out "ZeldaOracle\GameEditor\bin" -game "ZeldaOracle\Game\bin" -content "ZeldaOracle\GameContent\ZeldaContent.contentproj"

REM # Check if there was an error
if %ERRORLEVEL% NEQ 0 (
  echo.
  echo ERROR: Failure with GameBuilder.exe
  pause
  exit
)

REM # Launch the editor
REM # Enter the default world path to open after ZeldaEditor.exe
cd "ZeldaOracle\GameEditor\bin\Debug"
start ZeldaEditor.exe -dev
exit