// This file is Copyright 2005 OpenArrow Software.
//
// OpenArrow Software grants you a license to copy, modify, 
// use, and distribute this file according to the terms of the 
// Common Public License version 1.0. You should have a copy 
// of this license in the license.txt file located at the root
// of this project. If not, Visit 
// http://www.opensource.org/licenses/cpl1.0.php or 
// send e-mail to openarrow@gmail.com to view a copy of this license

#region Using directives

using System;
using System.Collections.Generic;

#endregion

namespace Nova.Server
{
	/// <summary>
	/// Provides an interface to a runtime loader
	/// </summary>
	/// <remarks>A runtime loader is responsble for extracting custom data from an Xml Element ("Runtime") in the component registry and, when instructed to do so, loading the component into a .Net accessible form.</remarks>
	public interface IRuntime
	{
		/// <summary>
		/// Loads the component specified by this runtime loader
		/// </summary>
		/// <returns>An instance of IComponent</returns>
		Nova.Framework.IComponent Load();

		/// <summary>
		/// Initializes the runtime loader
		/// </summary>
		/// <remarks>This method can read any attributes or child elements from the XmlElement it is given. The list of attributes and elements and their values should be published with the runtime loader's documentation. NOTE: The "loader" attribute is reserved and should not be used to store loader-specific data from the "Runtime" element</remarks>
		/// <param name="data">The "Runtime" element from the component registry</param>
		/// <param name="comname">The name of the component represented by this runtime</param>
		void Initialize(string comname, System.Xml.XmlElement data);

		/// <summary>
		/// Gets or sets the name used to identify this runtime loader
		/// </summary>
		/// <remarks>This value will be matched against the "loader" attribute of "Runtime" elements to determine the loader to use.</remarks>
		/// <value>A string</value>
		string Name
		{
			get;
		}
	}
}
