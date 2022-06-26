/*
 *  File:   DistortionFilterOptions.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2022
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */

namespace Lavalink4NET.Filters;

using System.Text.Json.Serialization;


public sealed class DistortionFilterOptions : IFilterOptions
{
    public const string Name = "distortion";

    /// <inheritdoc/>
    string IFilterOptions.Name => Name;

    [JsonPropertyName("sinOffset")]
    public float SinOffset { get; set; } = 0F;

    [JsonPropertyName("sinScale")]
    public float SinScale { get; set; } = 1F;

    [JsonPropertyName("cosOffset")]
    public float CosOffset { get; set; } = 0F;

    [JsonPropertyName("cosScale")]
    public float CosScale { get; set; } = 1F;

    [JsonPropertyName("tanOffset")]
    public float TanOffset { get; set; } = 0F;

    [JsonPropertyName("tanScale")]
    public float TanScale { get; set; } = 1F;

    [JsonPropertyName("offset")]
    public float Offset { get; set; } = 0F;

    [JsonPropertyName("scale")]
    public float Scale { get; set; } = 1F;
}
