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

using log4net;

using Nova.Collections;
using Nova.Framework;
using Nova.Server;
#endregion

namespace OpenArrow.ComponentServices
{
	/// <summary>
	/// Represents a system-wide NovaServer
	/// </summary>
	/// <remarks>
	/// IMPORTANT: A system-wide server cannot have local components so any attempt to call AttachComponent
	/// results in an exception. However, one could still add components to the ComponentManager through the
	/// Manager property. It should be noted that doing this only serves to take up memory uselessly as
	/// the local manager is ONLY checked for contracts and the component list is completely ignored.
	/// </remarks>
	public class SystemNovaServer : INovaServer
	{
		#region Private Fields
		// Component Manager for local components
		private ComponentManager manager;
		// Remote Component Table
		private Dictionary<string, RemoteComponent> remoteComponents = new Dictionary<string, RemoteComponent>();
		// Message Queue
		private MessageQueue messages = new MessageQueue();
		// Dispatch Thread List
		private List<DispatchThread> dispatchers = new List<DispatchThread>();
		// Recieve Thread List
		private List<RecieveThread> recievers = new List<RecieveThread>();
		// Protocol List
		private Dictionary<string, IProtocol> protocols = new Dictionary<string, IProtocol>();
		// Name
		private string name;
		// Log
		private ILog log;
		// Disco Server List
		private List<string> discos = new List<string>();
		#endregion

		#region Public Constructors
		/// <summary>
		/// Creates a new SystemNovaServer with the specified name
		/// </summary>
		/// <param name="name"></param>
		public SystemNovaServer(string name)
		{
			this.name = name;
			log = LogManager.GetLogger("NovaServer:" + name);
			manager = new ComponentManager(this);
		}
		#endregion

		#region INovaServer Members
		/// <summary>
		/// Gets or sets the queue of messages that have arrived at this server
		/// </summary>
		/// <value>A MessageQueue object</value>
		public MessageQueue Messages
		{
			get
			{
				return messages;
			}

			set
			{
				messages = value;
			}
		}

		/// <summary>
		/// Gets or sets a list of dispatch threads
		/// </summary>
		/// <value>A list of DispatchThread objects</value>
		public List<DispatchThread> Dispatchers
		{
			get
			{
				return dispatchers;
			}
		}

		/// <summary>
		/// Gets or sets a table of recieve threads, index by the protocol they are recieving for
		/// </summary>
		/// <remarks>The same recieve thread can be registered to recieve for multiple protocols.</remarks>
		/// <value>A dictionary with IProtocol keys and RecieveThread values</value>
		public List<RecieveThread> Recievers
		{
			get
			{
				return recievers;
			}
		}

		/// <summary>
		/// Gets or sets a component manager used to locate and load components
		/// </summary>
		public ComponentManager Manager
		{
			get
			{
				return manager;
			}
		}

		/// <summary>
		/// Gets or sets a table of remote component names and the addresses of the server managing them
		/// </summary>
		/// <value>A dictionary with string keys and string values</value>
		public Dictionary<string, RemoteComponent> RemoteComponents
		{
			get
			{
				return remoteComponents;
			}

		}

		/// <summary>
		/// Gets or sets the name of this server
		/// </summary>
		/// <remarks>This should be a unique name based on the DNS name for the server computer.</remarks>
		/// <value>A string</value>
		public string Name
		{
			get
			{
				return name;
			}

			set
			{
				string oldName = name;
				name = value;
				log.Info("Name changed from \"" + oldName + "\" to \"" + name + "\"");
				log = LogManager.GetLogger("NovaServer:" + name);
			}
		}

		/// <summary>
		/// Contact all available disco server to attempt to discover information about the specified component
		/// </summary>
		/// <remarks>
		/// This method causes the specified component to be added to the remote component set (RCS), but
		/// does not add it to the RCS registry on disk so the RCS must be saved back to disk in order
		/// to keep this component in the RCS. This method has no effect if the component could not
		/// be discovered
		/// </remarks>
		/// <param name="name">The component to discover</param>
		/// <returns>true is the component was discovered, false if not</returns>
		public bool Discover(string name)
		{
			string address = "";
			return false;
		}

		/// <summary>
		/// Attempts to route the specified message
		/// </summary>
		/// <remarks>The server should check if the target of this message is a remote component and 
		/// if not, attempt to discover it. Any exception thrown by this message will be used to 
		/// return an error message to the sender</remarks>
		/// <param name="m">The message to route</param>
		public void RouteMessage(Message m)
		{
			RemoteComponent rc = remoteComponents[m.Target];
			string scheme = rc.Address.Substring(0, rc.Address.IndexOf(':'));
			if (!protocols.ContainsKey(scheme))
			{
				string msg = "Invalid Scheme (" + scheme + "), could not find matching protocol handler";
				log.Error("[RouteMessage] " + msg);
				throw new InvalidOperationException(msg);
			}
			IProtocol protocol = protocols[scheme];
			protocol.Send(m, rc.Address);
			log.Info("[RouteMessage] Message for component \"" + m.Target + "\" routed to: " + rc.Address);
		}

		/// <summary>
		/// Dispatches the specified control message
		/// </summary>
		/// <param name="m">A control message to dispatch</param>
		public object DispatchControl(Message m)
		{
			object r = null;
			switch (m.Action.ToUpper())
			{
				case "ADDRECIEVER":
					AddReciever();
					log.Info("[Dispatch] AddReciever");
					return null;
				case "REMOVERECIEVER":
					RemoveReciever();
					log.Info("[Dispatch] RemoveReciever");
					return null;
				case "ADDDISPATCHER":
					AddDispatcher();
					log.Info("[Dispatch] AddDispatcher");
					return null;
				case "REMOVEDISPATCHER":
					RemoveDispatcher();
					log.Info("[Dispatch] RemoveDispatcher");
					return null;
				case "GETRTHREADCOUNT":
					r = (object)GetRThreadCount();
					log.Info("[Dispatch] GetRThreadCount");
					return r;
				case "GETDTHREADCOUNT":
					r = (object)GetDThreadCount();
					log.Info("[Dispatch] GetDThreadCount");
					return r;
				case "REGISTERCOMPONENT":
					RegisterComponent((RemoteComponent)m.Arguments[0]);
					log.Info("[Dispatch] RegisterComponent");
					return null;
				case "UNREGISTERCOMPONENT":
					UnregisterComponent((string)m.Arguments[0]);
					log.Info("[Dispatch] UnregisterComponent");
					return null;
				case "RESOLVECONTRACT":
					r = (object)ResolveContract((string)m.Arguments[0]);
					log.Info("[Dispatch] ResolveContract");
					return r;
				case "GETNAME":
					log.Info("[Dispatch] GetName");
					return name;
				case "SENDMESSAGE":
					SendMessage((Message)m.Arguments[0]);
					log.Info("[Dispatch] SendMessage");
					return null;
			}
			return null;
		}

		#endregion

		#region IComponentServer Members

		/// <summary>
		/// Attaches a Component to the Component Server
		/// </summary>
		/// <remarks>A Component can only be attached to one Component Server. The Components attached to a particular server make up its Local Component Set (LCS). Attaching a Component is the local equivalent of Registering a remote Component</remarks>
		/// <param name="component">The Component to attach</param>
		public void AttachComponent(IComponent component)
		{
			log.Error("[AttachComponent] System-wide server cannot have local components");
			throw new NotSupportedException("Cannot attach components to a system-wide server");
		}

		/// <summary>
		/// Detaches a Component from the Component Server
		/// </summary>
		/// <remarks>Once detached, a Component is removed from the Server's LCS and is free to be attached to a different server. Detaching is the local equivalent to Unregistering a remote Control</remarks>
		/// <param name="cref">The Component Reference Address (CRA) for the component to detach</param>
		/// <param name="address">The Address of the Component to detach</param>
		/// <param name="name">The Name of the Component to detach</param>
		public void DetachComponent(string name)
		{
			log.Error("[DetachComponent] System-wide server cannot have local components");
			throw new NotSupportedException("Cannot detach components from a system-wide server");
		}

		/// <summary>
		/// Registers a Component in the Component Server
		/// </summary>
		/// <remarks>When a Component is registered with a Component Server it becomes part of its Remote Component Set (RCS). Registering a Component is the remote equivalent of Attaching a local Component. The name parameter specifies not only the name of the component on the remote server, but also the name which will be used to refer to the component locally and must be unique (all components must have unique names anyway)</remarks>
		/// <param name="component">A RemoteComponent structure containing information about the component</param>
		public void RegisterComponent(RemoteComponent component)
		{
			if (remoteComponents.ContainsKey(component.Name))
				log.Warn("[RegisterComponent] Remote component with specified name (" + component.Name + ") exists, component will be replaced");
			remoteComponents[component.Name] = component;
			log.Info("[RegisterComponent] Registered: " + component.Name);
		}

		/// <summary>
		/// Unregisters a Component in the Component Server
		/// </summary>
		/// <remarks>When a Component is unregistered from a Component Server it is removed from the Server's Remote Component Set (RCS). Unregistering a Component is the remote equivalent of Detaching a local Component</remarks>
		/// <param name="address">The Address of the Component to unregister</param>
		/// <param name="name">The Name of the Component to unregister</param>
		public void UnregisterComponent(string name)
		{
			if (remoteComponents.ContainsKey(name))
			{
				remoteComponents.Remove(name);
				log.Info("[UnregisterComponent] Unregistered: " + name);
			}
			else
			{
				log.Warn("[UnregisterComponent] Requested component (" + name + ") could not be unregistered as it was never registered: " + name);
			}
		}

		/// <summary>
		/// Sends a message to another component
		/// </summary>
		/// <param name="address">The destination address of the message</param>
		/// <param name="message">The message to send</param>
		public void SendMessage(Message message)
		{
			// Enqueue the message for processing by Dispatch threads
			messages.Enqueue(message);
		}

		/// <summary>
		/// Registers a protocol with the Component Server
		/// </summary>
		/// <remarks>The Component Server requires protocols to be registered with it so that it is able to communicate with other servers. Each protocol registers itself with a specific URI scheme (i.e. "http" or "ftp", etc.) and whenever an Component Server address is given (i.e. in RegisterComponent) the protocol specified by the address' scheme will be used to communicate</remarks>
		/// <param name="protocol">The protocol to register</param>
		public void RegisterProtocol(IProtocol protocol)
		{
			if (protocols.ContainsKey(protocol.Scheme))
			{
				log.Warn("[RegisterProtocol] Protocol with specified scheme (" + protocol.Scheme + ") exists, protocol will be replaced");
				UnregisterProtocol(protocols[protocol.Scheme]);
			}
			foreach (RecieveThread t in recievers)
			{
				t.AddProtocol(protocol);
			}
			
			protocols.Add(protocol.Scheme, protocol);
			log.Info("[RegisterProtocol] Registered protocol: " + protocol.Scheme);
		}

		/// <summary>
		/// Unregisters a protocol from the Component Server
		/// </summary>
		/// <param name="protocol">The protocol to unregister</param>
		public void UnregisterProtocol(IProtocol protocol)
		{
			protocols.Remove(protocol.Scheme);
			foreach (RecieveThread t in recievers)
			{
				t.RemoveProtocol(protocol);
			}
			log.Info("[UnregisterProtocol] Unregistered protocol: " + protocol.Scheme);
		}

		/// <summary>
		/// Gets a list of the components avaliable to local components
		/// </summary>
		/// <remarks>This is only a list of names, information on the components must be retrieved by sending messages to them</remarks>
		/// <value>A List of strings</value>
		public ReadOnlyCollection<string> Components
		{
			get
			{
				// TODO: Make custom List class with events so that we can cache this collection and invalidate it when items are changed
				List<string> l = new List<string>();
				l.AddRange(remoteComponents.Keys);
				l.AddRange(manager.Components);
				return new ReadOnlyCollection<string>(l);
			}
		}

		/// <summary>
		/// Retrieves a table of components available that conform to the specified contract
		/// </summary>
		/// <remarks>The key of each element in the returned dictionary is the name of the component that can be used to send messages. The value is a ComponentInfo structure that describes the component. The ComponentInfo structure could be used to present a list of conforming components to a user, etc.</remarks>
		/// <returns>A dictionary with string keys and ComponentInfo value</returns>
		public Dictionary<string, ComponentInfo> ResolveContract(string contractName)
		{
			Dictionary<string, ComponentInfo> dict = new Dictionary<string, ComponentInfo>();
			// TODO: Use a log4net NDC to contain messages caused during the resolution of this contract
			foreach (string comp in remoteComponents.Keys)
			{
				RemoteComponent rc = remoteComponents[comp];
				if (rc.Info.SupportedContracts.Contains(contractName))
					dict.Add(rc.Name, rc.Info);
			}

			log.Info("[ResolveContract] Resolved contract: " + contractName);
			return dict;
		}
		
		#endregion

		#region Control Info
		private TimeSpan recvStopTimeout = TimeSpan.FromMilliseconds(200);
		private TimeSpan dispStopTimeout = TimeSpan.FromMilliseconds(200);
		#endregion

		#region Control Methods
		/// <summary>
		/// Adds a recieve thread
		/// </summary>
		private void AddReciever()
		{
			List<IProtocol> l = new List<IProtocol>();
			l.AddRange(protocols.Values);
			recievers.Add(new RecieveThread(l, this));
			log.Info("[AddReciever] Added Recieve Thread");
		}

		/// <summary>
		/// Removes a recieve thread
		/// </summary>
		private void RemoveReciever()
		{
			if (recievers.Count > 0)
			{
				recievers[0].Stop(recvStopTimeout);
				recievers.RemoveAt(0);
				log.Info("[RemoveReciever] Removed Recieve Thread");
			}
			log.Info("[RemoveReciever] Could not remove a recieve thread because there are no active recieve threads");
		}

		/// <summary>
		/// Adds a dispatch thread
		/// </summary>
		private void AddDispatcher()
		{
			dispatchers.Add(new DispatchThread(this));
			log.Info("[AddDispatcher] Added Dispatch Thread");
		}
	
		/// <summary>
		/// Removes a dispatch thread
		/// </summary>
		private void RemoveDispatcher()
		{
			if (dispatchers.Count > 0)
			{
				dispatchers[0].Stop(dispStopTimeout);
				dispatchers.RemoveAt(0);
				log.Info("[RemoveDispatcher] Removed Dispatch Thread");
			}
			log.Info("[RemoveDispatcher] Could not remove a dispatch thread because there are no active dispatch threads");
		}

		/// <summary>
		/// Gets the number of active recieve threads
		/// </summary>
		/// <returns>An integer</returns>
		private int GetRThreadCount()
		{
			return recievers.Count;
		}

		/// <summary>
		/// Gets the number of active dispatch threads
		/// </summary>
		/// <returns>An integer</returns>
		private int GetDThreadCount()
		{
			return dispatchers.Count;
		}

		/// <summary>
		/// Registers the address as a component discovery (disco) server
		/// </summary>
		/// <remarks>
		/// Disco servers provide a system for a server to discover new components without maintaining
		/// a complete list of all remote components. When a server has to send a message to a component
		/// that it cannot locate in its local tables, the server contacts registered disco servers
		/// and requests information on the component. Disco servers are designed to be interconnected,
		/// so disco servers can contact other disco servers if they do not have information on a component.
		/// </remarks>
		/// <param name="address">The address of the disco server (using a registered protocol)</param>
		public void RegisterDisco(string address)
		{
			if (discos.Contains(address))
			{
				log.Warn("[RegisterDisco] Disco server (" + address + ") already registered");
				return;
			}
			discos.Add(address);
			log.Info("[RegisterDisco] Registered disco server: " + address);
		}

		/// <summary>
		/// Unregisters the address as a component discovery (disco) server
		/// </summary>
		/// <remarks>
		/// Disco servers provide a system for a server to discover new components without maintaining
		/// a complete list of all remote components. When a server has to send a message to a component
		/// that it cannot locate in its local tables, the server contacts registered disco servers
		/// and requests information on the component. Disco servers are designed to be interconnected,
		/// so disco servers can contact other disco servers if they do not have information on a component.
		/// </remarks>
		/// <param name="address">The address of the disco server (using a registered protocol)</param>
		public void UnregisterDisco(string address)
		{
			if (!discos.Contains(address))
			{
				log.Warn("[UnregisterDisco] Disco server (" + address + ") not registered, cannot remove");
				return;
			}
			discos.Remove(address);
			log.Info("[UnregisterDisco] Unregistered disco server: " + address);
		}

		#endregion

		
	}
}
