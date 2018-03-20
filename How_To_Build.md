```
$ nuget restore MimeKit.sln
$ msbuild /p:Configuration=Release /p:Platform="Any CPU" /t:Rebuild MimeKit.sln
$ nuget pack nuget/MimeKit.nuspec
```
-> MimeKit.X.X.X.nupkg is generated at the top of the repository.
