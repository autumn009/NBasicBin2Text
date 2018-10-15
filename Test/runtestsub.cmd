dotnet ..\NBasicBin2Text\NBasicBin2Text\bin\Debug\netcoreapp2.1\NBasicBin2Text.dll %opt% TSTBIN %dst%
fc /b %dst% %diff%
