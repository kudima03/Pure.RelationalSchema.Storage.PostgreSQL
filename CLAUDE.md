# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repository is

An aggregation of the PostgreSQL storage packages for the Pure relational schema ecosystem. It holds **no code of its own**:

- `src/Pure.RelationalSchema.Storage.PostgreSQL/` is a meta-package project (`IncludeBuildOutput=false`) that only references the six packages below. Do not add source files to it.
- `modules/` contains the package repositories as git submodules, pinned to released tags:
  - [Abstractions](https://github.com/kudima03/Pure.RelationalSchema.Storage.PostgreSQL.Abstractions)
  - [Creation](https://github.com/kudima03/Pure.RelationalSchema.Storage.PostgreSQL.Creation)
  - [Querying](https://github.com/kudima03/Pure.RelationalSchema.Storage.PostgreSQL.Querying)
  - [Insertion](https://github.com/kudima03/Pure.RelationalSchema.Storage.PostgreSQL.Insertion)
  - [Migration](https://github.com/kudima03/Pure.RelationalSchema.Storage.PostgreSQL.Migration)
  - [Destruction](https://github.com/kudima03/Pure.RelationalSchema.Storage.PostgreSQL.Destruction)
- `src/Pure.RelationalSchema.Storage.PostgreSQL.slnx` is a combined solution over all of them.

Code changes belong in the package repositories, not here. Do not edit files under `modules/`; bump the submodule pointer instead.

## Commands

Clone with `--recurse-submodules`. All `dotnet` commands must be run from the `./src` directory.

```bash
dotnet restore
dotnet build --no-restore -warnaserror
dotnet pack Pure.RelationalSchema.Storage.PostgreSQL/Pure.RelationalSchema.Storage.PostgreSQL.csproj --configuration Release -p:Version=<version> --output .
```

Pack the meta-package project explicitly: `dotnet pack` without a project would pack every library in the solution. Tests are not run here; they run in each package's repository (they need Docker for Testcontainers).

## Releasing

Pushing a semver tag matching `*.*.*` triggers `.github/workflows/publish-nuget.yml`, which packs the meta-package with the tag as its version and publishes it to GitHub Packages and NuGet.org. Before tagging, make sure the `PackageReference` versions in the csproj and the submodule pointers match the releases of the six packages.

## Commit Messages

Do not mention Claude or AI assistance in commit messages.
