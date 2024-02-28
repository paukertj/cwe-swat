Did you know that you can write an "`if` - `else`" statement in [MSBuild](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-reference)? The tool for this job is the [`Choose` element](https://learn.microsoft.com/en-us/visualstudio/msbuild/choose-element-msbuild), which allows you to conditionally select [`ItemGroup`](https://learn.microsoft.com/en-us/visualstudio/msbuild/itemgroup-element-msbuild) or [`PropertyGroup`](https://learn.microsoft.com/en-us/visualstudio/msbuild/propertygroup-element-msbuild) elements.

Consider you have three environments and you want to decide which `appsettings.json` file you will use for each of them during the build process. This ensures that you do not accidentally deliver production configuration along with development configuration, etc. Assume you have the following structure:
```
-   AwesomeSolution
    -   StartProject
        |   Program.cs
        |   appsettings.json
        |   appsettings.development.json
        |   appsettings.production.json
```

Where `appsettings.json` is for local development, and other `appsettings.*.json` files are dedicated to a particular environment based on their suffix, it can be handy to decide which file you will use directly from the [dotnet CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/), which you can easily call from a pipeline, for example. Here, the [`Choose` element](https://learn.microsoft.com/en-us/visualstudio/msbuild/choose-element-msbuild) can assist you. Consider a `.csproj` file similar to this:

```
<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>
		<OutputType>Exe</OutputType>
		<TargetFramework>net8.0</TargetFramework>
	</PropertyGroup>

</Project>
```

The first thing I need to do is to define an [MSBuild property](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-properties), which will be our input from the [dotnet CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). I will call this property `EnvConfiguration`:

```
<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>
		<OutputType>Exe</OutputType>
		<TargetFramework>net8.0</TargetFramework>
	</PropertyGroup>

 	<PropertyGroup>
		<EnvConfiguration />
	</PropertyGroup>

</Project>
```

Now, I can pass this property from the [dotnet CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/) using the [`-p` option](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-publish#msbuild):
```
dotnet publish -c:Release -p:EnvConfiguration=Whatever_I_Want_Here
```
But nothing will happen until there is some logic to process the value from that. As mentioned above, I want to pass only the `appsettings` that match my environment or the default one without a suffix. I am fine with values `-p:EnvConfiguration=development` and `-p:EnvConfiguration=production`, so I can use a condition like:
```
If (there is any parameter)
    use appsettings{parameter}.json
else
    use appsettings.json
```
So, let's rewrite this using the [`Choose` element](https://learn.microsoft.com/en-us/visualstudio/msbuild/choose-element-msbuild):
```
<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>
		<OutputType>Exe</OutputType>
		<TargetFramework>net8.0</TargetFramework>
	</PropertyGroup>

	<PropertyGroup>
		<EnvConfiguration />
	</PropertyGroup>

	<Choose>
		<When Condition="$(EnvConfiguration) != ''">
			<ItemGroup>
				<None Update="appsettings.$(EnvConfiguration).json">
					<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
				</None>
			</ItemGroup>
		</When>
		<Otherwise>
			<ItemGroup>
				<None Update="appsettings.json">
					<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
				</None>
			</ItemGroup>
		</Otherwise>
	</Choose>

</Project>
```
This implementation will work well if there is any `EnvConfiguration`, the `appsettings.json` file will be copied accordingly. If there is no value, then the default `appsettings.json` file will be used. A problem could only occur in the case where there is no file for the provided `EnvConfiguration`. Let me fix it! There is a predefined condition type for this in [MSBuild](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-reference). It is called [`Exists()`](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-conditions) and verifies if the provided path exists or not. So, let's refactor my original condition `"$(EnvConfiguration) != ''"` to one that actually verifies if there is any file for the provided `EnvConfiguration` - `Exists('appsettings.$(EnvConfiguration).json')`. Then, when an incorrect `EnvConfiguration` is entered, the default `appsettings.json` will be used instead.

> Found a bug or have additional questions? Let me know in the comments! I created this post on behalf of the CWE [**SWAT Workgroup**](https://wiki.ciklum.net/display/CGNA/SWAT+Workgroup). You can reach me and other group members at swat@ciklum.com. #SWAT