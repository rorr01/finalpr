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

namespace finalpr.Controllers
{
    public class itemsController : Controller
    {
        private readonly finalprContext _context;

        public itemsController(finalprContext context)
        {
            _context = context;
        }

        // GET: items
        public async Task<IActionResult> Index()
        {
              return _context.items != null ? 
                          View(await _context.items.ToListAsync()) :
                          Problem("Entity set 'finalprContext.items'  is null.");
        }

        // GET: items/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            ViewData["role"] = HttpContext.Session.GetString("role");
            if (id == null || _context.items == null)
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
            return View();
        }

        // POST: items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,description,price,quantity,discount,category,image")] items items)
        {
            if (ModelState.IsValid)
            {
                _context.Add(items);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(items);
        }

        // GET: items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.items == null)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,description,price,quantity,discount,category,image")] items items)
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
            if (id == null || _context.items == null)
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
            if (_context.items == null)
            {
                return Problem("Entity set 'finalprContext.items'  is null.");
            }
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
          return (_context.items?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        //image slider
        public IActionResult ourproducts()
        {
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("finalprContext");
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=cvv;Integrated Security=True");

            List<items> list = new List<items>();
            string sql = "select * from items ";
            SqlCommand command = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = command.ExecuteReader();
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

                    image = (string)reader["image"]
                });
            }
            conn.Close();
            reader.Close();
            return View(list);
        }


        public IActionResult dashboard()
        {

            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("finalprContext");
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=cvv;Integrated Security=True");
            string sql = "select count(quantity) from items where category= 'Fantasy' ";
            SqlCommand command = new SqlCommand(sql, conn);
            conn.Open();

            ViewData["c1"] = (int)command.ExecuteScalar();
            sql = "select count(quantity) from items where category= 'Mystery' ";
            command = new SqlCommand(sql, conn);
            ViewData["c2"] = (int)command.ExecuteScalar();
            sql = "select count(quantity) from items where category= 'Adventure' ";
            command = new SqlCommand(sql, conn);
            ViewData["c3"] = command.ExecuteScalar();
            conn.Close();
            return View();
        }
    }
}

