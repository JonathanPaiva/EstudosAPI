using EstudosAPI.Models;

namespace EstudosAPI.Repository
{
    //Devido a não ter uma conexão com banco de dados e ser necessário deixar a classe em uso durante a execução da aplicação, necessário deixar a classe como static
    public static class ProductRepository
    {
        public static List<Product> Products { get; set; }

        public static void Add(Product product)
        {
            if(Products == null)
            {
                Products = new List<Product>();
            }
               
            Products.Add(product);
        }

        public static List<Product> GetProducts()
        {
            return Products;
        }

        public static Product GetProduct(int code)
        {
            return Products.FirstOrDefault(p => p.Code == code);  
        }

        public static void RemoveProduct(Product product)
        {
            Products.Remove(product);
        }
    }
}
