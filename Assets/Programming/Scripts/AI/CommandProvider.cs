using UnityEngine;

public interface CommandProvider
{
    //returns list of commands
    //contains all commands that should be provided/implemented
    
    public Command[] ProvideCommands();
}
