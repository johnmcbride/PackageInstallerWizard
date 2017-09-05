//using Microsoft.VisualStudio.TemplateWizard;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TemplateWizard;
using NuGet.VisualStudio;
using PackageInstallerWizard.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PackageInstallerWizard
{
    class NugetPackageInstaller : IWizard
    {
        //list of packages that are loaded from the wizarddata element 
        //that is loaded from the .vstemplate
        private List<Classes.Package> packages = null;

        ServiceProvider ProjectServiceProvider
        {
            get;set;
        }
       
        public void BeforeOpeningFile(global::EnvDTE.ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(global::EnvDTE.Project project)
        {
            var componentModel = (IComponentModel)ProjectServiceProvider.GetService(typeof(SComponentModel));
            IVsPackageInstaller packageInstaller = componentModel.GetService<IVsPackageInstaller>();


            var outputWindow = (IVsOutputWindowPane)ProjectServiceProvider.GetService(typeof(SVsGeneralOutputWindowPane));

            if (packages != null)
            {
                foreach (Classes.Package package in packages)
                {
                    try
                    {
                        packageInstaller.InstallPackage(null, project, package.ID, package.Version, false);
                        //write to successfull install into the output window
                        outputWindow.OutputString($"Installed nuget package {package.ID}, version {package.Version} into project");
                        outputWindow.Activate(); // Brings this pane into view
                    }
                    catch (System.Exception packageInstallError)
                    {

                        //write output to the 
                        outputWindow.OutputString($"Error trying to install package {package.ID}, version {package.Version} into project");
                        outputWindow.OutputString($"Error: {packageInstallError.Message}");
                        outputWindow.Activate(); // Brings this pane into view
                    }
                }
            }
            
        }

        public void ProjectItemFinishedGenerating(global::EnvDTE.ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            //get the DTE object
            DTE _dte = (DTE)automationObject;
            //get the service provider for the DTE
            ProjectServiceProvider = new ServiceProvider((_dte as Microsoft.VisualStudio.OLE.Interop.IServiceProvider));

            //get the wizarddata element from the .vstemplate file
            string wizardData = replacementsDictionary["$wizarddata$"];
            if ( wizardData != null )
            {
                //load the xml from the wizard data element in the VSTemplate file.
                XmlDocument packagesDocument = new XmlDocument();
                packagesDocument.LoadXml(wizardData);

                //add the namespace (vstemplate)
                XmlNamespaceManager templateNamespaceManager = new XmlNamespaceManager(packagesDocument.NameTable);
                templateNamespaceManager.AddNamespace("pkg", "http://schemas.microsoft.com/developer/vstemplate/2005");

                //loop through all packages and load them into the list
                XmlNodeList xmlNodePackages = packagesDocument.SelectNodes("//pkg:package", templateNamespaceManager);
                if (xmlNodePackages.Count > 0 )
                {
                    packages = new List<Classes.Package>();

                    foreach ( XmlNode xmlNodePackage in xmlNodePackages)
                    {
                        string id = xmlNodePackage.Attributes["id"].Value;
                        string version = xmlNodePackage.Attributes["version"].Value;

                        packages.Add(new Classes.Package { ID = id, Version = version });
                    }
                }


            }
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
