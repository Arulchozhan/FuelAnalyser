using System.Threading;
namespace PaeoniaTechSpectroMeter.Station
{
    public class MachineEvents
    {


        public EventWaitHandle[] StationResetDoneEvents = null;
        public MachineEvents()
        {



            StationResetDoneEvents = new EventWaitHandle[]
            {

            };


        }
        public bool CheckIfOtherAllStationResetDone(int timeout)
        {
            bool alldone = false;
            do
            {
                alldone = StationResetDoneEvents == null || StationResetDoneEvents.Length == 0;
                if (alldone) break;

                bool onedone = false;
                for (int i = 0; i < StationResetDoneEvents.Length; i++)
                {
                    onedone = StationResetDoneEvents[i].WaitOne(timeout);
                    if (!onedone) break;
                }

                alldone = onedone;
            }
            while (false);
            return alldone;

        }

    }
}
