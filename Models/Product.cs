namespace EstudosAPI.Models
{
    public class Product
    {
        public int Code { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Produto \n\rCódigo: {Code} - Nome: {Name}";
        }
    }
}
