using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Final_Project_Web2.Data;
using Final_Project_Web2.Models;
using System.Text.Json;

namespace Final_Project_Web2.Controllers
{
    public class ordersController : Controller
    {
        private readonly Final_Project_Web2Context _context;

        public ordersController(Final_Project_Web2Context context)
        {
            _context = context;
        }

        // Catalogue
        public async Task<IActionResult> CatalogueBuy()

        {
            ViewData["role"] = HttpContext.Session.GetString("Role");
            string ctname = HttpContext.Session.GetString("Name");
            return View(await _context.items.ToListAsync());

        }
        // BuyDetail
        public async Task<IActionResult> BuyDetail(int? id)
        {
            ViewData["role"] = HttpContext.Session.GetString("Role");
            var item = await _context.items.FindAsync(id);
            return View(item);
        }

        // cartadd
        List<buyitems> itemlist = new List<buyitems>();
        [HttpPost]
        public async Task<IActionResult> cartadd(int Id, int quantity)
        {
            await HttpContext.Session.LoadAsync();

            ViewData["role"] = HttpContext.Session.GetString("Role");

            await HttpContext.Session.LoadAsync();
            var sessionString = HttpContext.Session.GetString("Cart");
            if (sessionString is not null)
            {
                itemlist = JsonSerializer.Deserialize<List<buyitems>>(sessionString);
            }

            var item = await _context.items.FromSqlRaw("select * from items  where Id= '" + Id + "'  ").FirstOrDefaultAsync();

            itemlist.Add(new buyitems
            {
                name = item.name,
                Price = item.price,
                quant = quantity
            });

            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(itemlist));
            return RedirectToAction("CartBuy");
        }
        // CartBuy
        public async Task<IActionResult> CartBuy()
        {
            await HttpContext.Session.LoadAsync();

            ViewData["role"] = HttpContext.Session.GetString("Role");


            await HttpContext.Session.LoadAsync();
            var sessionString = HttpContext.Session.GetString("Cart");
            if (sessionString is not null)
            {
                itemlist = JsonSerializer.Deserialize<List<buyitems>>(sessionString);
            }
            return View(itemlist);
        }

        // Buy
        public async Task<IActionResult> Buy()
        {
            await HttpContext.Session.LoadAsync();
            ViewData["role"] = HttpContext.Session.GetString("Role");


            var sessionString = HttpContext.Session.GetString("Cart");
            if (sessionString is not null)
            {
                itemlist = JsonSerializer.Deserialize<List<buyitems>>(sessionString);
            }

            string ctname = HttpContext.Session.GetString("Name");
            orders itemorder = new orders();
            itemorder.total = 0;
            itemorder.custname = ctname;
            itemorder.orderdate = DateTime.Today;
            _context.orders.Add(itemorder);
            await _context.SaveChangesAsync();
            var bord = await _context.orders.FromSqlRaw("select * from orders  where custname= '" + ctname + "' ").OrderByDescending(e => e.Id).FirstOrDefaultAsync();
            int ordid = bord.Id;
            decimal tot = 0;
            foreach (var bk in itemlist.ToList())
            {
                orderline oline = new orderline();
                oline.orderid = ordid;
                oline.itemname = bk.name;
                oline.itemquant = bk.quant;
                oline.itemprice = (int)bk.Price;
                _context.orderline.Add(oline);
                await _context.SaveChangesAsync();
                var bkk = await _context.items.FromSqlRaw("select * from items  where name= '" + bk.name + "' ").FirstOrDefaultAsync();
                bkk.quantity = bkk.quantity - bk.quant;
                _context.Update(bkk);
                await _context.SaveChangesAsync();

                tot = tot + (bk.quant * bk.Price);
            }
            bord.total = Convert.ToInt16(tot);
            _context.Update(bord);
            await _context.SaveChangesAsync();

            ViewData["Message"] = "Thank you";
            itemlist = new List<buyitems>();
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(itemlist));
            return RedirectToAction("MyOrder");
        }
        // MyOrder
        public async Task<IActionResult> MyOrder(string custname)
        {
            await HttpContext.Session.LoadAsync();
            string ss = HttpContext.Session.GetString("Name");
            ViewData["role"] = HttpContext.Session.GetString("Role");

            return View(await _context.orders.FromSqlRaw("select * from orders  where custname = '" + ss + "' ").ToListAsync());
        }

        // GET: orders
        public async Task<IActionResult> Index()
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "admin")
            {
                return RedirectToAction("Login", "UsersAccounts");
            }
            ViewData["role"] = role;
            var orItems = await _context.report.FromSqlRaw("SELECT custname, SUM(total) as total FROM orders GROUP BY custname  ").ToListAsync();
            return View(orItems);
        }
        public async Task<IActionResult> ordersdetail(string? custname)
        {
            var orItems = await _context.orders.FromSqlRaw("select * from orders  where  custname = '" + custname + "'  ").ToListAsync();
            return View(orItems);
        }
        // Orderline
        public async Task<IActionResult> Orderline(int? orid)
        {
            await HttpContext.Session.LoadAsync();

            ViewData["role"] = HttpContext.Session.GetString("Role");
            var buybk = await _context.orderline.FromSqlRaw("select * from orderline where  orderid = '" + orid + "'  ").ToListAsync();
            return View(buybk);
        }

        // GET: orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // GET: orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,custname,orderdate,total")] orders orders)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orders);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orders);
        }

        // GET: orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            return View(orders);
        }

        // POST: orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,custname,orderdate,total")] orders orders)
        {
            if (id != orders.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ordersExists(orders.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(orders);
        }

        // GET: orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orders = await _context.orders.FindAsync(id);
            if (orders != null)
            {
                _context.orders.Remove(orders);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ordersExists(int id)
        {
            return _context.orders.Any(e => e.Id == id);
        }
    }
}
