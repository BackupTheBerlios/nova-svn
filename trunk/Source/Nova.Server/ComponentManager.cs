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

using Nova.Framework;
#endregion

namespace Nova.Server
{
	public class ComponentManager
	{
		/// <summary>
		/// Contains a table of components that will be lazy loaded
		/// </summary>
		private Dictionary<string, LazyComponent> lazyComponents = new Dictionary<string, LazyComponent>();
		/// <summary>
		/// Contains a table of components that have been loaded
		/// </summary>
		private Dictionary<string, IComponent> components = new Dictionary<string,IComponent>();

		private ILog log;

		/// <summary>
		/// Creates a new ComponentManager for the specified server
		/// </summary>
		/// <param name="server">The server to manage components for</param>
		public ComponentManager(INovaServer server)
		{
			log = LogManager.GetLogger(String.Format("{0}:{1}", typeof(ComponentManager).FullName, server.Name));
		}

		/// <summary>
		/// Adds a "lazy-loaded" component to the manager
		/// </summary>
		/// <remarks>A "lazy-loaded" component is one that is not loaded until it is required. This process allows the system to defer loading of a component until it is required. However, there are many times when it is preferable to load a component immidiately, this is where AddComponent should be used</remarks>
		/// <param name="comp">The LazyComponent to add</param>
		public void AddLazyComponent(LazyComponent comp)
		{
			if (lazyComponents.ContainsKey(comp.Name))
				log.Warn("[AddLazyComponent] Component with specified name (" + comp.Name + ") exists, replacing it");
			else if (components.ContainsKey(comp.Name))
			{
				string message = "Cannot replace loaded component: " + comp.Name;
				log.Error("[AddLazyComponent] " + message);
				throw new InvalidOperationException(message);
			}

			lazyComponents[comp.Name] = comp;
			log.Info("[AddLazyComponent] Added Lazy Component: " + comp.Name);
		}

		/// <summary>
		/// Adds a component to the manager
		/// </summary>
		/// <remarks>The component to be added must already be loaded (i.e. an IComponent). If lazy-loading is prefered, use AddLazyComponent</remarks>
		/// <param name="component">The component to load</param>
		public void AddComponent(IComponent component)
		{
			if (components.ContainsKey(component.Name))
			{
				string message = "Cannot replace loaded component: " + component.Name;
				log.Error("[AddComponent] " + message);
				throw new InvalidOperationException(message);
			}
			if (lazyComponents.ContainsKey(component.Name))
			{
				log.Warn("[AddComponent] Component (" + component.Name + ") is scheduled for lazy loading and will be replaced");
				lazyComponents.Remove(component.Name);
			}

			components.Add(component.Name, component);
		}

		/// <summary>
		/// Retrieves a component from the manager
		/// </summary>
		/// <remarks>This method will retrieve an instance of the component specified. If the component is a lazy-loaded component and has not yet been loaded, this method will load it.</remarks>
		/// <param name="name">The name of the component to retrieve</param>
		/// <returns>An instance of IComponent</returns>
		public IComponent GetComponent(string name)
		{
			if (components.ContainsKey(name))
			{
				IComponent c = components[name];
				return c;
			}
			else if (lazyComponents.ContainsKey(name))
			{
				IComponent c = lazyComponents[name].Runtime.Load();
				log.Info("[GetComponent] Component Lazy-Loaded: " + c.Name);
				lazyComponents.Remove(name);
				components.Add(c.Name, c);
				return c;
			}
			else
			{
				log.Error("[GetComponent] Requested component not found: " + name);
				return null;
			}
		}

		/// <summary>
		/// Retrieves information about the specified component
		/// </summary>
		/// <param name="name">The component to retrieve info for</param>
		/// <returns>A ComponentInfo structure</returns>
		public ComponentInfo GetComponentInfo(string name)
		{
			if (components.ContainsKey(name))
				return components[name].Info;
			else if (lazyComponents.ContainsKey(name))
				return lazyComponents[name].Info;
			
			log.Error("[GetComponentInfo] Requested component not found: " + name);
			return null;
		}

		/// <summary>
		/// Determines if the specified component exists in the manager
		/// </summary>
		/// <param name="name">The name of the component to find</param>
		/// <returns>A boolean</returns>
		public bool Contains(string name)
		{
			return (lazyComponents.ContainsKey(name)) || (components.ContainsKey(name));
		}

		/// <summary>
		/// Adds a messaging contract to the manager
		/// </summary>
		/// <param name="contract">The contract to add</param>
		public void AddContract(IContract contract)
		{
			if (contracts.ContainsKey(contract.Name))
				log.Warn("[AddContract] Contract (" + contract.Name + ") exists in manager, replacing");
			contracts[contract.Name] = contract;
		}

		/// <summary>
		/// Contains a table of messaging contracts
		/// </summary>
		public Dictionary<string, IContract> contracts = new Dictionary<string,IContract>();

		/// <summary>
		/// Retrieves a contract from the manager
		/// </summary>
		/// <returns>An instance of IContract</returns>
		/// <param name="name">The contract to retrieve</param>
		public IContract GetContract(string name)
		{
			if (contracts.ContainsKey(name))
				return contracts[name];
			log.Error("[GetContract] Requested contract not found: " + name);
			return null;
		}

		/// <summary>
		/// Removes a component from the manager
		/// </summary>
		/// <remarks>Note: All instances of the IComponent object representing the component will still be valid!</remarks>
		/// <param name="name">The component to remove</param>
		public void RemoveComponent(string name)
		{
			if (components.ContainsKey(name))
			{
				components.Remove(name);
				log.Info("[RemoveComponent] Removed loaded component: " + name);
			}
			else if (lazyComponents.ContainsKey(name))
			{
				lazyComponents.Remove(name);
				log.Info("[RemoveComponent] Removed component scheduled for lazy-loading: " + name);
			}
			else
				log.Info("[RemoveComponent] Requested component could not be removed as it does not exist: " + name);
		}

		/// <summary>
		/// Removes a contract from the manager
		/// </summary>
		/// <remarks>Note: All instances of the IContract object representing the contract will still be valid!</remarks>
		/// <param name="name">The contract to remove</param>
		public void RemoveContract(string name)
		{
			if (contracts.ContainsKey(name))
			{
				contracts.Remove(name);
				log.Info("[RemoveContract] Removed contract: " + name);
			}
			else
				log.Info("[RemoveContract] Requested contract could not be removed as it does not exist: " + name);
		}

		/// <summary>
		/// Gets a list of the components managed by this manager
		/// </summary>
		/// <remarks>This is only a list of names, information on the components must be retrieved by sending messages to them</remarks>
		/// <value>A List of strings</value>
		public ReadOnlyCollection<string> Components
		{
			get
			{
				// TODO: Cache-ize this (see OpenArrow.ComponentServices.SystemNovaServer.Components)
				List<string> l = new List<string>();
				l.AddRange(components.Keys);
				l.AddRange(lazyComponents.Keys);
				return new ReadOnlyCollection<string>(l);
			}
		}
	}
}
