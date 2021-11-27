#Sample plugin assembly

```csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework> <!-- net6.0 is the only currently supported sdk -->
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <None Remove="Resources\ssh.png" />
  </ItemGroup>

  <ItemGroup>
    <EmbeddedResource Include="Resources\ssh.png" /> <!-- Profile icons should be embedded as resources -->
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="..\Tapanga.Plugin\Tapanga.Plugin.csproj" / >
  </ItemGroup>

</Project>
```