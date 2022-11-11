public record ProductRequest(int Code, string Name, 
                            string Description, int CategoryId, 
                            List<string>Tags
    );