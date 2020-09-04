using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using productsAndCategories.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

// Other using statements
namespace HomeController.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return Redirect("categories/form");
        }

        [HttpGet("categories/{CategoryId}")]
        public IActionResult ShowCategory(int CategoryId)
        {
            Category categoryWithAssociationAndProduct = dbContext.Categories
            .Include(c => c.ProductsOfCategories)
            .ThenInclude(c => c.Product)
            .FirstOrDefault(c => c.CategoryId == CategoryId);

            List<Product> unrelatedProducts = dbContext.Products
            .Include(c => c.CategorizedBy)
            .ThenInclude(p => p.Category)
            .Where(a => a.CategorizedBy.All(x => x.CategoryId != CategoryId))
            .ToList();

            ViewBag.cat= categoryWithAssociationAndProduct;
            return View(unrelatedProducts);
        }

        [HttpGet("product/{ProductId}")]
        public IActionResult ShowProduct(int ProductId)
        {
            Product productWithAssociationAndProduct = dbContext.Products
            .Include(p => p.CategorizedBy)
            .ThenInclude(p => p.Category)
            .FirstOrDefault(p => p.ProductId == ProductId);

            List<Category> unrelatedCategories = dbContext.Categories
            .Include(c => c.ProductsOfCategories)
            .ThenInclude(p => p.Product)
            .Where(a => a.ProductsOfCategories.All(x => x.ProductId != ProductId))
            .ToList();

            ViewBag.prod = productWithAssociationAndProduct;
            return View(unrelatedCategories);
        }

        [HttpPost("products/submit")]
        public IActionResult SubmitProduct(Product newProduct)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(newProduct);
                dbContext.SaveChanges();
                return Redirect($"/products/{newProduct.ProductId}");
            }
            else
            {
                TempData["Name"] = newProduct.Name;
                TempData["Description"] = newProduct.Description;
                TempData["Price"] = newProduct.Price;
                return View("ProductsForm");
            }
        }

        [HttpPost("categories/submit")]
        public IActionResult SubmitCategory(Category newCategory)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(newCategory);
                dbContext.SaveChanges();
                return Redirect($"/categories/{newCategory.CategoryId}");
            }
            else
            {
                TempData["Name"] = newCategory.Name;
                return View("CategoriesForm");
            }
        }

        [HttpGet("categories/form")]
        public IActionResult CategoriesForm()
        {
            List<Category> existingCategories = dbContext.Categories
            .ToList();

            ViewBag.existingCategories = existingCategories;
            return View();
        }

        [HttpGet("products/form")]
        public IActionResult ProductsForm()
        {
            List<Product> existingProducts = dbContext.Products
            .ToList();

            ViewBag.existingProducts = existingProducts;
            return View();
        }

        [HttpPost("category/{CategoryId}/addproduct")]
        public IActionResult AddProductAssociation(Category newProductAssociation, int CategoryId,int ProductId)
        {

            Association newAssoc = new Association();
            newAssoc.ProductId = ProductId;
            newAssoc.CategoryId = CategoryId;
            dbContext.Associations.Add(newAssoc);
            dbContext.SaveChanges(); 
            return Redirect($"/category/{CategoryId}");
        }

        [HttpPost("products/{ProductId}/addcategory")]
        public IActionResult AddCategoryAssociation (Product newCategoryAssociaton, int ProductId, int CategoryId)
        {
            Association newAssoc = new Association();
            newAssoc.CategoryId = CategoryId;
            newAssoc.ProductId = ProductId;
            dbContext.Associations.Add(newAssoc);
            dbContext.SaveChanges();
            return Redirect($"/product/{ProductId}");
        }


    }
}