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
using System.Threading;
using System.Collections.Generic;

using log4net;

using Nova.Framework;
#endregion

namespace Nova.Server
{
	public class RecieveThread
	{
		private static long NextId = 1;
		private long id;

		/// <summary>
		/// Gets the number of protocols managed by this recieve thread
		/// </summary>
		/// <value>An integer</value>
		public int ProtocolCount
		{
			get
			{
				int i;
				lock (protocols)
					i = protocols.Count;
				return i;
			}
		}

		/// <summary>
		/// Creates a RecieveThread to recieve messages on the specified protocol
		/// </summary>
		/// <param name="protocol">The protocol to recieve messages from</param>
		/// <param name="server">The server to recieve for</param>
		public RecieveThread(IProtocol protocol, INovaServer server)
		{
			protocols.Add(protocol);
			this.server = server;
			thread = new Thread(RecieveLoop);
			log = LogManager.GetLogger(typeof(RecieveThread));
		}

		/// <summary>
		/// Creates a RecieveThread to recieve messages on the specified protocols
		/// </summary>
		/// <param name="protocols">A list of protocols to recieve messages from</param>
		/// <param name="server">The server to recieve for</param>
		public RecieveThread(List<IProtocol> protocols, INovaServer server)
		{
			this.protocols = protocols;
			this.server = server;
			thread = new Thread(RecieveLoop);

			id = RecieveThread.NextId; // Assign an ID
			if (RecieveThread.NextId == long.MaxValue) // We might run out, but its unlikely
				RecieveThread.NextId = -1; // Use the "Ran out of IDs" id :)
			else if(RecieveThread.NextId > 0) // If we haven't "run out"
				RecieveThread.NextId++; // Increment ID
			log = LogManager.GetLogger(String.Format("{2}:{0}#{1}", typeof(RecieveThread).FullName, id, server.Name));
		}

		/// <summary>
		/// Adds a protocol to be managed by this thread
		/// </summary>
		/// <param name="protocol">The protocol to add</param>
		public void AddProtocol(IProtocol protocol)
		{
			if (protocols.Contains(protocol))
			{
				log.Warn("[AddProtocol] Requested protocol was not added because it is already managed by this thread: " + protocol.Scheme);
				return;
			}
			lock (protocols)
				protocols.Add(protocol);
			if (protocols.Count > 0)
				Start();
		}

		/// <summary>
		/// Removes a protocol from the list of those managed by this thread
		/// </summary>
		/// <param name="protocol">The protocol to remove</param>
		public void RemoveProtocol(IProtocol protocol)
		{
			if (!protocols.Contains(protocol))
			{
				log.Warn("[RemoveProtocol] Requested protocol was not removed because it is not managed by this thread: " + protocol.Scheme);
				return;
			}
			if (protocols.Count == 1)
				Stop(TimeSpan.FromMilliseconds(200));
			lock (protocols)
				protocols.Remove(protocol);
		}

		///<summary>
		/// Starts the RecieveThread
		/// </summary>			
		public void Start()
		{
			log.Info("[Start] Starting Recieve Thread");
			if (thread.ThreadState != ThreadState.Running)
			{
				thread.Start();
				log.Info("[Start] Recieve Thread Started");
			}
			log.Warn("[Start] Recieve Thread was asked to start but is already running");
		}

		/// <summary>
		/// Stops the RecieveThread
		/// </summary>
		/// <remarks>This method will block until the thread actually stops or the timeout expires</remarks>
		public void Stop(TimeSpan timeout)
		{
			log.Info("[Stop] Stopping Recieve Thread");
			lock (locker)
				stop = true;
			TimeSpan ts = new TimeSpan(0, 0, 0, 0, 200);
			while (thread.ThreadState == ThreadState.Running) // Check if the thread is running
			{
				if (timeout.Ticks == 0)
				{
					thread.Abort();
					log.Info("[Stop] Recieve Thread did not respond to stop message and was aborted");
					break;
				}
				if (timeout.Ticks < ts.Ticks)
				{
					Thread.Sleep(timeout);
					timeout = new TimeSpan(0);
				}
				else
				{
					Thread.Sleep(ts);
					timeout = timeout.Subtract(ts);
				}
			}
			log.Info("[Stop] Recieve Thread Stopped");
		}


		private Thread thread;
		private List<IProtocol> protocols = new List<IProtocol>();
		private INovaServer server;
		private ILog log;

		private void RecieveLoop()
		{
			// Start at 0
			int i = 0;

			if (protocols.Count == 0)
				return;

			Monitor.Enter(locker);
			while (!stop)
			{
				// Setup variables
				Message m = null;
				IProtocol p = null;

				// Lock the protocol list and get a protocol
				Monitor.Enter(protocols);
				p = protocols[i];
				Monitor.Exit(protocols);

				// Recieve a Message
				m = p.Recieve();

				if (m != null)
				{
					// TODO: Use log4net Nested Diagnostic Context to log messages that occur during the processing of a message

					// If the target is local, enqueue the message
					if (server.Manager.Contains(m.Target))
						server.Messages.Enqueue(m);
					// If the target is remote, or we can discover it, route the message
					else if ((server.RemoteComponents.ContainsKey(m.Target)) || (server.Discover(m.Target)))
						server.SendMessage(m);
					// Otherwise, if we don't know the component, we can't recieve the message
					else
					{
						// Log the error
						string message = "Unable to locate target component (" + m.Target + ") for this message";
						log.Error(message);
						// If we do know how to reach the sender (i.e. its local or known remote)
						if ((server.Manager.Contains(m.Sender)) || (server.RemoteComponents.ContainsKey(m.Sender)))
						{
							// Create an exception message and send it with the same RefId but with our server name as the sender
							Message err = new Message("ERR", m.Sender, "SRV:" + server.Name);
							err.Arguments.Add(new NullReferenceException(message));
							err.Type = MessageTypeEnum.Exception;
							err.RefId = m.RefId;
							server.SendMessage(err);
						}
						// Don't log that we recieved the message
						continue;
					}
					log.Info("Message Recieved by " + p.Scheme + " protocol");
				}

				// If we are on the last protocol, go back to 0
				if (i == (protocols.Count - 1))
					i = 0;
				// Otherwise, move on to the next protocol
				else
					i++;

				// Let others modify the stop flag
				Monitor.Wait(locker);
			}
		}

		// Implements Starting and Stopping
		private bool stop;
		private object locker = new object();
	}
}
