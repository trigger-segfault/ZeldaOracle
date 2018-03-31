@echo off

REM # Update the bin folder's content and binaries
echo ZeldaOracle\GameBuilder\bin\Release\GameBuilder.exe -debug -compile-silent -out "ZeldaOracle\GameEditor\bin" -game "ZeldaOracle\Game\bin" -content "ZeldaOracle\GameContent\ZeldaContent.contentproj"
ZeldaOracle\GameBuilder\bin\Release\GameBuilder.exe -debug -compile-silent -out "ZeldaOracle\GameEditor\bin" -game "ZeldaOracle\Game\bin" -content "ZeldaOracle\GameContent\ZeldaContent.contentproj"

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
echo Launching World Editor
start ZeldaEditor.exe -dev
exit