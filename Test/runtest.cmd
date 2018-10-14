dotnet run --project ..\NBasicBin2Text\NBasicBin2Text\NBasicBin2Text.csproj TSTBIN >temp.txt
copy nul result.txt
FOR /F "skip=1 tokens=* usebackq" %%i IN (temp.txt) DO @echo %%i>> result.txt
del temp.txt
fc result.txt tstasc
pause
