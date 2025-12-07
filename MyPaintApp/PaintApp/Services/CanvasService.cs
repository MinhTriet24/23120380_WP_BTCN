using Microsoft.EntityFrameworkCore;
using PaintApp_Data.Context;
using PaintApp_Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaintApp.Services
{
    public class CanvasService
    {
        private readonly AppDbContext _context;

        public CanvasService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveCanvasAsync(DrawingCanvas canvas)
        {
            if (canvas.Id == 0)
                _context.DrawingCanvases.Add(canvas);
            else
                _context.DrawingCanvases.Update(canvas);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCanvasAsync(int id)
        {
            var item = await _context.DrawingCanvases.FindAsync(id);
            if (item != null)
            {
                _context.DrawingCanvases.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddTemplateAsync(ShapeTemplate template)
        {
            _context.ShapeTemplates.Add(template);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ShapeTemplate>> GetAllTemplatesAsync()
        {
            return await _context.ShapeTemplates.ToListAsync();
        }

        public async Task DeleteTemplateAsync(int id)
        {
            var item = await _context.ShapeTemplates.FindAsync(id);
            if (item != null)
            {
                _context.ShapeTemplates.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<DrawingCanvas>> GetAllCanvasesAsync()
        {
            return await _context.DrawingCanvases.AsNoTracking().OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task<DrawingCanvas> GetCanvasByIdAsync(int id)
        {
            return await _context.DrawingCanvases.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}