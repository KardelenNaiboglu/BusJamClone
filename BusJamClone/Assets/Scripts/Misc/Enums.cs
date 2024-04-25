public enum GridSlotState
{
    Empty = 0,
    Disabled = 1,
    Occupied = 2
}

public enum GridSlotObjectType
{
    Passenger = 0,
}

public enum PassengerType
{
    Red = 0,
    Green = 1,
    Blue = 2,
    Yellow = 3
}

public enum GameState
{
    Initializing = 0,
    ReadyToStart = 1,
    Playing = 2,
    Win = 3,
    Fail = 4,
}