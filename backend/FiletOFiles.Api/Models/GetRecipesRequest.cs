using System;

namespace FiletOFiles.Api.Models;

public record GetRecipesRequest(
    string TextFragment,
    IEnumerable<long> TagIds,
    int Take = 100,
    int Skip = 0
);
