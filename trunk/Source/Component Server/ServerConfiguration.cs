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
using System.Text;

using Nova.Framework;
#endregion

namespace OpenArrow.ComponentServices
{
	/// <summary>
	/// Represents a single server configuration.
	/// </summary>
	/// <remarks>
	/// A server configuration is a set of configuration settings and a name that represent a single
	/// server.
	/// </remarks>
	public class ServerConfiguration
	{

		/// <summary>
		/// Creates a new server configuration
		/// </summary>
		public ServerConfiguration(string name)
		{

		}

		/// <summary>
		/// Loads server configuration from the specified file
		/// </summary>
		/// <param name="serverConfFile">The file to load from</param>
		/// <returns>A ServerConfiguration instance representing the configuration in the file</returns>
		public static ServerConfiguration Load(string serverConfFile)
		{
			throw new NotImplementedException("This method is not yet implemented"); // TODO: Implement
		}

		private string name;
		private int dthreads;
		private int rthreads;
		private List<IContract> contracts;
	}
}
