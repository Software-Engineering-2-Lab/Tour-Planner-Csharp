
using TourPlanner.backend.DTOs;
using TourPlanner.backend.Entities;
using TourPlanner.backend.Repositories;

namespace TourPlanner.backend.Services;

public class LogService : ILogService
{

  private readonly ILogRepository _logRepository;
  private readonly ITourRepository _tourRepository;

  public LogService (ILogRepository logRepository, ITourRepository tourRepository)
  {
    _logRepository = logRepository;
    _tourRepository = tourRepository;
  }

  public async Task<IEnumerable<LogDto>> FindByTourIdAsync(long tourId)
  {
    var logs = await _logRepository.FindByTourIdAsync(tourId);

    return logs.Select(log => MapToDto(log)).ToList();
  }

  public async Task<LogDto> CreateAsync (long tourId, LogDto dto)
  {
   var tour = await _tourRepository.GetByIdAsync(tourId);
    if (tour == null)
    {
      throw new Exception("Tour not found.");
    }

    var log = new Log
    {
      DateTime = DateTime.TryParse(dto.DateTime, out var parsedDate) ? DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc) : DateTime.UtcNow,
      Comment = dto.Comment ?? string.Empty,
      Difficulty = dto.Difficulty,
      TotalDistance = dto.TotalDistance,
      TotalTime = dto.TotalTime,
      Rating = dto.Rating,
      TourId = tourId 
    };

    var createdLog = await _logRepository.AddAsync(log);

    return MapToDto(createdLog);
  }

  public async Task<LogDto> UpdateAsync(long tourId, long logId, LogDto dto)
  {
    var tour = await _tourRepository.GetByIdAsync(tourId);

    if(tour==null)
    {
      throw new Exception("Tour not found.");
    }

    var existingLog =await _logRepository.GetByIdAsync(logId);
    
     if (existingLog == null)
    {
      throw new Exception("Log not found.");
    }

    existingLog.DateTime = DateTime.TryParse(dto.DateTime, out var parsedDate) ? DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc) : existingLog.DateTime;
    existingLog.Comment = dto.Comment ?? string.Empty;
    existingLog.Difficulty = dto.Difficulty;
    existingLog.TotalDistance = dto.TotalDistance;
    existingLog.TotalTime = dto.TotalTime;
    existingLog.Rating = dto.Rating;
    existingLog.TourId = tourId;

    await _logRepository.UpdateAsync(existingLog);

    return MapToDto(existingLog);
  }

  public async Task DeleteAsync(long id)
  {
    var log = await _logRepository.GetByIdAsync(id);
    if (log != null)
    {
        await _logRepository.DeleteAsync(log);
    }
  }
  private LogDto MapToDto (Log log)
  {
    return new LogDto
    {
      Id=log.Id,
      DateTime = log.DateTime.ToString("o"), 
      Comment = log.Comment,
      Difficulty = log.Difficulty,
      TotalDistance = log.TotalDistance,
      TotalTime = log.TotalTime,
      Rating = log.Rating,
      TourId = log.TourId 
    };
  }
}