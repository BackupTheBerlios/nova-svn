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
	/// Provides an interface to a Messaging Contract
	/// </summary>
	/// <remarks>A Messaging Contract is a lightweight object designed to verify that a message will be accepted by a component, without actually sending the message and checking for an exception. In many ways, these contracts are like Interfaces in an Object-Oriented Programming (OOP) system. The recommended way to define a contract is an XML file that can be easily transferred between machines.</remarks>
	public interface IContract
	{
		/// <summary>
		/// Verifies that a message satisfies the Messaging Contract specified by this object
		/// </summary>
		/// <remarks>Because a Messaging Contract is designed to be lightweight, this method should NOT verify the message unless it can be done without time-intensive activities.</remarks>
		/// <param name="message">The message to verify</param>
		/// <returns>True if the message conforms to the contract, False if not</returns>
		bool Verify(Message message);

		/// <summary>
		/// Gets the name of the Messaging Contract
		/// </summary>
		/// <remarks>This name should be formed similar to a component name</remarks>
		/// <value>A string</value>
		string Name
		{
			get;
		}
	}
}
