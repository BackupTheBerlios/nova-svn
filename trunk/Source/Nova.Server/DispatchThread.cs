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
using System.Reflection;
using System.Collections.Generic;

using log4net;

using Nova.Framework;
#endregion

namespace Nova.Server
{
	public class DispatchThread
	{
		private static long NextId = 1;
		private long id;

		private bool stop;
		private object locker = new object();
		private INovaServer server;
		private ILog log;

		/// <summary>
		/// Starts the dispatch thread
		/// </summary>
		public void Start()
		{
			log.Info("[Start] Starting Dispatch Thread");
			if (thread.ThreadState != ThreadState.Running)
			{
				thread.Start();
				log.Info("[Start] Dispatch Thread Started");
			}
			log.Warn("[Start] Dispatch Thread was asked to start but is already running");
		}

		/// <summary>
		/// Stops the dispatch thread
		/// </summary>
		/// <remarks>This method blocks until the thread has been stopped or the specified timeout expires</remarks>
		public void Stop(TimeSpan timeout)
		{
			log.Info("[Stop] Stopping Dispatch Thread");
			lock (locker)
				stop = true;
			TimeSpan ts = new TimeSpan(0, 0, 0, 0, 200);
			while (thread.ThreadState == ThreadState.Running) // Check if the thread is running
			{
				if (timeout.Ticks == 0)
				{
					thread.Abort();
					log.Info("[Stop] Dispatch Thread did not respond to stop message and was aborted");
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
			log.Info("[Stop] Dispatch Thread Stopped");
		}

		/// <summary>
		/// Creates a DispatchThread for the specified server
		/// </summary>
		/// <param name="server">The server to dispatch messages for</param>
		public DispatchThread(INovaServer server)
		{
			this.server = server;
			id = DispatchThread.NextId; // Assign an ID
			if (DispatchThread.NextId == long.MaxValue) // We might run out, but its unlikely
				DispatchThread.NextId = -1; // Use the "Ran out of IDs" id :)
			else if (DispatchThread.NextId > 0)
				DispatchThread.NextId++;
			log = LogManager.GetLogger(String.Format("{2}:{0}#{1}", typeof(DispatchThread).FullName, id, server.Name));
			thread = new Thread(DispatchLoop);
		}

		private Thread thread;

		private void DispatchLoop()
		{
			Monitor.Enter(locker);
			while (!stop)
			{
				// Get a message from the queue
				Message m = server.Messages.Dequeue();

				if (m == null)
				{
					// Sleep and re-check
					Thread.Sleep(200);
					continue;
				}
				bool fail = false;
				object ret = null;

				// If the message is a server message
				if(m.Target.StartsWith("SRV"))
				{
					// If it is directed to us
					string serverName = m.Target.Substring(4);
					if (serverName.ToLower() == server.Name.ToLower())
					{
						// Dispatch and handle errors/retval (see below)
						try
						{
							ILog dLog = LogManager.GetLogger(m.Target + ":Recieved");
							dLog.Info(String.Format("{0} From {1}", m.Action, m.Sender));
							ret = server.DispatchControl(m);
						}
						catch (Exception e)
						{
							ILog dLog = LogManager.GetLogger(m.Target + ":Errors");
							dLog.Info(String.Format("{0} In {1} Sent By {2}", e.GetType().Name, m.Action, m.Sender));
							server.SendMessage(MessageBuilder.BuildServerErr(m, e, server.Name));
							fail = true;
						}
						if (!fail)
						{
							server.SendMessage(MessageBuilder.BuildServerReciept(m, ret, server.Name));
						}
					}
					else
					{
						// We can't dispatch this
						server.SendMessage(MessageBuilder.BuildServerErr(m, new InvalidOperationException("Server cannot route control message"), server.Name));
					}
				}

				// Check if the destination is on this component server
				IComponent c = server.Manager.GetComponent(m.Target);
				
				// If not...
				if (c == null)
				{
					// Check if we can route it, or discover where to route it
					if ((server.RemoteComponents.ContainsKey(m.Target)) || (server.Discover(m.Target)))
					{
						// We can!
						try
						{
							server.RouteMessage(m);
						}
						catch (Exception e)
						{
							server.SendMessage(MessageBuilder.BuildServerErr(m, e, server.Name));
						}
					}
					else
					{
						string message = "Unable to locate target component (" + m.Target + ") for message";
						log.Error("[DispatchLoop] " + message);
						server.SendMessage(MessageBuilder.BuildServerErr(m, new NullReferenceException(message), server.Name));
					}
				}

				// Dispatch and handle errors/retval
				try
				{
					ILog dLog = LogManager.GetLogger(m.Target + ":Recieved");
					dLog.Info(String.Format("{0} From {1}", m.Action, m.Sender));
					ret = c.Dispatch(m);
				}
				catch (Exception e)
				{
					ILog dLog = LogManager.GetLogger(m.Target + ":Errors");
					dLog.Info(String.Format("{0} In {1} Sent By {2}", e.GetType().Name, m.Action, m.Sender));
					// If an exception occured, log it and send an exception report
					server.SendMessage(MessageBuilder.BuildErr(m, e));
					// Set the failure flag
					// TODO: Is there a better way? I think this is wrong
					fail = true;
				}
				// If the message was successful
				if (!fail)
				{
					// Send a reciept
					server.SendMessage(MessageBuilder.BuildReciept(m, ret));
				}

				// Check the stop flag again
				Monitor.Wait(locker);
			}
		}
	}
}
