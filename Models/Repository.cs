using Microsoft.EntityFrameworkCore;

namespace Assignment3_Backend.Models
{
    public class Repository:IRepository
    {
        private readonly AppDbContext _appDbContext;

        public Repository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void Add<T>(T entity) where T : class
        {
            _appDbContext.Add(entity);
        }
        //get products
        public async Task<Product[]> GetAllProductsAsync()
        {
            IQueryable<Product> query = _appDbContext.Products;
            return await query.ToArrayAsync();
        }
        //get product Type
        public async Task<ProductType[]> GetAllProdTypesAsync()
        {
            IQueryable<ProductType> query = _appDbContext.ProductTypes;
            return await query.ToArrayAsync();
        }
        //get brand
        public async Task<Brand[]> GetAllBrands()
        {
            IQueryable<Brand> query = _appDbContext.Brands;
            return await query.ToArrayAsync();
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _appDbContext.SaveChangesAsync() > 0;
        }
    }
}
