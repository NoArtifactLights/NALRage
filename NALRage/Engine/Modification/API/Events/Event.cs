using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NALRage.Engine.Modification.API.Events
{
    /// <summary>
    /// Represents an event.
    /// </summary>
    public abstract class Event
    {
        internal GameFiber fiber;

        public abstract void Start();
        public abstract void Process();

        public virtual bool IsEnded { get; set; }

        internal void Fiber()
        {
            Start();
            while(!IsEnded)
            {
                GameFiber.Yield();
                Process();
            }
        }
    }
}
