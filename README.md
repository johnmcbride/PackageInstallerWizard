# Package Installer Wizard

### Overview
This is a generic IWizard Visual Studio Template Wizard
that allows you to add it into a template you are building and initiate
a nuget package restore after the template has been created by the user.
The package uses the WizardData xml element available within the
.vstemplate file to let yo specify that packages you would like
to restore into the project.

### How to use

1. Nuget
2. Download source

```xml
<WizardExtension>
    <Assembly>PackageInstallerWizard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=a00dba6dcc017162</Assembly>
    <FullClassName>PackageInstallerWizard.NugetPackageInstaller</FullClassName>
</WizardExtension>
```

```xml
<WizardData>
    <packages>
        <package id="Citrix.Storefront.Headless.Authentication" version="1.8.4" />
    </packages>
</WizardData>
```