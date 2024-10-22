using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using OrderTaker.Data;
using OrderTaker.Models;

namespace OrderTaker.Controllers
{
    public class SkusController : Controller
    {
        private readonly OrderTakerContext _context;

        public SkusController(OrderTakerContext context)
        {
            _context = context;
        }

        // GET: Skus
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sku.IgnoreQueryFilters().ToListAsync());
        }

        // GET: Skus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sku = await _context.Sku.IgnoreQueryFilters()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sku == null)
            {
                return NotFound();
            }

            return Json(sku);
        }

        // POST: Skus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,Code,UnitPrice,IsActive,ImagePath,Id,CreatedDate,CreatedBy,Timestamp,IsDeleted")] Sku sku, IFormFile imagePath)
        {

            if (!SkuExists(sku))
            {

                if (imagePath != null && imagePath.Length > 0)
                {
                    // Create a unique filename and path to save the image
                    var fileName = Path.GetFileNameWithoutExtension(imagePath.FileName);
                    var extension = Path.GetExtension(imagePath.FileName);
                    
                    var newFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
                    var imagePathInServer = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/skus", newFileName);
                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/skus")))
                    {
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/skus"));
                    }
                    using (var stream = new FileStream(imagePathInServer, FileMode.Create))
                    {
                        await imagePath.CopyToAsync(stream);
                    }

                    sku.ImagePath = "/images/skus/" + newFileName;

                    sku.CreatedBy = "System";
                    _context.Add(sku);
                    await _context.SaveChangesAsync();
                    return Json(new { status = "success", message = "SKU created successfully!" });
                }
                else
                {
                    return Json(new { status = "error", message = "Image requireed!" });
                }
            }
            else
            {
                return Json(new { status = "error", message = "SKU Name/Code already exists!" });
            }

        }

        // POST: Skus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Code,UnitPrice,IsActive,ImagePath,Id,CreatedDate,CreatedBy,Timestamp,IsDeleted")] Sku sku, IFormFile imagePath)
        {

            if (!SkuExists(sku))
            {
                var currentSku = await _context.Sku.FindAsync(sku.Id);
                if (currentSku != null)
                {
                    currentSku.UnitPrice = sku.UnitPrice;
                    currentSku.Code = sku.Code;
                    currentSku.Name = sku.Name;
                    currentSku.IsActive = sku.IsActive;
                    if (imagePath != null && imagePath.Length > 0)
                    {
                        // Create a unique filename and path to save the image
                        var fileName = Path.GetFileNameWithoutExtension(imagePath.FileName);
                        var extension = Path.GetExtension(imagePath.FileName);
                        var newFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
                        var imagePathInServer = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/skus", newFileName);

                        using (var stream = new FileStream(imagePathInServer, FileMode.Create))
                        {
                            await imagePath.CopyToAsync(stream);
                        }

                        currentSku.ImagePath = "/images/skus/" + newFileName;
                    }
                    _context.Update(currentSku);
                    await _context.SaveChangesAsync();
                    return Json(new { status = "success", message = "SKU successfully updated!" });
                }
                else
                {
                    return Json(new { status = "error", message = "SKU Name/Code already exists!" });
                }
            }
            else
            {
                return Json(new { status = "error", message = "Concurrency error/SKU does not exists!" });
            }
        }


        // POST: Skus/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sku = await _context.Sku.FindAsync(id);
            if (sku != null)
            {
                _context.Sku.Remove(sku);
            }

            await _context.SaveChangesAsync();
            return Json(new { status = "success", message = "SKU successfully deleted!" });
        }

        private bool SkuExists(int id)
        {
            return _context.Sku.Any(e => e.Id == id);
        }

        public bool SkuExists(Sku sku)
        {
            return _context.Sku.Any(x => x.Id != sku.Id && (sku.Code == x.Code || sku.Name == x.Name));
        }
    }
}
