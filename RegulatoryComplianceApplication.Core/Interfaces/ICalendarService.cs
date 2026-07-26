using RegulatoryComplianceApplication.Core.Models;

namespace RegulatoryComplianceApplication.Core.Interfaces
{
    public interface ICalendarService
    {
        Task<List<CalendarEvent>> GetEventsAsync();
    }
}