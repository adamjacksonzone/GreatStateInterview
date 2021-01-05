// personally I store usings within the namespace, but whatever is the company convention should be applied.
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

// not particularly descriptive name space as per below, would rename this to ElevatorEngine
namespace InterviewExerciseApi.LiftEngine
{
    // Responsible for coordinating the individual lifts.

    // might want to think about introducing versioning in your routing.
    public class LiftController : ILiftController
    {
        private static ILift _lift1;
        private static ILift _lift2;
        private static ILift _lift3;

        // as per above, rename to _elevators
        private List<ILift> _lifts = new List<ILift>();

        // what is the purpose of this controller, need some description and context added as comments
        // what happens if the lift1/2/3 are null referenced, theres no allowance for exceptions.
        // as per above lifts is not a very descriptive name, perhaps elevator would be better - no ambiguity
        public LiftController(ILift lift1, ILift lift2, ILift lift3, ILift expressElevator)
        {
            // you have static properties but you are already adding this class as a singleton so its unnecesary abstraction, there will only 
            // be one instance of this class, so to for the members.
            _lift1 = lift1;
            _lift2 = lift2;
            _lift3 = lift3;

            // you could just populate this directly from method arguments (unnecessary extra step above)            
            _lifts.Add(_lift1);
            _lifts.Add(_lift2);
            _lifts.Add(_lift3);
            _lifts.Add(expressElevator);
        }

        public LiftController()
        {
        }

        // not typed the return - which looks to be of type List<Dto>
        // method name not very descriptive, get status of what - could be offset by ensuring your url structure reflects this
        // not limited the expected call http type to get
        // Returns some status information about each lift        
        public object GetStatus()
        {
            var data = new List<LiftDto>();

            foreach (var lift in _lifts)
            {
                data.Add(new LiftDto {
                    Floor = lift.CurrentFloor(),
                    Status = lift.Status
                });
            }

            return data;
        }

        // not very descriptive method name, something like MoveToFloor would be more appropriate
        // simple non optional argument type so a url segment is approrpriate, include attribute for http type and routing
        // would include response return types in commenting
        // would include attribute references for use with swagger
        // Calls a lift to current        
        public bool Call(int floor)
        {
            // not handling OutOfService state
            // should probably return http 503 service unavailable
            // non debugging comments should be removed
            // if lifts are all out of order
            //  return false;            

            //how are unexpected exception handled - no graceful handling here.
            //think about setting up a base controller for handling common functions such as error handling
            //where you can pass the state up, maybe two abstract methods of success<T> and failure<T>.  You can then handle the
            //appropriate success states like 200 (in this case) or 201 if created object.

            // your assuming lifts will always be populated, safe on current execution but might be worth thinking about including null checks given the 
            // use of first and in case there are future changes to the constructor or lifecycle.
            // you have a magic string "OutOfService" - move to constant.
            var liftToCall = _lifts.Where(lift => lift.Status != "OutOfService").OrderBy(lift => Math.Abs(lift.CurrentFloor() - floor)).First();

            // do you need to check floor is a valid value for the building?
            liftToCall.MoveTo(floor);
            return true;
        }
    }

    public class LiftDto
    {     
        public int Floor;     
        public string Status;
    }
}
