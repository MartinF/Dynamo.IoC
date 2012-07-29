.nuget\NuGet.exe update -self

%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe Dynamo.Ioc.sln /t:Clean,Rebuild /p:Configuration=Release /fileLogger
.nuget\NuGet.exe pack Dynamo.Ioc.nuspec
.nuget\NuGet.exe pack Dynamo.Ioc.Extensions.nuspec
.nuget\NuGet.exe pack Dynamo.Ioc.Web.nuspec