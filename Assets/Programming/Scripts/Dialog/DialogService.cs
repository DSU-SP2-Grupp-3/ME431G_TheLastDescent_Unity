using UnityEngine;

public class DialogService : Service<DialogService>
{
    private Locator<DialogService> dialogService;
    void Start()
    {
        Register();
    }
}
