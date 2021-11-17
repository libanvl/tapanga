# Tapanga (Terminal Profile N-Generator)

## What is Tapanga?

* Tapanga is a pluggable framework that makes it easy to write “generators” for Windows Terminal profiles. It uses the Windows Terminal JSON fragment extension feature.
* Tapanga Plugins are assemblies that provide one or more generators that take user input and turn it into structured profile data such as command-line, profile name, tab title, starting directory, etc…
* Tapanga itself manages the JSON files, updating them when a generator is run.
* Tapanga ships with one plugin, which provides the core.ssh generator. This generator creates simple ssh profiles for WT.

## What is the Tapanga.CommandLine tool?

* This is the command line interface for Tapanga.
* It finds plugins and loads the generators from those plugins.
* Generators can be run as full cli tools, requiring arguments to be passed on the command line. This is the run command. Example:
  * tapanga.exe generator core.ssh run --destination user@server.tld
* Generators can also be run in interactive mode. In this mode, the user is prompted to provide values for each argument. This is the go command. Example:
  * tapanga.exe generator core.ssh go
* Profiles created by Tapanga generators can be managed using the profile command. It has several subcommands which can be seen using --help. Example:
  * tapanga.exe profile --help

## Install

Install the cli -- Tapanga.CommandLine -- as a dotnet global tool: `dotnet tool install --global Tapanga.CommandLine --prerelease`

_Icons made by [Freepik](https://www.freepik.com) from [www.flaticon.com](https://www.flaticon.com/)_
