using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RegulatoryComplianceApplication.Core.Interfaces;

namespace RegulatoryComplianceApplication.ViewComponents
{
    public class NotificationBellViewComponent : ViewComponent
    {
        private readonly INotificationService _notificationService;

        public NotificationBellViewComponent(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? false)
            {
                return View(0);
            }

            var userId = int.Parse(
                UserClaimsPrincipal.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);

            var unreadCount =
                await _notificationService.GetUnreadCountAsync(userId);

            return View(unreadCount);
        }
    }
}