using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWebApplication.Models;

namespace TestWebApplication.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext context;

        public TransactionController(ApplicationDbContext context)
        {
            this.context = context;
        }

        #region GET REPORT
        public async Task<JsonResult> GetReport(DateTime startDate, DateTime endDate, int? accountId)
        {
            int status;
            string message = string.Empty;
            List<Transaction> model = new List<Transaction>();
            try
            {
                model = await context.Transactions.Include(x => x.Customer).Where(x => x.TransactionDate >= startDate && x.TransactionDate <= endDate).ToListAsync();
                if (accountId.HasValue)
                {
                    model = model.Where(x => x.CustomerId == accountId.Value).ToList();
                }
                status = 200;
            }
            catch (Exception ex)
            {
                status = 500;
                message = ex.Message;
            }

            return Json(new { status, message, list = model });
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await context.Transactions.Include(x => x.Customer).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<object> list = new List<object>();
            foreach (var c in await context.Customers.OrderBy(x => x.Name).ToListAsync())
            {
                list.Add(new
                {
                    c.Id,
                    Name = $"{c.Name} (Account ID: {c.Id})"
                });
            }
            ViewBag.Customer = new SelectList(list, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Transaction model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    #region POINT
                    var customer = await context.Customers.FindAsync(model.CustomerId);
                    if (customer != null)
                    {
                        decimal custPoint = customer.TotalPoint;
                        if (model.Description == "Beli Pulsa")
                        {
                            if (model.Amount > 10000 && model.Amount <= 30000)
                            {
                                custPoint += (model.Amount / 1000) * 1;
                            }
                            else if (model.Amount > 30000)
                            {
                                custPoint += (model.Amount / 1000) * 2;
                            }
                        }
                        else if (model.Description == "Bayar Listrik")
                        {
                            if (model.Amount > 50000 && model.Amount <= 100000)
                            {
                                custPoint += (model.Amount / 2000) * 1;
                            }
                            else if (model.Amount > 100000)
                            {
                                custPoint += (model.Amount / 2000) * 2;
                            }
                        }

                        customer.TotalPoint = custPoint;
                        context.Customers.Update(customer);
                    }
                    #endregion

                    context.Transactions.Add(model);
                    await context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            List<object> list = new List<object>();
            foreach (var c in await context.Customers.OrderBy(x => x.Name).ToListAsync())
            {
                list.Add(new
                {
                    c.Id,
                    Name = $"{c.Name} (Account ID: {c.Id})"
                });
            }
            ViewBag.Customer = new SelectList(list, "Id", "Name");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Report()
        {
            List<object> list = new List<object>();
            foreach (var c in await context.Customers.OrderBy(x => x.Name).ToListAsync())
            {
                list.Add(new
                {
                    c.Id,
                    Name = $"{c.Name} (Account ID: {c.Id})"
                });
            }
            ViewBag.Customer = new SelectList(list, "Id", "Name");
            return View();
        }
    }
}
