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
	/// Provides an interface to a component
	/// </summary>
	/// <remarks>Implementors of this interface provide methods to communicate between the component and the component server. The component itself need not be implemented in this class, or even in .Net. However, no matter how the component is implemented, every component must provide an implementation of this interface to communicate with a component server</remarks>
	public interface IComponent
	{
		/// <summary>
		/// Dispatches a message to the component
		/// </summary>
		/// <remarks>The implementing class is responsible for dispatching the message specified to the component.</remarks>
		/// <param name="message">The message to dispatch</param>
		object Dispatch(Message message);

		/// <summary>
		/// Initializes the component
		/// </summary>
		/// <remarks>Implementors of this interface must use this method to perform initialization. Components should use the component server specified by this method to communicate</remarks>
		void Initialize(IComponentServer server);

		/// <summary>
		/// Gets a list of supported Messaging Contracts
		/// </summary>
		/// <value>A list of objects implementing IContract</value>
		IContract SupportedContracts
		{
			get;
		}

		/// <summary>
		/// Gets the name of the component
		/// </summary>
		/// <remarks>This name should be formed similar to a Java Package name, by inverting the domain name of the provider's website: "mycompany.com" becomes "com.mycompany" and adding a name qualified by '.' characters. For example: "com.mycompany.mycategory.mysuite.mycomponent"</remarks>
		/// <value>A string</value>
		string Name
		{
			get;
		}

		/// <summary>
		/// Gets or sets the status of the component
		/// </summary>
		/// <value>A value from the ComponentStatusEnum enumeration</value>
		ComponentStatusEnum Status
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets information about this component
		/// </summary>
		/// <value>A ComponentInfo instance</value>
		ComponentInfo Info
		{
			get;
			set;
		}
	}
}
