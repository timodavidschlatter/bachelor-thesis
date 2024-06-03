using eBauGISTriageApi.Models;
using System.Collections.Generic;

namespace eBauGISTriageApi.Helper.DTO
{
    /// <summary>
    /// Represents a DTO (Data Transfer Object) for geometry data.
    /// </summary>
    public record GeometryDTO(List<PointResult> PointResults, ShapeResult ShapeResult);
}

