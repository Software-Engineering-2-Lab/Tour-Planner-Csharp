export interface TourLog {
    id: number | null;
    tourId: number;
    dateTime: string;
    comment: string;
    difficulty: number; 
    totalDistance: number;
    totalTime: number;
    rating: number; 
}