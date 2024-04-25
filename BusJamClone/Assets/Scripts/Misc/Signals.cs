using UnityEngine;

public struct FingerDownSignal
{
    public Vector3 Position;

    public FingerDownSignal(Vector3 position)
    {
        Position = position;
    }
}

public struct FingerUpdateSignal
{
    public Vector3 Position;

    public FingerUpdateSignal(Vector3 position)
    {
        Position = position;
    }
}

public struct FingerUpSignal
{
    public Vector3 Position;

    public FingerUpSignal(Vector3 position)
    {
        Position = position;
    }
}

public struct GameStateChangedSignal
{
    public GameState GameState;

    public GameStateChangedSignal(GameState gameState)
    {
        GameState = gameState;
    }
}

public struct TimerChangedSignal
{
    public int TimeInSeconds;

    public TimerChangedSignal(int timeInSeconds)
    {
        TimeInSeconds = timeInSeconds;
    }
}

public struct TimesUpSignal
{
}

public struct SavedGameLoadedSignal
{
}