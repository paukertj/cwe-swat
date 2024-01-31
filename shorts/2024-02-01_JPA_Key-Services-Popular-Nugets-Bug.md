Did you know that [`ServiceDescriptor.ImplementationType`](https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.DependencyInjection.Abstractions/src/ServiceDescriptor.cs#L158) can throw a `System.InvalidOperationException` in scenarios involving [Keyed services](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-8.0#keyed-services)? This behavior can lead to unexpected issues during Dependency Injection composition.

Consider the following example:
```csharp
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddKeyedSingleton<FooService>("fooKey");
services.AddSingleton<BarService>();
var barDescriptor = services.FirstOrDefault(x => x.ImplementationType == typeof(BarService));

sealed class FooService { }
sealed class BarService { }
```

In this example, I am essentially iterating over service descriptors that are already registered and searching for the `BarService` descriptor. However, instead of retrieving the descriptor, a `System.InvalidOperationException` is thrown. 

Iterating over service descriptors is a common practice in many libraries that perform service analysis using their own compositor. Unfortunately, this approach often introduces a similar problem to the one illustrated in the code above. For example [here](https://github.com/AzureAD/microsoft-identity-web/blob/master/src/Microsoft.Identity.Web.TokenAcquisition/ServiceCollectionExtensions.cs#L47) in [Microsoft.Identity.Web.TokenAcquisition](https://www.nuget.org/packages/Microsoft.Identity.Web.TokenAcquisition) or [here](https://github.com/microsoft/ApplicationInsights-dotnet/blob/main/NETCORE/src/Shared/Extensions/ApplicationInsightsExtensions.cs#L297C13) in [Microsoft.ApplicationInsights.AspNetCore](https://www.nuget.org/packages/Microsoft.ApplicationInsights.AspNetCore) but you can find more examples. Check issues [2604](https://github.com/AzureAD/microsoft-identity-web/issues/2604) and [2828](https://github.com/microsoft/ApplicationInsights-dotnet/issues/2828) if you want to learn more about it.

Currently, the easiest workaround for this issue is to register keyed services after performing service analysis. Referring to the example above, this approach ensures that the retrieval of the `BarService` descriptor occurs without issues:

```csharp
// ...

services.AddSingleton<BarService>();
var barDescriptor = services.FirstOrDefault(x => x.ImplementationType == typeof(BarService));

services.AddKeyedSingleton<FooService>("fooKey");

// ...
```

For our example also following solution will work:
```csharp
// ...

services.AddSingleton<BarService>();
services.AddKeyedSingleton<FooService>("fooKey");
var barDescriptor = services.FirstOrDefault(x => x.ImplementationType == typeof(BarService));

// ...
```
However, while this solution works in straightforward implementations like our example, it may not be effective in third-party libraries where the implementation can be more complex.