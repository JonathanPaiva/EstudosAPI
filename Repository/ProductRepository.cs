using EstudosAPI.Models;

namespace EstudosAPI.Repository
{
    //Devido a não ter uma conexão com banco de dados e ser necessário deixar a classe em uso durante a execução da aplicação, necessário deixar a classe como static
    public static class ProductRepository
    {
        public static List<Product> Products { get; set; } = Products = new List<Product>();

        public static void Init(IConfiguration configuration)
        {
            List<Product> products = configuration.GetSection("ProductsBase").Get<List<Product>>();

            Products = products;
        }

        public static void Add(Product product)
        {               
            Products.Add(product);
        }

        public static List<Product> GetProducts()
        {
            return Products;
        }

        public static Product GetProduct(int id)
        {
            return Products.FirstOrDefault(p => p.Id == id);  
        }

        public static void RemoveProduct(Product product)
        {
            Products.Remove(product);
        }
    }
}
