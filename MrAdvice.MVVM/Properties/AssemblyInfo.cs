#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

using System;
using System.Reflection;
using System.Windows.Markup;

[assembly: XmlnsDefinition("urn:MrAdvice/View", "ArxOne.MrAdvice.MVVM.View")]
[assembly: XmlnsPrefix("urn:MrAdvice/View", "mrAdvice")]

[assembly: AssemblyTitle("Mr. Advice MVVM")]

[assembly: CLSCompliant(true)]
