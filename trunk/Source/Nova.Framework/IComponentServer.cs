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
	/// Provides an interface to a Component Server
	/// </summary>
	/// <remarks>A Component Server is responsible for managing two component sets. The LCS, or Local Component Set, is the set of all components directly managed by the current server. The RCS, or Remote Component Set, is the set of all remote components known to the server. All components must be part of one and only one LCS, but can be part of zero or more RCSes</remarks>
	public interface IComponentServer
	{
		/// <summary>
		/// Attaches a Component to the Component Server
		/// </summary>
		/// <remarks>A Component can only be attached to one Component Server. The Components attached to a particular server make up its Local Component Set (LCS). Attaching a Component is the local equivalent of Registering a remote Component</remarks>
		/// <param name="component">The Component to attach</param>
		void AttachComponent(Nova.Framework.IComponent component);

		/// <summary>
		/// Detaches a Component from the Component Server
		/// </summary>
		/// <remarks>Once detached, a Component is removed from the Server's LCS and is free to be attached to a different server. Detaching is the local equivalent to Unregistering a remote Control</remarks>
		/// <param name="cref">The Component Reference Address (CRA) for the component to detach</param>
		/// <param name="address">The Address of the Component to detach</param>
		/// <param name="name">The Name of the Component to detach</param>
		void DetachComponent(string name);

		/// <summary>
		/// Registers a Component in the Component Server
		/// </summary>
		/// <remarks>When a Component is registered with a Component Server it becomes part of its Remote Component Set (RCS). Registering a Component is the remote equivalent of Attaching a local Component. The name parameter specifies not only the name of the component on the remote server, but also the name which will be used to refer to the component locally and must be unique (all components must have unique names anyway)</remarks>
		/// <param name="component">A RemoteComponent structure containing information about the component</param>
		void RegisterComponent(RemoteComponent component);

		/// <summary>
		/// Unregisters a Component in the Component Server
		/// </summary>
		/// <remarks>When a Component is unregistered from a Component Server it is removed from the Server's Remote Component Set (RCS). Unregistering a Component is the remote equivalent of Detaching a local Component</remarks>
		/// <param name="address">The Address of the Component to unregister</param>
		/// <param name="name">The Name of the Component to unregister</param>
		void UnregisterComponent(string name);

		/// <summary>
		/// Sends a message to another component
		/// </summary>
		/// <param name="address">The destination address of the message</param>
		/// <param name="message">The message to send</param>
		void SendMessage(Message message);

		/// <summary>
		/// Registers a protocol with the Component Server
		/// </summary>
		/// <remarks>The Component Server requires protocols to be registered with it so that it is able to communicate with other servers. Each protocol registers itself with a specific URI scheme (i.e. "http" or "ftp", etc.) and whenever an Component Server address is given (i.e. in RegisterComponent) the protocol specified by the address' scheme will be used to communicate</remarks>
		/// <param name="protocol">The protocol to register</param>
		void RegisterProtocol(IProtocol protocol);

		/// <summary>
		/// Unregisters a protocol from the Component Server
		/// </summary>
		/// <param name="protocol">The protocol to unregister</param>
		void UnregisterProtocol(IProtocol protocol);

		/// <summary>
		/// Gets a list of the components avaliable to local components
		/// </summary>
		/// <remarks>This is only a list of names, information on the components must be retrieved by sending messages to them</remarks>
		/// <value>A List of strings</value>
		ReadOnlyCollection<string> Components
		{
			get;
		}

		/// <summary>
		/// Retrieves a table of components available that conform to the specified contract
		/// </summary>
		/// <remarks>The key of each element in the returned dictionary is the name of the component that can be used to send messages. The value is a ComponentInfo structure that describes the component. The ComponentInfo structure could be used to present a list of conforming components to a user, etc.</remarks>
		/// <returns>A dictionary with string keys and ComponentInfo value</returns>
		Dictionary<string, ComponentInfo> ResolveContract(string contractName);
	}
}
