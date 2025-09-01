using System;

namespace FiletOFiles.Api.Models;

public class GetRecipesRequest
{
    public int Take { get; set; }
    public int Skip { get; set; }
    public string TextFragment { get; set; }
    public IEnumerable<long> Tags { get; set; }
}
