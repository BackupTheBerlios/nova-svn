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
	public class Message
	{
		private string action;
		/// <summary>
		/// Gets or sets the action to be performed by the reciever
		/// </summary>
		/// <remarks>The action of a message is similar to the name of a method or function in most programming languages, it is used to identify the routines that will be executed by the reciever</remarks>
		/// <value>A string</value>
		public string Action
		{
			get
			{
				return action;
			}

			set
			{
				action = value;
			}
		}

		private List<object> arguments = new List<object>();
		/// <summary>
		/// Gets or sets a list of arguments to be sent with the message
		/// </summary>
		/// <remarks>Arguments are similar to arguments usually associated with methods and functions, they provide data to be processed by the reciever when the message is recieved</remarks>
		/// <value>A List of objects</value>
		public List<object> Arguments
		{
			get
			{
				return arguments;
			}

			set
			{
				arguments = value;
			}
		}

		private string sender;
		/// <summary>
		/// Gets or sets a target name that represents the sender
		/// </summary>
		/// <remarks>This value allows the reciever to ensure responses to a message that has been recieved are properly directed.</remarks>
		/// <value>A string</value>
		public string Sender
		{
			get
			{
				return sender;
			}

			set
			{
				sender = value;
			}
		}

		private string refid;
		/// <summary>
		/// Gets or sets a reference id used by the sender to identify the message
		/// </summary>
		/// <remarks>The reference id (RefId) of a message is used by the sender to identify the message for the purpose of properly handling any receipts sent in response to the message. Because it is only used by the sender, the RefId need only be unique within all messages sent by the sender in the current session. Finally, the reciever of a message must NEVER change the RefId and must transfer the id, exactly as recieved, onto any receipts sent back to the sender.</remarks>
		/// <value>A string</value>
		public string RefId
		{
			get
			{
				return refid;
			}

			set
			{
				refid = value;
			}
		}

		private MessageTypeEnum type;
		/// <summary>
		/// Gets or sets the type of this message
		/// </summary>
		/// <remarks>This value identifies the type of this message and is used to ensure correct processing of the message.</remarks>
		/// <value>A value from the MessageTypeEnum enumeration</value>
		public MessageTypeEnum Type
		{
			get
			{
				return type;
			}

			set
			{
				type = value;
			}
		}

		private MessagePriorityEnum priority = MessagePriorityEnum.Normal;
		/// <summary>
		/// Gets or sets the priority of this message
		/// </summary>
		/// <remarks>The priority of a message determines when it will be executed by the target. A Low priority message will only be executed if no other messages are waiting. A Medium priority message will be executed if there are no High priority messages waiting. Finally, a High priority message will execute as soon as possible.</remarks>
		/// <value>A value from the MessagePriorityEnum enumeration</value>
		public MessagePriorityEnum Priority
		{
			get
			{
				return priority;
			}

			set
			{
				priority = value;
			}
		}

		private string target;
		/// <summary>
		/// Gets or sets the target of this message
		/// </summary>
		/// <remarks>The target of a message is a string that identifies the destination of the message, usually a Component ID.</remarks>
		/// <value>A string</value>
		public string Target
		{
			get
			{
				return target;
			}

			set
			{
				target = value;
			}
		}

		private TimeSpan ttl = TimeSpan.Zero;
		/// <summary>
		/// Gets or sets the time to live of this message
		/// </summary>
		/// <remarks>This value defines the time span that this message can stay alive for, once this time expires, the message is discarded and a TimeoutException is sent back to the sender. The default value is TimeSpan.Zero meaning the message will never die (setting the TTL to TimeSpan.Zero also means the message will not die unless the TTL is changed to a non-zero value).</remarks>
		/// <value>A TimeSpan</value>
		public TimeSpan TTL
		{
			get
			{
				return ttl;
			}

			set
			{
				ttl = value;
			}
		}

		private DateTime sent;
		/// <summary>
		/// Gets or sets the time that this message was sent
		/// </summary>
		/// <remarks>The sum of this value and the TTL of the message is the time that the message will expire. This is automatically set by the component server that initially sends the message</remarks>
		/// <value>A DateTime structure</value>
		public DateTime Sent
		{
			get
			{
				return sent;
			}

			set
			{
				sent = value;
			}
		}

		/// <summary>
		/// Creates a message with the specified action, target, and sender
		/// </summary>
		/// <param name="action">The action for the reciever to perform</param>
		/// <param name="target">The destination of the message</param>
		/// <param name="sender">The sender of the message</param>
		public Message(string action, string target, string sender)
		{
			this.action = action;
			this.target = target;
			this.sender = sender;
		}
		/// <summary>
		/// Creates a message with the specified action, target, sender, and arguments
		/// </summary>
		/// <param name="action">The action for the reciever to perform</param>
		/// <param name="target">The destination of the message</param>
		/// <param name="sender">The sender of the message</param>
		/// <param name="arguments">The arguments to send with the message</param>
		public Message(string action, List<object> arguments, string target, string sender) : this(action, target, sender)
		{
			this.arguments = arguments;
		}
		/// <summary>
		/// Creates a message with the specified action, target, sender, and arguments
		/// </summary>
		/// <param name="action">The action for the reciever to perform</param>
		/// <param name="target">The destination of the message</param>
		/// <param name="sender">The sender of the message</param>
		/// <param name="arguments">The arguments to send with the message</param>
		public Message(string action, object[] arguments, string target, string sender) : this(action, target, sender)
		{
			this.arguments.AddRange(arguments);
		}
		/// <summary>
		/// Creates a message with the specified action, target, sender, and priority
		/// </summary>
		/// <param name="action">The action for the reciever to perform</param>
		/// <param name="target">The destination of the message</param>
		/// <param name="sender">The sender of the message</param>
		/// <param name="priority">The priority of the message</param>
		public Message(string action, MessagePriorityEnum priority, string target, string sender) : this(action, target, sender)
		{
			this.priority = priority;
		}
		/// <summary>
		/// Creates a message with the specified action, target, sender, priority, and arguments
		/// </summary>
		/// <param name="action">The action for the reciever to perform</param>
		/// <param name="target">The destination of the message</param>
		/// <param name="sender">The sender of the message</param>
		/// <param name="priority">The priority of the message</param>
		/// <param name="arguments">The arguments to send with the message</param>
		public Message(string action, MessagePriorityEnum priority, List<object> arguments, string target, string sender) : this(action, priority, target, sender)
		{
			this.arguments = arguments;
		}
		/// <summary>
		/// Creates a message with the specified action, target, sender, priority, and arguments
		/// </summary>
		/// <param name="action">The action for the reciever to perform</param>
		/// <param name="target">The destination of the message</param>
		/// <param name="sender">The sender of the message</param>
		/// <param name="priority">The priority of the message</param>
		/// <param name="arguments">The arguments to send with the message</param>
		public Message(string action, MessagePriorityEnum priority, object[] arguments, string target, string sender) : this(action, priority, target, sender)
		{
			this.arguments.AddRange(arguments);
		}
	}
}
