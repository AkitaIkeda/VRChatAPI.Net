# IMPORTANT
This Repo is **OUTDATED**

# VRChatAPI.Net

VRChat API Wrapper for dotnet written in .Net Standard 2.0

https://www.nuget.org/packages/VRChatAPI.Net/

# Installation

## By commands

```shell
> cd [ProjectPath]
> dotnet add package VRChatAPI.Net
```
OR
```
> dotnet add [ProjectPath] package VRChatAPI.Net
```

## By editing .csproj

Add PackageReference under the ItemGroup in your .csproj
```xml
<PackageReference Include="VRChatAPI.Net" Version="[VERSION]" />
```
Be Like
```xml
<Project Sdk="Microsoft.NET.Sdk" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">

	<!-- Other elements -->

  <ItemGroup>
		<!-- Other References -->
		<PackageReference Include="VRChatAPI.Net" Version="[VERSION]" />
  </ItemGroup>

</Project>
```
Then run
```shell
> dotnet restore
```

# Usage
See [Github Wiki](https://github.com/mueru/VRChatAPI.Net/wiki)
