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
using System.Reflection;
using System.Collections.Generic;
using System.Text;

#endregion

namespace Nova.Framework
{
	/// <summary>
	/// Provides utility methods to build a message
	/// </summary>
	public class MessageBuilder
	{
		public static Message BuildErr(Message original, Exception e)
		{
			Message err = new Message("ERR", original.Sender, original.Target);
			err.Arguments.Add(new TargetInvocationException("An exception was thrown by the recieving component", e));
			err.RefId = original.RefId;
			return err;
		}
		public static Message BuildServerErr(Message original, Exception e, string serverName)
		{
			Message err = new Message("ERR", original.Sender, "SRV:" + serverName);
			err.Arguments.Add(new TargetInvocationException("An exception was thrown by the recieving component", e));
			err.RefId = original.RefId;
			return err;
		}
		public static Message BuildReciept(Message original, object retVal)
		{
			Message rcpt = new Message("RCPT", original.Sender, original.Target);
			rcpt.Arguments.Add(retVal);
			rcpt.RefId = original.RefId;
			return rcpt;
		}
		public static Message BuildServerReciept(Message original, object retVal, string serverName)
		{
			Message rcpt = new Message("RCPT", original.Sender, "SRV:" + serverName);
			rcpt.Arguments.Add(retVal);
			rcpt.RefId = original.RefId;
			return rcpt;
		}
	}
}
