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
	/// <summary>
	/// Contains information about a remote component
	/// </summary>
	public class RemoteComponent
	{
		private string name;
		/// <summary>
		/// Gets or sets the name of the remote component
		/// </summary>
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

		private string address;
		/// <summary>
		/// Gets or sets the address of the server managing this component
		/// </summary>
		/// <value>A string</value>
		public string Address
		{
			get
			{
				return address;
			}

			set
			{
				address = value;
			}
		}

		private ComponentInfo info;
		/// <summary>
		/// Gets or sets a ComponentInfo structure containing information about the component
		/// </summary>
		/// <value>A ComponentInfo structure</value>
		public ComponentInfo Info
		{
			get
			{
				return info;
			}

			set
			{
				info = value;
			}
		}
		
		/// <summary>
		/// Creates a new RemoteComponent with the specified name, address and info
		/// </summary>
		/// <param name="name">The name of the component</param>
		/// <param name="address">The address of the server managing the component</param>
		/// <param name="info">Information on the component</param>
		public RemoteComponent(string name, string address, ComponentInfo info)
		{
			this.name = name;
			this.address = address;
			this.info = info;
		}

	}
}
