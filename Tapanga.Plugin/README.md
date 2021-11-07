#Sample plugin assembly

```csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework> <!-- net6.0 is the only currently supported sdk -->
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <EnableDynamicLoading>true</EnableDynamicLoading> <!-- Dyanmic Loading must be enabled -->
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <None Remove="Resources\ssh.png" />
  </ItemGroup>

  <ItemGroup>
    <EmbeddedResource Include="Resources\ssh.png" /> <!-- Profile icons should be embedded as resources -->
  </ItemGroup>

  <ItemGroup>
    <AssemblyAttribute Include="Tapanga.Plugin.PluginAssemblyAttribute" /> <!-- The assembly must have this attribute -->
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="..\Tapanga.Plugin\Tapanga.Plugin.csproj" > <!-- Plugin API package reference must exclude runtime assets -->
      <Private>false</Private>
      <ExcludeAssets>runtime</ExcludeAssets>
    </PackageReference>
  </ItemGroup>

</Project>
```