# Kusachius.QueryMapper

`Kusachius.QueryMapper` provides LINQ-style query mapping extensions for
Entity Framework Core and PostgreSQL.

## Installation

Version 1.10.0 targets .NET 10, Entity Framework Core 10, and Npgsql 10.
Building and testing requires the .NET 10 SDK.

```shell
dotnet add package Kusachius.QueryMapper
```

## Usage

```csharp
using Kusachius.QueryMapper;

var sql = context
    .From<SourceEntity>()
    .InsertInto<DestinationEntity>()
    .ToSql();
```

The package is tested with Entity Framework Core and Npgsql for PostgreSQL.

## Publishing

The GitHub Actions workflow in [`.github/workflows/publish.yml`](.github/workflows/publish.yml)
restores, builds, tests, packages, and publishes the library to NuGet.org.

Before publishing, configure a NuGet.org trusted publisher for the GitHub
repository hosting this solution and the `publish.yml` workflow file. Add the
NuGet.org package-owner username as a GitHub Actions secret named `NUGET_USER`.
Then push a version tag, for example `v1.10.0`.

The workflow uses GitHub OIDC and `NuGet/login@v1`; it does not require a
long-lived NuGet API key.
