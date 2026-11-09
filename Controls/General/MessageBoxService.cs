using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Threading.Tasks;

namespace AdminDesktop.Controls.General;

public static class MessageBoxService
{
    public static Task ShowErrorAsync(string title, string message)
    {
        return ShowStandardAsync(title, message);
    }

    public static Task ShowInfoAsync(string title, string message)
    {
        return ShowStandardAsync(title, message);
    }

    private static async Task ShowStandardAsync(string title, string message)
    {
        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            title,
            message,
            ButtonEnum.Ok);

        await messageBox.ShowAsync();
    }
}
