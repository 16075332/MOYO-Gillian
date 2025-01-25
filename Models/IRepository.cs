namespace Assignment3_Backend.Models
{
    public interface IRepository
    {
        Task<bool> SaveChangesAsync();
        
        void Add<T>(T entity) where T : class;

        Task<Product[]> GetAllProductsAsync(); // ADDED product relatiionship
        Task<ProductType[]> GetAllProdTypesAsync();  // ADDED product relatiionship
        Task<Brand[]> GetAllBrands();  // ADDED product relatiionship
    }
}
