
cd "C:\Users\GeorgeM\source\repos\NAPOO\RegiX.NapooStudentDocumentsAdapter\RegiX.NapooStudentDocumentsAdapter"


$Major=1

$Minor=0

$Patch=28

dotnet build -c release -p:Major=$Major -p:Minor=$Minor -p:Patch=$Patch

.\nuget.exe pack .\RegiX.NapooStudentDocumentsAdapter\RegiX.NapooStudentDocumentsAdapter.csproj -OutputDirectory . -version "$Major.$Minor.$Patch" -Build -Properties Configuration=Release -Properties Major=$Major -Properties Minor=$Minor -Properties Patch=$Patch


