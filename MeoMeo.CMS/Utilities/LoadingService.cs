namespace MeoMeo.Utilities;

public class LoadingService
{
    public event Func<Task>? OnChange;
    public bool IsLoading { get; private set; }

    public async Task StartAsync()
    {
        IsLoading = true;
        if (OnChange != null)
            await OnChange.Invoke();
    }

    public async Task StopAsync()
    {
        IsLoading = false;
        if (OnChange != null)
            await OnChange.Invoke();
    }
}

