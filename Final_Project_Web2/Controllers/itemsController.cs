using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Final_Project_Web2.Data;
using Final_Project_Web2.Models;
using Microsoft.Data.SqlClient;

namespace Final_Project_Web2.Controllers
{
    public class itemsController : Controller
    {
        private readonly Final_Project_Web2Context _context;

        public itemsController(Final_Project_Web2Context context)
        {
            _context = context;
        }
        // DashBoard
        public async Task<IActionResult> DashBoard()
        {

            string role = HttpContext.Session.GetString("Role");
            if (role != "admin")
            {
                return RedirectToAction("Login", "UsersAccounts");
            }
            string sql = "";

                var builder = WebApplication.CreateBuilder();
                string conStr = builder.Configuration.GetConnectionString("Final_Project_Web2Context");
                SqlConnection conn = new SqlConnection(conStr);

                SqlCommand comm;
                conn.Open();
                sql = "SELECT COUNT(Id) FROM Items where category =1";
                comm = new SqlCommand(sql, conn);
                ViewData["d1"] = (int)comm.ExecuteScalar();
                sql = "SELECT COUNT( Id) FROM Items where category =2";
                comm = new SqlCommand(sql, conn);
                ViewData["d2"] = (int)comm.ExecuteScalar();

                return View();
            
        }

        // GET: items
        public async Task<IActionResult> Index()
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "admin")
            {
                return RedirectToAction("Login", "UsersAccounts");
            }
            ViewData["role"] = role;
            return View(await _context.items.ToListAsync());
        }

        // GET: items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["role"] = HttpContext.Session.GetString("Role");
            if (id == null)
            {
                return NotFound();
            }

            var items = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // GET: items/Create
        public IActionResult Create()
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "admin")
            {
                return RedirectToAction("Login", "UsersAccounts");
            }
            return View();
        }

        // POST: items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file, [Bind("Id,name,description,price,discount,makedate,category,quantity,imgfile")] items items)
        {
            {
    
                if (file != null)
                {
                    string filename = file.FileName;
                    string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"));
                    using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    { await file.CopyToAsync(filestream); }

                    items.imgfile = filename;
                }
                _context.Add(items);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "admin")
            {
                return RedirectToAction("Login", "UsersAccounts");
            }
            if (id == null)
            {
                return NotFound();
            }

            var items = await _context.items.FindAsync(id);
            if (items == null)
            {
                return NotFound();
            }
            return View(items);
        }

        // POST: items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,description,price,discount,makedate,category,quantity,imgfile")] items items)
        {
            if (id != items.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(items);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!itemsExists(items.Id))
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
            return View(items);
        }

        // GET: items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "admin")
            {
                return RedirectToAction("Login", "UsersAccounts");
            }
            ViewData["role"] = role;
            if (id == null)
            {
                return NotFound();
            }

            var items = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // POST: items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var items = await _context.items.FindAsync(id);
            if (items != null)
            {
                _context.items.Remove(items);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool itemsExists(int id)
        {
            return _context.items.Any(e => e.Id == id);
        }

        // item list
        public async Task<IActionResult> List()
        {
            ViewData["role"] = HttpContext.Session.GetString("Role");


            var items = await _context.items.OrderBy(item => item.category).ToListAsync();
            return View(items);

        }
    }
}
