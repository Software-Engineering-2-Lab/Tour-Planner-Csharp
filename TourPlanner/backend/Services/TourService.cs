using TourPlanner.backend.Entities;
using TourPlanner.backend.DTOs;
using TourPlanner.backend.Repositories;
using TourPlanner.backend.Enums;


namespace TourPlanner.backend.Services;

public class TourService : ITourService 
{
   private readonly ITourRepository _tourRepository;
   private readonly IGeocodingService _geocodingService;

   public TourService (IGeocodingService geocodingService ,ITourRepository tourRepository)
  {
    _tourRepository=tourRepository;
    _geocodingService=geocodingService;
  }

  
   public async Task<TourDto> CreateAsync (TourDto dto)
  {
    var (startLon,startLat, _) = await _geocodingService.GetCoordinatesAsync(dto.FromLocation);
    var (endLon, endLat, _) = await _geocodingService.GetCoordinatesAsync(dto.ToLocation);

    var(distance,duration,geometry)= await _geocodingService.GetRouteAsync((startLon,startLat),(endLon, endLat),dto.TransportType);

    var tour = new Tour
    {
      Id = dto.Id,
      Name=dto.Name,
      Description=dto.Description,
      FromLocation=dto.FromLocation,
      ToLocation=dto.ToLocation,
      TransportType=dto.TransportType,
      Distance=distance,
      EstimatedTime=duration,
      RouteImagePath=geometry,
      Popularity=dto.Popularity,
      ChildFriendliness=dto.ChildFriendliness,
      UserId=dto.UserId
    };

    var createdTour = await _tourRepository.AddAsync(tour);

    return MapToDto(createdTour);
  }

  public async Task<TourDto> UpdateAsync(long id, TourDto dto)
  {
    var existingTour = await _tourRepository.GetByIdAsync(id);

    if(existingTour==null)
    {
      throw new Exception("Tour not found.");
    }

    bool locationChanged = dto.FromLocation != existingTour.FromLocation || dto.ToLocation != existingTour.ToLocation;

    bool TransportTypeChanged = dto.TransportType != existingTour.TransportType;

    if (locationChanged || TransportTypeChanged)
    {
      var (startLon,startLat, _) = await _geocodingService.GetCoordinatesAsync(dto.FromLocation);

      var (endLon, endLat, _) = await _geocodingService.GetCoordinatesAsync(dto.ToLocation);
      
      var(distance,duration,geometry)= await _geocodingService.GetRouteAsync((startLon,startLat),(endLon, endLat),dto.TransportType);

     
      existingTour.FromLocation=dto.FromLocation;
      existingTour.ToLocation=dto.ToLocation;
      existingTour.TransportType=dto.TransportType;
      existingTour.Distance=distance;
      existingTour.EstimatedTime=duration;
      existingTour.RouteImagePath=geometry;
      
    }
    
    existingTour.Name=dto.Name;
    existingTour.Description=dto.Description;
    existingTour.Popularity=dto.Popularity;
    existingTour.ChildFriendliness=dto.ChildFriendliness;
    existingTour.UserId=dto.UserId;
    
    await _tourRepository.UpdateAsync(existingTour);

    return MapToDto(existingTour);
  }

  public async Task DeleteAsync(long id)
  {
    var tour = await _tourRepository.GetByIdAsync(id);
    if (tour != null)
    {
        await _tourRepository.DeleteAsync(tour);
    }
  }
  private TourDto MapToDto (Tour tour)
  {
    return new TourDto
    {
      Id = tour.Id,
      Name=tour.Name,
      Description=tour.Description,
      FromLocation=tour.FromLocation,
      ToLocation=tour.ToLocation,
      TransportType=tour.TransportType,
      Distance=tour.Distance,
      EstimatedTime=tour.EstimatedTime,
      RouteImagePath=tour.RouteImagePath,
      Popularity=tour.Popularity??0,
      ChildFriendliness=tour.ChildFriendliness??0.0,
      UserId=tour.UserId,
      TourImages = tour.TourImages?.Select(img => new TourImageDto
            {
                Id = img.Id,
                FileName = img.FileName,
                Url = $"/api/tours/{tour.Id}/photos/{img.Id}",
                TourId = tour.Id,
                CreatedAt = img.CreatedAt
            }).ToList() ?? new List<TourImageDto>()
    };
  }

  public async Task<TourDto?> FindByIdAsync(long id)
  {
    var tour = await _tourRepository.GetByIdAsync(id);

    return tour != null ? MapToDto(tour) : null;
  }

  public async Task<List<TourDto>> FindByUserIdAsync(long userId)
  {
    var tours = await _tourRepository.FindByUserIdAsync(userId);

    return tours.Select(tour => MapToDto(tour)).ToList();
  }

  public async Task<SearchResultDto> SearchAsync(string query)
  {
    var tours = await _tourRepository.FullTextSearchAsync(query);

    SearchResultDto finalResult = new SearchResultDto();

    return new SearchResultDto
    {
        Tours = tours.Select(tour => MapToDto(tour)).ToList(), 
        TotalResults = tours.Count,                           
        SearchQuery = query
    };
  }
}