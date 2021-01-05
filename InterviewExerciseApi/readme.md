# Great State interview exercise

## Overview

This code is simulating a bank of three lifts in a hotel lobby.

There are two public APIs which can be called to interact with the lift system.

## The approach

The approach is deliberately incomplete and may be sub-optimal.

There are two public API endpoints, one for calling a lift, the other for getting the status of all lifts.


All persistence happens in-memory - no database has been implemented.

The lift controller contains the business logic for attempting to summon the closest lift.
The lifts themselves encapsulate some of their logic inside a state machine.

The state machine configuration determines which 'state' the lift transitions into (e.g. stopped, moving) and 
what triggers the state transition (e.g. lift has been called, lift has arrived).

## Your tasks

x 1. Talk us through a code review of the 'Call()' method in LiftController.cs. Some areas to think about:
    - Is it readable?
    - Are there any obvious code smells?
    - How else would you improve it?
    
x 2. Talk us through some examples of unit tests you would write for the 'Call()' method mentioned in the previous question?

x 3. Implement the unit tests discussed in the previous question.

4. Identify the problem causing lifts to malfunction after they have been called once, and fix it.

5. Implement a new type of ILift that instantly transports to the correct floor, and inject it into the LiftController.

6. Implement a front end that displays the call buttons and the current lift statuses.