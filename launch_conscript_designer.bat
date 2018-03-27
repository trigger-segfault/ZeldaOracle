@echo off

REM # Update the bin folder's content and binaries
echo GameBuilder.exe -debug -compile-silent -out "ZeldaOracle\ConscriptDesigner\bin" -game "ZeldaOracle\Game\bin" -editor "ZeldaOracle\GameEditor\bin" -content "ZeldaOracle\GameContent\ZeldaContent.contentproj"
GameBuilder.exe -debug -compile-silent -out "ZeldaOracle\ConscriptDesigner\bin" -game "ZeldaOracle\Game\bin" -editor "ZeldaOracle\GameEditor\bin" -content "ZeldaOracle\GameContent\ZeldaContent.contentproj"

REM # Check if there was an error
if %ERRORLEVEL% NEQ 0 (
  echo.
  echo ERROR: Failure with GameBuilder.exe
  pause
  exit
)

REM # Launch the designer
cd "ZeldaOracle\ConscriptDesigner\bin\Debug"
echo Launching Conscript Designer
start ConscriptDesigner.exe "..\..\..\GameContent\ZeldaContent.contentproj"
exit