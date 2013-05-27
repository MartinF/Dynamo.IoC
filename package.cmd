NuGet\NuGet.exe update -self

msbuild.exe Dynamo.Ioc.sln /t:Clean,Rebuild /p:Configuration=Release /fileLogger
NuGet\NuGet.exe pack Dynamo.Ioc.nuspec
NuGet\NuGet.exe pack Dynamo.Ioc.Extensions.nuspec
NuGet\NuGet.exe pack Dynamo.Ioc.Web.nuspec