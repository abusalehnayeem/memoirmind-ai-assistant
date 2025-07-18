﻿using Microsoft.Extensions.VectorData;

namespace MemoirMind.Api.Models.VectorMemory;

public sealed record VectorStoreRecord<TKey>
{
    [VectorStoreKey] public TKey? Key { get; set; }

    [VectorStoreData] public string Data { get; set; } = string.Empty;

    [VectorStoreVector(1536)] public ReadOnlyMemory<float>? Vector { get; set; }
}