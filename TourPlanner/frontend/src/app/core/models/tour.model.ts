import { TourImageDto } from "../services/photos.service";

export interface Tour {
    id: number;
    name: string;
    description: string;
    fromLocation: string;
    toLocation: string;
    transportType: 'BIKE' | 'HIKE' | 'RUNNING' | 'VACATION';
    distance: number;
    estimatedTime: number;
    routeImagePath: string | null;
    popularity: number;
    childFriendliness: number;
    userId: number;
    tourImages?: TourImageDto[];
}

export interface CreateTourDto extends Omit<Tour, 'id' | 'popularity' | 'childFriendliness'> {
    popularity: number;
    childFriendliness: number;
    userId: number;
}