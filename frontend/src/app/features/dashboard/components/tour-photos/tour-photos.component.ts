import { Component, Input, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-tour-photos',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './tour-photos.component.html',
    styleUrl: './tour-photos.component.scss'
})
export class TourPhotosComponent {
    @Input() tourId?: number;
    uploadedPhotos: { url: string, date: Date }[] = [];
    isDragging = false; 

	constructor(private cdr: ChangeDetectorRef) {}

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
        Array.from(files).forEach(file => {
            if (!file.type.startsWith('image/')) return;

            const reader = new FileReader();
            reader.onload = (e: any) => {
                this.uploadedPhotos.unshift({
                    url: e.target.result,
                    date: new Date()
                });
				this.cdr.detectChanges();
            };
            reader.readAsDataURL(file);
        });
    }

    deletePhoto(index: number): void {
        this.uploadedPhotos.splice(index, 1);
    }
}