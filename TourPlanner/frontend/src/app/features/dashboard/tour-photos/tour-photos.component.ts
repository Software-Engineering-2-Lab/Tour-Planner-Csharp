import { Component, Input, Output, EventEmitter, ChangeDetectorRef, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PhotoService, TourImageDto } from '../../../core/services/photos.service';

@Component({
    selector: 'app-tour-photos',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './tour-photos.component.html',
    styleUrl: './tour-photos.component.scss'
})
export class TourPhotosComponent implements OnInit, OnChanges {
    @Input() tourId?: number;
    @Input() initialImages: TourImageDto[] = [];
    @Output() photoAdded = new EventEmitter<TourImageDto>();
    uploadedPhotos: { id?: number; url: string; fileName: string; createdAt: string }[] = [];
    isDragging = false; 

    constructor(
        private photoService: PhotoService,
        private cdr: ChangeDetectorRef
    ) {}

    ngOnInit(): void {
        this.reloadImages();
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes['initialImages'] || changes['tourId']) {
            this.reloadImages();
        }
    }

    private reloadImages(): void {
        if (this.initialImages) {
            this.uploadedPhotos = this.initialImages.map(img => ({
                id: img.id,
                url: img.url,
                fileName: img.fileName,
                createdAt: img.createdAt
            }));
            this.cdr.detectChanges();
        }
    }

    onFileSelected(event: any): void {
        this.handleFiles(event.target.files);

    }

    onDragOver(event: DragEvent): void {
        event.preventDefault();
        event.stopPropagation();
        this.isDragging = true; 
    }

    onDragLeave(event: DragEvent): void {
        event.preventDefault();
        event.stopPropagation();
        this.isDragging = false;
    }

    onFileDropped(event: DragEvent): void {
        event.preventDefault();
        event.stopPropagation();
        this.isDragging = false;
        
        if (event.dataTransfer?.files) {
            this.handleFiles(event.dataTransfer.files);
        }
    }

    private handleFiles(files: FileList): void {
        if (!this.tourId) return;

        Array.from(files).forEach(file => {

            if (file.type !== 'image/jpeg' && file.type !== 'image/png') {
                alert('Invalid format. Only .png and .jpg/.jpeg files accepted');
                console.log('Upload blocat: format fișier neacceptat -', file.type);
                return;
            }

            this.photoService.uploadPhoto(this.tourId!, file).subscribe({
                next: (savedImage: TourImageDto) => {
                    this.uploadedPhotos.unshift({
                        id: savedImage.id,
                        url: savedImage.url,
                        fileName: savedImage.fileName,
                        createdAt: savedImage.createdAt
                    });
                    this.photoAdded.emit(savedImage);
                    this.cdr.detectChanges();
                },
                error: (err: any) => {
                    console.error('Failed to upload image:', err);
                }
            });
        });
    }

    deletePhoto(index: number): void {
        const photoToDelete = this.uploadedPhotos[index];
        if (!this.tourId || !photoToDelete.id) return;

        this.uploadedPhotos.splice(index, 1);
    }
}