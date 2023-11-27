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
    public class ordersController : Controller
    {
        private readonly finalprContext _context;

        public ordersController(finalprContext context)
        {
            _context = context;
        }

        // GET: orders
        public async Task<IActionResult> Index()
        {
              return _context.orders != null ? 
                          View(await _context.orders.ToListAsync()) :
                          Problem("Entity set 'finalprContext.orders'  is null.");
        }

        // GET: orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.orders == null)
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
        public async Task<IActionResult> Create([Bind("Id,userid,itemid,buyDate,quantity")] orders orders)
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
            if (id == null || _context.orders == null)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,userid,itemid,buyDate,quantity")] orders orders)
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
            if (id == null || _context.orders == null)
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
            if (_context.orders == null)
            {
                return Problem("Entity set 'finalprContext.orders'  is null.");
            }
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
          return (_context.orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }




        public IActionResult mypurchase()
        {
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("finalprContext");
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=cvv;Integrated Security=True");
            string sql = "select * from orders where userid= (select Id from userid where userid= '" + HttpContext.Session.GetString("userid") + "' ) ";

            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();


            SqlDataReader reader = comm.ExecuteReader();
            List<orders> list = new List<orders>();
            while (reader.Read())
            {
                list.Add(new orders
                {

                    itemid = (int)reader["itemid"],
                    userid = (int)reader["userid"],
                    quantity = (int)reader["quantity"],
                    buyDate = (DateTime)reader["buyDate"]


                });
            }
            conn.Close();

            return View(list);
        }

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

    }
}
