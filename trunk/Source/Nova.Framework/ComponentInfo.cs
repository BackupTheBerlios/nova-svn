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

namespace Nova.Framework
{
	public class ComponentInfo
	{
		private string company;
		/// <summary>
		/// Gets or sets the name of the company that provides this component
		/// </summary>
		/// <value>A string</value>
		public string Company
		{
			get
			{
				return company;
			}

			set
			{
				company = value;
			}
		}

		private string website;
		/// <summary>
		/// Gets or sets the website for this component
		/// </summary>
		/// <value>A string</value>
		public string Website
		{
			get
			{
				return website;
			}

			set
			{
				website = value;
			}
		}

		private string version;
		/// <summary>
		/// Gets or sets the version of this component
		/// </summary>
		/// <remarks>This version string is ONLY for informative purposes</remarks>
		/// <value>A string</value>
		public string Version
		{
			get
			{
				return version;
			}

			set
			{
				version = value;
			}
		}

		private string description;
		/// <summary>
		/// Gets or sets a description of this component
		/// </summary>
		/// <value>A string</value>
		public string Description
		{
			get
			{
				return description;
			}

			set
			{
				description = value;
			}
		}

		private string copyright;
		/// <summary>
		/// Gets or sets copyright information for this component
		/// </summary>
		/// <value>A string</value>
		public string Copyright
		{
			get
			{
				return copyright;
			}

			set
			{
				copyright = value;
			}
		}

		private string name;
		/// <summary>
		/// Gets or sets a descriptive, human-readable, name for the component
		/// </summary>
		/// <remarks>This is different from IComponent.Name in that it is a descriptive name like "ZIP Compression Component"</remarks>
		/// <value>A string</value>
		public string Name
		{
			get
			{
				return name;
			}

			set
			{
				name = value;
			}
		}
		/// <summary>
		/// Creates a ComponentInfo structure with no arguments
		/// </summary>
		public ComponentInfo()
		{
		}
		/// <summary>
		/// Creates a ComponentInfo structure with the specified name
		/// </summary>
		/// <param name="name">The name of the component</param>
		public ComponentInfo(string name)
		{
			this.name = name;
		}
		/// <summary>
		/// Creates a ComponentInfo structure with the specified name and author
		/// </summary>
		/// <param name="name">The name of the component</param>
		/// <param name="author">The author of this component</param>
		public ComponentInfo(string name, string author) : this(name)
		{
			this.author = author;
		}
		/// <summary>
		/// Creates a ComponentInfo structure with the specified name, author and website
		/// </summary>
		/// <param name="name">The name of the component</param>
		/// <param name="author">The author of this component</param>
		/// <param name="website">The website for this component</param>
		public ComponentInfo(string name, string author, string website) : this(name, author)
		{
			this.website = website;
		}
		/// <summary>
		/// Creates a ComponentInfo structure with the specified name, author, website and description
		/// </summary>
		/// <param name="name">The name of the component</param>
		/// <param name="author">The author of this component</param>
		/// <param name="website">The website for this component</param>
		/// <param name="description">A description of the component</param>
		public ComponentInfo(string name, string author, string website, string description) : this(name, author, website)
		{
			this.description = description;
		}
		/// <summary>
		/// Creates a ComponentInfo structure with the specified name, author, website, description and company
		/// </summary>
		/// <param name="name">The name of the component</param>
		/// <param name="author">The author of this component</param>
		/// <param name="website">The website for this component</param>
		/// <param name="description">A description of the component</param>
		/// <param name="company">The company providing this component</param>
		public ComponentInfo(string name, string author, string website, string description, string company) : this(name, author, website, description)
		{
			this.company = company;
		}
		/// <summary>
		/// Creates a ComponentInfo structure with the specified name, author, website, description, company and version
		/// </summary>
		/// <param name="name">The name of the component</param>
		/// <param name="author">The author of this component</param>
		/// <param name="website">The website for this component</param>
		/// <param name="description">A description of the component</param>
		/// <param name="company">The company providing this component</param>
		/// <param name="version">The version of this component</param>
		public ComponentInfo(string name, string author, string website, string description, string company, string version) : this(name, author, website, description, company)
		{
			this.version = version;
		}
		/// <summary>
		/// Creates a ComponentInfo structure with the specified name, author, website, description, company, version and copyright info
		/// </summary>
		/// <param name="name">The name of this component</param>
		/// <param name="author">The author of this component</param>
		/// <param name="website">The website for this component</param>
		/// <param name="description">A description of the component</param>
		/// <param name="company">The company providing this component</param>
		/// <param name="version">The version of this component</param>
		/// <param name="copyright">Copyright information for this component</param>
		public ComponentInfo(string name, string author, string website, string description, string company, string version, string copyright) : this(name, author, website, description, company, version)
		{
			this.copyright = copyright;
		}

		private string author;
		/// <summary>
		/// Gets or sets the author of this component
		/// </summary>
		/// <value>A string</value>
		public string Author
		{
			get
			{
				return author;
			}

			set
			{
				author = value;
			}
		}

		private List<string> supportedContracts = new List<string>();
		/// <summary>
		/// Gets or sets a list of the contracts this component supports
		/// </summary>
		/// <remarks>Unlike the SupportedContracts property of the IComponent interface, this method does not require the component to be loaded and contains only the names of the supported contracts</remarks>
		/// <value>A list of strings</value>
		public List<string> SupportedContracts
		{
			get
			{
				return supportedContracts;
			}
			set
			{
				supportedContracts = value;
			}
		}
	}
}
