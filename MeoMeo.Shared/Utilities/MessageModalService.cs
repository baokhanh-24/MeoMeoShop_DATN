
using System.Threading.Tasks;
using AntDesign;
using Microsoft.AspNetCore.Components;

namespace MeoMeo.Shared.Utilities;
public class MessageModalService
{
    private readonly NotificationService _notification;

    public MessageModalService(NotificationService notification)
    {
        _notification = notification;
    }

    public async Task Success(string content, int duration = 3)
    {
        await _notification.Open(new NotificationConfig
        {
            Placement = NotificationPlacement.BottomRight,
            Message = "Thành công",
            Description = content,
            Icon = RenderIcon("/images/actions/success.svg"),
            Duration = duration
        });
    }

    public async Task Error(string content, int duration = 3)
    {
        await _notification.Open(new NotificationConfig
        {
            Placement = NotificationPlacement.BottomRight,
            Message = "Thất bại",
            Description = content,
            Icon = RenderIcon("/images/actions/error.svg"),
            Duration = duration
        });
    }

    public async Task Info(string content, int duration = 3)
    {
        await _notification.Open(new NotificationConfig
        {
            Placement = NotificationPlacement.BottomRight,
            Message = "Thông tin",
            Description = content,
            Icon = builder =>
            {
                builder.OpenComponent<Icon>(0);
                builder.AddAttribute(1, "Type", "info-circle");
                builder.AddAttribute(2, "Theme", "filled");
                builder.AddAttribute(3, "Style", "color: #1890ff");
                builder.CloseComponent();
            },
            Duration = duration
        });
    }

    public async Task Warning(string content, int duration = 3)
    {
        await _notification.Open(new NotificationConfig
        {
            Placement = NotificationPlacement.BottomRight,
            Message = "Cảnh báo",
            Description = content,
            Icon = RenderIcon("/images/actions/warning.svg"),
            Duration = duration
        });
    }
    private RenderFragment RenderIcon(string src) => builder =>
    {
        builder.OpenElement(0, "img");
        builder.AddAttribute(1, "src", src);
        builder.CloseElement();
    };
    
}