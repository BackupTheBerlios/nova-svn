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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;

#endregion

namespace OpenArrow.ComponentServices
{
	public partial class ComSvc : ServiceBase
	{
		SystemNovaServer server;
		public ComSvc()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			ServerConfiguration sc;
			// Check for the serverconf file
			string serverconfPath = "";
			// If none was specified, load/create the default
			if ((args[0] == "") || (args[0] == null) || (args[0].ToLower() == "_default"))
			{
				serverconfPath = Path.Combine(
					Environment.GetFolderPath(
						Environment.SpecialFolder.CommonApplicationData),
					"Nova\\Servers\\_default\\serverconf.xml"
				);
			}
			else
			{
				serverconfPath = Path.Combine(
					Environment.GetFolderPath(
						Environment.SpecialFolder.CommonApplicationData),
					String.Format("Nova\\Servers\\{0}\\serverconf.xml", args[0])
				);
			}
			if (!File.Exists(serverconfPath))
				sc = CreateDefaultSC();
			else
				sc = ServerConfiguration.Load(serverconfPath);
		}

		protected override void OnStop()
		{
			// TODO: Add code here to perform any tear-down necessary to stop your service.
		}
	}
}
