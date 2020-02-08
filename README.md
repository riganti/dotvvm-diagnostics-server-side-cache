# DotVVM.Diagnostics.ServerSideCache

[DotVVM](https://github.com/riganti/dotvvm) 2.4 introduced the [Server-Side ViewModel Cache](https://tomasherceg.com/blog/post/why-i-am-so-excited-about-server-side-viewmodel-caching-in-dotvvm) feature which can dramatically reduce the amount of data transferred on postbacks.

Since the feature is a trade-off between transferred data and server-side memory resources, we have created this diagnostics tool that helps to uncover how the server-side cache is utilized and on which routes it is worth to enable it.

## How to use the tool

First, install the following NuGet package in your DotVVM project:

```
Install-Package DotVVM.Diagnostics.ServerSideCache
```

Second, register the extension in the `ConfigureServices` method in `DotvvmStartup`:

```
public void ConfigureServices(IDotvvmServiceCollection options)
{
    // ...
    options.AddServerSideCacheDiagnostics();
}
```

Then, open `/_diagnostics/cache` page in the browser to see the utilization of the cache.
