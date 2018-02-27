REM # Update the bin folder's content and binaries files
start GameBuilder.exe -debug -out "ZeldaOracle\GameEditor\bin" -game "ZeldaOracle\Game\bin" -content "ZeldaOracle\GameContent\ZeldaContent.contentproj"

REM # Launch the editor
REM # Enter the default world path to open after ZeldaEditor.exe
cd "ZeldaOracle\GameEditor\bin\Debug"
start ZeldaEditor.exe -dev
exit