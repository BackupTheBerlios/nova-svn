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
	/// Provides an interface to a communication protocol
	/// </summary>
	/// <remarks>
	/// IMPORTANT: A Protocol must be thread-safe, that is any call to Recieve or Send must not interfere with  simultaneous calls to these methods on different threads.</remarks>
	public interface IProtocol
	{
		/// <summary>
		/// Sends the specified message to the specified target address
		/// </summary>
		/// <remarks>This method requires an address to be specified because the target property of a message refers only to the component name and not a server. It is the responsibility of the Component Server to use its Remote Component Set to resolve the name to a server address and forward the message. This method should send the message asynchronously.</remarks>
		/// <param name="message">The message to send</param>
		/// <param name="address">The address to send it to</param>
		void Send(Message message, string address);

		/// <summary>
		/// Recieves a message incoming from the communication protocol if there is one
		/// </summary>
		/// <remarks>This method returns null if no message is avaliable at this time.</remarks>
		/// <param name="timeout">The maximum time to wait for a message</param>
		/// <returns>A message, or null if there is no message avaliable</returns>
		Message Recieve();

		/// <summary>
		/// Gets the URI scheme (i.e. "http") to associate with the protocol
		/// </summary>
		/// <remarks>The scheme is the initial portion of the URI, up to (but excluding) the first ":".</remarks>
		/// <value>A string</value>
		string Scheme
		{
			get;
		}
	}
}
