using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QuickMart.Data.DbContext;
using QuickMart.Data.Entities;
using QuickMart.Data.Repository.IRepository;

namespace QuickMart.Data.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        #region Constructor

        // Constructor injection of ApplicationDbContext and IWebHostEnvironment
        public ProductRepository(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        #endregion

        #region Get Product Methods

        // Step 1: Retrieve all products
        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync(int page, int pageSize, string sortBy, string sortOrder)
        {
            var query = from product in _context.Products
                            // LEFT JOIN ensures we get null for products without a category
                        join category in _context.Categories on product.CategoryId equals category.CategoryId into productCategory
                        from category in productCategory.DefaultIfEmpty()  // LEFT JOIN

                            // Inner Join: This will return only products that have a matching category
                            // join category in _context.Categories on product.CategoryId equals category.CategoryId  // INNER JOIN

                            // Right Join (not typically supported directly in LINQ, but can be simulated by swapping the tables in a LINQ query):
                            // from category in _context.Categories
                            // join product in _context.Products on category.CategoryId equals product.CategoryId into productCategory
                            // from product in productCategory.DefaultIfEmpty()  // RIGHT JOIN
                        select new ProductDTO
                        {
                            ProductId = product.ProductId,
                            Name = product.Name,
                            Description = product.Description,
                            Price = product.Price,
                            StockQuantity = product.StockQuantity,
                            IsActive = product.IsActive,
                            DiscountPrice = product.DiscountPrice,
                            CategoryId = product.CategoryId,
                            CategoryName = category != null ? category.Name : null, // Include category name
                            ImageUrl = product.ImageUrl
                        };

            // Sorting Logic
            switch (sortBy.ToLower())
            {
                case "price":
                    query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price);
                    break;
                case "name":
                    query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name);
                    break;
                case "stockquantity":
                    query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(p => p.StockQuantity) : query.OrderBy(p => p.StockQuantity);
                    break;
                default:
                    query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name);
                    break;
            }

            // Pagination Logic
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }


        // Step 2: Retrieve a product by its ID
        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                                  .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        #endregion

        #region Create and Save Product Methods

        // Step 3: Create a new product, including saving an image if provided
        public async Task<Product> CreateProductAsync(Product product, IFormFile? productImage)
        {
            // Handle the product image if provided
            if (productImage != null)
            {
                try
                {
                    product.ImageUrl = await SaveProductImageAsync(productImage);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error occurred while saving the product image.", ex);
                }
            }

            // Ensure that the CategoryId is valid (it should exist in the Categories table)
            var category = await _context.Categories.FindAsync(product.CategoryId);
            if (category == null)
            {
                throw new InvalidOperationException("Invalid CategoryId. The specified category does not exist.");
            }

            // Add product to the database
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Return the newly created product (CategoryName will be fetched when querying the products)
            return product;
        }


        // Helper method to save the product image
        private async Task<string> SaveProductImageAsync(IFormFile productImage)
        {
            try
            {
                // Ensure the images directory exists
                var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(imagesDirectory))
                {
                    Directory.CreateDirectory(imagesDirectory);
                }

                // Generate a safe file name
                var fileName = Path.GetFileNameWithoutExtension(productImage.FileName);
                var extension = Path.GetExtension(productImage.FileName);
                var safeFileName = fileName.Replace(" ", "_") + "_" + Guid.NewGuid().ToString() + extension;

                var imagePath = Path.Combine(imagesDirectory, safeFileName);

                // Save the image to the file system
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await productImage.CopyToAsync(stream);
                }

                // Return the relative URL of the image
                return $"/images/{safeFileName}";
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException("File IO error while saving the product image. Please check file system permissions.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An unexpected error occurred while saving the image.", ex);
            }
        }

        #endregion

        #region Update and Delete Product Methods

        // Step 4: Update an existing product
        public async Task<Product> UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        // Step 5: Delete a product by its ID
        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return false; // Product not found, deletion fails
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true; // Successfully deleted
        }

        #endregion
    }
}
