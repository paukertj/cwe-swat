In the last six months, I've encountered multiple .NET/C# projects named '`Ciklum.Portal.Api.csproj`', '`Ciklum.Portal.Infrastructure.Sql.csproj`', and so on. I don't know the motivation behind these names, but I can understand the need to have an assembly name or namespace with such a name.

For situations when we want a namespace different from the project name, we can use the [AssemblyName and RootNamespace](https://learn.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-properties?view=vs-2022) [MSBuild](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild?view=vs-2022) property. This allows us to have a short project file name, such as '`Infrastructure.Sql.csproj`', but retain the full name of the assembly or namespace (depending on the property).

Consider we have a file '`Infrastructure.Sql.csproj`' that should be compiled into the '`Ciklum.Portal.Infrastructure.Sql.dll`' assembly. We can easily achieve that by extending the `PropertyGroup` setting in our `csproj`:
```
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
  <AssemblyName>Ciklum.Portal.Infrastructure.Sql</AssemblyName>
</PropertyGroup>
```

We can also extend our configuration to use the '`Ciklum.Portal.Infrastructure.Sql`' namespace everywhere within this project:
```
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
  <AssemblyName>Ciklum.Portal.Infrastructure.Sql</AssemblyName>
  <RootNamespace>Ciklum.Portal.Infrastructure.Sql</RootNamespace>
</PropertyGroup>
```

If we want to ensure that these settings are adjusted regardless of the current project name, we can use the `MSBuildProjectName` [property](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-reserved-and-well-known-properties?view=vs-2022) to obtain the actual project name:
```
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
  <AssemblyName>Ciklum.Portal.$(MSBuildProjectName)</AssemblyName>
  <RootNamespace>Ciklum.Portal.$(MSBuildProjectName)</RootNamespace>
</PropertyGroup>
```

Until now, we've made all changes at the project level. This means that if we decide to use this approach, we have to adjust each `csproj` file manually. To simplify this, we can introduce some global configuration that will be imported by [MSBuild](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild?view=vs-2022). Usually, all projects have a shared parent folder (typically the folder where the `sln` file is placed). Let's assume that this is also our case. In that case, we can use [MSBuild build customization by directory](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory?view=vs-2022). For these settings, let's create a [`Directory.Build.props`](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory?view=vs-2022#directorybuildprops-and-directorybuildtargets) file in the same place as your sln file and put the global configuration there:
```
<Project>
  <PropertyGroup>
    <RootNamespace>Ciklum.Portal.$(MSBuildProjectName)</RootNamespace>
    <AssemblyName>Ciklum.Portal.$(MSBuildProjectName)</AssemblyName>
  </PropertyGroup>
</Project>
```
These settings apply a namespace and assembly prefix for all projects.

So, there is no need for explicit project names if the only motivation is the assembly name, namespace, or both.