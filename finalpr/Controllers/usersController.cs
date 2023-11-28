using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using finalpr.Data;
using finalpr.Models;
using Microsoft.Data.SqlClient;
using System.Net.Mail;

namespace finalpr.Controllers
{
    public class usersController : Controller
    {
        private readonly finalprContext _context;

        public usersController(finalprContext context)
        {
            _context = context;
        }

        // GET: users
        public async Task<IActionResult> Index()
        {
              return _context.users != null ? 
                          View(await _context.users.ToListAsync()) :
                          Problem("Entity set 'finalprContext.users'  is null.");
        }

        // GET: users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.users == null)
            {
                return NotFound();
            }

            var users = await _context.users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }


        // GET: users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Add admin
        public async Task<IActionResult> Create(string name)
        {
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("finalprContext");
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=cvv;Integrated Security=True");
            string sql = "Update users set role='admin' where name='" + name + "'";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            comm.ExecuteNonQuery();
            conn.Close();
            return View();
        }

        // GET: users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.users == null)
            {
                return NotFound();
            }

            var users = await _context.users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            return View(users);
        }

        // POST: users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,role,registerDate")] users users)
        {
            if (id != users.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(users);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!usersExists(users.Id))
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
            return View(users);
        }

        // GET: users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.users == null)
            {
                return NotFound();
            }

            var users = await _context.users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // POST: users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.users == null)
            {
                return Problem("Entity set 'finalprContext.users'  is null.");
            }
            var users = await _context.users.FindAsync(id);
            if (users != null)
            {
                _context.users.Remove(users);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool usersExists(int id)
        {
          return (_context.users?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        public IActionResult login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult login(string name, string password, bool autologin)
        {

            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("finalprContext");

            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=cvv;Integrated Security=True");
           

            string sql = "SELECT * FROM users where name= '" + name + "' and password = '" + password + "'";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();

            if (reader.Read())
            {

                string id = Convert.ToString((int)reader["Id"]);
                string name1 = (string)reader["name"];
                string role = (string)reader["role"];
                HttpContext.Session.SetString("userid", id);
                HttpContext.Session.SetString("name", name);
                HttpContext.Session.SetString("role", role);
                reader.Close();
                conn.Close();

                ViewData["name"] = HttpContext.Session.GetString("name");

                if (autologin)
                {
                    var cookieOptions = new CookieOptions
                    { Expires = DateTime.Now.AddDays(30) };
                    HttpContext.Response.Cookies.Append("name", name, cookieOptions);
                    HttpContext.Response.Cookies.Append("password", password, cookieOptions);
                }
                if (role == "customer")
                {


                    return RedirectToAction("customerPage");
                }
                else
                {
                    return View("adminPage");
                }



            }


            else
            {
                ViewData["wrongLoginInfo"] = "Wrong password or username";
                conn.Close();
                return View();
            }


        }



        public IActionResult register()
        {

            return View();
        }

        [HttpPost]
        public IActionResult register(string name, string password, string password2, bool agree)
        {

            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("finalprContext");
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=cvv;Integrated Security=True");
            string sql = "SELECT * FROM users where name= '" + name + "'and password= '" + password + "'";

            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read())
            {
                ViewData["userDoesExists"] = "Windows.alert(\"This user exists, Choose a different name\")";
                return View();
            }
            else if (password != password2)
            {
                ViewData["wrongPassword"] = "(the passwords donot match)";
            }

            else if (agree == false)
            {


                ViewData["doYouAgree"] = "You must agree to our terms";


            }

            else
            {
                reader.Close();
                sql = "insert into users (name,password,registerDate,role) values ('" + name + "','" + password + "',CURRENT_TIMESTAMP,'customer')";
                comm = new SqlCommand(sql, conn);
                comm.ExecuteNonQuery();
                conn.Close();
                return View("login");
            }


            conn.Close();
            return View("customerPage");


        }

        public async Task<IActionResult> adminPage()
        {
            return View();

        }



        public IActionResult customerPage()
        {

            if (HttpContext.Session.GetString("role") == "admin")
            {
                return View("login");
            }

            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("finalprContext");

            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=cvv;Integrated Security=True");
            string sql = "select * from items where discount='yes' ";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            List<items> list = new List<items>();
            SqlDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new items
                {

                    Id = (int)reader["Id"],
                    name = (string)reader["name"],
                    description = (string)reader["description"],
                    price = (int)reader["price"],
                    quantity = (int)reader["quantity"],
                    discount = (string)reader["discount"],
                    category = (string)reader["category"],
                    image = (string)reader["image"]

                });
            }
            ViewData["name"] = HttpContext.Session.GetString("name");
            conn.Close();
            return View(list);
        }


        public IActionResult email()
        {

            return View();
        }


        [HttpPost, ActionName("email")]
        [ValidateAntiForgeryToken]
        public IActionResult email(string address, string subject, string body)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            var mail = new MailMessage();
            mail.From = new MailAddress("roro.mulberry22@gmail.com");
            mail.To.Add(address); // receiver email address
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = body;
            SmtpServer.Port = 587;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential("roro.mulberry22@gmail.com", "jilcatxqpjhhietk");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
            ViewData["Message"] = "Email sent.";
            return View();

        }





        //User search and it has an APIUserscontroller
        public IActionResult roleslist()
        {
            List<users> list = new List<users>();
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("finalprContext");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "Select distinct(role) from users  ";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new users { role = (string)reader["role"] });

            }
            ViewData["state"] = "blanching";
            conn.Close();
            return View(list);
        }


        [HttpPost]
        public IActionResult roleslist(string role)
        {
            List<users> list = new List<users>();
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("webProject2Context");
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=cvv;Integrated Security=True");
            string sql = "Select * from users where role ='" + role + "' ";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new users
                {
                    Id = (int)reader["Id"],
                    name = (string)reader["name"],
                    password = (string)reader["password"],
                    registerDate = (DateTime)reader["registerDate"],
                    role = (string)reader["role"]
                });

            }
            ViewData["state"] = null;
            conn.Close();
            return View(list);
        }

        public IActionResult logout()
        {
            HttpContext.Session.Remove("userid");
            HttpContext.Session.Remove("role");
            HttpContext.Session.Remove("name");
            HttpContext.Response.Cookies.Delete("name");
            HttpContext.Response.Cookies.Delete("role");
            HttpContext.Response.Cookies.Delete("userid");
            return RedirectToAction("login", "Home");
        }
    }

    }

