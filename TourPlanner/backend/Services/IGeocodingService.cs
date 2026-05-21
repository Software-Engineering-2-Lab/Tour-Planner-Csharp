using System.Runtime.CompilerServices;
using TourPlanner.backend.Enums;
namespace TourPlanner.backend.Services;

public interface IGeocodingService
{
  Task<(double lon , double lat)> GetCoordinatesAsync (String adress);

  Task<(double distance , double duration , string geometry)> GetRouteAsync ((double lon , double lat)start , (double lon , double lat)end , TransportType transportType);

}