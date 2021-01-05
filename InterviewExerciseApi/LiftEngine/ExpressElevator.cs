namespace InterviewExerciseApi.LiftEngine
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Stateless;

    // 4. Identify the problem causing lifts to malfunction after they have been called once, and fix it.
    public class ExpressElevator : ILift
    {
        // Represents the State the lift is currently in
        public string Status
        {
            get
            {
                return _liftStateMachine.State.ToString();
            }
        }

        private enum State { Stopped, Moving, OutOfService }

        private enum Trigger { Call, Arrived, Malfunction }        

        private int _currentFloor = 0;

        private bool _doorsOpen = false;

        private readonly StateMachine<State, Trigger> _liftStateMachine;

        private readonly StateMachine<State, Trigger>.TriggerWithParameters<int> _callTrigger;

        public ExpressElevator()
        {
            // State machine representing the lift
            // Uses the Stateless library:
            // https://github.com/dotnet-state-machine/stateless

            _liftStateMachine = new StateMachine<State, Trigger>(State.Stopped);
            _callTrigger = _liftStateMachine.SetTriggerParameters<int>(Trigger.Call);

            _liftStateMachine.Configure(State.Stopped)
                .OnEntryFrom(Trigger.Arrived, OpenDoors)
                .Permit(Trigger.Call, State.Moving);

            _liftStateMachine.Configure(State.Moving)
                .OnEntryFrom(_callTrigger, Move)
                .Permit(Trigger.Arrived, State.Stopped)
                .Permit(Trigger.Malfunction, State.OutOfService);

            _liftStateMachine.Configure(State.OutOfService)
                .PermitReentry(Trigger.Malfunction)
                .PermitReentry(Trigger.Call)
                .PermitReentry(Trigger.Arrived)
                .OnEntry(Debugger.Break);            
        }

        // Returns the current floor of the lift
        public int CurrentFloor()
        {
            return _currentFloor;
        }
        
        public void MoveTo(int floor)
        {
            _liftStateMachine.Fire(_callTrigger, floor);
        }
                
        public bool AreDoorsOpen()
        {
            return _doorsOpen;
        }

        private void OpenDoors()
        {
            _doorsOpen = true;
        }
                
        private void CloseDoors()
        {
            _doorsOpen = false;
        }
        
        /// <summary>
        /// Determine if lift should move
        /// </summary>
        /// <param name="floor"></param>
        private void Move(int floor)
        {
            var moved = false;

            if(AreDoorsOpen())
            {
                CloseDoors();
            }
            
            if(floor != _currentFloor)
            {
                moved = GoDirectlyToFloor(floor);
            }
            
            if(moved)
            {
                _liftStateMachine.Fire(Trigger.Arrived);
            }            
        }

        /// <summary>
        /// Go directly to the desired floor
        /// </summary>
        /// <param name="desiredFloor"></param>
        /// <returns></returns>
        private bool GoDirectlyToFloor(int desiredFloor)
        {
            if (_doorsOpen)
            {
                _liftStateMachine.Fire(Trigger.Malfunction);
                return false;
            }

            var timeDelay = 100;
            Task.Delay(timeDelay).Wait();

            _currentFloor = desiredFloor;

            return true;
        }
    }
}