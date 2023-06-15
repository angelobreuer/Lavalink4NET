namespace Lavalink4NET.Protocol.Responses;

using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "loadType")]
[JsonDerivedType(typeof(TrackLoadResultModel), "track")]
[JsonDerivedType(typeof(PlaylistLoadResultModel), "playlist")]
[JsonDerivedType(typeof(SearchLoadResultModel), "search")]
[JsonDerivedType(typeof(EmptyLoadResultModel), "empty")]
[JsonDerivedType(typeof(ErrorLoadResultModel), "error")]
public record class LoadResultModel;
