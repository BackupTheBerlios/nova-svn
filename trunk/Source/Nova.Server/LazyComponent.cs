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

using Nova.Framework;
#endregion

namespace Nova.Server
{
	public class LazyComponent
	{
		private string name;
		/// <summary>
		/// Gets or sets the name of the component
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

		private ComponentInfo info;
		/// <summary>
		/// Gets or sets information about the component
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

		private IRuntime runtime;
		/// <summary>
		/// Gets or sets the runtime data for the component
		/// </summary>
		/// <value>An instance of IRuntime</value>
		public IRuntime Runtime
		{
			get
			{
				return runtime;
			}

			set
			{
				runtime = value;
			}
		}

		/// <summary>
		/// Creates a new LazyComponent with the specified name, info and runtime data
		/// </summary>
		/// <param name="name">The name of the component</param>
		/// <param name="info">The ComponentInfo structure</param>
		/// <param name="runtime">The runtime data for the component</param>
		public LazyComponent(string name, ComponentInfo info, IRuntime runtime)
		{
			this.name = name;
			this.info = info;
			this.runtime = runtime;
		}
	}
}
