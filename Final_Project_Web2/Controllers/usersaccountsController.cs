using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Final_Project_Web2.Data;
using Final_Project_Web2.Models;
using System.Net.Mail;
using Microsoft.Data.SqlClient;


namespace Final_Project_Web2.Controllers
{
    public class usersaccountsController : Controller
    {
        private readonly Final_Project_Web2Context _context;

        public usersaccountsController(Final_Project_Web2Context context)
        {
            _context = context;
        }

        // login
        public IActionResult Login()

        {
            if (!HttpContext.Request.Cookies.ContainsKey("Name"))
                return View();
            else
            {
                string na = HttpContext.Request.Cookies["Name"].ToString();
                string ro = HttpContext.Request.Cookies["Role"].ToString();
                HttpContext.Session.SetString("Name", na);
                HttpContext.Session.SetString("Role", ro);
                return View();


            }
        }
        [HttpPost, ActionName("login")]
        public async Task<IActionResult> login(string na, string pa, string auto)
        {
            var ur = await _context.usersaccounts.FromSqlRaw("SELECT * FROM usersaccounts where name = '" + na + "' and pass = '" + pa + "' ").FirstOrDefaultAsync();
            if (ur != null)
            {
                int id = ur.Id;
                string na1 = ur.name;
                string ro = ur.role;
                HttpContext.Session.SetString("userid", Convert.ToString(id));
                HttpContext.Session.SetString("Name", na1);
                HttpContext.Session.SetString("Role", ro);
                if (auto == "on")
                {
                    HttpContext.Response.Cookies.Append("Name", na1);
                    HttpContext.Response.Cookies.Append("Role", ro);
                }
                if (ro == "customer")
                    return RedirectToAction("customerhome", "usersaccounts");
                else if (ro == "admin")
                    return RedirectToAction("adminhome", "usersaccounts");
                else
                    return View();
            }
            else
            {
                ViewData["Message"] = "wrong user name or password";
                return View();
            }
        }
        // GET: adminhome
        public async Task<IActionResult> adminhome()
        {
            HttpContext.Session.LoadAsync();
            string ss = HttpContext.Session.GetString("Role");
            if (ss == "admin")
            {
                ViewData["name"] = HttpContext.Session.GetString("Name");
                ViewData["role"] = HttpContext.Session.GetString("Role");
                return View();
            }
            else
                return RedirectToAction("login", "usersaccounts");
        }
        //customer
        public async Task<IActionResult> customerhome()
        {
            HttpContext.Session.LoadAsync();
            string ss = HttpContext.Session.GetString("Role");

            if (ss == "customer")
            {
                ViewData["name"] = HttpContext.Session.GetString("Name");
                ViewData["role"] = HttpContext.Session.GetString("Role");

                var discount = await _context.items
                    .Where(b => b.discount == "yes")
                    .ToListAsync();
                return View(discount);
            }
            else
                return RedirectToAction("login", "usersaccount");
        }

        //Email

        public IActionResult email()
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "admin")
            {
                return RedirectToAction("Login", "UsersAccounts"); 
            }
            return View();
        }
        [HttpPost, ActionName("email")]
        [ValidateAntiForgeryToken]
        public IActionResult email(string address, string subject, string body)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            var mail = new MailMessage();
            mail.From = new MailAddress("phix376@gmail.com");
            mail.To.Add(address); // receiver email address 
            mail.Subject = "Welcome to the phone store";
            mail.IsBodyHtml = true;
            mail.Body = body;
            SmtpServer.Port = 587;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential("fahadalsarani1@gmail.com", "QWERTasdfg321");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
            ViewData["Message"] = "Email sent !!";
            return View();
        }
        /// Search for Customer
        public async Task<IActionResult> Users_search()
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "admin")
            {
                return RedirectToAction("Login", "UsersAccounts");
            }
            usersaccounts custs = new usersaccounts();
                return View(custs);
            
        }
        [HttpPost]
        public async Task<IActionResult> Users_search(string name)
        {
            var uAcc = await _context.usersaccounts.FromSqlRaw("select * from usersaccounts where Name = '" + name + "' ").FirstOrDefaultAsync();

            return View(uAcc);
        }

        //addadmin
        public IActionResult AddAdmin()
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "admin")
            {
                return RedirectToAction("Login", "UsersAccounts");
            }
            return View();

        }
        [HttpPost]
        public IActionResult AddAdmin(string UserName, string Password, string ConfirmPassword)
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "admin")
            {
                return RedirectToAction("Login", "UsersAccounts");
            }

            if (Password != ConfirmPassword)
            {
                ViewData["Message"] = "Passwords do not match.";
                return View();
            }

            if (_context.usersaccounts.Any(uname => uname.name == UserName))
            {
                ViewData["Message"] = "Admin Name already exists.";
                return View();
            }
            usersaccounts userModel = new usersaccounts
            {
                name = UserName,
                pass = Password,
                role = "admin",
            };
            _context.usersaccounts.Add(userModel);
            _context.SaveChanges();



            return RedirectToAction(nameof(Index));
        }

        // Logout 
        public IActionResult Logout()

        {
            HttpContext.Session.Remove("Name");
            HttpContext.Session.Remove("Role");
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "usersaccounts");

        }
        // registration 
        public IActionResult registration()
        {
            return View();
        }
        [HttpPost]

        public IActionResult registration(string Name, string Password, string ConfirmPassword, string Job, string Email, bool Married, string Gender, string Location)
        {
            if (Password != ConfirmPassword)
            {
                ViewData["Message"] = "Passwords do not match";
                return View();
            }

            if (_context.usersaccounts.Any(u => u.name == Name))
            {
                ViewData["Message"] = "Username already exists";
                return View();
            }
          
                usersaccounts userModel = new usersaccounts
                {
                    name = Name,
                    pass = Password,
                    role = "customer"
                };

                _context.usersaccounts.Add(userModel);
                _context.SaveChanges();

                var builder = WebApplication.CreateBuilder();
                string conStr = builder.Configuration.GetConnectionString("Final_Project_Web2Context");
                SqlConnection conn = new SqlConnection(conStr);
                conn.Open();
                string sql;
                sql = "INSERT INTO customer (name, email, job, married, gender, location)  values  ('" + Name + "','" + Email + "','" + Job + "','" + Married + "' ,'" + Gender + "' ,'" + Location + "')";

                SqlCommand comm = new SqlCommand(sql, conn);
                comm.ExecuteNonQuery();
                conn.Close();
                return RedirectToAction(nameof(Login));
            
        }


        // GET: usersaccounts
        public async Task<IActionResult> Index()
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "admin")
            {
                return RedirectToAction("Login", "UsersAccounts");
            }
            return View(await _context.usersaccounts.ToListAsync());
        }

        // GET: usersaccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usersaccounts = await _context.usersaccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usersaccounts == null)
            {
                return NotFound();
            }

            return View(usersaccounts);
        }

        // GET: usersaccounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: usersaccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,pass,role")] usersaccounts usersaccounts)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usersaccounts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usersaccounts);
        }

        // GET: usersaccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usersaccounts = await _context.usersaccounts.FindAsync(id);
            if (usersaccounts == null)
            {
                return NotFound();
            }
            return View(usersaccounts);
        }

        // POST: usersaccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,pass,role")] usersaccounts usersaccounts)
        {
            if (id != usersaccounts.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usersaccounts);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!usersaccountsExists(usersaccounts.Id))
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
            return View(usersaccounts);
        }

        // GET: usersaccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usersaccounts = await _context.usersaccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usersaccounts == null)
            {
                return NotFound();
            }

            return View(usersaccounts);
        }

        // POST: usersaccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usersaccounts = await _context.usersaccounts.FindAsync(id);
            if (usersaccounts != null)
            {
                _context.usersaccounts.Remove(usersaccounts);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool usersaccountsExists(int id)
        {
            return _context.usersaccounts.Any(e => e.Id == id);
        }
    }
}
