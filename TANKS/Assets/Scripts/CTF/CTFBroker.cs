using System;

public class CTFBroker 
{
    public static event Action FlagTaken;

    public static void CallFlagTaken()
    {
        if (FlagTaken != null)
        {
            FlagTaken();
        }
    }
}
