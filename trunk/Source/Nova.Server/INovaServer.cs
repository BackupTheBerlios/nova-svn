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
	public interface INovaServer : IComponentServer
	{
		/// <summary>
		/// Gets or sets the queue of messages that have arrived at this server
		/// </summary>
		/// <value>A MessageQueue object</value>
		Nova.Collections.MessageQueue Messages
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a list of dispatch threads
		/// </summary>
		/// <value>A list of DispatchThread objects</value>
		List<DispatchThread> Dispatchers
		{
			get;
		}

		/// <summary>
		/// Gets or sets a list of recieve threads
		/// </summary>
		/// <remarks>The same recieve thread can be registered to recieve for multiple protocols.</remarks>
		/// <value>A list of recieve threads</value>
		List<RecieveThread> Recievers
		{
			get;
		}

		/// <summary>
		/// Gets or sets a component manager used to locate and load components
		/// </summary>
		ComponentManager Manager
		{
			get;
		}

		/// <summary>
		/// Gets or sets a table of remote component names and RemoteComponent information for remote components registered on this server
		/// </summary>
		/// <value>A dictionary with string keys and RemoteComponent values</value>
		Dictionary<string, RemoteComponent> RemoteComponents
		{
			get;
		}

		/// <summary>
		/// Gets or sets the name of this server
		/// </summary>
		/// <remarks>This should be a unique name based on the DNS name for the server computer.</remarks>
		/// <value>A string</value>
		string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Contact all available disco server to attempt to discover information about the specified component
		/// </summary>
		/// <remarks>
		/// This method causes the specified component to be added to the remote component set (RCS), but
		/// does not add it to the RCS registry on disk so the RCS must be saved back to disk in order
		/// to keep this component in the RCS. This method has no effect if the component could not
		/// be discovered.
		/// 
		/// NOTE: This interface provides no method for registering/unregistering disco server, that is the
		/// responsibility of the server. This is because there are many cases when a server requires a
		/// constant set of disco servers (local server).
		/// </remarks>
		/// <param name="name">The component to discover</param>
		/// <returns>true is the component was discovered, false if not</returns>
		bool Discover(string name);

		/// <summary>
		/// Attempts to route the specified message
		/// </summary>
		/// <remarks>The server should check if the target of this message is a remote component and 
		/// if not, attempt to discover it. Any exception thrown by this message will be used to 
		/// return an error message to the sender</remarks>
		/// <param name="m">The message to route</param>
		void RouteMessage(Message m);

		/// <summary>
		/// Dispatches the specified control message
		/// </summary>
		/// <param name="m">A control message to dispatch</param>
		object DispatchControl(Message m);
	}
}
