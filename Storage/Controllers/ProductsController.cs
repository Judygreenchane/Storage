using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Storage.Data;
using Storage.Models;

namespace Storage.Controllers
{
    public class ProductsController : Controller
    {
        private readonly StorageContext _context;
        private IEnumerable<ProductViewModel> _productViewModel;

        public ProductsController(StorageContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Product.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Orderdate,Category,Shelf,Count,Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Orderdate,Category,Shelf,Count,Description")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
        public IActionResult ProductSummary()
        {
            
            var products = _context.Product.ToList();

            
            var productViewModels = products.Select(product => new ProductViewModel
            {
                Id=product.Id,
                Name = product.Name,
                Price = product.Price,
                Count = product.Count,
                InventoryValue = product.Price * product.Count 
            }).ToList();

            return View(productViewModels);
        }
       
        public IActionResult SearchByCategory(string category)
        {
           
            var products = string.IsNullOrEmpty(category)
                ? _context.Product.ToList()
                : _context.Product.Where(p => p.Category == category).ToList();

            return View(products); 
        }
        
        public IActionResult AdvancedSearch(string product,string category)
        {

            var categories = _context.Product
                                    .Select(p => p.Category)
                                    .Distinct()
                                    .ToList();

           
            ViewBag.CategoryList = new SelectList(categories);


            if (string.IsNullOrEmpty(category) && string.IsNullOrEmpty(product))
            {

                var products = _context.Product.ToList();
                
               
                
                return View(products);
            }
            else  if (!string.IsNullOrEmpty(category) && string.IsNullOrEmpty(product))
                {
                var products =  _context.Product.Where(p => p.Category == category).ToList();

                    return View(products);
                
            }
            else if (string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(product))
            {
                var products = _context.Product.Where(p => p.Name == product).ToList();

                    return View(products);
                
            }
            else if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(product))
            {
                var products = _context.Product.Where(p => p.Name == product && p.Category==category) .ToList();

                    return View(products);
                }
          
            return View();
        }
            
        }
}

