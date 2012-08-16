/*
Unity3d-TUIO connects touch tracking from a TUIO to objects in Unity3d.

Copyright 2011 - Mindstorm Limited (reg. 05071596)

Author - Simon Lerpiniere

This file is part of Unity3d-TUIO.

Unity3d-TUIO is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Unity3d-TUIO is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser Public License for more details.

You should have received a copy of the GNU Lesser Public License
along with Unity3d-TUIO.  If not, see <http://www.gnu.org/licenses/>.

If you have any questions regarding this library, or would like to purchase 
a commercial licence, please contact Mindstorm via www.mindstorm.com.
*/

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Linq;
using OSC;

/// <summary>
/// Tracking implementation for receiving TUIO data and acting upon the events with WPF.
/// </summary>
namespace Tuio
{
	public class TuioTracking
	{
	    #region Fields
	    TuioConfiguration config;
	    UdpClient udpreceiver;
	    Thread thr;
	    bool isrunning;
	    public Dictionary<int, Tuio2DCursor> current = new Dictionary<int, Tuio2DCursor>();
	    object m_lock = new object();
	    #endregion
	
		public void ConfigureFramework(TuioConfiguration config)
	    {
	        this.config = config as TuioConfiguration;
	    }
	
	    public void Start()
	    {
	        if (!isrunning)
	        {
	            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, this.config.Port);
	            this.udpreceiver = new UdpClient(endpoint);
	
	            this.isrunning = true;
	            this.thr = new Thread(new ThreadStart(this.receive));
	            this.thr.Start();
	        }
	    }
	
	    public void Stop()
	    {
	        if (isrunning)
	        {
	            isrunning = false;
	            close();
	        }
	    }
	
	    void close()
	    {
	        try
	        {
	            //Might throw an exception which is meaningless when we are shutting down
	            this.udpreceiver.Close();
	        }
	        catch
	        {
	
	        }
	    }
	
	    public void ForceRefresh()
	    {
	        // For tuio this is only useful to remove stuck points after a TUIO server restart
	        lock (m_lock)
	        {
	            this.current.Clear();
	        }
	    }
		
		public Tuio2DCursor[] GetTouchArray()
	    {
	        // For tuio this is only useful to remove stuck points after a TUIO server restart
	        lock (m_lock)
	        {
	            Tuio2DCursor[] ts = this.current.Values.ToArray();
				return ts;
	        }
	    }
	
	    void receive()
	    {
	        try
	        {
	            receiveData();
	        }
	        catch
	        {				
	        }
	        finally
	        {
	            // Try to stop cleanly on termination of the blocking receivedata function
	            this.Stop();
	        }
	    }
	
	    void receiveData()
	    {
	        while (isrunning)
	        {
				IPEndPoint ip = null;
	
	            byte[] buffer = this.udpreceiver.Receive(ref ip);
	            OSCBundle bundle = OSCPacket.Unpack(buffer) as OSCBundle;
	
	            if (bundle != null)
	            {
					//Not currently checked, we probably should!
	                //int fseq = TuioParser.GetSequenceNumber(bundle);
					if (!TuioParser.ContainsCursors(bundle)) continue;
					
	                List<int> alivecursors = TuioParser.GetAliveCursors(bundle);
	                Dictionary<int, Tuio2DCursor> newcursors = TuioParser.GetCursors(bundle);
	                
	                // Remove the deleted ones
	                removeNotAlive(alivecursors);
	
	                //Process held/updated items
	                updateSetCursors(newcursors, alivecursors);
	
	                //Process new items
	                addNewCursors(newcursors);
	            }
	        }
	    }
	
	    void addNewCursors(Dictionary<int, Tuio2DCursor> sets)
	    {
	        // Get all the cursors we've not got
	        var result = (from entry in sets
	                      where (!this.current.ContainsKey(entry.Key))
	                      select entry.Value);
	
	        // Add them
	        foreach (Tuio2DCursor cur in result)
	            this.TouchAdded(cur);
	    }
	
	    void updateSetCursors(Dictionary<int, Tuio2DCursor> sets, List<int> alive)
	    {
	        foreach (int curid in alive)
	        {
	            //Held cursor
	            if (!sets.ContainsKey(curid) && this.current.ContainsKey(curid))
	            {
	                this.TouchHeld(curid);
	            }
	            else
	            {
	                if (sets.ContainsKey(curid) && this.current.ContainsKey(curid))
	                {
	                    Tuio2DCursor cur = sets[curid];
	                    if (cur.IsEqual(this.current[curid]))
	                    {
	                        //Call touchheld if same value
	                        this.TouchHeld(curid);
	                    }
	                    else
	                    {
	                        this.TouchUpdated(cur);
	                    }
	                }
	            }
	        }
	    }
	
	    void removeNotAlive(List<int> alive)
	    {
	        // Get all the ones to delete
	        var result = (from entry in this.current
	                      where (!alive.Contains(entry.Key))
	                      select entry.Key).ToArray<int>();
	
	        // Delete them
	        foreach (int i in result)
	            this.TouchRemoved(i);
	    }
	
		protected void TouchHeld(int touchId)
	    {
		}
		
	    protected void TouchUpdated(Tuio2DCursor cur)
	    {
			lock(m_lock)
			{
	        	this.current[cur.SessionID] = cur;
			}
	    }
	
	    protected void TouchAdded(Tuio2DCursor cur)
	    {
			lock(m_lock)
			{
	        	this.current.Add(cur.SessionID, cur);
			}
	    }
	
	    protected void TouchRemoved(int touchId)
	    {
			lock(m_lock)
			{
	        	this.current.Remove(touchId);
			}
	    }
	}
}