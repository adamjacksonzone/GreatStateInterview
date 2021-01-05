using System.Diagnostics;
using System.Threading.Tasks;
using Stateless;

namespace InterviewExerciseApi.LiftEngine
{
    // 4. Identify the problem causing lifts to malfunction after they have been called once, and fix it.
    public class Lift : ILift
    {
        // Represents the State the lift is currently in
        public string Status
        {
            get
            {
                return _liftStateMachine.State.ToString();
            }
        }

        private enum State { Stopped, Moving, OutOfService };
        private enum Trigger { Call, Arrived, Malfunction }

        private int _currentFloor = 0;

        private bool _doorsOpen = false;

        private readonly StateMachine<State, Trigger> _liftStateMachine;
        private readonly StateMachine<State, Trigger>.TriggerWithParameters<int> _callTrigger;

        #region Lift public methods

        public Lift()
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

        // Moves the lift to the specified floor
        public void MoveTo(int floor)
        {
            _liftStateMachine.Fire(_callTrigger, floor);
        }

        // Returns a value indicating whether the doors are open
        public bool AreDoorsOpen()
        {
            return _doorsOpen;
        }

        #endregion

        #region Lift private methods

        private void OpenDoors()
        {
            _doorsOpen = true;
        }

        private void CloseDoors()
        {
            _doorsOpen = false;
        }

        // Moves the lift to the specified floor
        private void Move(int floor)
        {   
            var moved = false;

            if (AreDoorsOpen())
            {
                // close doors before moving
                CloseDoors();
            }

            if (floor > _currentFloor)
            {
                moved = AscendTo(floor);
            }
            else if(floor < _currentFloor)
            {
                moved = DescendTo(floor);
            }
            else
            {
                // already at current floor
                moved = true;
            }

            if (moved)
            {
                _liftStateMachine.Fire(Trigger.Arrived);
            }
        }

        private bool DescendTo(int desiredFloor)
        {
            if (_doorsOpen)
            {
                _liftStateMachine.Fire(Trigger.Malfunction);
                return false;
            }

            // this simulates the time taken for the lift to move a single floor
            var timeDelay = 100;
            Task.Delay(timeDelay).Wait();
            if (_currentFloor != desiredFloor)
            {
                _currentFloor = _currentFloor - 1;
                DescendTo(desiredFloor);
            }

            return true;
        }

        private bool AscendTo(int desiredFloor)
        {
            if (_doorsOpen)
            {
                _liftStateMachine.Fire(Trigger.Malfunction);
                return false;
            }

            // this simulates the time taken for the lift to move
            var timeDelay = 100;
            Task.Delay(timeDelay).Wait();
            if (_currentFloor != desiredFloor)
            {
                _currentFloor = _currentFloor + 1;
                AscendTo(desiredFloor);
            }

            return true;
        }

        #endregion
    }
}