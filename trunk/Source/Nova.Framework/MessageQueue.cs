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
using System.Text;

using Nova.Framework;
#endregion

namespace Nova.Collections
{
	/// <summary>
	/// A thread-safe prioritized message queue
	/// </summary>
	// TODO: Implement this in a better way?
	public class MessageQueue
	{
		/// <summary>
		/// Queue of all Low-Priority Messages
		/// </summary>
		private Queue<Message> lowQueue = new Queue<Message>();
		/// <summary>
		/// Queue of all Normal-Priority Messages
		/// </summary>
		private Queue<Message> midQueue = new Queue<Message>();
		/// <summary>
		/// Queue of all High-Priority Messages
		/// </summary>
		private Queue<Message> highQueue = new Queue<Message>();
		/// <summary>
		/// Lock object to ensure queue is not read or written at the same time
		/// </summary>
		private object locker = new object();

		/// <summary>
		/// Adds the specified message to the queue
		/// </summary>
		/// <param name="msg">The message to add to the queue</param>
		public void Enqueue(Message msg)
		{
			// Add message to appropriate sub-queue
			switch (msg.Priority)
			{
				case MessagePriorityEnum.Low:
					lock(locker)
						lowQueue.Enqueue(msg);
					return;
				case MessagePriorityEnum.Normal:
					lock(locker)
						midQueue.Enqueue(msg);
					return;
				case MessagePriorityEnum.High:
					lock(locker)
						highQueue.Enqueue(msg);
					return;
			}
		}
		/// <summary>
		/// Gets the next message from the queue
		/// </summary>
		/// <remarks>If there are high-priority messages waiting, those are return first, followed by medium-priority messages and finally low-priority messages.</remarks>
		/// <returns>A message, or null if no messages are queued</returns>
		public Message Dequeue()
		{
			// Select the appropriate sub-queue, high first, then medium, then low
			Queue<Message> queue = null;
			if (highQueue.Count > 0)
				queue = highQueue;
			else if (midQueue.Count > 0)
				queue = midQueue;
			else
				queue = lowQueue;
			
			// Dequeue a message from the selected queue, if there is one
			Message m = null;
			if (queue.Count > 0)
			{
				lock (locker)
					m = queue.Dequeue();
			}
			return m;
		}
		/// <summary>
		/// Gets the next message of the specified priority from the queue
		/// </summary>
		/// <param name="priority">The priority of the message to dequeue</param>
		/// <returns>A message, or null if there are no messages of the specified priority in the queue</returns>
		public Message Dequeue(MessagePriorityEnum priority)
		{
			// Select the sub-queue based on specified priority
			Queue<Message> queue = null;
			switch (priority)
			{
				case MessagePriorityEnum.Low:
					queue = lowQueue;
					break;
				case MessagePriorityEnum.Normal:
					queue = midQueue;
					break;
				case MessagePriorityEnum.High:
					queue = highQueue;
					break;
			}
			
			// Dequeue a message from the selected queue, if there is one
			Message m = null;
			if (queue.Count > 0)
			{
				lock(locker)
					m = queue.Dequeue();
			}
			return m;
		}
	}
}
