#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

using System;
using System.Reflection;
using System.Resources;
using System.Windows.Markup;

[assembly: XmlnsDefinition("urn:MrAdvice/View", "ArxOne.MrAdvice.MVVM.View")]
[assembly: XmlnsPrefix("urn:MrAdvice/View", "mrAdvice")]

[assembly: AssemblyDescription("MrAdvice.MVVM is a lightweight MVVM library. "
    + "It implements NotifyPropertyChanged, DependencyProperty (dependency and attached), A command binder and some threading management.")]
[assembly: AssemblyCompany("Arx One")]
[assembly: AssemblyProduct("MrAdvice")]
[assembly: AssemblyCopyright("MIT license http://opensource.org/licenses/mit-license.php")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]

[assembly: AssemblyTitle("MrAdvice.MVVM")]

[assembly: CLSCompliant(true)]


[assembly: AssemblyVersion(Product.Version)]

// ReSharper disable once CheckNamespace
internal static class Product
{
    public const string Version = "0.9.1";
}

