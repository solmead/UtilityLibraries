CD bin/Debug
FOR /F "delims=|" %%I IN ('DIR "*.*" /B /O:D') DO SET NewestFile=%%I
"..\..\..\..\.nuget\nuget" push %NewestFile% EAIAucit1~ -Source http://uc-nuget.azurewebsites.net/api/v2/package
cd ..
cd ..
