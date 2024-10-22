using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrderTaker.Data;
using OrderTaker.Models;

namespace OrderTaker.Controllers
{
    public class CustomersController : Controller
    {
        private readonly OrderTakerContext _context;

        public CustomersController(OrderTakerContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customer.IgnoreQueryFilters().ToListAsync());
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,MobileNumber,City,Id,CreatedDate,CreatedBy,Timestamp,IsDeleted,IsActive")] Customer customer)
        {
            if (!CustomerExists(customer))
            {
                customer.CreatedDate = DateTime.Now;
                customer.CreatedBy = "System";
                customer.FullName = $"{customer.FirstName} {customer.LastName}";
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return Json(new { status = "success", message = "Customer updated successfully!" });
            }
            return Json(new { status = "error", message = "Customer already exists." });
        }

        // GET: Skus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Json(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("FirstName,LastName,MobileNumber,City,Id,IsActive,CreatedDate,CreatedBy,Timestamp,IsDeleted")] Customer customer)
        {
            if (id != customer.Id)
            {
                return Json(new { status = "error", message = "Customer not found." });
            }
            if (CustomerExists(customer))
            {
                return Json(new { status = "error", message = "First name or last name already exists." });
            }

            var currentCustomer = await _context.Customer.FindAsync(customer.Id);
            if (currentCustomer!=null)
            {
                customer.FullName = $"{customer.FirstName} {customer.LastName}";
                currentCustomer.FirstName = customer.FirstName;
                currentCustomer.LastName = customer.LastName;
                currentCustomer.MobileNumber = customer.MobileNumber;
                currentCustomer.FullName = customer.FullName;
                currentCustomer.IsActive = customer.IsActive;
                try
                {
                    _context.Update(currentCustomer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return Json(new { status = "error", message = "Customer no longer exists." });
                    }
                    else
                    {
                        return Json(new { status = "error", message = "Concurrency error occurred." });
                    }
                }
                return Json(new { status = "success", message = "Customer updated successfully!" });
            }
            else
            {
                return Json(new { status = "error", message = "Customer no longer exists." });
            }
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<string> DeleteConfirmed(int id)
        {
            var customer = await _context.Customer.FindAsync(id);
            if (customer != null)
            {
                customer.IsActive = false;
            }

            await _context.SaveChangesAsync();
            return "success";
        }

        private bool CustomerExists(int id)
        {
            return _context.Customer.Any(e => e.Id == id);
        }
        public bool CustomerExists(Customer customer)
        {
            return _context.Customer.Any(c => c.Id != customer.Id && c.FirstName == customer.FirstName && c.LastName == customer.LastName);
        }
    }
}
