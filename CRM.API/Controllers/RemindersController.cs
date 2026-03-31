using CRM.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
[Route("api/[controller]")]
[ApiController]
public class RemindersController : ControllerBase
{
    private readonly AppDbContext _context;

    public RemindersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetReminders()
    {
        return Ok(await _context.Reminders.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> AddReminder(Reminder r)
    {
        _context.Reminders.Add(r);

        // ALSO CREATE NOTIFICATION
        _context.Notifications.Add(new Notification
        {
            Message = "Reminder set: " + r.Message
        });

        await _context.SaveChangesAsync();

        return Ok();
    }
}