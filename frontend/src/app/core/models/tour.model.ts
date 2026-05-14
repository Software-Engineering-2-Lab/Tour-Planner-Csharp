export interface Tour {
    id: number;
    name: string;
    description: string;
    fromLocation: string;
    toLocation: string;
    transportType: 'BIKE' | 'HIKE' | 'RUNNING' | 'VACATION';
    distance: number;
    estimatedTime: number;
    routeImagePath: string;
    popularity: number;
    childFriendliness: number;
    userId: number;
}

export interface CreateTourDto extends Omit<Tour, 'id' | 'popularity' | 'childFriendliness'> {
    popularity: number;
    childFriendliness: number;
    userId: number;
}