using CRM.API.Data;
using CRM.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRM.API.DTOs;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly AppDbContext _context;

    public NotesController(AppDbContext context)
    {
        _context = context;
    }

    // Add note to a customer
    [HttpPost]
    public async Task<IActionResult> CreateNote(CreateNoteDTO dto)
    {
        var note = new Note
        {
            CustomerId = dto.CustomerId,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notes.Add(note);
        await _context.SaveChangesAsync();

        return Ok(note);
    }
    [HttpGet]
    public async Task<IActionResult> GetNotes()
    {
        var notes = await _context.Notes.ToListAsync();
        return Ok(notes);
    }
    // Get notes for a customer (timeline)
    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetByCustomer(int customerId)
    {
        var notes = await _context.Notes
            .Where(n => n.CustomerId == customerId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return Ok(notes);
    }

    // Delete note (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note == null) return NotFound();

        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();
        return Ok("Note deleted");
    }
}