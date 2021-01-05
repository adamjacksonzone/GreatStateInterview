namespace InterviewExerciseApi.Tests.Controllers
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using LiftEngine;
    using FluentAssertions;

    // the naming convention used here aids order and grouping of appearance in tests runners.
    // would prefer xunit (less attributes needed) but was playing up with my local copy of vs so switched to ms test
            
    public class Controller_LiftTests
    {
        private const string OUT_OF_SERVICE = "OutOfService";
        private const string STOPPED = "Stopped";        

        [TestClass]
        public class CallShould : Controller_LiftTests
        {
            [TestMethod]
            public void ReturnTrueLiftOnCurrentFloor()
            {
                // arrange
                int currentFloor = 1;
                var mockLift1 = new Mock<ILift>();
                mockLift1.Setup(x => x.CurrentFloor()).Returns(currentFloor-1);
                mockLift1.Setup(x => x.Status).Returns(OUT_OF_SERVICE);

                var mockLift2 = new Mock<ILift>();
                mockLift2.Setup(x => x.CurrentFloor()).Returns(currentFloor+1);
                mockLift2.Setup(x => x.Status).Returns(OUT_OF_SERVICE);

                var mockLift3 = new Mock<ILift>();
                mockLift3.Setup(x => x.CurrentFloor()).Returns(currentFloor);
                mockLift3.Setup(x => x.Status).Returns(STOPPED);

                var mockExpressElevator = new Mock<ILift>();
                mockExpressElevator.Setup(x => x.CurrentFloor()).Returns(currentFloor+2);
                mockExpressElevator.Setup(x => x.Status).Returns(OUT_OF_SERVICE);

                var controller = new LiftController(mockLift1.Object, mockLift2.Object, mockLift3.Object, mockExpressElevator.Object);

                // act 
                var response = controller.Call(currentFloor);
                
                // assert - not alot to check here given the implementation, i would normally check types and service http result etc
                response.Should().BeTrue();

                // verify                
                mockLift1.Verify(x => x.Status, Times.AtLeastOnce);
                mockLift2.Verify(x => x.Status, Times.AtLeastOnce);
                mockLift3.Verify(x => x.Status, Times.AtLeastOnce);
                mockExpressElevator.Verify(x => x.Status, Times.AtLeastOnce);
            }

            // wont write all the other tests but will outline the case scenarios.  If any arrange setup is shared I would look at helpers or base controllers where appropriate.

            //public void ReturnTrueNoLiftOnFloor()            

            //public void ReturnTrueLiftDescendToFloor()

            //public void ReturnTrueLiftAscendToFloor();

            //public void ReturnFalseAllOutOfService()

            //public void ReturnTrueExpressElevatorMovesFromBottomToTopFloorInLessThan110MS()
        }

        // move on to the next action 

        public class GetStatusShould : Controller_LiftTests
        {
            //etc...
        }        

    }
}
