using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Threading.Tasks;

namespace AdminDesktop.Controls.General;

public static class MessageBoxService
{
    public static Task ShowErrorAsync(string message)
    {
        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            "Ошибка загрузки данных",
            message,
            ButtonEnum.Ok);

        return messageBox.ShowAsync();
    }
}
