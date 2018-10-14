dotnet ..\NBasicBin2Text\NBasicBin2Text\bin\Debug\netcoreapp2.1\NBasicBin2Text.dll %opt% TSTBIN >temp.txt
copy nul %dst%
FOR /F "skip=0 tokens=* usebackq" %%i IN (temp.txt) DO @echo %%i>> %dst%
del temp.txt
fc %dst% %diff%
