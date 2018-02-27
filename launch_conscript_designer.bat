REM # Update the bin folder's content and binaries files
start GameBuilder.exe -debug -out "ZeldaOracle\ConscriptDesigner\bin" -game "ZeldaOracle\Game\bin" -editor "ZeldaOracle\GameEditor\bin" -content "ZeldaOracle\GameContent\ZeldaContent.contentproj"

REM # Launch the designer
cd "ZeldaOracle\ConscriptDesigner\bin\Debug"
start ConscriptDesigner.exe "..\..\..\GameContent\ZeldaContent.contentproj"
exit