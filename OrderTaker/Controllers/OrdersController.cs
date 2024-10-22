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
    public class OrdersController : Controller
    {
        private readonly OrderTakerContext _context;

        public OrdersController(OrderTakerContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orderTakerContext = _context.Order.Include(o => o.Customer);
            return View(await orderTakerContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public async Task<IActionResult> CreateAsync()
        {

            // Create the ViewModel
            var viewModel = new OrderViewModel
            {
                Order = new Order(),
                CustomerList = await _context.Customer.ToListAsync(),
                SkuList = await _context.Sku.ToListAsync(),

            };

            return View(viewModel);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel orderViewModel)
        {
            var order = new Order
            {
                CustomerId = orderViewModel.Order.CustomerId,
                DeliveryDate = orderViewModel.Order.DeliveryDate,
                Status = orderViewModel.Order.Status,
                TotalAmount = orderViewModel.Order.TotalAmount,
                OrderItems = new List<OrderItem>(),
            };

            if(orderViewModel.OrderItems == null)
            {
                return Json(new { status = "error", message = "Please add an order item." });
            }

            // Add order items
            foreach (var item in orderViewModel.OrderItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    SkuId = item.SkuId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    CreatedBy="System",
                    IsActive = true
                });
            }

            order.CreatedBy = "System";
            order.IsActive = true;

            _context.Add(order);
            await _context.SaveChangesAsync();
            return Json(new { status = "success", redirectUrl = Url.Action(nameof(Index)) });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Order.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            var viewModel = new OrderViewModel
            {
                Order = order,
                CustomerList = await _context.Customer.ToListAsync(),
                SkuList = await _context.Sku.ToListAsync(),
                OrderItems = order.OrderItems.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderViewModel model)
        {
            var currentOrder = await _context.Order.Include(x=>x.OrderItems).FirstOrDefaultAsync(x=>x.Id==id);
            if (currentOrder == null)
            {
                return Json(new { status = "error", message = "Order does not exist." });
            }
            currentOrder.DeliveryDate = model.Order.DeliveryDate;
            currentOrder.Status = model.Order.Status;
            currentOrder.TotalAmount = model.Order.TotalAmount;

            _context.OrderItem.RemoveRange(currentOrder.OrderItems);

            if (model.OrderItems == null)
            {
                return Json(new { status = "error", message = "Please add an order item." });
            }
            foreach (var item in model.OrderItems)
            {
                currentOrder.OrderItems.Add(new OrderItem
                {
                    SkuId = item.SkuId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                });
            }

            _context.Update(currentOrder);
            await _context.SaveChangesAsync();
            return Json(new { status = "success", redirectUrl = Url.Action(nameof(Index)) });
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}
