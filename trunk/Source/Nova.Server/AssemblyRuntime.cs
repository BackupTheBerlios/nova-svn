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
using System.IO;
using System.Xml;
using System.Security;
using System.Reflection;
using System.Collections.Generic;

using log4net;

using Nova.Framework;
#endregion

namespace Nova.Server
{
	/// <summary>
	/// Loads a component from a .Net Assembly
	/// </summary>
	/// <remarks>This runtime loader adds three attributes to the "Runtime" element:
	/// "file" - File name of an assembly to load
	/// "name" - Full display name of an assembly to load
	/// "class" - Fully qualified (namespaces + class name) name of the component class (must implement IComponent)
	/// If both "file" and "name" are specified, only "file" will be used</remarks>
	public class AssemblyRuntime : Nova.Server.IRuntime
	{
		private string assemblyFile = "";
		private string assemblyName = "";
		private string typeName = "";
		private string comName = "";
		private ILog log = LogManager.GetLogger(typeof(AssemblyRuntime));
		IComponent component = null;

	#region IRuntime Members

		/// <summary>
		/// Loads the component specified by this runtime loader
		/// </summary>
		/// <returns>An instance of IComponent</returns>
		public IComponent Load()
		{
			// Check if we cached it from a previous load
			if (component != null)
				return component;

			// Load the assembly
			Assembly a;
			if (assemblyFile != "")
			{
				try
				{
					a = Assembly.LoadFile(assemblyFile);
				}
				catch (FileNotFoundException fe)
				{
					string message = "Assembly File specified was not found, either fix the value or use the full assembly name (assembly must be in the GAC)";
					log.Error("[Load] " + message);
					throw new InvalidDataException(message, fe);
				}
				catch (SecurityException se)
				{
					string message = "Unable to load assembly, user does not have required permissions";
					log.Error("[Load] " + message);
					throw new SecurityException(message, se);
				}
			}
			else if (assemblyName != "")
			{
				try 
				{
					a = Assembly.Load(assemblyName);
				}
				catch (FileNotFoundException fe)
				{
					string message = "Assembly Name specified was not found, either fix the value or use the full assembly path";
					log.Error("[Load] " + message);
					throw new InvalidDataException(message, fe);
				}
				catch (SecurityException se)
				{
					string message = "Unable to load assembly, user does not have required permissions";
					log.Error("[Load] " + message);
					throw new SecurityException(message, se);
				}
			}
			else
			{
				string message = "Assembly File or Name not specified";
				log.Error("[Load] " + message);
				throw new InvalidDataException(message);
			}

			// Get the type from the assembly
			Type t = null;
			try
			{
				t = a.GetType(typeName);
			}
			catch(ArgumentException ae)
			{
				string message = "Type Name specified was invalid";
				log.Error("[Load] " + message);
				throw new InvalidDataException(message, ae);
			}
			if(t == null)
			{
				string message = "Type Name specified was not found in the specified assembly, either fix the assembly or correct the type name";
				log.Error("[Load] " + message);
				throw new InvalidDataException(message);
			}

			// Create an instance of the type
			IComponent c = null;
			try
			{
				c = Activator.CreateInstance(t) as IComponent;
			}
			catch(Exception e)
			{
				string message = "Unknown Error";
				log.Error("[Load] " + message + ": " + e.Message);
				throw new InvalidDataException(message, e);
			}
			if(c == null)
			{
				string message = "Unable to load component, specified type could not be converted into an instance of IComponent";
				log.Error("[Load] " + message);
				throw new InvalidDataException(message);
			}
			
			// Cache and return the instance as an IComponent
			component = c;

			// Log and return
			log.Info("[Load] Component Loaded: " + c.Name);
			return c;
		}

		/// <summary>
		/// Initializes the runtime loader
		/// </summary>
		/// <remarks>This method can read any attributes or child elements from the XmlElement it is given. The list of attributes and elements and their values should be published with the runtime loader's documentation. NOTE: The "loader" attribute is reserved and should not be used to store loader-specific data from the "Runtime" element</remarks>
		/// <param name="data">The "Runtime" element from the component registry</param>
		/// <param name="comname">The name of the component represented by this runtime</param>
		public void Initialize(string comname, XmlElement data)
		{
			comName = comname;
			log = LogManager.GetLogger(String.Format("{0}:{1}", typeof(AssemblyRuntime).FullName, comName));
			
			assemblyFile = data.GetAttribute("file");
			assemblyName = data.GetAttribute("name");
			typeName = data.GetAttribute("class");
		}
		
		/// <summary>
		/// Gets or sets the name used to identify this runtime loader
		/// </summary>
		/// <remarks>This value will be matched against the "loader" attribute of "Runtime" elements to determine the loader to use.</remarks>
		/// <value>A string</value>
		public string Name
		{
			get
			{
				return "assembly";
			}
		}

		
		#endregion
	}
}
