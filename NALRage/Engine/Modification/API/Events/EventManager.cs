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
        private static List<Event> processing = new List<Event>();
        internal static bool disable = false;

        internal static void RegisterEvent(Type @event)
        {
            if (@event is null) return;
            if (!@event.IsAssignableFrom(typeof(Event))) return;
            Game.LogTrivial("Registering event " + @event.Name + " from assembly " + @event.Assembly.GetName().CodeBase);
            events.Add(@event);
        }

        internal static void Process()
        {
            foreach (Event e in processing)
            {
                GameFiber.Yield();
                if (e == null) continue;
                if (e.IsEnded)
                {
                    processing.Remove(e);
                    continue;
                }
                e.Process();
            }
        }

        internal static void StartRandomEvent(Ped p)
        {
            Logger.Info("EventManager", "Random event start triggered");
            Logger.Debug("EventManager", "There are total of " + events.Count + " events available");
            if (events.Count == 0) return;
            if (disable) return;
            Logger.Trace("EventManager", "Picking event");
            Random r = new Random();
            int result = r.Next(0, events.Count + 1); // this may prevent picker picking the last event
            if (p.IsInAnyVehicle(false)) p.Tasks.LeaveVehicle(LeaveVehicleFlags.BailOut);
            object obj = Activator.CreateInstance(events[result]);
            Event instance = (Event)obj;
            Logger.Trace("EventManager", "Starting " + events[result].Name + " event");
            try
            {
                instance.Start();
                //processing.Add(instance);
            }
            catch(Exception ex)
            {
                Logger.Error("EventManager", "Error while starting event");
                Logger.Error("EventManager", ex.ToString());
            }
        }
    }
}
