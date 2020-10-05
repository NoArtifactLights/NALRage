using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NALRage.Engine.Modification.API.Events
{
    internal static class EventManager
    {
        private static List<Type> events = new List<Type>();

        internal static void RegisterEvent(Type @event)
        {
            if (@event is null) return;
            if (!@event.IsAssignableFrom(typeof(Event))) return;
            Game.LogTrivial("Registering event " + @event.Name + " from assembly " + @event.Assembly.GetName().CodeBase);
            events.Add(@event);
        }

        internal static void Process()
        {

        }

        internal static void GetEvent()
        {

        }
    }
}
