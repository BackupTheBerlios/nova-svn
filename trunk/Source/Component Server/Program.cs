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

using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

#endregion

namespace OpenArrow.ComponentServices
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			ServiceBase[] ServicesToRun;

			// More than one user Service may run within the same process. To add
			// another service to this process, change the following line to
			// create a second service object. For example,
			//
			//   ServicesToRun = new ServiceBase[] {new Service1(), new MySecondUserService()};
			//
			ServicesToRun = new ServiceBase[] { new ComSvc() };

			ServiceBase.Run(ServicesToRun);
		}
	}
}