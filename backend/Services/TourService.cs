using TourPlanner.backend.Entities;
using TourPlanner.backend.DTOs;
using TourPlanner.backend.Repositories;
using TourPlanner.backend.Enums;


namespace TourPlanner.backend.Services;

public class TourService : ITourService 
{
   private readonly ITourRepository _tourRepository;
   private readonly ILogRepository _logRepository;
   private readonly IGeocodingService _geocodingService;

   public TourService (IGeocodingService geocodingService ,ITourRepository tourRepository,ILogRepository logRepository)
  {
    _tourRepository=tourRepository;
    _logRepository=logRepository;
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
      ChildFriendliness=tour.ChildFriendliness??0,
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

  public async Task CalculatesPopularityAsync(long tourId)
  {
    int logsCount= await _logRepository.GetTourLogsCountAsync(tourId);

    int popularityScore;

    switch (logsCount)
    {
        case 0:
            popularityScore = 0; 
            break;
        case 1:
            popularityScore = 1; 
            break;
        case 2:
            popularityScore = 2;
            break;
        case 3:
            popularityScore = 3;
            break;
        case 4:
            popularityScore = 4;
            break;
        case 5:
            popularityScore = 5; 
            break;

        default:
            popularityScore = 5; 
            break;
    }

    await _tourRepository.SetPopularityAsync(tourId,popularityScore);
    
  }

  public async Task CalculatesChildFriendlinessAsync(long tourId)
{
    var logs = await _logRepository.FindByTourIdAsync(tourId);

    if (logs == null)
    {
        await _tourRepository.SetChildFriendlinessAsync(tourId, 0);
        return;
    }

    double averageDifficulty = logs.Average(log => log.Difficulty);

    int childFriendlinessScore = 0;

    if (averageDifficulty <= 1.5) 
        childFriendlinessScore = 1; 
    else if (averageDifficulty <= 2.5) 
        childFriendlinessScore = 2;
    else if (averageDifficulty <= 3.5) 
        childFriendlinessScore = 3;
    else if (averageDifficulty <= 4.5) 
        childFriendlinessScore = 4;
    else 
        childFriendlinessScore = 5; 
    await _tourRepository.SetChildFriendlinessAsync(tourId, childFriendlinessScore);
}
  
}